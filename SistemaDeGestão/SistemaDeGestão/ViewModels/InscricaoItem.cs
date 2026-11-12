using SistemaDeGestão.Controls;
using SistemaDeGestão.Model;

namespace SistemaDeGestão.ViewModels
{
    /// <summary>Linha da lista de inscrições.</summary>
    public class InscricaoItem
    {
        public InscricaoItem(Inscricao modelo) => Modelo = modelo;

        public Inscricao Modelo { get; }

        public string Id => Modelo.Codigo;
        public int ParticipanteId => Modelo.ParticipanteId;
        public string Participante => Modelo.Participante?.Nome ?? "";
        public int EventoId => Modelo.EventoId;
        public string Evento => Modelo.Evento?.Titulo ?? "";
        public DateTime Data => Modelo.DataInscricao;
        public decimal Valor => Modelo.Pagamento?.Valor ?? Modelo.Evento?.PrecoInscricao ?? 0;
        public string Status => Modelo.Status.ToString();
        public string FormaPagamento => Modelo.Pagamento?.Metodo ?? "";

        public string DataTexto => Data.ToString("dd/MM/yyyy");
        public string ValorTexto => Formato.Moeda(Valor);
        public PillKind StatusKind => StatusKinds.Of(Status);
    }

    /// <summary>Inscrição exibida no cadastro do participante ("Inscrições vinculadas").</summary>
    public class InscricaoVinculo
    {
        public InscricaoVinculo(Inscricao modelo) => Modelo = modelo;

        public Inscricao Modelo { get; }

        public string Evento => Modelo.Evento?.Titulo ?? "";
        public string CodigoData => $"{Modelo.Codigo} · {Modelo.DataInscricao:dd/MM/yyyy}";
        public string Status => Modelo.Status.ToString();
        public PillKind StatusKind => StatusKinds.Of(Status);

        /// <summary>Inscrição confirmada em evento já encerrado.</summary>
        public bool Concluida => Modelo.Status == StatusInscricao.Confirmada
                                 && Modelo.Evento?.Status == StatusEvento.Encerrado;

        public string ConcluidaTexto =>
            Modelo.Participante?.Certificados.Any(c => c.EventoId == Modelo.EventoId && c.Emitido) == true
                ? "Concluída · certificado emitido"
                : "Concluída";
    }
}
