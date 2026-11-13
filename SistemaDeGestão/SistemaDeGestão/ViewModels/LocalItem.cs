using SistemaDeGestão.Model;

namespace SistemaDeGestão.ViewModels
{
    /// <summary>Linha da lista de locais.</summary>
    public class LocalItem
    {
        public LocalItem(Local modelo) => Modelo = modelo;

        public Local Modelo { get; }

        public string Id => Modelo.Codigo;
        public string Nome => Modelo.Nome;
        public string Endereco => Modelo.Endereco;
        public string EnderecoCompleto => Modelo.Endereco;
        public int Capacidade => Modelo.Capacidade;
        public int Vinculos => Modelo.Eventos.Count;

        /// <summary>Próximo evento agendado no local.</summary>
        private Evento? Proximo => Modelo.Eventos
            .Where(e => e.Status != StatusEvento.Encerrado && e.DataInicio >= DateTime.Today)
            .OrderBy(e => e.DataInicio)
            .FirstOrDefault();

        public string? ProximoEvento => Proximo is { } e ? $"{e.Titulo}, em {e.DataInicio:dd/MM}" : null;
        public int? LugaresDoEvento => Proximo?.Capacidade;

        public string CapacidadeTexto => $"{Capacidade} pessoas";
        public string VinculosTexto => Formato.Plural(Vinculos, "evento", "eventos");
    }
}
