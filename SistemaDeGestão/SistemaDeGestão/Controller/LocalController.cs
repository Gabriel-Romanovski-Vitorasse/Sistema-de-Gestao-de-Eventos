using Microsoft.EntityFrameworkCore;
using SistemaDeGestão.Database;
using SistemaDeGestão.Model;

namespace SistemaDeGestão.Controller
{
    public class LocalController
    {
        public List<Local> Listar()
        {
            using var db = new Db();
            return db.Locais
                .Include(l => l.Eventos)
                .AsNoTrackingWithIdentityResolution()
                .OrderBy(l => l.Nome)
                .ToList();
        }

        public Local? BuscarPorId(int id)
        {
            using var db = new Db();
            return db.Locais
                .Include(l => l.Eventos)
                .AsNoTrackingWithIdentityResolution()
                .FirstOrDefault(l => l.Id == id);
        }

        public void Salvar(Local l)
        {
            if (string.IsNullOrWhiteSpace(l.Nome) || string.IsNullOrWhiteSpace(l.Endereco))
                throw new InvalidOperationException("Nome e endereço são obrigatórios.");
            if (l.Capacidade <= 0)
                throw new InvalidOperationException("A capacidade deve ser maior que zero.");

            using var db = new Db();
            if (l.Id == 0)
            {
                db.Locais.Add(l);
            }
            else
            {
                var existente = db.Locais.Find(l.Id)
                    ?? throw new InvalidOperationException("Local não encontrado.");
                db.Entry(existente).CurrentValues.SetValues(l);
            }
            db.SaveChanges();
        }

        public void Excluir(int id)
        {
            using var db = new Db();
            var local = db.Locais.Find(id)
                ?? throw new InvalidOperationException("Local não encontrado.");

            if (db.Eventos.Any(e => e.LocalId == id))
                throw new InvalidOperationException("Local possui eventos vinculados e não pode ser excluído.");

            db.Locais.Remove(local);
            db.SaveChanges();
        }
    }
}
