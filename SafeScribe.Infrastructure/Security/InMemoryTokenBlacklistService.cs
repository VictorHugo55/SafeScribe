using System.Collections.Concurrent;
using SafeScribe.Application.Interfaces;

namespace SafeScribe.Infrastructure.Security;

public class InMemoryTokenBlacklistService : ITokenBlacklistService
{
    private readonly ConcurrentDictionary<string, DateTime> _blacklist = new();

    public void AddToken(string token)
    {
        _blacklist[token] = DateTime.UtcNow;
    }

    public bool IsTokenBlacklisted(string token)
    {
        return _blacklist.ContainsKey(token);
    }

}