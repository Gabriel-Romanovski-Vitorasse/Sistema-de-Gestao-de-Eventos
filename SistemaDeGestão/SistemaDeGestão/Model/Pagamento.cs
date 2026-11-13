namespace SistemaDeGestão.Model
{
    public class Pagamento
    {
        public int Id { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataPagamento { get; set; } = DateTime.Now;
        public string Metodo { get; set; } = string.Empty;

        // 1:1 (composição) - o pagamento pertence a uma única inscrição
        public int InscricaoId { get; set; }
        public Inscricao Inscricao { get; set; } = null!;
    }
}
