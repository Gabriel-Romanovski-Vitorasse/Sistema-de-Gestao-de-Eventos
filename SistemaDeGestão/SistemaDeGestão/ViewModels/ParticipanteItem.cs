using SistemaDeGestão.Model;

namespace SistemaDeGestão.ViewModels
{
    /// <summary>Linha da lista de participantes.</summary>
    public class ParticipanteItem
    {
        public ParticipanteItem(Participante modelo) => Modelo = modelo;

        public Participante Modelo { get; }

        public string Id => Modelo.Codigo;
        public string Nome => Modelo.Nome;
        public string Email => Modelo.Email;
        public string Cpf => Modelo.Cpf;
        public string Instituicao => Modelo.Instituicao;
        public int Inscricoes => Modelo.Inscricoes.Count(i => i.Status != StatusInscricao.Cancelada);

        public string Iniciais => Formato.Iniciais(Nome);
        public string InscricoesTexto => Formato.Plural(Inscricoes, "evento", "eventos");
    }

    /// <summary>Opção de participante nas caixas de seleção.</summary>
    public class ParticipanteOpcao
    {
        public ParticipanteOpcao(Participante modelo) => Modelo = modelo;

        public Participante Modelo { get; }

        public int Id => Modelo.Id;
        public string Nome => Modelo.Nome;
        public string Rotulo => $"{Modelo.Nome} · {Modelo.Codigo}";
    }
}
