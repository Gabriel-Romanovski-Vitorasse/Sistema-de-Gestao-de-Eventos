namespace SistemaDeGestão.Model.Relatorios
{
    public record RelatorioFinanceiroItem(int EventoId, string Evento, int TotalPagamentos, decimal ValorArrecadado);
}
