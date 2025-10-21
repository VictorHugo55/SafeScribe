namespace SafeScribe.Application.Interfaces;

public interface ITokenBlacklistService
{
    /// <summary>
    /// Adiciona o token à blacklist.
    /// </summary>
    void AddToken(string token);

    /// <summary>
    /// Verifica se o token está na blacklist.
    /// </summary>
    bool IsTokenBlacklisted(string token);

}