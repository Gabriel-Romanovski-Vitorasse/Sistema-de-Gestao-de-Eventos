namespace SistemaDeGestão.Model
{
    public abstract class Pessoa
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // 1:N - certificados recebidos pela pessoa (participante ou palestrante)
        public List<Certificado> Certificados { get; set; } = new();
    }
}
