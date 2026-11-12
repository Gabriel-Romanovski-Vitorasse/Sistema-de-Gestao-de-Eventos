using System.Windows;
using System.Windows.Controls;
using SistemaDeGestão.Controller;
using SistemaDeGestão.Model;
using SistemaDeGestão.ViewModels;

namespace SistemaDeGestão.Views;

public partial class PalestrantesView : UserControl
{
    private const string TodasAreas = "Todas as áreas";

    private readonly PalestranteController _palestrantes = new();
    private readonly CertificadoController _certificados = new();
    private List<PalestranteItem> _todos = [];

    private PalestranteItem? _editando;
    private readonly bool _pronto;

    public PalestrantesView()
    {
        InitializeComponent();
        _pronto = true;
        Recarregar();
        Lista.SelectedIndex = 0;
    }

    /// <summary>Busca os palestrantes de novo e refaz filtros e resumo.</summary>
    private void Recarregar(int? selecionarId = null)
    {
        _todos = _palestrantes.Listar().Select(p => new PalestranteItem(p)).ToList();

        var areas = _todos.Select(p => p.Especialidade).Distinct().Order().ToList();
        string? filtroAtual = FiltroArea.SelectedItem as string;
        FiltroArea.ItemsSource = new[] { TodasAreas }.Concat(areas).ToList();
        FiltroArea.SelectedItem = filtroAtual != null && areas.Contains(filtroAtual) ? filtroAtual : TodasAreas;

        int emitidos = _todos.Sum(p => p.Modelo.Certificados.Count(c => c.Emitido));
        ResumoTotal.Text = Formato.Plural(_todos.Count, "palestrante", "palestrantes");
        ResumoEspecialidades.Text = Formato.Plural(areas.Count, "especialidade", "especialidades");
        ResumoCertificados.Text = Formato.Plural(emitidos, "certificado emitido", "certificados emitidos");
        Atualizar(selecionarId);
    }

    private void Atualizar(int? selecionarId = null)
    {
        if (!_pronto) return;
        string termo = Busca.Text.Trim();
        string area = FiltroArea.SelectedItem as string ?? TodasAreas;

        var lista = _todos.Where(p =>
            (area == TodasAreas || p.Especialidade == area) &&
            (termo == "" ||
             p.Nome.Contains(termo, StringComparison.CurrentCultureIgnoreCase) ||
             p.Especialidade.Contains(termo, StringComparison.CurrentCultureIgnoreCase))).ToList();

        Lista.ItemsSource = lista;
        if (selecionarId != null) Lista.SelectedItem = lista.FirstOrDefault(p => p.Modelo.Id == selecionarId);
        Paginacao.Summary = lista.Count == 0
            ? "Nenhum palestrante encontrado"
            : $"Mostrando 1–{lista.Count} de {_todos.Count} palestrantes";
    }

    private void Preencher(PalestranteItem? p)
    {
        _editando = p;
        Erro.Visibility = Visibility.Collapsed;

        FormTitulo.Subtitle = p == null ? "Novo cadastro" : $"Editando {p.Nome}";
        CampoId.Text = p == null ? "Gerado automaticamente ao salvar" : $"{p.Id} · gerado automaticamente";
        CampoNome.Text = p?.Nome ?? "";
        CampoCpf.Text = p?.Cpf ?? "";
        CampoEmail.Text = p?.Email ?? "";
        CampoEspecialidade.Text = p?.Especialidade ?? "";
        CampoBio.Text = p?.Biografia ?? "";

        AvisoVinculo.Visibility = p?.EventoVinculado != null ? Visibility.Visible : Visibility.Collapsed;
        if (p?.EventoVinculado != null)
            AvisoVinculo.Text = $"{p.PrimeiroNome} está vinculado(a) ao {p.EventoVinculado}. A minibiografia será exibida nos detalhes do evento.";

        AtualizarCertificado();
        if (p == null) CampoNome.Focus();
    }

    private void AtualizarCertificado()
    {
        var p = _editando;
        if (p?.UltimoEventoEncerrado is { } evento)
        {
            CertEvento.Text = evento.Titulo;
            CertDetalhe.Text = $"{p.Nome} · palestra realizada em {p.DataPalestra}";
            bool emitido = p.Modelo.Certificados.Any(c => c.EventoId == evento.Id && c.Emitido);
            EmitirBotao.IsEnabled = !emitido;
            EmitirBotao.Content = emitido ? "Certificado emitido" : "Emitir certificado";
            EmitirBotao.Visibility = Visibility.Visible;
        }
        else
        {
            CertEvento.Text = "Nenhum evento concluído";
            CertDetalhe.Text = p == null
                ? "Salve o cadastro para vincular o palestrante a um evento."
                : $"{p.Nome} ainda não tem palestras concluídas.";
            EmitirBotao.Visibility = Visibility.Collapsed;
        }
    }

    private void Lista_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Lista.SelectedItem is PalestranteItem p) Preencher(p);
    }

    private void Novo_Click(object sender, RoutedEventArgs e)
    {
        Lista.SelectedItem = null;
        Preencher(null);
    }

    private void Cancelar_Click(object sender, RoutedEventArgs e)
    {
        if (_editando != null) Preencher(_editando);
        else Lista.SelectedIndex = 0;
    }

    private void Salvar_Click(object sender, RoutedEventArgs e)
    {
        if (new[] { CampoNome, CampoCpf, CampoEmail, CampoEspecialidade, CampoBio }.Any(t => string.IsNullOrWhiteSpace(t.Text)))
        {
            Erro.Text = "Preencha todos os campos obrigatórios (*).";
            Erro.Visibility = Visibility.Visible;
            return;
        }

        var alvo = _editando?.Modelo ?? new Palestrante();
        alvo.Nome = CampoNome.Text.Trim();
        alvo.Cpf = CampoCpf.Text.Trim();
        alvo.Email = CampoEmail.Text.Trim();
        alvo.Especialidade = CampoEspecialidade.Text.Trim();
        alvo.MiniBio = CampoBio.Text.Trim();
        _palestrantes.Salvar(alvo);
        Recarregar(alvo.Id);
    }

    private void Emitir_Click(object sender, RoutedEventArgs e)
    {
        if (_editando?.UltimoEventoEncerrado is not { } evento) return;
        _certificados.Emitir(_editando.Modelo.Id, evento.Id);
        Recarregar(_editando.Modelo.Id);
    }

    private void Busca_Changed(object sender, TextChangedEventArgs e) => Atualizar();
    private void Filtro_Changed(object sender, SelectionChangedEventArgs e) => Atualizar();
}
