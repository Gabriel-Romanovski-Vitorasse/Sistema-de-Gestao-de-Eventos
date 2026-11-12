namespace SistemaDeGestão.Model
{
    public abstract class Organizador
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;

        // 1:N - um organizador organiza vários eventos
        public List<Evento> Eventos { get; set; } = new();

        // Código exibido nas telas (não é mapeado pelo EF: só tem get)
        public string Codigo => $"ORG-{Id:000}";
    }
}
