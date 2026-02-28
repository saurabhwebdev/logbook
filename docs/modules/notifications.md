# 🔔 Notifications Module

In-app notification system with real-time delivery via SignalR.

---

## 📋 Overview

The Notifications module provides a comprehensive in-app notification system. Users receive notifications for workflow tasks, system events, mentions, and custom triggers. Notifications are delivered in real-time via SignalR and persist in the database for later viewing.

---

## ✨ Key Features

- ✅ **Real-time delivery** - Instant notification via SignalR
- ✅ **Persistent storage** - Notifications saved in database
- ✅ **Unread badge count** - Shows number of unread notifications
- ✅ **Mark as read/unread** - Individual or bulk operations
- ✅ **Notification types** - Info, Success, Warning, Error
- ✅ **Deep linking** - Click notification to navigate to related entity
- ✅ **Rich content** - Support for formatted messages
- ✅ **Multi-tenant scoped** - Notifications per tenant
- ✅ **Auto-cleanup** - Delete old read notifications

---

## 🗄️ Entities

### Notification

**File:** `src/CoreEngine.Domain/Entities/Notification.cs`

```csharp
public class Notification : TenantScopedEntity
{
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public string Type { get; set; }  // "Info", "Success", "Warning", "Error"
    public string? Link { get; set; }  // Deep link URL
    public string? EntityType { get; set; }  // "WorkflowTask", "Report"
    public string? EntityId { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }

    // Navigation properties
    public User User { get; set; }
}
```

---

## 🌐 API Endpoints

| Method | Endpoint | Description | Permission |
|--------|----------|-------------|------------|
| GET | `/api/notifications` | List notifications (paginated) | Authenticated |
| GET | `/api/notifications/unread-count` | Get unread count | Authenticated |
| PUT | `/api/notifications/{id}/mark-read` | Mark notification as read | Authenticated |
| PUT | `/api/notifications/mark-all-read` | Mark all as read | Authenticated |
| DELETE | `/api/notifications/{id}` | Delete notification | Authenticated |
| DELETE | `/api/notifications/clear-read` | Delete all read notifications | Authenticated |

### Query Parameters

**List Notifications:**
```
GET /api/notifications?
  isRead={bool}&
  type={Info|Success|Warning|Error}&
  pageNumber={int}&
  pageSize={int}
```

---

## 💻 Usage Examples

### Send Notification (Backend)

```csharp
public class SendNotificationCommand : IRequest
{
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public string Type { get; set; }  // "Info", "Success", "Warning", "Error"
    public string? Link { get; set; }
    public string? EntityType { get; set; }
    public string? EntityId { get; set; }
}

public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

    public async Task Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        // Save to database
        var notification = new Notification
        {
            UserId = request.UserId,
            Title = request.Title,
            Message = request.Message,
            Type = request.Type,
            Link = request.Link,
            EntityType = request.EntityType,
            EntityId = request.EntityId,
            IsRead = false
        };

        await _context.Notifications.AddAsync(notification, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // Send real-time via SignalR
        var notificationDto = new NotificationDto
        {
            Id = notification.Id,
            Title = notification.Title,
            Message = notification.Message,
            Type = notification.Type,
            Link = notification.Link,
            CreatedAt = notification.CreatedAt
        };

        await _hubContext.Clients
            .User(request.UserId.ToString())
            .ReceiveNotification(notificationDto);
    }
}
```

### Example: Workflow Task Assigned Notification

```csharp
// When a workflow task is assigned
public async Task AssignWorkflowTask(Guid taskId, Guid userId)
{
    var task = await _context.WorkflowTasks.FindAsync(taskId);

    // Send notification
    await _mediator.Send(new SendNotificationCommand
    {
        UserId = userId,
        Title = "New Task Assigned",
        Message = $"You have been assigned a new task: {task.TaskName}",
        Type = "Info",
        Link = $"/my-tasks/{taskId}",
        EntityType = "WorkflowTask",
        EntityId = taskId.ToString()
    });
}
```

---

## 🎨 Frontend Integration

### Notifications API

**File:** `frontend/src/api/notificationsApi.ts`

```typescript
export const notificationsApi = {
  getAll: async (isRead?: boolean, pageNumber = 1, pageSize = 20) => {
    const params = new URLSearchParams();
    if (isRead !== undefined) params.append('isRead', String(isRead));
    params.append('pageNumber', String(pageNumber));
    params.append('pageSize', String(pageSize));

    const { data } = await api.get<PaginatedResponse<Notification>>(
      `/notifications?${params.toString()}`
    );
    return data;
  },

  getUnreadCount: async (): Promise<number> => {
    const { data } = await api.get<number>('/notifications/unread-count');
    return data;
  },

  markAsRead: async (id: string): Promise<void> => {
    await api.put(`/notifications/${id}/mark-read`);
  },

  markAllAsRead: async (): Promise<void> => {
    await api.put('/notifications/mark-all-read');
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/notifications/${id}`);
  },

  clearRead: async (): Promise<void> => {
    await api.delete('/notifications/clear-read');
  },
};
```

### Notification Bell Component

```typescript
import { Badge, Dropdown, List, Button, Empty } from 'antd';
import { BellOutlined } from '@ant-design/icons';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { notificationsApi } from '@/api/notificationsApi';
import { useSignalR } from '@/contexts/SignalRContext';

export const NotificationBell: React.FC = () => {
  const queryClient = useQueryClient();
  const { connection } = useSignalR();

  // Fetch unread count
  const { data: unreadCount = 0 } = useQuery({
    queryKey: ['notifications', 'unread-count'],
    queryFn: notificationsApi.getUnreadCount,
    refetchInterval: 30000, // Refresh every 30 seconds
  });

  // Fetch recent notifications
  const { data: notifications } = useQuery({
    queryKey: ['notifications', 'recent'],
    queryFn: () => notificationsApi.getAll(false, 1, 10), // Unread only, first 10
  });

  // Mark as read mutation
  const markAsReadMutation = useMutation({
    mutationFn: notificationsApi.markAsRead,
    onSuccess: () => {
      queryClient.invalidateQueries(['notifications']);
    },
  });

  // Listen for real-time notifications
  useEffect(() => {
    if (!connection) return;

    connection.on('ReceiveNotification', (notification) => {
      message.info({
        content: notification.message,
        onClick: () => {
          if (notification.link) {
            navigate(notification.link);
          }
        },
      });

      // Invalidate queries to refresh UI
      queryClient.invalidateQueries(['notifications']);
    });

    return () => {
      connection.off('ReceiveNotification');
    };
  }, [connection]);

  const handleNotificationClick = (notification: Notification) => {
    markAsReadMutation.mutate(notification.id);

    if (notification.link) {
      navigate(notification.link);
    }
  };

  const menuItems = (
    <div style={{ width: 400, maxHeight: 500, overflow: 'auto' }}>
      <div style={{ padding: '12px 16px', borderBottom: '1px solid #f0f0f0', display: 'flex', justifyContent: 'space-between' }}>
        <span style={{ fontWeight: 600 }}>Notifications</span>
        <Button type="link" size="small" onClick={() => navigate('/notifications')}>
          View All
        </Button>
      </div>

      {notifications?.items.length === 0 ? (
        <Empty description="No new notifications" style={{ padding: 40 }} />
      ) : (
        <List
          dataSource={notifications?.items}
          renderItem={(item) => (
            <List.Item
              style={{
                padding: '12px 16px',
                cursor: 'pointer',
                background: item.isRead ? 'white' : '#f6f9ff',
              }}
              onClick={() => handleNotificationClick(item)}
            >
              <List.Item.Meta
                title={item.title}
                description={
                  <>
                    <div>{item.message}</div>
                    <small style={{ color: '#86868b' }}>
                      {formatDistanceToNow(new Date(item.createdAt), { addSuffix: true })}
                    </small>
                  </>
                }
              />
            </List.Item>
          )}
        />
      )}
    </div>
  );

  return (
    <Dropdown overlay={menuItems} trigger={['click']} placement="bottomRight">
      <Badge count={unreadCount} offset={[-5, 5]}>
        <BellOutlined style={{ fontSize: 20, cursor: 'pointer' }} />
      </Badge>
    </Dropdown>
  );
};
```

---

## 🎨 Frontend Pages

### 1. Notifications Page (`/notifications`)

**Features:**
- List all notifications (read and unread)
- Filter by read/unread
- Filter by type (Info, Success, Warning, Error)
- Mark individual as read/unread
- Mark all as read button
- Delete individual notification
- Clear all read notifications button
- Click notification to navigate to related entity

**Screenshot:**
```
┌─────────────────────────────────────────────────────────────┐
│  Notifications                      [Mark All Read] [Clear] │
├─────────────────────────────────────────────────────────────┤
│  Filters: [All ▼] [All Types ▼]                Search...   │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ 🔵 NEW                                               │  │
│  │ New Task Assigned                    2 minutes ago   │  │
│  │ You have been assigned: Manager Approval             │  │
│  │ [Mark Read] [Delete]                                 │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ ✅ Report Generated Successfully     1 hour ago      │  │
│  │ Your monthly sales report is ready to download       │  │
│  │ [View Report]                                        │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ ⚠️  Workflow Rejected                Yesterday       │  │
│  │ Your purchase order was rejected by Director         │  │
│  │ [View Details]                                       │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

---

## ⚙️ Configuration

### Notification Settings

**File:** `appsettings.json`

```json
{
  "NotificationSettings": {
    "EnableRealTime": true,
    "RetentionDays": 90,  // Auto-delete read notifications older than 90 days
    "MaxUnreadCount": 100,  // Show "99+" if more than 100 unread
    "DefaultTypes": ["Info", "Success", "Warning", "Error"]
  }
}
```

### Auto-Cleanup Job

```csharp
public class CleanupOldNotificationsJob
{
    public async Task Execute()
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-90);

        // Delete read notifications older than 90 days
        await _context.Notifications
            .Where(n => n.IsRead && n.ReadAt < cutoffDate)
            .ExecuteDeleteAsync();
    }
}

// Schedule weekly
RecurringJob.AddOrUpdate<CleanupOldNotificationsJob>(
    "cleanup-old-notifications",
    job => job.Execute(),
    Cron.Weekly()
);
```

---

## 📊 Notification Types

### Info (Blue)
- New task assigned
- Report ready
- General updates

### Success (Green)
- Workflow approved
- Report generated successfully
- Action completed

### Warning (Orange)
- Pending deadline
- Action required
- Review needed

### Error (Red)
- Workflow rejected
- Operation failed
- Error occurred

---

## 🔔 Common Notification Triggers

### 1. Workflow Events
```csharp
// Task assigned
new Notification { Title = "New Task Assigned", Type = "Info" }

// Task approved
new Notification { Title = "Task Approved", Type = "Success" }

// Task rejected
new Notification { Title = "Task Rejected", Type = "Error" }
```

### 2. Report Events
```csharp
// Report generated
new Notification { Title = "Report Ready", Type = "Success" }

// Report failed
new Notification { Title = "Report Generation Failed", Type = "Error" }
```

### 3. System Events
```csharp
// Account locked
new Notification { Title = "Account Locked", Type = "Warning" }

// Password changed
new Notification { Title = "Password Changed Successfully", Type = "Success" }
```

---

## 🚨 Important Notes

1. **Real-time Delivery** - Notifications sent via SignalR AND saved to database
2. **Offline Users** - Notifications saved in database, user sees them on next login
3. **Multi-Device** - User gets notification on all connected devices
4. **Tenant Isolation** - Users only see notifications from their tenant
5. **Performance** - Unread count cached for 30 seconds to reduce DB queries

---

## 📚 Related Documentation

- [SignalR Module](signalr.md) - Real-time communication
- [Workflows Module](workflows.md) - Workflow task notifications
- [Background Jobs Module](background-jobs.md) - Cleanup jobs

---

**[← Back to Modules](../MODULES.md)** | **[Next: Email Templates →](email-templates.md)**
