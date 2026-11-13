using Microsoft.EntityFrameworkCore;
using SistemaDeGestão.Database;
using SistemaDeGestão.Model;

namespace SistemaDeGestão.Controller
{
    public class CertificadoController
    {
        /// <summary>
        /// Certificados já gravados e, para cada evento encerrado, os ainda não gerados de quem tem direito
        /// (participantes com inscrição confirmada e palestrantes). Esses últimos têm Id 0 e ficam
        /// "Pronto para emitir" até o organizador chamar Emitir.
        /// </summary>
        public List<Certificado> Listar()
        {
            using var db = new Db();
            var certificados = db.Certificados
                .Include(c => c.Pessoa)
                .Include(c => c.Evento).ThenInclude(e => e.Organizador)
                .AsNoTrackingWithIdentityResolution()
                .OrderBy(c => c.Codigo)
                .ToList();

            var encerrados = db.Eventos
                .Where(e => e.Status == StatusEvento.Encerrado)
                .Include(e => e.Organizador)
                .Include(e => e.Palestrantes)
                .Include(e => e.Inscricoes.Where(i => i.Status == StatusInscricao.Confirmada)).ThenInclude(i => i.Participante)
                .AsSplitQuery()
                .AsNoTrackingWithIdentityResolution()
                .ToList();

            var existentes = certificados.Select(c => (c.PessoaId, c.EventoId)).ToHashSet();
            foreach (var evento in encerrados)
            {
                var pessoas = evento.Inscricoes.Select(i => (Pessoa)i.Participante).Concat(evento.Palestrantes);
                foreach (var pessoa in pessoas.Where(p => !existentes.Contains((p.Id, evento.Id))))
                {
                    certificados.Add(new Certificado
                    {
                        Codigo = "—",
                        PessoaId = pessoa.Id, Pessoa = pessoa,
                        EventoId = evento.Id, Evento = evento,
                    });
                }
            }
            return certificados;
        }

        public Certificado? BuscarPorId(int id)
        {
            using var db = new Db();
            return db.Certificados
                .Include(c => c.Pessoa)
                .Include(c => c.Evento)
                .AsNoTrackingWithIdentityResolution()
                .FirstOrDefault(c => c.Id == id);
        }

        // Emite o certificado da pessoa (Participante ou Palestrante) no evento.
        // Usado na tela de Certificados e no botão "Emitir certificado" da tela de Palestrantes.
        public Certificado Emitir(int pessoaId, int eventoId)
        {
            using var db = new Db();

            var certificado = db.Certificados.FirstOrDefault(c => c.PessoaId == pessoaId && c.EventoId == eventoId);
            if (certificado?.Emitido == true)
                throw new InvalidOperationException("O certificado já foi emitido.");

            if (certificado == null)
            {
                VerificarDireito(db, pessoaId, eventoId);
                certificado = new Certificado { PessoaId = pessoaId, EventoId = eventoId, Codigo = NovoCodigo(db) };
                db.Certificados.Add(certificado);
            }

            certificado.DataEmissao = DateTime.Now;
            db.SaveChanges();
            return certificado;
        }

        // Só recebe certificado quem participou de um evento já encerrado:
        // participante com inscrição confirmada ou palestrante vinculado ao evento.
        private static void VerificarDireito(Db db, int pessoaId, int eventoId)
        {
            var evento = db.Eventos.Find(eventoId)
                ?? throw new InvalidOperationException("Evento não encontrado.");
            if (evento.Status != StatusEvento.Encerrado)
                throw new InvalidOperationException("O certificado só pode ser emitido após o encerramento do evento.");

            var pessoa = db.Pessoas.Find(pessoaId)
                ?? throw new InvalidOperationException("Pessoa não encontrada.");

            bool temDireito = pessoa switch
            {
                Participante => db.Inscricoes.Any(i => i.ParticipanteId == pessoaId && i.EventoId == eventoId
                                                       && i.Status == StatusInscricao.Confirmada),
                Palestrante => db.Eventos.Any(e => e.Id == eventoId && e.Palestrantes.Any(p => p.Id == pessoaId)),
                _ => false,
            };
            if (!temDireito)
                throw new InvalidOperationException("Esta pessoa não participou do evento.");
        }

        // Código no formato ELO-AAAA-NNNN, sequencial dentro do ano.
        private static string NovoCodigo(Db db)
        {
            string prefixo = $"ELO-{DateTime.Now.Year}-";
            int ultimo = db.Certificados
                .Where(c => c.Codigo.StartsWith(prefixo))
                .Select(c => c.Codigo)
                .AsEnumerable()
                .Select(c => int.TryParse(c[prefixo.Length..], out int n) ? n : 0)
                .DefaultIfEmpty(0)
                .Max();
            return $"{prefixo}{ultimo + 1:0000}";
        }
    }
}
