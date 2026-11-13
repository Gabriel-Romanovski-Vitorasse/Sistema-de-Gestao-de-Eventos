using SistemaDeGestão.Database;
using SistemaDeGestão.Model;
using SistemaDeGestão.Services;

namespace SistemaDeGestão.Controller
{
    public class AutenticacaoController
    {
        private const int TamanhoMinimoSenha = 6;

        /// <summary>Procura o e-mail entre organizadores e pessoas e confere a senha.</summary>
        public UsuarioLogado Entrar(string email, string senha)
        {
            email = email.Trim();
            if (email == "" || senha == "")
                throw new InvalidOperationException("Informe o e-mail e a senha.");

            using var db = new Db();

            var organizador = db.Organizadores.FirstOrDefault(o => o.Email == email);
            if (organizador != null && SenhaConfere(senha, organizador.SenhaHash))
                return new UsuarioLogado(organizador.Id, organizador.Nome, organizador.Email, Perfil.Organizador);

            var pessoa = db.Pessoas.FirstOrDefault(p => p.Email == email);
            if (pessoa != null && SenhaConfere(senha, pessoa.SenhaHash))
            {
                var perfil = pessoa is Palestrante ? Perfil.Palestrante : Perfil.Participante;
                return new UsuarioLogado(pessoa.Id, pessoa.Nome, pessoa.Email, perfil);
            }

            throw new InvalidOperationException("E-mail ou senha inválidos.");
        }

        /// <summary>Cria a conta de um Participante ou Palestrante e já devolve o usuário logado.</summary>
        public UsuarioLogado CriarConta(Pessoa pessoa, string senha, string confirmacao)
        {
            pessoa.SenhaHash = ValidarSenha(senha, confirmacao);
            switch (pessoa)
            {
                case Participante p: new ParticipanteController().Salvar(p); break;
                case Palestrante p: new PalestranteController().Salvar(p); break;
                default: throw new InvalidOperationException("Tipo de conta inválido.");
            }
            var perfil = pessoa is Palestrante ? Perfil.Palestrante : Perfil.Participante;
            return new UsuarioLogado(pessoa.Id, pessoa.Nome, pessoa.Email, perfil);
        }

        /// <summary>Cria a conta de um Organizador (Pessoa Física ou Jurídica) e já devolve o usuário logado.</summary>
        public UsuarioLogado CriarConta(Organizador organizador, string senha, string confirmacao)
        {
            organizador.SenhaHash = ValidarSenha(senha, confirmacao);
            new OrganizadorController().Salvar(organizador);
            return new UsuarioLogado(organizador.Id, organizador.Nome, organizador.Email, Perfil.Organizador);
        }

        private static string ValidarSenha(string senha, string confirmacao)
        {
            if (senha.Length < TamanhoMinimoSenha)
                throw new InvalidOperationException($"A senha deve ter pelo menos {TamanhoMinimoSenha} caracteres.");
            if (senha != confirmacao)
                throw new InvalidOperationException("A confirmação não confere com a senha.");
            return Senha.GerarHash(senha);
        }

        // Conta criada pelo organizador nas telas de cadastro ainda não tem senha.
        private static bool SenhaConfere(string senha, string hash) => hash != "" && Senha.Conferir(senha, hash);
    }
}
