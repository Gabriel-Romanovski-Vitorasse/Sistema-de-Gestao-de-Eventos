using System.Security.Cryptography;

namespace SistemaDeGestão.Services;

/// <summary>Gera e confere o hash das senhas (PBKDF2 com SHA-256 e sal aleatório).</summary>
public static class Senha
{
    private const int TamanhoSal = 16;
    private const int TamanhoHash = 32;
    private const int Iteracoes = 100_000;

    /// <summary>Formato salvo no banco: "sal:hash", ambos em Base64.</summary>
    public static string GerarHash(string senha)
    {
        byte[] sal = RandomNumberGenerator.GetBytes(TamanhoSal);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(senha, sal, Iteracoes, HashAlgorithmName.SHA256, TamanhoHash);
        return $"{Convert.ToBase64String(sal)}:{Convert.ToBase64String(hash)}";
    }

    public static bool Conferir(string senha, string hashSalvo)
    {
        var partes = hashSalvo.Split(':');
        if (partes.Length != 2) return false;

        byte[] sal = Convert.FromBase64String(partes[0]);
        byte[] esperado = Convert.FromBase64String(partes[1]);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(senha, sal, Iteracoes, HashAlgorithmName.SHA256, esperado.Length);
        return CryptographicOperations.FixedTimeEquals(hash, esperado);
    }
}
