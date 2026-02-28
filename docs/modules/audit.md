# 📝 Audit Logging Module

Automatic change tracking for all entities with who, what, when details.

---

## 📋 Overview

The Audit Logging module provides comprehensive audit trail functionality. Every create, update, and delete operation is automatically logged with user context, timestamps, and change details. Built using EF Core interceptors and SaveChanges override.

---

## ✨ Key Features

- ✅ **Automatic tracking** - No manual audit code needed
- ✅ **Who** - User ID and email
- ✅ **What** - Entity type, ID, and action
- ✅ **When** - Precise timestamp
- ✅ **Changes** - Before/after values in JSON
- ✅ **Soft delete support** - Track deletions
- ✅ **Multi-tenant scoped** - Audit logs per tenant
- ✅ **Performance optimized** - Batch inserts
- ✅ **Queryable** - Filter by user, entity, date range

---

## 🗄️ Entities

### AuditLog

**File:** `src/CoreEngine.Domain/Entities/AuditLog.cs`

```csharp
public class AuditLog : TenantScopedEntity
{
    public Guid UserId { get; set; }
    public string UserEmail { get; set; }
    public string Action { get; set; }  // "Create", "Update", "Delete"
    public string EntityType { get; set; }  // "User", "WorkflowDefinition"
    public string EntityId { get; set; }
    public string? Changes { get; set; }  // JSON of before/after values
    public DateTime Timestamp { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }

    // Navigation properties
    public User User { get; set; }
}
```

**Changes JSON Structure:**
```json
{
  "before": {
    "firstName": "John",
    "lastName": "Doe",
    "isActive": true
  },
  "after": {
    "firstName": "John",
    "lastName": "Smith",
    "isActive": true
  }
}
```

---

## 🔧 Implementation Details

### AuditableEntityInterceptor

**File:** `src/CoreEngine.Infrastructure/Persistence/Interceptors/AuditableEntityInterceptor.cs`

```csharp
public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        var context = eventData.Context;
        if (context == null) return result;

        var entries = context.ChangeTracker.Entries<Entity>();
        var userId = _currentUserService.UserId;
        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.CreatedBy = userId;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedAt = now;
                entry.Entity.ModifiedBy = userId;
            }
        }

        return result;
    }
}
```

### SaveChangesAsync Override

**File:** `src/CoreEngine.Infrastructure/Persistence/ApplicationDbContext.cs`

```csharp
public override async Task<int> SaveChangesAsync(
    CancellationToken cancellationToken = default)
{
    var auditLogs = new List<AuditLog>();
    var userId = _currentUserService.UserId;
    var userEmail = _currentUserService.Email;
    var tenantId = _tenantContext.TenantId;

    // Track changes BEFORE saving
    foreach (var entry in ChangeTracker.Entries<Entity>())
    {
        if (entry.State == EntityState.Added ||
            entry.State == EntityState.Modified ||
            entry.State == EntityState.Deleted)
        {
            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                UserId = userId ?? Guid.Empty,
                UserEmail = userEmail ?? "System",
                Action = entry.State.ToString(),
                EntityType = entry.Entity.GetType().Name,
                EntityId = entry.Entity.Id.ToString(),
                Changes = SerializeChanges(entry),
                Timestamp = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            auditLogs.Add(auditLog);
        }
    }

    // Save entity changes
    var result = await base.SaveChangesAsync(cancellationToken);

    // Insert audit logs
    if (auditLogs.Any())
    {
        await AuditLogs.AddRangeAsync(auditLogs, cancellationToken);
        await base.SaveChangesAsync(cancellationToken);
    }

    return result;
}

private string? SerializeChanges(EntityEntry entry)
{
    if (entry.State == EntityState.Added)
    {
        return JsonSerializer.Serialize(new
        {
            after = entry.CurrentValues.Properties
                .ToDictionary(p => p.Name, p => entry.CurrentValues[p])
        });
    }
    else if (entry.State == EntityState.Modified)
    {
        var before = entry.OriginalValues.Properties
            .ToDictionary(p => p.Name, p => entry.OriginalValues[p]);
        var after = entry.CurrentValues.Properties
            .ToDictionary(p => p.Name, p => entry.CurrentValues[p]);

        return JsonSerializer.Serialize(new { before, after });
    }
    else if (entry.State == EntityState.Deleted)
    {
        return JsonSerializer.Serialize(new
        {
            before = entry.OriginalValues.Properties
                .ToDictionary(p => p.Name, p => entry.OriginalValues[p])
        });
    }

    return null;
}
```

---

## 🌐 API Endpoints

| Method | Endpoint | Description | Permission |
|--------|----------|-------------|------------|
| GET | `/api/audit-logs` | List audit logs (paginated) | AuditLog.Read |
| GET | `/api/audit-logs/{id}` | Get audit log by ID | AuditLog.Read |
| GET | `/api/audit-logs/by-entity` | Get logs for specific entity | AuditLog.Read |
| GET | `/api/audit-logs/by-user` | Get logs for specific user | AuditLog.Read |
| GET | `/api/audit-logs/export` | Export audit logs to CSV | AuditLog.Export |

### Query Parameters

**List Audit Logs:**
```
GET /api/audit-logs?
  userId={guid}&
  entityType={string}&
  action={Create|Update|Delete}&
  startDate={date}&
  endDate={date}&
  pageNumber={int}&
  pageSize={int}
```

**Example:**
```bash
curl -X GET "https://api.coreengine.com/api/audit-logs?\
  entityType=User&\
  action=Update&\
  startDate=2026-02-01&\
  endDate=2026-02-28&\
  pageNumber=1&\
  pageSize=20" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

---

## 💻 Usage Examples

### Get Audit Logs for User Updates

```bash
curl -X GET "https://api.coreengine.com/api/audit-logs?\
  entityType=User&\
  action=Update" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

**Response:**
```json
{
  "items": [
    {
      "id": "123e4567-e89b-12d3-a456-426614174000",
      "userId": "456e7890-e89b-12d3-a456-426614174001",
      "userEmail": "admin@coreengine.local",
      "action": "Update",
      "entityType": "User",
      "entityId": "789e0123-e89b-12d3-a456-426614174002",
      "changes": "{\"before\":{\"firstName\":\"John\",\"isActive\":true},\"after\":{\"firstName\":\"John\",\"isActive\":false}}",
      "timestamp": "2026-02-27T14:30:00Z",
      "ipAddress": "192.168.1.100"
    }
  ],
  "pageNumber": 1,
  "totalPages": 5,
  "totalCount": 87
}
```

### Get Audit Logs for Specific Entity

```bash
curl -X GET "https://api.coreengine.com/api/audit-logs/by-entity?\
  entityType=WorkflowDefinition&\
  entityId=123e4567-e89b-12d3-a456-426614174000" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

**Response:**
```json
{
  "items": [
    {
      "action": "Create",
      "userEmail": "admin@coreengine.local",
      "timestamp": "2026-02-20T10:00:00Z",
      "changes": "{\"after\":{\"name\":\"Purchase Order Approval\",\"isActive\":true}}"
    },
    {
      "action": "Update",
      "userEmail": "manager@coreengine.local",
      "timestamp": "2026-02-25T15:30:00Z",
      "changes": "{\"before\":{\"isActive\":true},\"after\":{\"isActive\":false}}"
    }
  ],
  "totalCount": 2
}
```

---

## 🎨 Frontend Pages

### 1. Audit Logs Page (`/audit-logs`)

**Features:**
- Paginated audit log table
- Filter by:
  - User (dropdown)
  - Entity type (dropdown)
  - Action (Create/Update/Delete)
  - Date range picker
- Search by entity ID
- View changes modal (before/after comparison)
- Export to CSV
- Real-time updates (via SignalR)

**Table Columns:**
- Timestamp
- User (with avatar)
- Action (color-coded badge)
- Entity Type
- Entity ID (clickable link)
- View Changes (button)

**Screenshot:**
```
┌─────────────────────────────────────────────────────────────────┐
│  Audit Logs                            [Export CSV] [🔍 Filter] │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  Filters: [All Users ▼] [All Entities ▼] [All Actions ▼]       │
│  Date: [Feb 1, 2026] - [Feb 28, 2026]                          │
│                                                                  │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │ Time       │ User      │ Action  │ Entity  │ Details    │  │
│  ├──────────────────────────────────────────────────────────┤  │
│  │ 14:30:15   │ Admin     │ 🔵 Create │ User    │ [View]    │  │
│  │ 14:25:03   │ Manager   │ 🟠 Update │ Role    │ [View]    │  │
│  │ 13:10:22   │ Admin     │ 🔴 Delete │ Report  │ [View]    │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                  │
│  Showing 1-20 of 1,247 logs        [← Prev] [1 2 3...] [Next →]│
└─────────────────────────────────────────────────────────────────┘
```

### 2. Entity History Modal

**Features:**
- Shows all audit logs for a specific entity
- Timeline view (newest first)
- Before/after diff highlighting
- User avatars
- Relative timestamps ("2 hours ago")

---

## ⚙️ Configuration

### Audit Settings

**File:** `appsettings.json`

```json
{
  "AuditSettings": {
    "EnableAuditLogging": true,
    "IncludeIpAddress": true,
    "IncludeUserAgent": true,
    "RetentionDays": 365,  // Auto-delete logs older than 1 year
    "ExcludedEntityTypes": [
      "AuditLog",  // Don't audit the audit logs
      "RefreshToken"  // Don't audit refresh tokens
    ],
    "ExcludedProperties": [
      "PasswordHash",
      "SecurityStamp",
      "ConcurrencyStamp"
    ]
  }
}
```

### Automatic Cleanup Job

**File:** `src/CoreEngine.Infrastructure/BackgroundJobs/CleanupOldAuditLogsJob.cs`

```csharp
public class CleanupOldAuditLogsJob
{
    public async Task Execute()
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-365);

        await _context.AuditLogs
            .Where(a => a.CreatedAt < cutoffDate)
            .ExecuteDeleteAsync();
    }
}
```

**Hangfire Schedule:** Daily at 2:00 AM

---

## 🔐 Permissions

- **AuditLog.Read** - View audit logs
- **AuditLog.Export** - Export audit logs to CSV

**Default Roles:**
- **SuperAdmin**: AuditLog.Read, AuditLog.Export
- **Admin**: AuditLog.Read
- **User**: None (cannot view audit logs)

---

## 🧪 Testing Audit Logging

### Verify Audit Log Created

```csharp
// Create a user
var user = new User
{
    Email = "test@example.com",
    FirstName = "Test",
    LastName = "User"
};

await _context.Users.AddAsync(user);
await _context.SaveChangesAsync();

// Verify audit log exists
var auditLog = await _context.AuditLogs
    .Where(a => a.EntityType == "User" && a.Action == "Create")
    .OrderByDescending(a => a.Timestamp)
    .FirstOrDefaultAsync();

Assert.NotNull(auditLog);
Assert.Equal("Create", auditLog.Action);
Assert.Contains("test@example.com", auditLog.Changes);
```

### Example Audit Log Entry

```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "tenantId": "00000000-0000-0000-0000-000000000001",
  "userId": "456e7890-e89b-12d3-a456-426614174001",
  "userEmail": "admin@coreengine.local",
  "action": "Update",
  "entityType": "WorkflowDefinition",
  "entityId": "789e0123-e89b-12d3-a456-426614174002",
  "changes": "{\"before\":{\"name\":\"Old Name\",\"isActive\":true},\"after\":{\"name\":\"New Name\",\"isActive\":false}}",
  "timestamp": "2026-02-27T14:30:15.123Z",
  "ipAddress": "192.168.1.100",
  "userAgent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64)..."
}
```

---

## 📊 Audit Statistics

### Common Queries

**Most Active Users:**
```sql
SELECT UserId, UserEmail, COUNT(*) as ActionCount
FROM AuditLogs
WHERE Timestamp >= DATEADD(day, -30, GETUTCDATE())
GROUP BY UserId, UserEmail
ORDER BY ActionCount DESC
```

**Entity Changes by Type:**
```sql
SELECT EntityType, Action, COUNT(*) as Count
FROM AuditLogs
WHERE Timestamp >= DATEADD(day, -7, GETUTCDATE())
GROUP BY EntityType, Action
ORDER BY Count DESC
```

**Recent Deletions:**
```sql
SELECT *
FROM AuditLogs
WHERE Action = 'Delete'
AND Timestamp >= DATEADD(day, -7, GETUTCDATE())
ORDER BY Timestamp DESC
```

---

## 🚨 Important Notes

1. **Performance** - Audit logs are inserted in a separate SaveChanges call to avoid circular dependencies
2. **Tenant Isolation** - Audit logs respect multi-tenancy filters
3. **System Actions** - Actions performed by background jobs show "System" as user
4. **Sensitive Data** - Password hashes and security stamps are excluded from audit logs
5. **Retention** - Logs older than 365 days are automatically deleted (configurable)

---

## 📚 Related Documentation

- [IAM Module](iam.md) - User authentication
- [Security Module](security.md) - Security features
- [Multi-Tenancy Module](multi-tenancy.md) - Tenant isolation

---

**[← Back to Modules](../MODULES.md)** | **[Next: Security Module →](security.md)**
