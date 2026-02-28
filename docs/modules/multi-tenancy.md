# 🏢 Multi-Tenancy Module

Complete tenant isolation system using custom EF Core global query filters.

---

## 📋 Overview

The Multi-Tenancy module provides complete data isolation between tenants. Each tenant gets their own data partition with automatic filtering at the database level. Tenant resolution happens via subdomain (e.g., `acme.coreengine.com` → Acme tenant).

---

## ✨ Key Features

- ✅ **Automatic tenant filtering** on all queries
- ✅ **Subdomain-based routing** (e.g., `tenant1.app.com`)
- ✅ **Per-tenant theming** (logo, colors)
- ✅ **TenantId auto-injection** on all create operations
- ✅ **Middleware-based resolution** (no manual tenant passing)
- ✅ **Global query filters** (EF Core level isolation)
- ✅ **Tenant-scoped configuration**

---

## 🗄️ Entities

### Tenant

**File:** `src/CoreEngine.Domain/Entities/Tenant.cs`

```csharp
public class Tenant : Entity
{
    public string Name { get; set; }               // Tenant name
    public string Subdomain { get; set; }          // Unique subdomain
    public string? ConnectionString { get; set; }  // Optional dedicated DB
    public bool IsActive { get; set; }             // Enable/disable tenant
    public string? LogoUrl { get; set; }           // Custom logo
    public string? PrimaryColor { get; set; }      // Brand color
    public string? SidebarColor { get; set; }      // Sidebar color
    public string? SidebarTextColor { get; set; }  // Sidebar text color
    public string? Settings { get; set; }          // JSON settings
}
```

### TenantScopedEntity (Base Class)

**File:** `src/CoreEngine.Domain/Common/TenantScopedEntity.cs`

```csharp
public abstract class TenantScopedEntity : Entity
{
    public Guid TenantId { get; set; }  // Foreign key to Tenant
}
```

**All entities that need tenant isolation extend this base class:**
- User, Role, Department
- WorkflowDefinition, WorkflowInstance, WorkflowTask
- EmailTemplate, EmailQueue
- FileMetadata, ReportDefinition
- And 20+ more...

---

## 🔧 Implementation Details

### Global Query Filter

**File:** `src/CoreEngine.Infrastructure/Persistence/ApplicationDbContext.cs`

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Get current tenant ID from context
    var tenantId = _tenantContext.TenantId;

    // Apply filter to all tenant-scoped entities
    modelBuilder.Entity<User>()
        .HasQueryFilter(e => e.TenantId == tenantId);

    modelBuilder.Entity<Role>()
        .HasQueryFilter(e => e.TenantId == tenantId);

    // ... all other TenantScopedEntity types
}
```

**Effect:** All queries automatically include `WHERE TenantId = @currentTenantId`

### Tenant Resolution Middleware

**File:** `src/CoreEngine.API/Middleware/TenantResolutionMiddleware.cs`

```csharp
public async Task InvokeAsync(HttpContext context)
{
    // Extract subdomain from host
    var subdomain = ExtractSubdomain(context.Request.Host);

    // Lookup tenant by subdomain
    var tenant = await _tenantService.GetBySubdomainAsync(subdomain);

    if (tenant == null || !tenant.IsActive)
        throw new UnauthorizedException("Invalid tenant");

    // Set tenant in scoped context
    _tenantContext.SetTenant(tenant.Id);

    await _next(context);
}

private string ExtractSubdomain(HostString host)
{
    var parts = host.Host.Split('.');
    return parts.Length > 2 ? parts[0] : "default";
}
```

### SaveChanges Override

**File:** `src/CoreEngine.Infrastructure/Persistence/ApplicationDbContext.cs`

```csharp
public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
{
    // Auto-set TenantId on new entities
    foreach (var entry in ChangeTracker.Entries<TenantScopedEntity>())
    {
        if (entry.State == EntityState.Added)
        {
            entry.Entity.TenantId = _tenantContext.TenantId;
        }
    }

    return await base.SaveChangesAsync(cancellationToken);
}
```

---

## 🌐 API Endpoints

| Method | Endpoint | Description | Permission |
|--------|----------|-------------|------------|
| GET | `/api/tenants` | List all tenants | Tenant.Read |
| GET | `/api/tenants/{id}` | Get tenant by ID | Tenant.Read |
| POST | `/api/tenants` | Create new tenant | Tenant.Create |
| PUT | `/api/tenants/{id}` | Update tenant | Tenant.Update |
| DELETE | `/api/tenants/{id}` | Delete tenant (soft delete) | Tenant.Delete |

### Example: Create Tenant

**Request:**
```bash
curl -X POST https://api.coreengine.com/api/tenants \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Acme Corporation",
    "subdomain": "acme",
    "primaryColor": "#FF5733",
    "sidebarColor": "#2C3E50"
  }'
```

**Response:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Acme Corporation",
  "subdomain": "acme",
  "isActive": true,
  "primaryColor": "#FF5733",
  "sidebarColor": "#2C3E50",
  "createdAt": "2026-02-28T10:30:00Z"
}
```

---

## 🎨 Frontend Integration

### Frontend Pages

1. **TenantsPage.tsx** (`/tenants`)
   - List all tenants
   - Create/edit tenant
   - Activate/deactivate tenant
   - Upload logo

2. **ThemingPage.tsx** (`/theming`)
   - Customize tenant branding
   - Upload logo
   - Set colors (primary, sidebar, text)

### Tenant Context Hook

**File:** `frontend/src/contexts/ThemeContext.tsx`

```typescript
export const useTenantTheme = () => {
  const { theme } = useContext(ThemeContext);

  return {
    theme: {
      primaryColor: theme?.primaryColor || '#0071e3',
      sidebarColor: theme?.sidebarColor || '#ffffff',
      sidebarTextColor: theme?.sidebarTextColor || '#1d1d1f',
      logoUrl: theme?.logoUrl
    }
  };
};
```

### Automatic Tenant Header

**File:** `frontend/src/api/client.ts`

```typescript
apiClient.interceptors.request.use(config => {
  const tenantId = localStorage.getItem('tenantId');
  if (tenantId) {
    config.headers['X-Tenant-ID'] = tenantId;
  }
  return config;
});
```

---

## ⚙️ Configuration

### Default Tenant

**Database Seed:** One default tenant is created automatically with ID:
```
00000000-0000-0000-0000-000000000001
```

**Subdomain:** `default`

### Subdomain Routing

**Development:**
```
http://localhost:5173  → default tenant
```

**Production:**
```
https://acme.coreengine.com  → acme tenant
https://globex.coreengine.com  → globex tenant
```

### Custom Domain Support

Edit `appsettings.json`:
```json
{
  "MultiTenancy": {
    "Strategy": "Subdomain",  // or "CustomDomain"
    "DefaultTenant": "default"
  }
}
```

---

## 🔐 Permissions

- **Tenant.Create** - Create new tenants
- **Tenant.Read** - View tenant list
- **Tenant.Update** - Modify tenant settings
- **Tenant.Delete** - Remove tenants
- **Tenant.Theme** - Customize tenant branding

**Default Roles:**
- **SuperAdmin**: All tenant permissions
- **Admin**: Tenant.Read, Tenant.Update, Tenant.Theme
- **User**: None (cannot manage tenants)

---

## 🧪 Testing Tenant Isolation

### Verify Query Filters Work

```csharp
// Login as Tenant A user
var usersInTenantA = await _context.Users.ToListAsync();
// Returns only Tenant A users

// Login as Tenant B user
var usersInTenantB = await _context.Users.ToListAsync();
// Returns only Tenant B users (completely isolated)
```

### SQL Query Generated

```sql
SELECT * FROM Users
WHERE TenantId = '123e4567-e89b-12d3-a456-426614174000'
AND IsDeleted = 0
```

The `TenantId` filter is **automatic** and **cannot be bypassed**.

---

## 🚨 Important Notes

1. **Never bypass tenant filters** - All queries go through EF Core
2. **Tenant ID is immutable** - Cannot change after entity creation
3. **Cross-tenant queries prohibited** - Intentionally blocked for security
4. **SuperAdmin can switch tenants** - Via special permission
5. **Soft delete respects tenants** - Deleted records stay in same tenant

---

## 📚 Related Documentation

- [IAM Module](iam.md) - User and role management
- [Theming Module](theming.md) - Customization options
- [Configuration Module](configuration.md) - Tenant-specific settings

---

**[← Back to Modules](../MODULES.md)** | **[Next: IAM Module →](iam.md)**
