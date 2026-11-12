using SistemaDeGestão.Controls;
using SistemaDeGestão.Model;

namespace SistemaDeGestão.ViewModels
{
    /// <summary>Linha da tabela de eventos.</summary>
    public class EventoItem
    {
        public EventoItem(Evento modelo) => Modelo = modelo;

        public Evento Modelo { get; }

        public string Id => Modelo.Codigo;
        public string Titulo => Modelo.Titulo;
        public string Subtitulo => $"{Modelo.Codigo} · {Modelo.Local?.Nome}";
        public string Tipo => Modelo is Workshop ? "Workshop" : "Conferência";
        public DateTime Inicio => Modelo.DataInicio;
        public string Data => Formato.PeriodoCurto(Modelo.DataInicio, Modelo.DataFim);
        public string Vagas => $"{Modelo.VagasDisponiveis()} de {Modelo.Capacidade}";
        public string Inscricao => Formato.Moeda(Modelo.PrecoInscricao);
        public string Status => Formato.StatusDoEvento(Modelo);
        public PillKind StatusKind => StatusKinds.Of(Status);
    }

    /// <summary>Opção de evento nas caixas de seleção.</summary>
    public class EventoOpcao
    {
        public EventoOpcao(Evento modelo) => Modelo = modelo;

        public Evento Modelo { get; }

        public int Id => Modelo.Id;
        public string NomeCurto => Modelo.Titulo;
        public decimal Preco => Modelo.PrecoInscricao;
        public string Periodo => Formato.PeriodoNumerico(Modelo.DataInicio, Modelo.DataFim);
        public string Local => Modelo.Local?.Nome ?? "";
        public string Rotulo => $"{Modelo.Titulo} · {Modelo.Codigo}";
    }
}
