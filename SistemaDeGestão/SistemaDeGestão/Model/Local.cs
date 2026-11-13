namespace SistemaDeGestão.Model
{
    public class Local
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public int Capacidade { get; set; }

        // 1:N (agregação) - um local sedia vários eventos
        public List<Evento> Eventos { get; set; } = new();

        // Código exibido nas telas (não é mapeado pelo EF: só tem get)
        public string Codigo => $"LOC-{Id:000}";
    }
}
