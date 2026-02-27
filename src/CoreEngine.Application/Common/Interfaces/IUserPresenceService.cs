namespace CoreEngine.Application.Common.Interfaces;

public interface IUserPresenceService
{
    Task SetOnlineAsync(string userId);
    Task SetOfflineAsync(string userId);
    Task<List<string>> GetOnlineUsersAsync();
    Task<bool> IsUserOnlineAsync(string userId);
}
