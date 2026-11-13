using Microsoft.EntityFrameworkCore;
using SistemaDeGestão.Database;
using SistemaDeGestão.Model;

namespace SistemaDeGestão.Controller
{
    public class PalestranteController
    {
        public List<Palestrante> Listar()
        {
            using var db = new Db();
            return db.Palestrantes
                .Include(p => p.Eventos)
                .Include(p => p.Certificados)
                .AsSplitQuery()
                .AsNoTrackingWithIdentityResolution()
                .OrderBy(p => p.Nome)
                .ToList();
        }

        public Palestrante? BuscarPorId(int id)
        {
            using var db = new Db();
            return db.Palestrantes
                .Include(p => p.Eventos)
                .Include(p => p.Certificados)
                .AsSplitQuery()
                .AsNoTrackingWithIdentityResolution()
                .FirstOrDefault(p => p.Id == id);
        }

        public void Salvar(Palestrante p)
        {
            if (string.IsNullOrWhiteSpace(p.Nome) || string.IsNullOrWhiteSpace(p.Cpf) || string.IsNullOrWhiteSpace(p.Email))
                throw new InvalidOperationException("Nome, CPF e e-mail são obrigatórios.");

            using var db = new Db();
            ValidacaoPessoa.VerificarDuplicidade(db, p);

            if (p.Id == 0)
            {
                db.Palestrantes.Add(p);
            }
            else
            {
                // Só os dados do palestrante; os vínculos com eventos são feitos no cadastro do evento.
                var existente = db.Palestrantes.Find(p.Id)
                    ?? throw new InvalidOperationException("Palestrante não encontrado.");
                if (p.SenhaHash == "") p.SenhaHash = existente.SenhaHash; // a tela não altera a senha
                db.Entry(existente).CurrentValues.SetValues(p);
            }
            db.SaveChanges();
        }

        public void Excluir(int id)
        {
            using var db = new Db();
            var palestrante = db.Palestrantes.Find(id)
                ?? throw new InvalidOperationException("Palestrante não encontrado.");

            if (db.Certificados.Any(c => c.PessoaId == id))
                throw new InvalidOperationException("Palestrante possui certificados e não pode ser excluído.");

            // Os vínculos em EventoPalestrante são removidos em cascata.
            db.Palestrantes.Remove(palestrante);
            db.SaveChanges();
        }
    }
}
