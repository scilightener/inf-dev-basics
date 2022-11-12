using Microsoft.Extensions.Caching.Memory;

namespace HttpServer.ServerLogic.SessionLogic;

public class SessionManager
{
    private readonly MemoryCache _cache = new(new MemoryCacheOptions());

    public void CreateSession(int accountId, string email, DateTime created)
    {
        var session = new Session(Guid.NewGuid(), accountId, email, created);
        var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(2));
        _cache.Set(session.Id, session, cacheOptions);
    }

    public bool CheckSession(Guid id) => _cache.TryGetValue(id, out _);

    public Session? GetSessionInfo(Guid id) => CheckSession(id) ? _cache.Get(id) as Session : null;
}