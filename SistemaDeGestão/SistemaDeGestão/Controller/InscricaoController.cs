using SistemaDeGestão.Model;
using SistemaDeGestão.Model.Relatorios;

namespace SistemaDeGestão.Controller
{
    public class InscricaoController
    {
        public List<Inscricao> Listar()
        {
            throw new NotImplementedException("Implementação manual");
        }

        public Inscricao Inscrever(int participanteId, int eventoId)
        {
            throw new NotImplementedException("Implementação manual");
        }

        public void Pagar(int inscricaoId, string metodo)
        {
            throw new NotImplementedException("Implementação manual");
        }

        public void Cancelar(int inscricaoId)
        {
            throw new NotImplementedException("Implementação manual");
        }

        // Será SQL puro (manual)
        public IndicadoresInscricoes ObterIndicadores()
        {
            throw new NotImplementedException("Implementação manual");
        }

        // Será SQL puro (manual)
        public List<RelatorioFinanceiroItem> RelatorioFinanceiro()
        {
            throw new NotImplementedException("Implementação manual");
        }
    }
}
