using SistemaDeGestão.Model;

namespace SistemaDeGestão.ViewModels
{
    /// <summary>Linha da lista de palestrantes.</summary>
    public class PalestranteItem
    {
        public PalestranteItem(Palestrante modelo) => Modelo = modelo;

        public Palestrante Modelo { get; }

        public string Id => Modelo.Codigo;
        public string Nome => Modelo.Nome;
        public string Email => Modelo.Email;
        public string Cpf => Modelo.Cpf;
        public string Especialidade => Modelo.Especialidade;
        public string Biografia => Modelo.MiniBio;
        public int Eventos => Modelo.Eventos.Count;

        /// <summary>Último evento encerrado (dá direito a certificado).</summary>
        public Evento? UltimoEventoEncerrado => Modelo.Eventos
            .Where(e => e.Status == StatusEvento.Encerrado)
            .OrderByDescending(e => e.DataInicio)
            .FirstOrDefault();

        public string? EventoConcluido => UltimoEventoEncerrado?.Titulo;
        public string? DataPalestra => UltimoEventoEncerrado?.DataInicio.ToString("dd/MM/yyyy");

        /// <summary>Próximo evento ainda não encerrado.</summary>
        public string? EventoVinculado => Modelo.Eventos
            .Where(e => e.Status != StatusEvento.Encerrado)
            .OrderBy(e => e.DataInicio)
            .FirstOrDefault()?.Titulo;

        public string Iniciais => Formato.Iniciais(Nome);
        public string PrimeiroNome => Nome.Split(' ')[0];
    }

    /// <summary>Palestrante na lista de seleção do cadastro de evento.</summary>
    public class PalestranteSelecao
    {
        public PalestranteSelecao(Palestrante modelo, bool selecionado)
        {
            Item = new PalestranteItem(modelo);
            Selecionado = selecionado;
        }

        public PalestranteItem Item { get; }
        public bool Selecionado { get; set; }
    }
}
