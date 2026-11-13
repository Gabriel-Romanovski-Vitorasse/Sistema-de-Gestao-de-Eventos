namespace SistemaDeGestão.Model
{
    public class Certificado
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;

        // Nula enquanto o certificado não foi emitido
        public DateTime? DataEmissao { get; set; }

        // N:1 - quem recebe (Participante ou Palestrante)
        public int PessoaId { get; set; }
        public Pessoa Pessoa { get; set; } = null!;

        // N:1 - evento a que o certificado se refere
        public int EventoId { get; set; }
        public Evento Evento { get; set; } = null!;

        // Propriedade calculada (não é mapeada pelo EF: só tem get)
        public bool Emitido => DataEmissao != null;
    }
}
