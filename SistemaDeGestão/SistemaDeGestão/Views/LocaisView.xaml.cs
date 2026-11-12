using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SistemaDeGestão.Controller;
using SistemaDeGestão.Model;
using SistemaDeGestão.ViewModels;

namespace SistemaDeGestão.Views;

public partial class LocaisView : UserControl
{
    private readonly LocalController _locais = new();
    private List<LocalItem> _todos = [];

    private LocalItem? _editando;
    private readonly bool _pronto;

    public LocaisView()
    {
        InitializeComponent();
        _pronto = true;
        Recarregar();
        Lista.SelectedIndex = 0;
    }

    /// <summary>Busca os locais de novo e refaz a lista e o resumo.</summary>
    private void Recarregar(int? selecionarId = null)
    {
        _todos = _locais.Listar().Select(l => new LocalItem(l)).ToList();
        ResumoTotal.Text = _todos.Count == 1 ? "1 local cadastrado" : $"{_todos.Count} locais cadastrados";
        Atualizar(selecionarId);
    }

    private void Atualizar(int? selecionarId = null)
    {
        if (!_pronto) return;
        string termo = Busca.Text.Trim();
        Func<LocalItem, bool> faixa = FiltroCapacidade.SelectedIndex switch
        {
            1 => l => l.Capacidade <= 50,
            2 => l => l.Capacidade is > 50 and <= 200,
            3 => l => l.Capacidade > 200,
            _ => _ => true,
        };

        var lista = _todos.Where(l => faixa(l) &&
            (termo == "" ||
             l.Nome.Contains(termo, StringComparison.CurrentCultureIgnoreCase) ||
             l.EnderecoCompleto.Contains(termo, StringComparison.CurrentCultureIgnoreCase))).ToList();

        Lista.ItemsSource = lista;
        if (selecionarId != null) Lista.SelectedItem = lista.FirstOrDefault(l => l.Modelo.Id == selecionarId);
        Paginacao.Summary = lista.Count == 0
            ? "Nenhum local encontrado"
            : $"Mostrando 1–{lista.Count} de {_todos.Count} locais";
    }

    private void Preencher(LocalItem? l)
    {
        _editando = l;
        Erro.Visibility = Visibility.Collapsed;
        FormTitulo.Subtitle = l == null ? "Novo cadastro" : $"Editando {l.Nome}";
        CampoId.Text = l == null ? "Gerado automaticamente ao salvar" : $"{l.Id} · gerado automaticamente";
        CampoNome.Text = l?.Nome ?? "";
        CampoEndereco.Text = l?.EnderecoCompleto ?? "";
        CampoCapacidade.Text = l?.Capacidade.ToString() ?? "";

        // Destaque: a foto do Figma é do Centro de Convenções Aurora (primeiro local dos dados iniciais).
        Foto.Visibility = l?.Modelo.Id == 1 ? Visibility.Visible : Visibility.Collapsed;
        DestaqueNome.Text = l?.Nome ?? "Novo local";
        DestaqueProximo.Text = l?.ProximoEvento is string prox ? $"Próximo evento: {prox}." : "Nenhum evento agendado.";
        DestaqueOcupacao.Visibility = l?.LugaresDoEvento != null ? Visibility.Visible : Visibility.Collapsed;
        if (l?.LugaresDoEvento is int lugares)
            DestaqueOcupacao.Text = $"{lugares} de {l.Capacidade} lugares destinados ao evento";

        if (l == null) CampoNome.Focus();
    }

    private void Lista_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Lista.SelectedItem is LocalItem l) Preencher(l);
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

    private void SoNumeros(object sender, TextCompositionEventArgs e) => e.Handled = !e.Text.All(char.IsDigit);

    private void Salvar_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(CampoNome.Text) || string.IsNullOrWhiteSpace(CampoEndereco.Text) ||
            !int.TryParse(CampoCapacidade.Text, out int capacidade) || capacidade <= 0)
        {
            Erro.Text = "Preencha nome, endereço e uma capacidade maior que zero.";
            Erro.Visibility = Visibility.Visible;
            return;
        }

        var alvo = _editando?.Modelo ?? new Local();
        alvo.Nome = CampoNome.Text.Trim();
        alvo.Endereco = CampoEndereco.Text.Trim();
        alvo.Capacidade = capacidade;
        _locais.Salvar(alvo);
        Recarregar(alvo.Id);
    }

    private void Busca_Changed(object sender, TextChangedEventArgs e) => Atualizar();
    private void Filtro_Changed(object sender, SelectionChangedEventArgs e) => Atualizar();
}
