# 📡 SignalR Real-time Module

Real-time notifications and user presence tracking via WebSocket connections.

---

## 📋 Overview

The SignalR module provides real-time bidirectional communication between server and clients. It powers live notifications, user presence indicators (online/offline), and real-time dashboard updates.

---

## ✨ Key Features

- ✅ **Real-time notifications** - Instant delivery to connected users
- ✅ **Presence tracking** - See who's online
- ✅ **Auto-reconnect** - Handles connection drops gracefully
- ✅ **Tenant-scoped** - Users only see their tenant's data
- ✅ **Typed hubs** - Strongly-typed client methods
- ✅ **Connection mapping** - Track user connections
- ✅ **Broadcast** - Send to all users or specific groups
- ✅ **Performance optimized** - Efficient message packing

---

## 🗄️ SignalR Hubs

### NotificationHub

**File:** `src/CoreEngine.API/Hubs/NotificationHub.cs`

```csharp
[Authorize]
public class NotificationHub : Hub<INotificationClient>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ITenantContext _tenantContext;
    private readonly IConnectionManager _connectionManager;

    public override async Task OnConnectedAsync()
    {
        var userId = _currentUserService.UserId;
        var tenantId = _tenantContext.TenantId;

        // Add to tenant group
        await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant_{tenantId}");

        // Track connection
        await _connectionManager.AddConnectionAsync(userId, Context.ConnectionId);

        // Notify others user is online
        await Clients.Group($"tenant_{tenantId}")
            .UserPresenceChanged(userId, true);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = _currentUserService.UserId;
        var tenantId = _tenantContext.TenantId;

        // Remove connection
        await _connectionManager.RemoveConnectionAsync(userId, Context.ConnectionId);

        // Check if user still has other connections
        var hasOtherConnections = await _connectionManager
            .HasConnectionsAsync(userId);

        if (!hasOtherConnections)
        {
            // Notify others user is offline
            await Clients.Group($"tenant_{tenantId}")
                .UserPresenceChanged(userId, false);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task MarkNotificationAsRead(Guid notificationId)
    {
        // Client can call this method to mark notification as read
        // Server will broadcast to all user's connections
        var userId = _currentUserService.UserId;

        await Clients.User(userId.ToString())
            .NotificationMarkedAsRead(notificationId);
    }
}
```

### INotificationClient (Typed Interface)

**File:** `src/CoreEngine.API/Hubs/INotificationClient.cs`

```csharp
public interface INotificationClient
{
    Task ReceiveNotification(NotificationDto notification);
    Task NotificationMarkedAsRead(Guid notificationId);
    Task UserPresenceChanged(Guid userId, bool isOnline);
    Task DashboardUpdated(DashboardStatsDto stats);
}
```

### PresenceHub

**File:** `src/CoreEngine.API/Hubs/PresenceHub.cs`

```csharp
[Authorize]
public class PresenceHub : Hub
{
    private readonly IPresenceTracker _presenceTracker;

    public override async Task OnConnectedAsync()
    {
        var userId = _currentUserService.UserId;

        await _presenceTracker.UserConnected(userId, Context.ConnectionId);

        // Send current online users to newly connected user
        var onlineUsers = await _presenceTracker.GetOnlineUsers();
        await Clients.Caller.SendAsync("OnlineUsers", onlineUsers);

        // Notify others
        await Clients.Others.SendAsync("UserOnline", userId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = _currentUserService.UserId;

        var isOffline = await _presenceTracker.UserDisconnected(userId, Context.ConnectionId);

        if (isOffline)
        {
            await Clients.Others.SendAsync("UserOffline", userId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}
```

---

## 🔌 Connection Manager

**File:** `src/CoreEngine.Application/Services/ConnectionManager.cs`

```csharp
public class ConnectionManager : IConnectionManager
{
    // In-memory dictionary (use Redis for multi-server)
    private readonly ConcurrentDictionary<Guid, HashSet<string>> _connections = new();

    public Task AddConnectionAsync(Guid userId, string connectionId)
    {
        _connections.AddOrUpdate(userId,
            new HashSet<string> { connectionId },
            (key, connections) =>
            {
                lock (connections)
                {
                    connections.Add(connectionId);
                }
                return connections;
            });

        return Task.CompletedTask;
    }

    public Task RemoveConnectionAsync(Guid userId, string connectionId)
    {
        if (_connections.TryGetValue(userId, out var connections))
        {
            lock (connections)
            {
                connections.Remove(connectionId);

                if (connections.Count == 0)
                {
                    _connections.TryRemove(userId, out _);
                }
            }
        }

        return Task.CompletedTask;
    }

    public Task<bool> HasConnectionsAsync(Guid userId)
    {
        return Task.FromResult(_connections.ContainsKey(userId));
    }

    public Task<IEnumerable<Guid>> GetOnlineUsersAsync()
    {
        return Task.FromResult<IEnumerable<Guid>>(_connections.Keys.ToList());
    }
}
```

---

## 🌐 Backend Integration

### Sending Real-time Notifications

**File:** `src/CoreEngine.Application/Notifications/SendNotificationCommand.cs`

```csharp
public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand>
{
    private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

    public async Task Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        // Save to database
        var notification = new Notification
        {
            UserId = request.UserId,
            Title = request.Title,
            Message = request.Message,
            Type = request.Type
        };

        await _context.Notifications.AddAsync(notification, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // Send real-time via SignalR
        var notificationDto = _mapper.Map<NotificationDto>(notification);

        await _hubContext.Clients
            .User(request.UserId.ToString())
            .ReceiveNotification(notificationDto);
    }
}
```

### Broadcasting Dashboard Updates

```csharp
public class DashboardService
{
    private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

    public async Task BroadcastDashboardUpdate()
    {
        var stats = await CalculateDashboardStats();
        var tenantId = _tenantContext.TenantId;

        await _hubContext.Clients
            .Group($"tenant_{tenantId}")
            .DashboardUpdated(stats);
    }
}
```

---

## 🎨 Frontend Integration

### SignalR Context

**File:** `frontend/src/contexts/SignalRContext.tsx`

```typescript
import { createContext, useContext, useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';

interface SignalRContextType {
  connection: signalR.HubConnection | null;
  isConnected: boolean;
  onlineUsers: string[];
}

const SignalRContext = createContext<SignalRContextType>({
  connection: null,
  isConnected: false,
  onlineUsers: [],
});

export const SignalRProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
  const [isConnected, setIsConnected] = useState(false);
  const [onlineUsers, setOnlineUsers] = useState<string[]>([]);

  useEffect(() => {
    const token = localStorage.getItem('accessToken');
    if (!token) return;

    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl('/hubs/notifications', {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: (retryContext) => {
          // Exponential backoff: 0s, 2s, 10s, 30s
          const delays = [0, 2000, 10000, 30000];
          return delays[Math.min(retryContext.previousRetryCount, delays.length - 1)];
        },
      })
      .configureLogging(signalR.LogLevel.Information)
      .build();

    // Event handlers
    newConnection.on('ReceiveNotification', (notification) => {
      console.log('New notification:', notification);
      // Show toast notification
      message.info(notification.message);
      // Update notification badge count
      queryClient.invalidateQueries(['notifications', 'unread-count']);
    });

    newConnection.on('UserPresenceChanged', (userId: string, isOnline: boolean) => {
      setOnlineUsers((prev) =>
        isOnline ? [...prev, userId] : prev.filter((id) => id !== userId)
      );
    });

    newConnection.on('DashboardUpdated', (stats) => {
      // Update dashboard data in React Query cache
      queryClient.setQueryData(['dashboard', 'stats'], stats);
    });

    // Connection lifecycle
    newConnection.onreconnecting(() => {
      console.log('SignalR reconnecting...');
      setIsConnected(false);
    });

    newConnection.onreconnected(() => {
      console.log('SignalR reconnected');
      setIsConnected(true);
    });

    newConnection.onclose(() => {
      console.log('SignalR connection closed');
      setIsConnected(false);
    });

    // Start connection
    newConnection
      .start()
      .then(() => {
        console.log('SignalR connected');
        setIsConnected(true);
        setConnection(newConnection);
      })
      .catch((err) => console.error('SignalR connection error:', err));

    // Cleanup
    return () => {
      newConnection.stop();
    };
  }, []);

  return (
    <SignalRContext.Provider value={{ connection, isConnected, onlineUsers }}>
      {children}
    </SignalRContext.Provider>
  );
};

export const useSignalR = () => useContext(SignalRContext);
```

### Using SignalR in Components

**Example: Online Status Indicator**

```typescript
import { useSignalR } from '@/contexts/SignalRContext';

const UserAvatar: React.FC<{ userId: string }> = ({ userId }) => {
  const { onlineUsers } = useSignalR();
  const isOnline = onlineUsers.includes(userId);

  return (
    <Badge dot color={isOnline ? 'green' : 'gray'}>
      <Avatar src={user.profilePhotoUrl} />
    </Badge>
  );
};
```

**Example: Connection Status Indicator**

```typescript
const ConnectionStatus: React.FC = () => {
  const { isConnected } = useSignalR();

  return (
    <div>
      {isConnected ? (
        <Tag color="green" icon={<CheckCircleOutlined />}>
          Connected
        </Tag>
      ) : (
        <Tag color="red" icon={<CloseCircleOutlined />}>
          Disconnected
        </Tag>
      )}
    </div>
  );
};
```

---

## ⚙️ Configuration

### Backend Configuration

**File:** `src/CoreEngine.API/Program.cs`

```csharp
// Add SignalR
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
});

// Add connection manager
builder.Services.AddSingleton<IConnectionManager, ConnectionManager>();
builder.Services.AddSingleton<IPresenceTracker, PresenceTracker>();

// Map hubs
app.MapHub<NotificationHub>("/hubs/notifications");
app.MapHub<PresenceHub>("/hubs/presence");
```

### Frontend Configuration

**File:** `frontend/src/main.tsx`

```typescript
import { SignalRProvider } from './contexts/SignalRContext';

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <SignalRProvider>
      <App />
    </SignalRProvider>
  </React.StrictMode>
);
```

---

## 🔐 Security

### Authentication

- SignalR connections require valid JWT token
- Token passed via `accessTokenFactory` in frontend
- Hub methods decorated with `[Authorize]`

### Tenant Isolation

```csharp
// Users automatically added to tenant group
await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant_{tenantId}");

// Broadcast only to tenant
await Clients.Group($"tenant_{tenantId}").ReceiveNotification(notification);
```

### Connection Validation

```csharp
public override async Task OnConnectedAsync()
{
    var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userId))
    {
        Context.Abort();
        return;
    }

    await base.OnConnectedAsync();
}
```

---

## 📊 Use Cases

### 1. Real-time Notifications

**Scenario:** User receives approval task assignment

```
Backend: Workflow task created
   ↓
Backend: Send SignalR notification
   ↓
Frontend: Toast notification appears
   ↓
Frontend: Notification badge count updates
   ↓
User: Clicks notification, navigates to task
```

### 2. Presence Tracking

**Scenario:** Show online users in team collaboration

```
User A connects
   ↓
SignalR: Broadcast "User A is online"
   ↓
User B's UI: Green dot appears next to User A's avatar
   ↓
User A disconnects (closes browser)
   ↓
SignalR: Broadcast "User A is offline"
   ↓
User B's UI: Gray dot appears next to User A's avatar
```

### 3. Dashboard Live Updates

**Scenario:** Background job completes, dashboard updates

```
Hangfire Job: Completes data import
   ↓
Backend: Recalculate dashboard stats
   ↓
SignalR: Broadcast updated stats to tenant group
   ↓
Frontend: Dashboard cards update without refresh
```

---

## 🧪 Testing SignalR

### Test Connection

```typescript
// In browser console
const connection = new signalR.HubConnectionBuilder()
  .withUrl('https://localhost:5001/hubs/notifications', {
    accessTokenFactory: () => 'YOUR_JWT_TOKEN',
  })
  .build();

connection.start().then(() => console.log('Connected'));

connection.on('ReceiveNotification', (notification) => {
  console.log('Received:', notification);
});
```

### Test Backend Broadcasting

```csharp
// In a controller or service
await _hubContext.Clients.All.ReceiveNotification(new NotificationDto
{
    Id = Guid.NewGuid(),
    Title = "Test Notification",
    Message = "This is a test",
    Type = "Info",
    CreatedAt = DateTime.UtcNow
});
```

---

## 🚀 Performance Optimization

### 1. Use Redis Backplane (Multi-Server)

**File:** `Program.cs`

```csharp
builder.Services.AddSignalR()
    .AddStackExchangeRedis("localhost:6379", options =>
    {
        options.Configuration.ChannelPrefix = "CoreEngine";
    });
```

### 2. Message Packing

```csharp
builder.Services.AddSignalR()
    .AddMessagePackProtocol();
```

**Frontend:**
```bash
npm install @microsoft/signalr-protocol-msgpack
```

```typescript
import { MessagePackHubProtocol } from '@microsoft/signalr-protocol-msgpack';

const connection = new signalR.HubConnectionBuilder()
  .withUrl('/hubs/notifications')
  .withHubProtocol(new MessagePackHubProtocol())
  .build();
```

### 3. Connection Pooling

Limit concurrent connections per user:

```csharp
public class ConnectionManager
{
    private const int MAX_CONNECTIONS_PER_USER = 5;

    public async Task<bool> AddConnectionAsync(Guid userId, string connectionId)
    {
        var connections = _connections.GetOrAdd(userId, new HashSet<string>());

        if (connections.Count >= MAX_CONNECTIONS_PER_USER)
        {
            // Remove oldest connection
            var oldestConnection = connections.First();
            connections.Remove(oldestConnection);
        }

        connections.Add(connectionId);
        return true;
    }
}
```

---

## 🚨 Important Notes

1. **Auto-Reconnect** - Frontend automatically reconnects on connection drop
2. **Multi-Tab Support** - User can have multiple tabs open (multiple connections)
3. **Redis Required** - For multi-server deployments, use Redis backplane
4. **Token Expiry** - Reconnect with new token when access token expires
5. **Graceful Degradation** - App works without SignalR (falls back to polling)

---

## 📚 Related Documentation

- [Notifications Module](notifications.md) - In-app notifications
- [Background Jobs Module](background-jobs.md) - Hangfire integration
- [IAM Module](iam.md) - Authentication

---

**[← Back to Modules](../MODULES.md)** | **[Next: Background Jobs →](background-jobs.md)**
