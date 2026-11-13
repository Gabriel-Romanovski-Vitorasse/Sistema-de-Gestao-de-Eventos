using Microsoft.EntityFrameworkCore;
using SistemaDeGestão.Database;
using SistemaDeGestão.Model;

namespace SistemaDeGestão.Controller
{
    public class EventoController
    {
        public List<Evento> Listar()
        {
            using var db = new Db();
            return db.Eventos
                .Include(e => e.Local)
                .Include(e => e.Organizador)
                .Include(e => e.Palestrantes)
                .Include(e => e.Inscricoes)
                .AsSplitQuery()
                .AsNoTrackingWithIdentityResolution()
                .OrderBy(e => e.DataInicio)
                .ToList();
        }

        public Evento? BuscarPorId(int id)
        {
            using var db = new Db();
            return db.Eventos
                .Include(e => e.Local)
                .Include(e => e.Organizador)
                .Include(e => e.Palestrantes)
                .Include(e => e.Inscricoes).ThenInclude(i => i.Participante)
                .Include(e => e.Inscricoes).ThenInclude(i => i.Pagamento)
                .AsSplitQuery()
                .AsNoTrackingWithIdentityResolution()
                .FirstOrDefault(e => e.Id == id);
        }

        public void Salvar(Evento evento)
        {
            Validar(evento);

            using var db = new Db();
            using var transacao = db.Database.BeginTransaction();

            if (!db.Locais.Any(l => l.Id == evento.LocalId))
                throw new InvalidOperationException("Local não encontrado.");
            if (!db.Organizadores.Any(o => o.Id == evento.OrganizadorId))
                throw new InvalidOperationException("Organizador não encontrado.");

            var idsPalestrantes = evento.Palestrantes.Select(p => p.Id).ToList();
            var palestrantes = db.Palestrantes.Where(p => idsPalestrantes.Contains(p.Id)).ToList();

            if (evento.Id == 0)
            {
                // O objeto vem da tela com Local/Organizador/Palestrantes preenchidos.
                // Grava só pelas FKs para o EF não tentar inserir essas entidades de novo.
                var local = evento.Local;
                var organizador = evento.Organizador;
                var palestrantesTela = evento.Palestrantes;

                evento.Local = null!;
                evento.Organizador = null!;
                evento.Palestrantes = palestrantes;

                db.Eventos.Add(evento);
                db.SaveChanges();
                transacao.Commit();

                evento.Local = local;
                evento.Organizador = organizador;
                evento.Palestrantes = palestrantesTela;
                return;
            }

            var atual = db.Eventos.AsNoTracking().FirstOrDefault(e => e.Id == evento.Id)
                ?? throw new InvalidOperationException("Evento não encontrado.");

            // Troca de modalidade (Conferência <-> Workshop): na herança TPT o EF não muda o tipo de
            // uma entidade, então a linha da tabela filha é trocada direto no banco.
            if (atual.GetType() != evento.GetType())
                TrocarModalidade(db, evento);

            var existente = db.Eventos.Include(e => e.Palestrantes).First(e => e.Id == evento.Id);
            db.Entry(existente).CurrentValues.SetValues(evento);

            existente.Palestrantes.RemoveAll(p => !idsPalestrantes.Contains(p.Id));
            foreach (var p in palestrantes.Where(p => existente.Palestrantes.All(x => x.Id != p.Id)))
                existente.Palestrantes.Add(p);

            db.SaveChanges();
            transacao.Commit();
        }

        public void Excluir(int id)
        {
            using var db = new Db();
            var evento = db.Eventos.Find(id)
                ?? throw new InvalidOperationException("Evento não encontrado.");

            // Inscrições, pagamentos e certificados são apagados em cascata (composição).
            db.Eventos.Remove(evento);
            db.SaveChanges();
        }

        private static void Validar(Evento evento)
        {
            if (string.IsNullOrWhiteSpace(evento.Titulo))
                throw new InvalidOperationException("Informe o título do evento.");
            if (evento.DataFim < evento.DataInicio)
                throw new InvalidOperationException("A data de término não pode ser anterior à de início.");
            if (evento.Capacidade <= 0)
                throw new InvalidOperationException("A capacidade deve ser maior que zero.");
            if (evento.PrecoInscricao < 0)
                throw new InvalidOperationException("O preço da inscrição não pode ser negativo.");
        }

        private static void TrocarModalidade(Db db, Evento evento)
        {
            db.Database.ExecuteSql($"DELETE FROM Conferencias WHERE Id = {evento.Id}");
            db.Database.ExecuteSql($"DELETE FROM Workshops WHERE Id = {evento.Id}");

            switch (evento)
            {
                case Conferencia c:
                    db.Database.ExecuteSql(
                        $"INSERT INTO Conferencias (Id, Tema, NumeroSessoes) VALUES ({c.Id}, {c.Tema}, {c.NumeroSessoes})");
                    break;
                case Workshop w:
                    db.Database.ExecuteSql(
                        $"INSERT INTO Workshops (Id, CargaHoraria, Material) VALUES ({w.Id}, {w.CargaHoraria}, {w.Material})");
                    break;
            }
        }
    }
}
