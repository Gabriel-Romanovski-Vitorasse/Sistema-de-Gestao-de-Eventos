using System.Windows;
using System.Windows.Controls;
using SistemaDeGestão.Services;
using SistemaDeGestão.ViewModels;
using SistemaDeGestão.Views;

namespace SistemaDeGestão;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MostrarUsuario();

        Navigator.Navigated += ShowPage;
        Closed += (_, _) => Navigator.Navigated -= ShowPage;
        Navigator.Go(AppPage.Eventos);
    }

    /// <summary>Nome do usuário logado e menu só com as seções do perfil dele.</summary>
    private void MostrarUsuario()
    {
        var usuario = Sessao.Usuario!;
        ContaNome.Text = usuario.Nome;
        ContaIniciais.Text = Formato.Iniciais(usuario.Nome);
        AreaTexto.Text = usuario.Perfil switch
        {
            Perfil.Participante => "Área do participante",
            Perfil.Palestrante => "Área do palestrante",
            _ => "Área de gestão",
        };

        foreach (var item in NavSections.Children.OfType<RadioButton>())
            item.Visibility = item.Tag is AppPage p && Sessao.PodeAcessar(p) ? Visibility.Visible : Visibility.Collapsed;
    }

    private void ShowPage(AppPage page)
    {
        if (!Sessao.PodeAcessar(page))
        {
            Navigator.Go(AppPage.Eventos);
            return;
        }

        PageHost.Content = page switch
        {
            AppPage.Eventos => new EventosView(),
            AppPage.EventoDetalhes => new EventoDetalhesView(),
            AppPage.EventoCadastro => new EventoCadastroView(),
            AppPage.Participantes => new ParticipantesView(),
            AppPage.Palestrantes => new PalestrantesView(),
            AppPage.Organizadores => new OrganizadoresView(),
            AppPage.Locais => new LocaisView(),
            AppPage.Inscricoes => new InscricoesView(),
            AppPage.Certificados => new CertificadosView(),
            _ => null,
        };

        var section = Navigator.SectionOf(page);
        Breadcrumb.Text = $"Espaço de trabalho  /  {Sessao.Usuario!.Perfil}  /  {Navigator.SectionName(section)}";

        foreach (var item in NavSections.Children.OfType<RadioButton>())
            item.IsChecked = item.Tag is AppPage p && p == section;

        PageScroll.ScrollToTop();
    }

    private void Nav_Click(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton { Tag: AppPage page } && Navigator.Current != page) Navigator.Go(page);
    }

    private void Brand_Click(object sender, RoutedEventArgs e) => Navigator.Go(AppPage.Eventos);

    private void Conta_Click(object sender, RoutedEventArgs e)
    {
        var resposta = MessageBox.Show(this, $"Sair da conta de {Sessao.Usuario!.Nome}?", "elo",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (resposta != MessageBoxResult.Yes) return;

        Sessao.Sair();
        var login = new LoginWindow();
        Application.Current.MainWindow = login;
        login.Show();
        Close();
    }

    private void Support_Click(object sender, RoutedEventArgs e) =>
        MessageBox.Show(this, "A Central de suporte será aberta aqui.", "elo", MessageBoxButton.OK, MessageBoxImage.Information);
}
