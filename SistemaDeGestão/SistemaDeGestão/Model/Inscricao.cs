namespace SistemaDeGestão.Model
{
    public class Inscricao : IPagamento
    {
        public int Id { get; set; }
        public DateTime DataInscricao { get; set; } = DateTime.Now;
        public StatusInscricao Status { get; set; } = StatusInscricao.Pendente;

        public int ParticipanteId { get; set; }
        public Participante Participante { get; set; } = null!;

        // Composição: a inscrição pertence a um evento
        public int EventoId { get; set; }
        public Evento Evento { get; set; } = null!;

        // 1:1 (composição) - pagamento da inscrição (nulo enquanto pendente)
        public Pagamento? Pagamento { get; set; }

        // Código exibido nas telas (não é mapeado pelo EF: só tem get)
        public string Codigo => $"INS-{Id:000}";

        public void Pagar()
        {
            if (Status == StatusInscricao.Cancelada)
                throw new InvalidOperationException("Inscrição cancelada não pode ser paga.");

            if (Status == StatusInscricao.Confirmada)
                throw new InvalidOperationException("Inscrição já foi paga.");

            Pagamento = new Pagamento
            {
                Valor = Evento.PrecoInscricao,
                DataPagamento = DateTime.Now
            };
            Status = StatusInscricao.Confirmada;
        }

        public void Cancelar()
        {
            Status = StatusInscricao.Cancelada;
        }
    }
}
