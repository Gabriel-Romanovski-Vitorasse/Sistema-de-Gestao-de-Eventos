using SistemaDeGestão.Model;

namespace SistemaDeGestão.ViewModels
{
    /// <summary>Linha da lista de organizadores (Pessoa Física ou Jurídica).</summary>
    public class OrganizadorItem
    {
        public OrganizadorItem(Organizador modelo) => Modelo = modelo;

        public Organizador Modelo { get; }

        public string Id => Modelo.Codigo;
        public string Nome => Modelo.Nome;
        public string Email => Modelo.Email;
        public string Telefone => Modelo.Telefone;
        public bool Juridica => Modelo is PessoaJuridica;
        public string Cnpj => (Modelo as PessoaJuridica)?.Cnpj ?? "";
        public string RazaoSocial => (Modelo as PessoaJuridica)?.RazaoSocial ?? "";
        public string Cpf => (Modelo as PessoaFisica)?.Cpf ?? "";
        public int Vinculos => Modelo.Eventos.Count;

        public List<EventoStatus> Eventos =>
            Modelo.Eventos.Select(e => new EventoStatus(e.Titulo, Formato.StatusDoEvento(e))).ToList();

        public string Tipo => Juridica ? "Pessoa jurídica" : "Pessoa física";
        public string Rotulo => $"{Nome} · {Tipo}";
        public string VinculosTexto => Formato.Plural(Vinculos, "evento", "eventos");
    }
}
