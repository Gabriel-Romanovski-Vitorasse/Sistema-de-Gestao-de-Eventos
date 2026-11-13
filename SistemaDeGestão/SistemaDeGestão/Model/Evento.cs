namespace SistemaDeGestão.Model
{
    public abstract class Evento
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public int Capacidade { get; set; }
        public decimal PrecoInscricao { get; set; }
        public StatusEvento Status { get; set; } = StatusEvento.Rascunho;

        // N:1 - organizador do evento
        public int OrganizadorId { get; set; }
        public Organizador Organizador { get; set; } = null!;

        // N:1 (agregação) - local onde o evento acontece
        public int LocalId { get; set; }
        public Local Local { get; set; } = null!;

        // N:N - palestrantes do evento
        public List<Palestrante> Palestrantes { get; set; } = new();

        // 1:N (composição) - inscrições do evento
        public List<Inscricao> Inscricoes { get; set; } = new();

        // 1:N - certificados emitidos para o evento
        public List<Certificado> Certificados { get; set; } = new();

        // Propriedades calculadas (não são mapeadas pelo EF: só têm get)
        public string Codigo => $"EVT-{Id:000}";
        public bool Esgotado => VagasDisponiveis() == 0;

        public int VagasDisponiveis()
        {
            int ocupadas = Inscricoes.Count(i => i.Status != StatusInscricao.Cancelada);
            return Capacidade - ocupadas;
        }
    }
}
