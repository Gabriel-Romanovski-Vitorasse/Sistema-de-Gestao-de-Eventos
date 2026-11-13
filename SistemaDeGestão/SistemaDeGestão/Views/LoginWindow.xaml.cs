using System.Windows;
using System.Windows.Controls;
using SistemaDeGestão.Controller;
using SistemaDeGestão.Model;
using SistemaDeGestão.Services;

namespace SistemaDeGestão.Views;

public partial class LoginWindow : Window
{
    private readonly AutenticacaoController _autenticacao = new();

    // Ordem dos itens do combo "Tipo de conta"
    private enum TipoConta { Participante, Palestrante, OrganizadorPf, OrganizadorPj }

    public LoginWindow()
    {
        InitializeComponent();
        AplicarTipo();
        Loaded += (_, _) => EntrarEmail.Focus();
    }

    private TipoConta Tipo => (TipoConta)Math.Max(CadTipo.SelectedIndex, 0);

    // ----- Entrar -----

    private void Entrar_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            AbrirSistema(_autenticacao.Entrar(EntrarEmail.Text, EntrarSenha.Password));
        }
        catch (InvalidOperationException erro)
        {
            MostrarErro(EntrarErro, erro.Message);
        }
    }

    // ----- Criar conta -----

    private void Tipo_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (IsLoaded) AplicarTipo();
    }

    /// <summary>Mostra só os campos do tipo de conta escolhido.</summary>
    private void AplicarTipo()
    {
        static Visibility Se(bool mostrar) => mostrar ? Visibility.Visible : Visibility.Collapsed;
        bool organizador = Tipo is TipoConta.OrganizadorPf or TipoConta.OrganizadorPj;

        CampoCpf.Visibility = Se(Tipo != TipoConta.OrganizadorPj);
        CampoInstituicao.Visibility = Se(Tipo == TipoConta.Participante);
        CampoEspecialidade.Visibility = Se(Tipo == TipoConta.Palestrante);
        CampoBio.Visibility = Se(Tipo == TipoConta.Palestrante);
        CampoRazao.Visibility = Se(Tipo == TipoConta.OrganizadorPj);
        CampoCnpj.Visibility = Se(Tipo == TipoConta.OrganizadorPj);
        CampoTelefone.Visibility = Se(organizador);
    }

    private void Criar_Click(object sender, RoutedEventArgs e)
    {
        string nome = CadNome.Text.Trim(), email = CadEmail.Text.Trim(), cpf = CadCpf.Text.Trim();
        var obrigatorios = Tipo switch
        {
            TipoConta.Participante => new[] { nome, email, cpf },
            TipoConta.Palestrante => [nome, email, cpf, CadEspecialidade.Text, CadBio.Text],
            TipoConta.OrganizadorPf => [nome, email, cpf],
            _ => [nome, email, CadRazao.Text, CadCnpj.Text],
        };
        if (obrigatorios.Any(string.IsNullOrWhiteSpace))
        {
            MostrarErro(CadErro, "Preencha todos os campos obrigatórios (*).");
            return;
        }

        try
        {
            string senha = CadSenha.Password, confirmacao = CadConfirmacao.Password;
            UsuarioLogado usuario = Tipo switch
            {
                TipoConta.Participante => _autenticacao.CriarConta(new Participante
                {
                    Nome = nome, Email = email, Cpf = cpf, Instituicao = CadInstituicao.Text.Trim(),
                }, senha, confirmacao),
                TipoConta.Palestrante => _autenticacao.CriarConta(new Palestrante
                {
                    Nome = nome, Email = email, Cpf = cpf,
                    Especialidade = CadEspecialidade.Text.Trim(), MiniBio = CadBio.Text.Trim(),
                }, senha, confirmacao),
                TipoConta.OrganizadorPf => _autenticacao.CriarConta(new PessoaFisica
                {
                    Nome = nome, Email = email, Cpf = cpf, Telefone = CadTelefone.Text.Trim(),
                }, senha, confirmacao),
                _ => _autenticacao.CriarConta(new PessoaJuridica
                {
                    Nome = nome, Email = email, Telefone = CadTelefone.Text.Trim(),
                    RazaoSocial = CadRazao.Text.Trim(), Cnpj = CadCnpj.Text.Trim(),
                }, senha, confirmacao),
            };
            AbrirSistema(usuario);
        }
        catch (InvalidOperationException erro)
        {
            MostrarErro(CadErro, erro.Message);
        }
    }

    // ----- Navegação entre os painéis -----

    private void IrParaCadastro_Click(object sender, RoutedEventArgs e)
    {
        PainelEntrar.Visibility = Visibility.Collapsed;
        PainelCadastro.Visibility = Visibility.Visible;
        BotaoEntrar.IsDefault = false;
        BotaoCriar.IsDefault = true;
        CadEmail.Text = EntrarEmail.Text;
        CadNome.Focus();
    }

    private void IrParaEntrar_Click(object sender, RoutedEventArgs e)
    {
        PainelCadastro.Visibility = Visibility.Collapsed;
        PainelEntrar.Visibility = Visibility.Visible;
        BotaoCriar.IsDefault = false;
        BotaoEntrar.IsDefault = true;
        EntrarEmail.Focus();
    }

    private static void MostrarErro(TextBlock alvo, string mensagem)
    {
        alvo.Text = mensagem;
        alvo.Visibility = Visibility.Visible;
    }

    private void AbrirSistema(UsuarioLogado usuario)
    {
        Sessao.Entrar(usuario);
        var principal = new MainWindow();
        Application.Current.MainWindow = principal;
        principal.Show();
        Close();
    }
}
