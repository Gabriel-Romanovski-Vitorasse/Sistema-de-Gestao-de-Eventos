namespace SistemaDeGestão.Model
{
    public class PessoaJuridica : Organizador
    {
        public string Cnpj { get; set; } = string.Empty;
        public string RazaoSocial { get; set; } = string.Empty;
    }
}
