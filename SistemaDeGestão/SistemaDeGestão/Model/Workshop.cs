namespace SistemaDeGestão.Model
{
    public class Workshop : Evento
    {
        public int CargaHoraria { get; set; }
        public string Material { get; set; } = string.Empty;
    }
}
