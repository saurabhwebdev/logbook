# 🌐 API Reference

Complete REST API documentation for CoreEngine.

---

## 📋 Overview

CoreEngine provides a comprehensive RESTful API built with ASP.NET Core 9. All endpoints require JWT authentication (except auth endpoints) and respect multi-tenant isolation.

**Base URL:** `https://api.coreengine.com/api`
**Development URL:** `http://localhost:5034/api`

---

## 🔑 Authentication

All API requests (except `/auth/login` and `/auth/refresh`) require a valid JWT token in the `Authorization` header:

```bash
Authorization: Bearer YOUR_JWT_TOKEN
```

**Token Expiration:** 1 hour
**Refresh Token Expiration:** 30 days

---

## 📚 API Modules

### 🔐 Core & Security

- **[Authentication API](api/auth.md)** - Login, refresh, logout, password reset
- **[Users API](api/users.md)** - User management (CRUD, assign roles, upload photo)
- **[Roles API](api/roles.md)** - Role management and permission assignment
- **[Departments API](api/departments.md)** - Department hierarchy management
- **[Permissions API](api/permissions.md)** - List all permissions
- **[Tenants API](api/tenants.md)** - Multi-tenant management
- **[Audit Logs API](api/audit-logs.md)** - View audit trail

### 📨 Communication & Workflows

- **[Workflow Definitions API](api/workflow-definitions.md)** - Create and manage workflow templates
- **[Workflow Instances API](api/workflow-instances.md)** - Start and track workflow executions
- **[Workflow Tasks API](api/workflow-tasks.md)** - Task assignment and completion
- **[Notifications API](api/notifications.md)** - In-app notifications
- **[Email Templates API](api/email-templates.md)** - Email template management
- **[Email Queue API](api/email-queue.md)** - Email sending queue

### 📊 Data & Configuration

- **[Files API](api/files.md)** - File upload, download, delete
- **[Reports API](api/reports.md)** - Generate and export reports (Excel, CSV, PDF)
- **[Configuration API](api/configuration.md)** - System configuration settings
- **[Feature Flags API](api/feature-flags.md)** - Feature toggle management
- **[State Machine API](api/state-machine.md)** - Entity state transitions

### 🔌 Integration & Help

- **[API Integration API](api/api-integration.md)** - API keys and webhooks
- **[Help API](api/help.md)** - Help articles and contextual help

---

## 🔄 Common Patterns

### Pagination

Most list endpoints support pagination:

```bash
GET /api/users?pageNumber=1&pageSize=20
```

**Response:**
```json
{
  "items": [...],
  "pageNumber": 1,
  "totalPages": 5,
  "totalCount": 87,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

### Filtering

Endpoints support various filters:

```bash
GET /api/users?searchTerm=john&departmentId=123&isActive=true
```

### Sorting

Use `sortBy` and `sortDirection`:

```bash
GET /api/users?sortBy=lastName&sortDirection=asc
```

### Error Responses

**400 Bad Request:**
```json
{
  "type": "ValidationError",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Email": ["The Email field is required."]
  }
}
```

**401 Unauthorized:**
```json
{
  "type": "UnauthorizedError",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Invalid or expired token."
}
```

**403 Forbidden:**
```json
{
  "type": "ForbiddenError",
  "title": "Forbidden",
  "status": 403,
  "detail": "You do not have permission to perform this action. Required: User.Create"
}
```

**404 Not Found:**
```json
{
  "type": "NotFoundError",
  "title": "Not Found",
  "status": 404,
  "detail": "User with ID '123e4567-...' was not found."
}
```

**500 Internal Server Error:**
```json
{
  "type": "InternalServerError",
  "title": "An error occurred while processing your request.",
  "status": 500,
  "traceId": "00-abc123..."
}
```

---

## 🔐 Permissions

All endpoints are protected by permission checks. Each endpoint lists required permissions in its documentation.

**Permission Format:** `Module.Action`

**Examples:**
- `User.Create`
- `Report.Export`
- `WorkflowDefinition.Read`

**Roles with All Permissions:**
- **SuperAdmin** - All permissions across all modules

---

## 📊 Rate Limiting

**Authentication Endpoints:**
- 10 requests per minute per IP

**General API Endpoints:**
- 1000 requests per minute per user

**Response Headers:**
```
X-RateLimit-Limit: 1000
X-RateLimit-Remaining: 999
X-RateLimit-Reset: 1709140800
```

**429 Too Many Requests:**
```json
{
  "type": "RateLimitError",
  "title": "Too Many Requests",
  "status": 429,
  "detail": "Rate limit exceeded. Try again in 60 seconds."
}
```

---

## 🧪 Testing the API

### Using cURL

```bash
# Login
TOKEN=$(curl -s -X POST "http://localhost:5034/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@coreengine.local","password":"Admin@123"}' \
  | jq -r '.accessToken')

# Get users
curl -X GET "http://localhost:5034/api/users?pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer $TOKEN"
```

### Using Postman

1. Import the [CoreEngine Postman Collection](../postman/CoreEngine.postman_collection.json)
2. Set environment variables:
   - `baseUrl`: `http://localhost:5034`
   - `email`: `admin@coreengine.local`
   - `password`: `Admin@123`
3. Run the "Login" request to get a token
4. Other requests automatically use the token

### Using Swagger/OpenAPI

Access interactive API documentation:
- **Development:** `http://localhost:5034/swagger`
- **Production:** Disabled for security

---

## 📝 Example Workflows

### 1. Create a User

```bash
# 1. Login
curl -X POST "http://localhost:5034/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@coreengine.local",
    "password": "Admin@123"
  }'

# Response: { "accessToken": "eyJhbGci...", ... }

# 2. Create user
curl -X POST "http://localhost:5034/api/users" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john.doe@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "password": "Password@123",
    "departmentId": "123e4567-...",
    "isActive": true
  }'

# Response: "456e7890-..." (User ID)

# 3. Assign role
curl -X POST "http://localhost:5034/api/users/456e7890-.../assign-roles" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "roleIds": ["789e0123-..."]
  }'
```

### 2. Start a Workflow

```bash
# 1. Get workflow definitions
curl -X GET "http://localhost:5034/api/WorkflowDefinitions" \
  -H "Authorization: Bearer YOUR_TOKEN"

# 2. Start workflow instance
curl -X POST "http://localhost:5034/api/WorkflowInstances" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "workflowDefinitionId": "123e4567-...",
    "entityType": "PurchaseOrder",
    "entityId": "po-2024-001",
    "contextJson": "{\"amount\": 5000, \"vendor\": \"Acme Corp\"}"
  }'

# Response: "456e7890-..." (Workflow Instance ID)
```

### 3. Generate a Report

```bash
# Generate PDF report
curl -X POST "http://localhost:5034/api/reports/generate" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "reportType": "UserList",
    "format": "PDF",
    "filters": {
      "departmentId": "123e4567-...",
      "isActive": true
    }
  }' \
  --output report.pdf
```

---

## 🔗 Quick Reference

### Authentication Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/login` | Login with email/password |
| POST | `/api/auth/refresh` | Refresh access token |
| POST | `/api/auth/logout` | Logout (invalidate refresh token) |
| POST | `/api/auth/change-password` | Change current user's password |
| POST | `/api/auth/forgot-password` | Request password reset email |
| POST | `/api/auth/reset-password` | Reset password with token |

### User Management Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/users` | List users (paginated) |
| GET | `/api/users/{id}` | Get user by ID |
| POST | `/api/users` | Create new user |
| PUT | `/api/users/{id}` | Update user |
| DELETE | `/api/users/{id}` | Delete user (soft delete) |
| POST | `/api/users/{id}/assign-roles` | Assign roles to user |
| POST | `/api/users/{id}/upload-photo` | Upload profile photo |

### Workflow Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/WorkflowDefinitions` | List workflow definitions |
| POST | `/api/WorkflowDefinitions` | Create workflow definition |
| GET | `/api/WorkflowInstances` | List workflow instances |
| POST | `/api/WorkflowInstances` | Start workflow |
| GET | `/api/WorkflowTasks/my-tasks` | Get my pending tasks |
| POST | `/api/WorkflowTasks/{id}/complete` | Approve/reject task |

---

## 📚 Detailed Documentation

Click on any module above to view detailed API documentation including:
- Complete endpoint list
- Request/response schemas
- Example requests (cURL)
- Required permissions
- Error responses

---

## 🚨 Important Notes

1. **All timestamps are in UTC** - Convert to local time in the frontend
2. **GUIDs are lowercase** - Use lowercase GUIDs in all requests
3. **Soft deletes** - Deleted entities are marked with `IsDeleted=true`, not physically removed
4. **Tenant isolation** - Users can only access data from their tenant
5. **Case sensitivity** - Email is case-insensitive, other fields are case-sensitive

---

**[← Back to Documentation](../README.md)**
