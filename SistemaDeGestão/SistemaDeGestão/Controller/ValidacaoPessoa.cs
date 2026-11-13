using SistemaDeGestão.Database;
using SistemaDeGestão.Model;

namespace SistemaDeGestão.Controller
{
    /// <summary>Regra comum a Participante e Palestrante: CPF e e-mail únicos na tabela Pessoas.</summary>
    internal static class ValidacaoPessoa
    {
        public static void VerificarDuplicidade(Db db, Pessoa pessoa)
        {
            if (db.Pessoas.Any(x => x.Cpf == pessoa.Cpf && x.Id != pessoa.Id))
                throw new InvalidOperationException("Já existe uma pessoa cadastrada com este CPF.");
            if (db.Pessoas.Any(x => x.Email == pessoa.Email && x.Id != pessoa.Id))
                throw new InvalidOperationException("Já existe uma pessoa cadastrada com este e-mail.");
        }
    }
}
