using System.Globalization;
using SistemaDeGestão.Controls;
using SistemaDeGestão.Model;

namespace SistemaDeGestão.ViewModels
{
    /// <summary>Formatação de textos exibidos nas telas (datas, valores, nomes e status).</summary>
    public static class Formato
    {
        public static readonly CultureInfo PtBr = new("pt-BR");

        public static string Moeda(decimal valor) => valor == 0 ? "Gratuito" : valor.ToString("C2", PtBr);

        /// <summary>Primeira letra do primeiro e do último nome.</summary>
        public static string Iniciais(string nome)
        {
            var partes = nome.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (partes.Length == 0) return "";
            if (partes.Length == 1) return partes[0][..1].ToUpperInvariant();
            return $"{char.ToUpperInvariant(partes[0][0])}{char.ToUpperInvariant(partes[^1][0])}";
        }

        public static string Plural(int quantidade, string singular, string plural) =>
            quantidade == 1 ? $"1 {singular}" : $"{quantidade} {plural}";

        /// <summary>Período curto, ex.: "26 nov. 2026" ou "24–25 nov. 2026".</summary>
        public static string PeriodoCurto(DateTime inicio, DateTime fim)
        {
            static string Mes(DateTime d) => d.ToString("MMM", PtBr).TrimEnd('.') + ".";
            if (inicio.Date == fim.Date) return $"{inicio:dd} {Mes(inicio)} {inicio:yyyy}";
            if (inicio.Month == fim.Month && inicio.Year == fim.Year) return $"{inicio:dd}–{fim:dd} {Mes(fim)} {fim:yyyy}";
            return $"{inicio:dd} {Mes(inicio)} – {fim:dd} {Mes(fim)} {fim:yyyy}";
        }

        /// <summary>Período numérico, ex.: "26/11/2026" ou "24–25/11/2026".</summary>
        public static string PeriodoNumerico(DateTime inicio, DateTime fim)
        {
            if (inicio.Date == fim.Date) return inicio.ToString("dd/MM/yyyy");
            if (inicio.Month == fim.Month && inicio.Year == fim.Year) return $"{inicio:dd}–{fim:dd/MM/yyyy}";
            return $"{inicio:dd/MM/yyyy} – {fim:dd/MM/yyyy}";
        }

        /// <summary>Período por extenso, ex.: "15 e 16 de outubro de 2026".</summary>
        public static string PeriodoExtenso(DateTime inicio, DateTime fim)
        {
            static string Mes(DateTime d) => d.ToString("MMMM", PtBr);
            if (inicio.Date == fim.Date) return $"{inicio.Day} de {Mes(inicio)} de {inicio.Year}";
            if (inicio.Month == fim.Month && inicio.Year == fim.Year)
            {
                string ligacao = (fim.Date - inicio.Date).Days == 1 ? "e" : "a";
                return $"{inicio.Day} {ligacao} {fim.Day} de {Mes(fim)} de {fim.Year}";
            }
            return $"{inicio.Day} de {Mes(inicio)} a {fim.Day} de {Mes(fim)} de {fim.Year}";
        }

        /// <summary>Status exibido do evento: "Esgotado" é calculado a partir das vagas.</summary>
        public static string StatusDoEvento(Evento e) =>
            e.Status == StatusEvento.Publicado && e.Esgotado ? "Esgotado" : e.Status.ToString();
    }

    /// <summary>Cor da etiqueta para cada status usado no sistema.</summary>
    public static class StatusKinds
    {
        public static PillKind Of(string status) => status switch
        {
            "Publicado" or "Confirmada" or "Emitido" => PillKind.Success,
            "Esgotado" or "Pendente" => PillKind.Warning,
            "Cancelada" => PillKind.Danger,
            _ => PillKind.Neutral,
        };
    }

    /// <summary>Evento vinculado com seu status (para listas e etiquetas).</summary>
    public record EventoStatus(string Evento, string Status)
    {
        public PillKind Kind => StatusKinds.Of(Status);
    }
}
