using System.Collections.Concurrent;
using CoreEngine.Application.Common.Interfaces;

namespace CoreEngine.Infrastructure.Services;

public class UserPresenceService : IUserPresenceService
{
    private readonly ConcurrentDictionary<string, DateTime> _onlineUsers = new();

    public Task SetOnlineAsync(string userId)
    {
        _onlineUsers.AddOrUpdate(userId, DateTime.UtcNow, (key, oldValue) => DateTime.UtcNow);
        return Task.CompletedTask;
    }

    public Task SetOfflineAsync(string userId)
    {
        _onlineUsers.TryRemove(userId, out _);
        return Task.CompletedTask;
    }

    public Task<List<string>> GetOnlineUsersAsync()
    {
        // Clean up stale connections (users who haven't sent a heartbeat in 5 minutes)
        var staleThreshold = DateTime.UtcNow.AddMinutes(-5);
        var staleUsers = _onlineUsers
            .Where(kvp => kvp.Value < staleThreshold)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var userId in staleUsers)
        {
            _onlineUsers.TryRemove(userId, out _);
        }

        return Task.FromResult(_onlineUsers.Keys.ToList());
    }

    public Task<bool> IsUserOnlineAsync(string userId)
    {
        return Task.FromResult(_onlineUsers.ContainsKey(userId));
    }
}
