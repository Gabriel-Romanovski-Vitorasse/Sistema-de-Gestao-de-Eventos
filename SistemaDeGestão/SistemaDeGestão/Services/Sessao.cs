namespace SistemaDeGestão.Services;

public enum Perfil
{
    Organizador,
    Participante,
    Palestrante,
}

/// <summary>Quem está usando o sistema. O Id é o do Organizador ou o da Pessoa, conforme o perfil.</summary>
public record UsuarioLogado(int Id, string Nome, string Email, Perfil Perfil);

/// <summary>Usuário logado no momento e o que cada perfil pode acessar.</summary>
public static class Sessao
{
    public static UsuarioLogado? Usuario { get; private set; }

    public static bool EhOrganizador => Usuario?.Perfil == Perfil.Organizador;

    public static void Entrar(UsuarioLogado usuario) => Usuario = usuario;

    public static void Sair() => Usuario = null;

    /// <summary>Organizador acessa tudo; Participante vê eventos, as próprias inscrições e certificados;
    /// Palestrante vê eventos e os próprios certificados.</summary>
    public static bool PodeAcessar(AppPage pagina) => Usuario?.Perfil switch
    {
        Perfil.Organizador => true,
        Perfil.Participante => pagina is AppPage.Eventos or AppPage.EventoDetalhes or AppPage.Inscricoes or AppPage.Certificados,
        Perfil.Palestrante => pagina is AppPage.Eventos or AppPage.EventoDetalhes or AppPage.Certificados,
        _ => false,
    };
}
