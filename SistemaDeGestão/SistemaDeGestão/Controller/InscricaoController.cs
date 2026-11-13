using Microsoft.EntityFrameworkCore;
using SistemaDeGestão.Database;
using SistemaDeGestão.Model;
using SistemaDeGestão.Model.Relatorios;

namespace SistemaDeGestão.Controller
{
    public class InscricaoController
    {
        public List<Inscricao> Listar()
        {
            using var db = new Db();
            return db.Inscricoes
                .Include(i => i.Participante)
                .Include(i => i.Evento)
                .Include(i => i.Pagamento)
                .AsNoTrackingWithIdentityResolution()
                .OrderByDescending(i => i.DataInscricao)
                .ToList();
        }

        public Inscricao Inscrever(int participanteId, int eventoId)
        {
            using var db = new Db();

            if (!db.Participantes.Any(p => p.Id == participanteId))
                throw new InvalidOperationException("Participante não encontrado.");

            var evento = db.Eventos.Include(e => e.Inscricoes).FirstOrDefault(e => e.Id == eventoId)
                ?? throw new InvalidOperationException("Evento não encontrado.");

            if (evento.Status != StatusEvento.Publicado)
                throw new InvalidOperationException("Só é possível se inscrever em eventos publicados.");
            if (evento.Esgotado)
                throw new InvalidOperationException("Evento esgotado: não há vagas disponíveis.");

            // O índice único (ParticipanteId, EventoId) impede duas inscrições no mesmo evento.
            // Se a anterior foi cancelada, ela volta a ficar pendente.
            var anterior = evento.Inscricoes.FirstOrDefault(i => i.ParticipanteId == participanteId);
            if (anterior != null)
            {
                if (anterior.Status != StatusInscricao.Cancelada)
                    throw new InvalidOperationException("O participante já está inscrito neste evento.");

                anterior.Status = StatusInscricao.Pendente;
                anterior.DataInscricao = DateTime.Now;
                db.SaveChanges();
                return anterior;
            }

            var inscricao = new Inscricao { ParticipanteId = participanteId, EventoId = eventoId };
            db.Inscricoes.Add(inscricao);
            db.SaveChanges();
            return inscricao;
        }

        public void Pagar(int inscricaoId, string metodo)
        {
            if (string.IsNullOrWhiteSpace(metodo))
                throw new InvalidOperationException("Informe a forma de pagamento.");

            using var db = new Db();
            var inscricao = db.Inscricoes
                .Include(i => i.Evento)
                .Include(i => i.Pagamento)
                .FirstOrDefault(i => i.Id == inscricaoId)
                ?? throw new InvalidOperationException("Inscrição não encontrada.");

            // A regra (não pagar cancelada nem pagar duas vezes) fica no model.
            inscricao.Pagar();
            inscricao.Pagamento!.Metodo = metodo;
            db.SaveChanges();
        }

        public void Cancelar(int inscricaoId)
        {
            using var db = new Db();
            var inscricao = db.Inscricoes.Find(inscricaoId)
                ?? throw new InvalidOperationException("Inscrição não encontrada.");

            if (inscricao.Status == StatusInscricao.Cancelada)
                throw new InvalidOperationException("A inscrição já está cancelada.");

            inscricao.Cancelar();
            db.SaveChanges();
        }

        // SQL puro: contagem por status e valor das inscrições pendentes.
        public IndicadoresInscricoes ObterIndicadores()
        {
            using var db = new Db();
            return db.Database.SqlQuery<IndicadoresInscricoes>($"""
                SELECT
                    COUNT(CASE WHEN i.Status = 'Confirmada' THEN 1 END) AS Confirmadas,
                    COUNT(CASE WHEN i.Status = 'Pendente'   THEN 1 END) AS Pendentes,
                    COUNT(CASE WHEN i.Status = 'Cancelada'  THEN 1 END) AS Canceladas,
                    CAST(COALESCE(SUM(CASE WHEN i.Status = 'Pendente' THEN e.PrecoInscricao END), 0) AS decimal(10, 2)) AS ValorEmAberto
                FROM Inscricoes i
                INNER JOIN Eventos e ON e.Id = i.EventoId
                """).AsEnumerable().First();
        }

        // SQL puro: JOIN Pagamentos - Inscricoes - Eventos, total arrecadado por evento
        // (só inscrições confirmadas; eventos sem pagamento aparecem com zero).
        public List<RelatorioFinanceiroItem> RelatorioFinanceiro()
        {
            using var db = new Db();
            return db.Database.SqlQuery<RelatorioFinanceiroItem>($"""
                SELECT
                    e.Id AS EventoId,
                    e.Titulo AS Evento,
                    COUNT(p.Id) AS TotalPagamentos,
                    CAST(COALESCE(SUM(p.Valor), 0) AS decimal(10, 2)) AS ValorArrecadado
                FROM Eventos e
                LEFT JOIN Inscricoes i ON i.EventoId = e.Id AND i.Status = 'Confirmada'
                LEFT JOIN Pagamentos p ON p.InscricaoId = i.Id
                GROUP BY e.Id, e.Titulo
                ORDER BY ValorArrecadado DESC
                """).AsEnumerable().ToList();
        }
    }
}
