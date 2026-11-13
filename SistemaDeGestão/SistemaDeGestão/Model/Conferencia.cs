namespace SistemaDeGestão.Model
{
    public class Conferencia : Evento
    {
        public string Tema { get; set; } = string.Empty;
        public int NumeroSessoes { get; set; }
    }
}
