namespace SistemaDeGestão.Services;

public enum AppPage
{
    Eventos,
    EventoDetalhes,
    EventoCadastro,
    Participantes,
    Palestrantes,
    Organizadores,
    Locais,
    Inscricoes,
    Certificados,
}

/// <summary>Navegação simples entre as telas do app.</summary>
public static class Navigator
{
    public static event Action<AppPage>? Navigated;

    public static AppPage Current { get; private set; } = AppPage.Eventos;

    /// <summary>Parâmetro opcional da última navegação (ex.: "nova" para abrir um formulário vazio).</summary>
    public static object? Parameter { get; private set; }

    public static void Go(AppPage page, object? parameter = null)
    {
        Current = page;
        Parameter = parameter;
        Navigated?.Invoke(page);
    }

    /// <summary>Seção do menu principal a que cada tela pertence.</summary>
    public static AppPage SectionOf(AppPage page) => page switch
    {
        AppPage.EventoDetalhes or AppPage.EventoCadastro => AppPage.Eventos,
        _ => page,
    };

    public static string SectionName(AppPage section) => section switch
    {
        AppPage.Eventos => "Eventos",
        AppPage.Participantes => "Participantes",
        AppPage.Palestrantes => "Palestrantes",
        AppPage.Organizadores => "Organizadores",
        AppPage.Locais => "Locais",
        AppPage.Inscricoes => "Inscrições",
        AppPage.Certificados => "Certificados",
        _ => "",
    };
}
