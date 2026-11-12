using SistemaDeGestão.Model;

namespace SistemaDeGestão.ViewModels
{
    /// <summary>Linha da lista de certificados e dados da prévia.</summary>
    public class CertificadoItem
    {
        public CertificadoItem(Certificado modelo) => Modelo = modelo;

        public Certificado Modelo { get; }

        public string Codigo => Modelo.Codigo;
        public string Destinatario => Modelo.Pessoa?.Nome ?? "";
        public string DestinatarioId => Modelo.Pessoa switch
        {
            Participante p => p.Codigo,
            Palestrante p => p.Codigo,
            _ => "",
        };
        public string Tipo => Modelo.Pessoa is Palestrante ? "Palestrante" : "Participante";
        public string Evento => Modelo.Evento?.Titulo ?? "";
        public string EventoPeriodo => Modelo.Evento is { } e ? Formato.PeriodoExtenso(e.DataInicio, e.DataFim) : "";

        /// <summary>Só o Workshop tem carga horária; na Conferência fica nula e é omitida.</summary>
        public string? CargaHoraria => Modelo.Evento is Workshop w ? $"{w.CargaHoraria} horas" : null;

        public DateTime? EmitidoEm => Modelo.DataEmissao;
        public bool Emitido => Modelo.Emitido;
        public string SituacaoTexto => Emitido ? "Emitido" : "Pronto para emitir";
        public string EmitidoTexto => EmitidoEm?.ToString("dd/MM/yyyy") ?? "";
        public string LinkValidacao => $"https://elo.example/validar/{Codigo}";

        public string Reconhecimento
        {
            get
            {
                string carga = CargaHoraria == null ? "" : $", com carga horária de {CargaHoraria}";
                return Tipo == "Palestrante"
                    ? $"compartilhou conhecimento como palestrante em {Evento}, realizado em {EventoPeriodo}{carga}."
                    : $"participou de {Evento}, realizado em {EventoPeriodo}{carga}.";
            }
        }
    }
}
