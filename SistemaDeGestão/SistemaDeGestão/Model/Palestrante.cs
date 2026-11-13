namespace SistemaDeGestão.Model
{
    public class Palestrante : Pessoa, ICertificado
    {
        public string MiniBio { get; set; } = string.Empty;
        public string Especialidade { get; set; } = string.Empty;

        // N:N - um palestrante palestra em vários eventos
        public List<Evento> Eventos { get; set; } = new();

        // Código exibido nas telas (não é mapeado pelo EF: só tem get)
        public string Codigo => $"PAL-{Id:000}";

        public void Emitir()
        {
            // TODO: gerar o certificado do palestrante
        }
    }
}
