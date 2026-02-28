# 👥 IAM (Identity & Access Management) Module

Complete user, role, and permission management system with RBAC.

---

## 📋 Overview

The IAM module provides comprehensive identity and access management capabilities. Built on ASP.NET Core Identity with custom extensions for permissions, departments, and hierarchical role management.

---

## ✨ Key Features

- ✅ **Role-Based Access Control (RBAC)** - Fine-grained permissions
- ✅ **JWT Authentication** - Token-based auth with refresh tokens
- ✅ **BCrypt Password Hashing** - Secure password storage
- ✅ **Department Hierarchy** - Organizational structure
- ✅ **Custom Permissions** - Flat "Module.Action" format
- ✅ **Account Lockout** - 5 failed attempts = 15 min lockout
- ✅ **Password Policy** - Min 8 chars, uppercase, lowercase, digit, special
- ✅ **Multi-Tenant Scoped** - All users belong to a tenant
- ✅ **Profile Management** - Update profile, change password, upload photo
- ✅ **User Activity Tracking** - Last login, login history

---

## 🗄️ Entities

### User

**File:** `src/CoreEngine.Domain/Entities/User.cs`

```csharp
public class User : IdentityUser<Guid>, ITenantScoped
{
    public Guid TenantId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public Guid? DepartmentId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsDeleted { get; set; }

    // Navigation properties
    public Tenant Tenant { get; set; }
    public Department? Department { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
}
```

### Role

**File:** `src/CoreEngine.Domain/Entities/Role.cs`

```csharp
public class Role : IdentityRole<Guid>, ITenantScoped
{
    public Guid TenantId { get; set; }
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }  // Cannot be deleted
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public bool IsDeleted { get; set; }

    // Navigation properties
    public Tenant Tenant { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
    public ICollection<RolePermission> RolePermissions { get; set; }
}
```

### Permission

**File:** `src/CoreEngine.Domain/Entities/Permission.cs`

```csharp
public class Permission : Entity
{
    public string Name { get; set; }  // e.g., "User.Create", "Report.Export"
    public string Module { get; set; }  // e.g., "User", "Report"
    public string Action { get; set; }  // e.g., "Create", "Export"
    public string? Description { get; set; }
    public bool IsSystemPermission { get; set; }

    // Navigation properties
    public ICollection<RolePermission> RolePermissions { get; set; }
}
```

### Department

**File:** `src/CoreEngine.Domain/Entities/Department.cs`

```csharp
public class Department : TenantScopedEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public Guid? ParentDepartmentId { get; set; }
    public Guid? ManagerId { get; set; }

    // Navigation properties
    public Department? ParentDepartment { get; set; }
    public User? Manager { get; set; }
    public ICollection<Department> SubDepartments { get; set; }
    public ICollection<User> Users { get; set; }
}
```

---

## 🔐 Authentication Flow

```
┌─────────────────────────────────────────────────────────────┐
│  User Login                                                  │
└───────────────────────┬─────────────────────────────────────┘
                        │
                        ↓
        ┌───────────────────────────────┐
        │  POST /api/auth/login         │
        │  { email, password }          │
        └───────────────┬───────────────┘
                        │
                        ↓
        ┌────────────────────────────────────┐
        │  Validate Credentials              │
        │  → Check email exists              │
        │  → Verify BCrypt password hash     │
        │  → Check account not locked        │
        └────────┬───────────────────────────┘
                 │
        ┌────────┴────────┐
        │   Valid?        │
        └────┬────────────┴┐
             │ Yes         │ No
             ↓             ↓
  ┌──────────────────┐  ┌────────────────┐
  │ Generate Tokens  │  │ Increment      │
  │ → Access Token   │  │ Failed Count   │
  │   (JWT, 1hr)     │  │ → Lock after 5 │
  │ → Refresh Token  │  └────────────────┘
  │   (30 days)      │
  └────┬─────────────┘
       │
       ↓
  ┌──────────────────┐
  │ Return Response  │
  │ {                │
  │   accessToken,   │
  │   refreshToken,  │
  │   user: {...}    │
  │ }                │
  └──────────────────┘
```

---

## 🔑 JWT Token Structure

### Access Token (1 hour expiry)

```json
{
  "sub": "123e4567-e89b-12d3-a456-426614174000",  // User ID
  "email": "john.doe@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "tenantId": "00000000-0000-0000-0000-000000000001",
  "permissions": [
    "User.Create",
    "User.Read",
    "User.Update",
    "Report.Export"
  ],
  "exp": 1709128800,  // Expiration timestamp
  "iss": "CoreEngine.API",
  "aud": "CoreEngine.Client"
}
```

### Refresh Token (30 days expiry)

- Stored in database as hashed value
- Used to obtain new access token without re-login
- Automatically rotated on refresh

---

## 🌐 API Endpoints

### Authentication

| Method | Endpoint | Description | Permission |
|--------|----------|-------------|------------|
| POST | `/api/auth/login` | Login with email/password | None (public) |
| POST | `/api/auth/refresh` | Refresh access token | None (public) |
| POST | `/api/auth/logout` | Logout current user | Authenticated |
| POST | `/api/auth/change-password` | Change password | Authenticated |
| POST | `/api/auth/forgot-password` | Request password reset | None (public) |
| POST | `/api/auth/reset-password` | Reset password with token | None (public) |

### Users

| Method | Endpoint | Description | Permission |
|--------|----------|-------------|------------|
| GET | `/api/users` | List users (paginated) | User.Read |
| GET | `/api/users/{id}` | Get user by ID | User.Read |
| POST | `/api/users` | Create new user | User.Create |
| PUT | `/api/users/{id}` | Update user | User.Update |
| DELETE | `/api/users/{id}` | Delete user (soft delete) | User.Delete |
| POST | `/api/users/{id}/assign-roles` | Assign roles to user | User.AssignRoles |
| POST | `/api/users/{id}/upload-photo` | Upload profile photo | Authenticated |
| DELETE | `/api/users/{id}/photo` | Delete profile photo | Authenticated |

### Roles

| Method | Endpoint | Description | Permission |
|--------|----------|-------------|------------|
| GET | `/api/roles` | List roles | Role.Read |
| GET | `/api/roles/{id}` | Get role by ID | Role.Read |
| POST | `/api/roles` | Create new role | Role.Create |
| PUT | `/api/roles/{id}` | Update role | Role.Update |
| DELETE | `/api/roles/{id}` | Delete role | Role.Delete |
| POST | `/api/roles/{id}/assign-permissions` | Assign permissions | Role.AssignPermissions |
| GET | `/api/roles/{id}/permissions` | Get role permissions | Role.Read |

### Departments

| Method | Endpoint | Description | Permission |
|--------|----------|-------------|------------|
| GET | `/api/departments` | List departments | Department.Read |
| GET | `/api/departments/hierarchy` | Get department tree | Department.Read |
| POST | `/api/departments` | Create department | Department.Create |
| PUT | `/api/departments/{id}` | Update department | Department.Update |
| DELETE | `/api/departments/{id}` | Delete department | Department.Delete |

### Permissions

| Method | Endpoint | Description | Permission |
|--------|----------|-------------|------------|
| GET | `/api/permissions` | List all permissions | Permission.Read |
| GET | `/api/permissions/by-module` | Group by module | Permission.Read |

---

## 💻 Usage Examples

### Login

```bash
curl -X POST https://api.coreengine.com/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@coreengine.local",
    "password": "Admin@123"
  }'
```

**Response:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "abc123def456...",
  "expiresIn": 3600,
  "user": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "email": "admin@coreengine.local",
    "firstName": "System",
    "lastName": "Administrator",
    "roles": ["SuperAdmin"]
  }
}
```

### Create User

```bash
curl -X POST https://api.coreengine.com/api/users \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john.doe@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "password": "Password@123",
    "departmentId": "123e4567-e89b-12d3-a456-426614174001",
    "isActive": true
  }'
```

### Assign Roles to User

```bash
curl -X POST https://api.coreengine.com/api/users/{userId}/assign-roles \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "roleIds": [
      "123e4567-e89b-12d3-a456-426614174010",
      "123e4567-e89b-12d3-a456-426614174011"
    ]
  }'
```

---

## 🎨 Frontend Pages

### 1. Login Page (`/login`)

**Features:**
- Email + password form
- "Remember me" checkbox
- "Forgot password?" link
- Client-side validation
- Error handling

### 2. Users Page (`/users`)

**Features:**
- Paginated user list
- Search by name/email
- Filter by department/role/status
- Create/edit user modal
- Assign roles
- Upload profile photo
- Activate/deactivate users

### 3. Roles Page (`/roles`)

**Features:**
- List all roles
- Create/edit role
- Assign permissions (grouped by module)
- Permission tree with checkboxes
- Cannot delete system roles

### 4. Departments Page (`/departments`)

**Features:**
- Tree view of department hierarchy
- Drag-and-drop to reorganize
- Assign department manager
- View department users

### 5. Profile Page (`/profile`)

**Features:**
- View/edit profile information
- Upload profile photo
- Change password
- Activity feed (last 50 actions)

---

## ⚙️ Configuration

### Password Policy

**File:** `appsettings.json`

```json
{
  "IdentityOptions": {
    "Password": {
      "RequireDigit": true,
      "RequiredLength": 8,
      "RequireNonAlphanumeric": true,
      "RequireUppercase": true,
      "RequireLowercase": true
    },
    "Lockout": {
      "MaxFailedAccessAttempts": 5,
      "DefaultLockoutTimeSpan": "00:15:00"  // 15 minutes
    },
    "User": {
      "RequireUniqueEmail": true
    }
  }
}
```

### JWT Settings

**File:** `appsettings.json` (secret in User Secrets)

```json
{
  "JwtSettings": {
    "SecretKey": "STORED_IN_USER_SECRETS",
    "Issuer": "CoreEngine.API",
    "Audience": "CoreEngine.Client",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 30
  }
}
```

### Seeded Data

**Default Tenant:** `00000000-0000-0000-0000-000000000001`

**Default Admin User:**
- Email: `admin@coreengine.local`
- Password: `Admin@123`
- Role: SuperAdmin

**System Roles:**
1. **SuperAdmin** - All permissions
2. **Admin** - Most permissions (no tenant management)
3. **User** - Basic read permissions

**System Permissions:** 60+ permissions across 15 modules

---

## 🔐 Permissions List

### User Module
- User.Create
- User.Read
- User.Update
- User.Delete
- User.AssignRoles

### Role Module
- Role.Create
- Role.Read
- Role.Update
- Role.Delete
- Role.AssignPermissions

### Department Module
- Department.Create
- Department.Read
- Department.Update
- Department.Delete

### Tenant Module
- Tenant.Create
- Tenant.Read
- Tenant.Update
- Tenant.Delete
- Tenant.Theme

### And 50+ more across other modules...

---

## 🔒 Security Features

### 1. Password Hashing
- **Algorithm:** BCrypt with work factor 12
- **Salt:** Automatically generated per password
- **Never store plain text passwords**

### 2. Account Lockout
- **Trigger:** 5 failed login attempts
- **Duration:** 15 minutes
- **Notification:** Email sent to user on lockout

### 3. JWT Security
- **Signing:** HMAC-SHA256
- **Secret Key:** Stored in User Secrets (not in code)
- **Token Rotation:** Refresh tokens are rotated on use
- **Revocation:** Logout invalidates refresh token

### 4. Authorization
- **Permission Check:** `[RequirePermission("User.Create")]` attribute
- **Claims-Based:** Permissions embedded in JWT claims
- **Tenant Isolation:** All queries filtered by TenantId

---

## 🧪 Testing

### Test User Credentials

```
SuperAdmin:
  Email: admin@coreengine.local
  Password: Admin@123

Manager (seeded):
  Email: manager@coreengine.local
  Password: Manager@123

Regular User (seeded):
  Email: user@coreengine.local
  Password: User@123
```

### Permission Testing

```csharp
// Example: Test permission check
[RequirePermission("User.Create")]
public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
{
    // Only executes if user has "User.Create" permission
    var userId = await _mediator.Send(command);
    return Ok(userId);
}
```

---

## 📚 Related Documentation

- [Multi-Tenancy Module](multi-tenancy.md) - Tenant isolation
- [Audit Logging Module](audit.md) - Track user actions
- [Security Module](security.md) - Advanced security features

---

**[← Back to Modules](../MODULES.md)** | **[Next: Audit Logging →](audit.md)**
