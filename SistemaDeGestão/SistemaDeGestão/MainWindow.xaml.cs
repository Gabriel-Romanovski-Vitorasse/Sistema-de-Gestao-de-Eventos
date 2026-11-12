using System.Windows;
using System.Windows.Controls;
using SistemaDeGestão.Services;
using SistemaDeGestão.Views;

namespace SistemaDeGestão;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Navigator.Navigated += ShowPage;
        Navigator.Go(AppPage.Eventos);
    }

    private void ShowPage(AppPage page)
    {
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
        Breadcrumb.Text = $"Espaço de trabalho  /  Conecta Educação  /  {Navigator.SectionName(section)}";

        foreach (var item in NavSections.Children.OfType<RadioButton>())
            item.IsChecked = item.Tag is AppPage p && p == section;

        PageScroll.ScrollToTop();
    }

    private void Nav_Click(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton { Tag: AppPage page } && Navigator.Current != page) Navigator.Go(page);
    }

    private void Brand_Click(object sender, RoutedEventArgs e) => Navigator.Go(AppPage.Eventos);

    private void Support_Click(object sender, RoutedEventArgs e) =>
        MessageBox.Show(this, "A Central de suporte será aberta aqui.", "elo", MessageBoxButton.OK, MessageBoxImage.Information);
}
