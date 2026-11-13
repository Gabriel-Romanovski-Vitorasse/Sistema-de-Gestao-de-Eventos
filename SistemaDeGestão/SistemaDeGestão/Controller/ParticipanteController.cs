using Microsoft.EntityFrameworkCore;
using SistemaDeGestão.Database;
using SistemaDeGestão.Model;

namespace SistemaDeGestão.Controller
{
    public class ParticipanteController
    {
        public List<Participante> Listar()
        {
            using var db = new Db();
            return db.Participantes
                .Include(p => p.Inscricoes).ThenInclude(i => i.Evento)
                .Include(p => p.Certificados)
                .AsSplitQuery()
                .AsNoTrackingWithIdentityResolution()
                .OrderBy(p => p.Nome)
                .ToList();
        }

        public Participante? BuscarPorId(int id)
        {
            using var db = new Db();
            return db.Participantes
                .Include(p => p.Inscricoes).ThenInclude(i => i.Evento)
                .Include(p => p.Certificados)
                .AsSplitQuery()
                .AsNoTrackingWithIdentityResolution()
                .FirstOrDefault(p => p.Id == id);
        }

        public void Salvar(Participante p)
        {
            if (string.IsNullOrWhiteSpace(p.Nome) || string.IsNullOrWhiteSpace(p.Cpf) || string.IsNullOrWhiteSpace(p.Email))
                throw new InvalidOperationException("Nome, CPF e e-mail são obrigatórios.");

            using var db = new Db();
            ValidacaoPessoa.VerificarDuplicidade(db, p);

            if (p.Id == 0)
            {
                db.Participantes.Add(p);
            }
            else
            {
                var existente = db.Participantes.Find(p.Id)
                    ?? throw new InvalidOperationException("Participante não encontrado.");
                if (p.SenhaHash == "") p.SenhaHash = existente.SenhaHash; // a tela não altera a senha
                db.Entry(existente).CurrentValues.SetValues(p);
            }
            db.SaveChanges();
        }

        public void Excluir(int id)
        {
            using var db = new Db();
            var participante = db.Participantes.Find(id)
                ?? throw new InvalidOperationException("Participante não encontrado.");

            if (db.Inscricoes.Any(i => i.ParticipanteId == id))
                throw new InvalidOperationException("Participante possui inscrições e não pode ser excluído.");
            if (db.Certificados.Any(c => c.PessoaId == id))
                throw new InvalidOperationException("Participante possui certificados e não pode ser excluído.");

            db.Participantes.Remove(participante);
            db.SaveChanges();
        }
    }
}
