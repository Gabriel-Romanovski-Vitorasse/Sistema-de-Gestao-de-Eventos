using System.Windows;
using System.Windows.Controls;
using SistemaDeGestão.Controller;
using SistemaDeGestão.Model;
using SistemaDeGestão.ViewModels;

namespace SistemaDeGestão.Views;

public partial class ParticipantesView : UserControl
{
    private const string TodasInstituicoes = "Todas as instituições";

    private readonly ParticipanteController _participantes = new();
    private List<ParticipanteItem> _todos = [];

    private ParticipanteItem? _editando;
    private readonly bool _pronto;

    public ParticipantesView()
    {
        InitializeComponent();
        _pronto = true;
        Recarregar();
        Lista.SelectedIndex = 0;
    }

    /// <summary>Busca os participantes de novo e refaz filtros e resumo.</summary>
    private void Recarregar(int? selecionarId = null)
    {
        _todos = _participantes.Listar().Select(p => new ParticipanteItem(p)).ToList();

        var instituicoes = _todos.Select(p => p.Instituicao).Distinct().Order().ToList();
        string? filtroAtual = FiltroInstituicao.SelectedItem as string;
        FiltroInstituicao.ItemsSource = new[] { TodasInstituicoes }.Concat(instituicoes).ToList();
        FiltroInstituicao.SelectedItem = filtroAtual != null && instituicoes.Contains(filtroAtual) ? filtroAtual : TodasInstituicoes;

        ResumoTotal.Text = Formato.Plural(_todos.Count, "participante", "participantes");
        ResumoInstituicoes.Text = Formato.Plural(instituicoes.Count, "instituição", "instituições");
        Atualizar(selecionarId);
    }

    private void Atualizar(int? selecionarId = null)
    {
        if (!_pronto) return;
        string termo = Busca.Text.Trim();
        string inst = FiltroInstituicao.SelectedItem as string ?? TodasInstituicoes;

        var lista = _todos.Where(p =>
            (inst == TodasInstituicoes || p.Instituicao == inst) &&
            (termo == "" ||
             p.Nome.Contains(termo, StringComparison.CurrentCultureIgnoreCase) ||
             p.Email.Contains(termo, StringComparison.CurrentCultureIgnoreCase) ||
             p.Cpf.Contains(termo))).ToList();

        Lista.ItemsSource = lista;
        if (selecionarId != null) Lista.SelectedItem = lista.FirstOrDefault(p => p.Modelo.Id == selecionarId);
        Paginacao.Summary = lista.Count == 0
            ? "Nenhum participante encontrado"
            : $"Mostrando 1–{lista.Count} de {_todos.Count} participantes";
    }

    private void Preencher(ParticipanteItem? p)
    {
        _editando = p;
        Erro.Visibility = Visibility.Collapsed;
        FormTitulo.Subtitle = p == null ? "Novo cadastro" : $"Editando {p.Nome}";
        CampoId.Text = p == null ? "Gerado automaticamente ao salvar" : $"{p.Id} · gerado automaticamente";
        CampoNome.Text = p?.Nome ?? "";
        CampoCpf.Text = p?.Cpf ?? "";
        CampoEmail.Text = p?.Email ?? "";
        CampoInstituicao.Text = p?.Instituicao ?? "";

        var vinculos = p?.Modelo.Inscricoes
            .OrderByDescending(i => i.DataInscricao)
            .Select(i => new InscricaoVinculo(i))
            .ToList() ?? [];
        InscricoesLista.ItemsSource = vinculos;
        InscricoesLista.Visibility = vinculos.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        InscricoesResumo.Visibility = vinculos.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
        InscricoesResumo.Text = "Nenhuma inscrição ainda.";

        if (p == null) CampoNome.Focus();
    }

    private void Lista_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Lista.SelectedItem is ParticipanteItem p) Preencher(p);
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
        if (new[] { CampoNome, CampoCpf, CampoEmail, CampoInstituicao }.Any(t => string.IsNullOrWhiteSpace(t.Text)))
        {
            Erro.Text = "Preencha todos os campos obrigatórios (*).";
            Erro.Visibility = Visibility.Visible;
            return;
        }

        var alvo = _editando?.Modelo ?? new Participante();
        alvo.Nome = CampoNome.Text.Trim();
        alvo.Cpf = CampoCpf.Text.Trim();
        alvo.Email = CampoEmail.Text.Trim();
        alvo.Instituicao = CampoInstituicao.Text.Trim();
        _participantes.Salvar(alvo);
        Recarregar(alvo.Id);
    }

    private void Busca_Changed(object sender, TextChangedEventArgs e) => Atualizar();
    private void Filtro_Changed(object sender, SelectionChangedEventArgs e) => Atualizar();
}
