using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using SistemaDeGestão.Controller;
using SistemaDeGestão.Controls;
using SistemaDeGestão.Model;
using SistemaDeGestão.Services;
using SistemaDeGestão.ViewModels;

namespace SistemaDeGestão.Views;

public partial class EventoCadastroView : UserControl
{
    private static readonly string[] FormatosData = ["dd/MM/yyyy HH:mm", "dd/MM/yyyy H:mm", "dd/MM/yyyy"];

    private readonly EventoController _eventos = new();
    private readonly LocalController _locais = new();
    private readonly OrganizadorController _organizadores = new();
    private readonly PalestranteController _palestrantes = new();

    private readonly Evento? _evento; // null = novo evento
    private readonly List<PalestranteSelecao> _opcoesPalestrantes;
    private readonly bool _pronto;

    public EventoCadastroView()
    {
        InitializeComponent();

        // O parâmetro da navegação é o Id do evento a editar (sem parâmetro = novo evento).
        _evento = Navigator.Parameter is int id ? _eventos.BuscarPorId(id) : null;

        LocalCombo.ItemsSource = _locais.Listar();
        OrganizadorCombo.ItemsSource = _organizadores.Listar().Select(o => new OrganizadorItem(o)).ToList();
        var vinculados = _evento?.Palestrantes.Select(p => p.Id).ToHashSet() ?? [];
        _opcoesPalestrantes = _palestrantes.Listar()
            .Select(p => new PalestranteSelecao(p, vinculados.Contains(p.Id)))
            .ToList();
        PalestrantesOpcoes.ItemsSource = _opcoesPalestrantes;

        Preencher(_evento);
        _pronto = true;
        AplicarTipo();
        AtualizarResumo();
    }

    private void Preencher(Evento? e)
    {
        Trilha.Text = e == null ? "← Eventos  /  Novo evento" : $"← Eventos  /  Editar {e.Codigo}";
        CampoId.Text = e == null ? "Gerado automaticamente ao salvar" : $"{e.Codigo} · gerado automaticamente";
        RascunhoPill.Text = e == null ? "Novo evento" : Formato.StatusDoEvento(e);
        RascunhoPill.Kind = e == null ? PillKind.Neutral : StatusKinds.Of(RascunhoPill.Text);

        Tipo.SelectedIndex = e is Workshop ? 1 : 0;
        Titulo.Text = e?.Titulo ?? "";
        Descricao.Text = e?.Descricao ?? "";
        DataInicio.Text = e == null ? "" : $"{e.DataInicio:dd/MM/yyyy}  ·  {e.DataInicio:HH:mm}";
        DataFim.Text = e == null ? "" : $"{e.DataFim:dd/MM/yyyy}  ·  {e.DataFim:HH:mm}";
        Capacidade.Text = e?.Capacidade.ToString() ?? "";
        Preco.Text = e == null ? "" : Formato.Moeda(e.PrecoInscricao);

        Tema.Text = (e as Conferencia)?.Tema ?? "";
        Sessoes.Text = (e as Conferencia)?.NumeroSessoes.ToString() ?? "";
        CargaHoraria.Text = (e as Workshop)?.CargaHoraria.ToString() ?? "";
        Material.Text = (e as Workshop)?.Material ?? "";

        LocalCombo.SelectedItem = e == null ? null : LocalCombo.Items.OfType<Local>().FirstOrDefault(l => l.Id == e.LocalId);
        OrganizadorCombo.SelectedItem = e == null ? null
            : OrganizadorCombo.Items.OfType<OrganizadorItem>().FirstOrDefault(o => o.Modelo.Id == e.OrganizadorId);
    }

    private bool EhWorkshop => Tipo.SelectedIndex == 1;

    private List<Palestrante> PalestrantesSelecionados() =>
        _opcoesPalestrantes.Where(o => o.Selecionado).Select(o => o.Item.Modelo).ToList();

    private static DateTime? LerData(string texto)
    {
        string normalizado = string.Join(' ', texto.Replace('·', ' ').Split(' ', StringSplitOptions.RemoveEmptyEntries));
        return DateTime.TryParseExact(normalizado, FormatosData, Formato.PtBr, DateTimeStyles.None, out var data) ? data : null;
    }

    private static decimal? LerPreco(string texto)
    {
        if (texto.Trim().Equals("Gratuito", StringComparison.CurrentCultureIgnoreCase)) return 0;
        return decimal.TryParse(texto, NumberStyles.Currency, Formato.PtBr, out var valor) ? valor : null;
    }

    private void AplicarTipo()
    {
        CamposWorkshop.IsEnabled = EhWorkshop;
        CamposConferencia.IsEnabled = !EhWorkshop;
    }

    private void AtualizarResumo()
    {
        if (!_pronto) return;

        ResumoTipo.Text = (Tipo.SelectedItem as ComboBoxItem)?.Content as string ?? "";
        ResumoTitulo.Text = string.IsNullOrWhiteSpace(Titulo.Text) ? "Sem título" : Titulo.Text;
        ResumoPeriodo.Text = LerData(DataInicio.Text) is DateTime ini && LerData(DataFim.Text) is DateTime fim
            ? Formato.PeriodoExtenso(ini, fim)
            : "Período a definir";
        ResumoCapacidade.Text = $"{Capacidade.Text} participantes · {Preco.Text}";

        var local = LocalCombo.SelectedItem as Local;
        ResumoLocal.Text = local?.Nome ?? "Local a definir";
        AvisoLocal.Text = local == null
            ? "A capacidade do evento não pode ultrapassar a capacidade do local selecionado."
            : $"A capacidade do evento não pode ultrapassar as {local.Capacidade} pessoas permitidas pelo local.";

        var selecionados = _opcoesPalestrantes.Where(o => o.Selecionado).Select(o => o.Item).ToList();
        PalestrantesChips.ItemsSource = selecionados;
        ResumoPalestrantes.Text = selecionados.Count == 1
            ? "1 palestrante selecionado"
            : $"{selecionados.Count} palestrantes selecionados";

        int ocupadas = _evento == null ? 0 : _evento.Capacidade - _evento.VagasDisponiveis();
        int vagas = int.TryParse(Capacidade.Text, out int capacidade) ? Math.Max(capacidade - ocupadas, 0) : 0;
        ResumoVagas.Text = $"{vagas} vagas disponíveis. Este total é calculado a partir das inscrições confirmadas.";
    }

    /// <summary>Valida o preenchimento dos campos e monta o evento. Retorna null se faltar algo.</summary>
    private Evento? MontarEvento(StatusEvento status)
    {
        var erros = new List<string>();
        if (string.IsNullOrWhiteSpace(Titulo.Text)) erros.Add("título");
        if (string.IsNullOrWhiteSpace(Descricao.Text)) erros.Add("descrição");
        var inicio = LerData(DataInicio.Text);
        var fim = LerData(DataFim.Text);
        if (inicio == null) erros.Add("data de início (dd/mm/aaaa · hh:mm)");
        if (fim == null) erros.Add("data de término (dd/mm/aaaa · hh:mm)");
        if (!int.TryParse(Capacidade.Text, out int capacidade) || capacidade <= 0) erros.Add("capacidade maior que zero");
        var preco = LerPreco(Preco.Text);
        if (preco == null) erros.Add("preço (ex.: R$ 249,00 ou Gratuito)");
        var local = LocalCombo.SelectedItem as Local;
        if (local == null) erros.Add("local");
        var organizador = OrganizadorCombo.SelectedItem as OrganizadorItem;
        if (organizador == null) erros.Add("organizador");
        if (PalestrantesSelecionados().Count == 0) erros.Add("ao menos um palestrante");

        int sessoes = 0, carga = 0;
        if (EhWorkshop)
        {
            if (!string.IsNullOrWhiteSpace(CargaHoraria.Text) && !int.TryParse(CargaHoraria.Text, out carga))
                erros.Add("carga horária em horas");
        }
        else
        {
            if (string.IsNullOrWhiteSpace(Tema.Text)) erros.Add("tema");
            if (!int.TryParse(Sessoes.Text, out sessoes) || sessoes <= 0) erros.Add("número de sessões");
        }

        if (erros.Count > 0)
        {
            MessageBox.Show(Window.GetWindow(this)!, "Confira os campos: " + string.Join(", ", erros) + ".",
                "Campos obrigatórios", MessageBoxButton.OK, MessageBoxImage.Warning);
            return null;
        }

        // Mantém o mesmo objeto ao editar sem trocar a modalidade; caso contrário cria um novo do tipo escolhido.
        Evento evento = EhWorkshop
            ? _evento as Workshop ?? new Workshop { Id = _evento?.Id ?? 0 }
            : _evento as Conferencia ?? new Conferencia { Id = _evento?.Id ?? 0 };

        evento.Titulo = Titulo.Text.Trim();
        evento.Descricao = Descricao.Text.Trim();
        evento.DataInicio = inicio!.Value;
        evento.DataFim = fim!.Value;
        evento.Capacidade = capacidade;
        evento.PrecoInscricao = preco!.Value;
        evento.Status = status;
        evento.LocalId = local!.Id;
        evento.Local = local;
        evento.OrganizadorId = organizador!.Modelo.Id;
        evento.Organizador = organizador.Modelo;
        evento.Palestrantes = PalestrantesSelecionados();

        switch (evento)
        {
            case Conferencia c:
                c.Tema = Tema.Text.Trim();
                c.NumeroSessoes = sessoes;
                break;
            case Workshop w:
                w.CargaHoraria = carga;
                w.Material = Material.Text.Trim();
                break;
        }
        return evento;
    }

    private void Tipo_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (!_pronto) return;
        AplicarTipo();
        AtualizarResumo();
    }

    private void Campo_Changed(object sender, RoutedEventArgs e) => AtualizarResumo();
    private void Palestrante_Changed(object sender, RoutedEventArgs e) => AtualizarResumo();

    private void Rascunho_Click(object sender, RoutedEventArgs e)
    {
        if (MontarEvento(StatusEvento.Rascunho) is not { } evento) return;
        _eventos.Salvar(evento);
        RascunhoPill.Text = $"Rascunho salvo às {DateTime.Now:HH:mm}";
        RascunhoPill.Kind = PillKind.Neutral;
    }

    private void Publicar_Click(object sender, RoutedEventArgs e)
    {
        if (MontarEvento(StatusEvento.Publicado) is not { } evento) return;
        _eventos.Salvar(evento);
        MessageBox.Show(Window.GetWindow(this)!, "Evento salvo e publicado.", "elo",
            MessageBoxButton.OK, MessageBoxImage.Information);
        Navigator.Go(AppPage.EventoDetalhes, evento.Id);
    }

    private void Voltar_Click(object sender, RoutedEventArgs e) => Navigator.Go(AppPage.Eventos);
}
