using System.Windows;
using System.Windows.Controls;
using SistemaDeGestão.Controller;
using SistemaDeGestão.Controls;
using SistemaDeGestão.Model;
using SistemaDeGestão.Services;
using SistemaDeGestão.ViewModels;

namespace SistemaDeGestão.Views;

public partial class EventoDetalhesView : UserControl
{
    private readonly EventoController _eventos = new();
    private readonly Evento? _evento;

    public EventoDetalhesView()
    {
        InitializeComponent();
        // O parâmetro da navegação é o Id do evento.
        _evento = Navigator.Parameter is int id ? _eventos.BuscarPorId(id) : null;
        if (_evento != null) Preencher(_evento);
        else Titulo.Text = "Evento não encontrado";
    }

    private void Preencher(Evento e)
    {
        var item = new EventoItem(e);
        Trilha.Text = $"← Todos os eventos  /  {e.Codigo}";
        Titulo.Text = e.Titulo;
        Chamada.Text = $"{item.Tipo} · {Formato.PeriodoExtenso(e.DataInicio, e.DataFim)}";

        Preco.Text = Formato.Moeda(e.PrecoInscricao);
        Vagas.Text = $"{e.VagasDisponiveis()} vagas disponíveis de {e.Capacidade}";
        Inicio.Text = $"Início: {e.DataInicio:dd/MM/yyyy} às {e.DataInicio:HH}h";
        Termino.Text = $"Término: {e.DataFim:dd/MM/yyyy} às {e.DataFim:HH}h";
        FormatoLocal.Text = $"Formato presencial · {e.Local?.Nome}";

        var organizador = new OrganizadorItem(e.Organizador);
        OrganizadorChip.Initials = Formato.Iniciais(organizador.Nome);
        OrganizadorChip.Title = organizador.Nome;
        OrganizadorChip.Subtitle = $"{organizador.Tipo} · {organizador.Id}";
        OrganizadorEmail.Text = organizador.Email;
        OrganizadorTelefone.Text = organizador.Telefone;

        PillTipo.Text = item.Tipo;
        PillStatus.Text = item.Status;
        PillStatus.Kind = item.StatusKind;

        Descricao.Text = e.Descricao;
        switch (e)
        {
            case Conferencia c:
                InfoRotulo1.Text = "TEMA";
                InfoValor1.Text = c.Tema;
                InfoRotulo2.Text = "NÚMERO DE SESSÕES";
                InfoValor2.Text = $"{c.NumeroSessoes:00} sessões";
                break;
            case Workshop w:
                InfoRotulo1.Text = "CARGA HORÁRIA";
                InfoValor1.Text = $"{w.CargaHoraria} horas";
                InfoRotulo2.Text = "MATERIAL";
                InfoValor2.Text = w.Material;
                break;
        }

        PalestrantesTitulo.Subtitle = e is Workshop
            ? "Palestrantes confirmados neste workshop"
            : "Palestrantes confirmados nesta conferência";
        PalestrantesLista.ItemsSource = e.Palestrantes.Select(p => new PalestranteItem(p)).ToList();
        PalestrantesBio.Text = string.Join(" ", e.Palestrantes.Select(p => p.MiniBio).Where(b => b != ""));

        LocalNome.Text = e.Local?.Nome ?? "";
        LocalEndereco.Text = e.Local?.Endereco ?? "";
        LocalDetalhe.Text = e.Local == null ? "" : $"{e.Local.Codigo} · Capacidade do local: {e.Local.Capacidade} pessoas";
    }

    /// <summary>Barra de ocupação: inscrições ativas em relação à capacidade.</summary>
    private void Barra_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_evento == null || _evento.Capacidade <= 0) return;
        double ocupacao = (double)(_evento.Capacidade - _evento.VagasDisponiveis()) / _evento.Capacidade;
        BarraOcupacao.Width = BarraTrilho.ActualWidth * Math.Clamp(ocupacao, 0, 1);
    }

    private void Voltar_Click(object sender, RoutedEventArgs e) => Navigator.Go(AppPage.Eventos);

    private void Editar_Click(object sender, RoutedEventArgs e)
    {
        if (_evento != null) Navigator.Go(AppPage.EventoCadastro, _evento.Id);
    }

    // Abre a tela de inscrições já com este evento escolhido para uma nova inscrição.
    private void Inscrever_Click(object sender, RoutedEventArgs e)
    {
        if (_evento != null) Navigator.Go(AppPage.Inscricoes, _evento.Id);
    }

    private void Contato_Click(object sender, RoutedEventArgs e)
    {
        if (_evento?.Organizador is not { } o) return;
        MessageBox.Show(Window.GetWindow(this)!, $"Contato: {o.Email} · {o.Telefone}",
            o.Nome, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void Localizacao_Click(object sender, RoutedEventArgs e)
    {
        if (_evento?.Local is not { } l) return;
        MessageBox.Show(Window.GetWindow(this)!, $"{l.Nome}\n{l.Endereco}",
            "Localização", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
