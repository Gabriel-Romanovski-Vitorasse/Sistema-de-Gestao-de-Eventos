using System.Windows;
using System.Windows.Controls;
using SistemaDeGestão.Controller;
using SistemaDeGestão.Model;
using SistemaDeGestão.Services;
using SistemaDeGestão.ViewModels;

namespace SistemaDeGestão.Views;

public partial class EventosView : UserControl
{
    private readonly EventoController _eventos = new();
    private readonly InscricaoController _inscricoes = new();

    private readonly List<EventoItem> _todos;
    private readonly Evento? _destaque;
    private bool? _ordemCrescente; // null = ordem original do cadastro
    private readonly bool _pronto;

    public EventosView()
    {
        InitializeComponent();
        _todos = _eventos.Listar().Select(e => new EventoItem(e)).ToList();
        _destaque = ProximoEvento(_todos);

        PreencherIndicadores();
        PreencherDestaque();
        TabTodos.Content = $"Todos os eventos  {_todos.Count}";
        TabConferencias.Content = $"Conferências  {_todos.Count(e => e.Tipo == "Conferência")}";
        TabWorkshops.Content = $"Workshops  {_todos.Count(e => e.Tipo == "Workshop")}";
        _pronto = true;
        Atualizar();
    }

    private static Evento? ProximoEvento(IEnumerable<EventoItem> eventos) => eventos
        .Select(e => e.Modelo)
        .Where(e => e.Status == StatusEvento.Publicado && e.DataInicio >= DateTime.Today)
        .OrderBy(e => e.DataInicio)
        .FirstOrDefault();

    private void PreencherIndicadores()
    {
        int conferencias = _todos.Count(e => e.Tipo == "Conferência");
        StatEventos.Value = _todos.Count.ToString("00");
        StatEventos.Caption = $"{Formato.Plural(conferencias, "conferência", "conferências")} · " +
                              $"{Formato.Plural(_todos.Count - conferencias, "workshop", "workshops")}";

        StatConfirmadas.Value = _inscricoes.ObterIndicadores().Confirmadas.ToString();

        StatVagas.Value = _todos.Select(e => e.Modelo)
            .Where(e => e.Status != StatusEvento.Encerrado)
            .Sum(e => e.VagasDisponiveis())
            .ToString();

        decimal receita = _inscricoes.RelatorioFinanceiro().Sum(r => r.ValorArrecadado);
        StatReceita.Value = receita.ToString("C0", Formato.PtBr);
    }

    private void PreencherDestaque()
    {
        if (_destaque == null)
        {
            Destaque.Visibility = Visibility.Collapsed;
            return;
        }

        int confirmadas = _destaque.Inscricoes.Count(i => i.Status == StatusInscricao.Confirmada);
        DestaqueData.Text = $"PRÓXIMO ENCONTRO · {_destaque.DataInicio.ToString("dd MMMM", Formato.PtBr).ToUpper(Formato.PtBr)}";
        DestaqueTitulo.Text = _destaque.Titulo;
        DestaqueTexto.Text = confirmadas == 1
            ? "1 pessoa já confirmou presença. Vamos preparar um grande encontro?"
            : $"{confirmadas} pessoas já confirmaram presença. Vamos preparar um grande encontro?";
    }

    private void Atualizar()
    {
        if (!_pronto) return;

        string tipo = (TabConferencias.IsChecked == true ? TabConferencias.Tag
                     : TabWorkshops.IsChecked == true ? TabWorkshops.Tag : "") as string ?? "";
        string termo = Busca.Text.Trim();
        string status = FiltroStatus.SelectedIndex > 0 ? ((ComboBoxItem)FiltroStatus.SelectedItem).Content as string ?? "" : "";

        IEnumerable<EventoItem> lista = _todos;
        if (tipo != "") lista = lista.Where(e => e.Tipo == tipo);
        if (status != "") lista = lista.Where(e => e.Status == status);
        if (termo != "")
            lista = lista.Where(e =>
                e.Titulo.Contains(termo, StringComparison.CurrentCultureIgnoreCase) ||
                e.Subtitulo.Contains(termo, StringComparison.CurrentCultureIgnoreCase));
        if (_ordemCrescente is bool asc)
            lista = asc ? lista.OrderBy(e => e.Inicio) : lista.OrderByDescending(e => e.Inicio);

        var resultado = lista.ToList();
        Tabela.ItemsSource = resultado;
        Vazio.Visibility = resultado.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        Contagem.Text = resultado.Count == 0
            ? "Nenhum evento para mostrar"
            : $"Mostrando 1–{resultado.Count} de {resultado.Count} eventos";
    }

    private void Tab_Checked(object sender, RoutedEventArgs e) => Atualizar();
    private void Busca_Changed(object sender, TextChangedEventArgs e) => Atualizar();
    private void Filtro_Changed(object sender, SelectionChangedEventArgs e) => Atualizar();

    private void Ordenar_Click(object sender, RoutedEventArgs e)
    {
        _ordemCrescente = _ordemCrescente != true;
        OrdenarTexto.Text = _ordemCrescente == true ? "Ordenar por: data do evento ↑" : "Ordenar por: data do evento ↓";
        Atualizar();
    }

    // O parâmetro da navegação é o Id do evento (sem parâmetro = novo evento).
    private void CriarEvento_Click(object sender, RoutedEventArgs e) => Navigator.Go(AppPage.EventoCadastro);

    private void VerEvento_Click(object sender, RoutedEventArgs e)
    {
        if (_destaque != null) Navigator.Go(AppPage.EventoDetalhes, _destaque.Id);
    }

    private void Detalhes_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: EventoItem item })
            Navigator.Go(AppPage.EventoDetalhes, item.Modelo.Id);
    }
}
