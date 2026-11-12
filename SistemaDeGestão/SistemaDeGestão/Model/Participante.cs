namespace SistemaDeGestão.Model
{
    public class Participante : Pessoa, ICertificado
    {
        public string Instituicao { get; set; } = string.Empty;

        // 1:N - um participante realiza várias inscrições
        public List<Inscricao> Inscricoes { get; set; } = new();

        // Código exibido nas telas (não é mapeado pelo EF: só tem get)
        public string Codigo => $"PAR-{Id:000}";

        public void Emitir()
        {
            // TODO: gerar o certificado do participante
        }
    }
}
