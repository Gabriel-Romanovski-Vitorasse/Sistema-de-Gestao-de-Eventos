using Microsoft.EntityFrameworkCore;
using SistemaDeGestão.Database;
using SistemaDeGestão.Model;

namespace SistemaDeGestão.Controller
{
    public class OrganizadorController
    {
        public List<Organizador> Listar()
        {
            using var db = new Db();
            return db.Organizadores
                .Include(o => o.Eventos).ThenInclude(e => e.Inscricoes)
                .AsSplitQuery()
                .AsNoTrackingWithIdentityResolution()
                .OrderBy(o => o.Nome)
                .ToList();
        }

        public Organizador? BuscarPorId(int id)
        {
            using var db = new Db();
            return db.Organizadores
                .Include(o => o.Eventos).ThenInclude(e => e.Inscricoes)
                .AsSplitQuery()
                .AsNoTrackingWithIdentityResolution()
                .FirstOrDefault(o => o.Id == id);
        }

        public void Salvar(Organizador o)
        {
            Validar(o);

            using var db = new Db();
            VerificarDuplicidade(db, o);

            if (o.Id == 0)
            {
                db.Organizadores.Add(o);
                db.SaveChanges();
                return;
            }

            using var transacao = db.Database.BeginTransaction();

            var atual = db.Organizadores.AsNoTracking().FirstOrDefault(x => x.Id == o.Id)
                ?? throw new InvalidOperationException("Organizador não encontrado.");

            // Troca Pessoa Física <-> Pessoa Jurídica: na herança TPT o EF não muda o tipo de
            // uma entidade, então a linha da tabela filha é trocada direto no banco.
            if (atual.GetType() != o.GetType())
                TrocarTipo(db, o);

            var existente = db.Organizadores.First(x => x.Id == o.Id);
            if (o.SenhaHash == "") o.SenhaHash = existente.SenhaHash; // a tela não altera a senha
            db.Entry(existente).CurrentValues.SetValues(o);
            db.SaveChanges();
            transacao.Commit();
        }

        public void Excluir(int id)
        {
            using var db = new Db();
            var organizador = db.Organizadores.Find(id)
                ?? throw new InvalidOperationException("Organizador não encontrado.");

            if (db.Eventos.Any(e => e.OrganizadorId == id))
                throw new InvalidOperationException("Organizador possui eventos e não pode ser excluído.");

            db.Organizadores.Remove(organizador);
            db.SaveChanges();
        }

        private static void Validar(Organizador o)
        {
            if (string.IsNullOrWhiteSpace(o.Nome) || string.IsNullOrWhiteSpace(o.Email))
                throw new InvalidOperationException("Nome e e-mail são obrigatórios.");

            switch (o)
            {
                case PessoaFisica pf when string.IsNullOrWhiteSpace(pf.Cpf):
                    throw new InvalidOperationException("Informe o CPF.");
                case PessoaJuridica pj when string.IsNullOrWhiteSpace(pj.Cnpj) || string.IsNullOrWhiteSpace(pj.RazaoSocial):
                    throw new InvalidOperationException("Informe o CNPJ e a razão social.");
            }
        }

        private static void VerificarDuplicidade(Db db, Organizador o)
        {
            if (db.Organizadores.Any(x => x.Email == o.Email && x.Id != o.Id))
                throw new InvalidOperationException("Já existe um organizador com este e-mail.");

            if (o is PessoaFisica pf && db.OrganizadoresPessoaFisica.Any(x => x.Cpf == pf.Cpf && x.Id != pf.Id))
                throw new InvalidOperationException("Já existe um organizador com este CPF.");

            if (o is PessoaJuridica pj && db.OrganizadoresPessoaJuridica.Any(x => x.Cnpj == pj.Cnpj && x.Id != pj.Id))
                throw new InvalidOperationException("Já existe um organizador com este CNPJ.");
        }

        private static void TrocarTipo(Db db, Organizador o)
        {
            db.Database.ExecuteSql($"DELETE FROM PessoasFisicas WHERE Id = {o.Id}");
            db.Database.ExecuteSql($"DELETE FROM PessoasJuridicas WHERE Id = {o.Id}");

            switch (o)
            {
                case PessoaFisica pf:
                    db.Database.ExecuteSql($"INSERT INTO PessoasFisicas (Id, Cpf) VALUES ({pf.Id}, {pf.Cpf})");
                    break;
                case PessoaJuridica pj:
                    db.Database.ExecuteSql(
                        $"INSERT INTO PessoasJuridicas (Id, Cnpj, RazaoSocial) VALUES ({pj.Id}, {pj.Cnpj}, {pj.RazaoSocial})");
                    break;
            }
        }
    }
}
