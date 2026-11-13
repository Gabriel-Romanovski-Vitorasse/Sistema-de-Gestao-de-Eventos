using System.Windows;
using System.Windows.Controls;
using SistemaDeGestão.Controller;
using SistemaDeGestão.Controls;
using SistemaDeGestão.Model.Relatorios;
using SistemaDeGestão.Services;
using SistemaDeGestão.ViewModels;

namespace SistemaDeGestão.Views;

public partial class InscricoesView : UserControl
{
    private readonly InscricaoController _inscricoes = new();
    private readonly ParticipanteController _participantes = new();
    private readonly EventoController _eventos = new();

    private readonly List<ParticipanteOpcao> _opcoesParticipantes;
    private readonly List<EventoOpcao> _opcoesEventos;
    private List<InscricaoItem> _todas = [];

    private InscricaoItem? _selecionada; // null = nova inscrição em preenchimento
    private bool _preenchendo;
    private readonly bool _pronto;

    public InscricoesView()
    {
        InitializeComponent();
        // O participante só vê e cria as próprias inscrições.
        _opcoesParticipantes = _participantes.Listar()
            .Where(p => Sessao.EhOrganizador || p.Id == Sessao.Usuario!.Id)
            .Select(p => new ParticipanteOpcao(p))
            .ToList();
        _opcoesEventos = _eventos.Listar().Select(e => new EventoOpcao(e)).ToList();
        CampoParticipante.ItemsSource = _opcoesParticipantes;
        CampoEvento.ItemsSource = _opcoesEventos;
        _pronto = true;
        Recarregar();

        // Parâmetro: "nova" (nova inscrição) ou o Id do evento (nova inscrição já com o evento escolhido).
        if (Navigator.Parameter is int eventoId) NovaInscricao(eventoId);
        else if (Equals(Navigator.Parameter, "nova")) NovaInscricao(null);
        else Lista.SelectedIndex = 0;
    }

    /// <summary>Busca as inscrições e os indicadores de novo (após pagar, cancelar ou inscrever).</summary>
    private void Recarregar(int? selecionarId = null)
    {
        _todas = _inscricoes.Listar()
            .Where(i => Sessao.EhOrganizador || i.ParticipanteId == Sessao.Usuario!.Id)
            .Select(i => new InscricaoItem(i))
            .ToList();
        AtualizarIndicadores();
        Atualizar(selecionarId);
    }

    private void AtualizarIndicadores()
    {
        // Organizador: indicadores gerais (SQL). Participante: calculados só com as inscrições dele.
        var ind = Sessao.EhOrganizador
            ? _inscricoes.ObterIndicadores()
            : new IndicadoresInscricoes(
                _todas.Count(i => i.Status == "Confirmada"),
                _todas.Count(i => i.Status == "Pendente"),
                _todas.Count(i => i.Status == "Cancelada"),
                _todas.Where(i => i.Status == "Pendente").Sum(i => i.Valor));
        IndConfirmadas.Value = ind.Confirmadas.ToString("00");
        IndPendentes.Value = ind.Pendentes.ToString("00");
        IndPendentes.Caption = $"{ind.ValorEmAberto.ToString("C2", Formato.PtBr)} em aberto";
        IndCanceladas.Value = ind.Canceladas.ToString("00");
    }

    private void Atualizar(int? selecionarId = null)
    {
        if (!_pronto) return;
        string termo = Busca.Text.Trim();
        string status = FiltroStatus.SelectedIndex > 0 ? ((ComboBoxItem)FiltroStatus.SelectedItem).Content as string ?? "" : "";

        var lista = _todas.Where(i =>
            (status == "" || i.Status == status) &&
            (termo == "" ||
             i.Participante.Contains(termo, StringComparison.CurrentCultureIgnoreCase) ||
             i.Evento.Contains(termo, StringComparison.CurrentCultureIgnoreCase) ||
             i.Id.Contains(termo, StringComparison.CurrentCultureIgnoreCase))).ToList();

        Lista.ItemsSource = lista;
        if (selecionarId != null) Lista.SelectedItem = lista.FirstOrDefault(i => i.Modelo.Id == selecionarId);
        Paginacao.Summary = lista.Count == 0
            ? "Nenhuma inscrição encontrada"
            : $"Mostrando 1–{lista.Count} de {_todas.Count} inscrições";
    }

    private void Preencher(InscricaoItem i)
    {
        _selecionada = i;
        _preenchendo = true;

        DadosTitulo.Subtitle = $"{i.Id} · criada em {i.Data:dd/MM/yyyy}, às {i.Data:HH:mm}";
        DadosStatus.Text = i.Status == "Pendente" ? "Pendente de pagamento" : i.Status;
        DadosStatus.Kind = i.StatusKind;
        CampoParticipante.SelectedItem = _opcoesParticipantes.FirstOrDefault(p => p.Id == i.ParticipanteId);
        CampoEvento.SelectedItem = _opcoesEventos.FirstOrDefault(e => e.Id == i.EventoId);
        CampoData.Text = i.DataTexto;
        CampoStatus.Text = i.Status;
        var evento = _opcoesEventos.FirstOrDefault(e => e.Id == i.EventoId);
        DadosEvento.Text = evento == null ? "" : $"{evento.Periodo} · {evento.Local}";

        // Participante e evento são escolhidos só ao criar a inscrição; a forma de pagamento, enquanto pendente.
        bool pendente = i.Status == "Pendente";
        CampoParticipante.IsEnabled = CampoEvento.IsEnabled = false;
        CampoForma.IsEnabled = pendente;

        ValorTotal.Text = i.ValorTexto;
        BotaoPagar.Content = i.Status switch
        {
            "Pendente" => $"Pagar {i.ValorTexto}",
            "Confirmada" => "Pagamento confirmado",
            _ => "Inscrição cancelada",
        };
        BotaoPagar.IsEnabled = pendente;
        BotaoCancelar.IsEnabled = i.Status != "Cancelada";
        AvisoPagamento.Text = i.Status switch
        {
            "Pendente" => "Ao confirmar o pagamento, a inscrição passa para Confirmada e a vaga é garantida.",
            "Confirmada" => "Pagamento aprovado. A vaga está garantida para este participante.",
            _ => "Esta inscrição foi cancelada e a vaga voltou a ficar disponível.",
        };

        _preenchendo = false;
    }

    /// <summary>Abre o formulário para uma nova inscrição (gravada ao clicar em "Confirmar inscrição").</summary>
    private void NovaInscricao(int? eventoId)
    {
        Lista.SelectedItem = null;
        _selecionada = null;
        _preenchendo = true;

        DadosTitulo.Subtitle = "Nova inscrição";
        DadosStatus.Text = "Nova";
        DadosStatus.Kind = PillKind.Neutral;
        CampoParticipante.SelectedIndex = _opcoesParticipantes.Count > 0 ? 0 : -1;
        CampoEvento.SelectedItem = _opcoesEventos.FirstOrDefault(e => e.Id == eventoId) ?? _opcoesEventos.FirstOrDefault();
        CampoData.Text = DateTime.Today.ToString("dd/MM/yyyy");
        CampoStatus.Text = "Pendente";
        CampoParticipante.IsEnabled = Sessao.EhOrganizador;
        CampoEvento.IsEnabled = true;
        CampoForma.IsEnabled = false;
        BotaoCancelar.IsEnabled = false;
        AvisoPagamento.Text = "A inscrição começa como Pendente. Depois, confirme o pagamento para garantir a vaga.";

        _preenchendo = false;
        AtualizarNovaInscricao();
        CampoParticipante.Focus();
    }

    private void AtualizarNovaInscricao()
    {
        var evento = CampoEvento.SelectedItem as EventoOpcao;
        string valor = Formato.Moeda(evento?.Preco ?? 0);
        DadosEvento.Text = evento == null ? "" : $"{evento.Periodo} · {evento.Local}";
        ValorTotal.Text = valor;
        BotaoPagar.Content = "Confirmar inscrição";
        BotaoPagar.IsEnabled = evento != null && CampoParticipante.SelectedItem is ParticipanteOpcao;
    }

    private void Lista_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Lista.SelectedItem is InscricaoItem i) Preencher(i);
    }

    private void Participante_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (!_preenchendo && _selecionada == null) AtualizarNovaInscricao();
    }

    private void Evento_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (!_preenchendo && _selecionada == null) AtualizarNovaInscricao();
    }

    private void Nova_Click(object sender, RoutedEventArgs e)
    {
        Busca.Text = "";
        FiltroStatus.SelectedIndex = 0;
        NovaInscricao(null);
    }

    private void Pagar_Click(object sender, RoutedEventArgs e)
    {
        if (_selecionada == null)
        {
            if (CampoParticipante.SelectedItem is not ParticipanteOpcao p || CampoEvento.SelectedItem is not EventoOpcao ev) return;
            var nova = _inscricoes.Inscrever(p.Id, ev.Id);
            Recarregar(nova.Id);
            return;
        }

        if (_selecionada.Status != "Pendente") return;
        string metodo = (CampoForma.SelectedItem as ComboBoxItem)?.Content as string ?? "";
        _inscricoes.Pagar(_selecionada.Modelo.Id, metodo);
        Recarregar(_selecionada.Modelo.Id);
    }

    private void Cancelar_Click(object sender, RoutedEventArgs e)
    {
        if (_selecionada is not { } i || i.Status == "Cancelada") return;
        var resposta = MessageBox.Show(Window.GetWindow(this)!,
            $"Cancelar a inscrição {i.Id} de {i.Participante}? A vaga será liberada.",
            "Cancelar inscrição", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (resposta != MessageBoxResult.Yes) return;

        _inscricoes.Cancelar(i.Modelo.Id);
        Recarregar(i.Modelo.Id);
    }

    private void Busca_Changed(object sender, TextChangedEventArgs e) => Atualizar();
    private void Filtro_Changed(object sender, SelectionChangedEventArgs e) => Atualizar();
}
