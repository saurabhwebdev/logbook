# 👥 Users API

User management endpoints with CRUD operations, role assignment, and profile photo upload.

---

## Endpoints

### GET /api/users

List users with pagination and filtering.

**Authorization:** Required
**Permission:** `User.Read`

**Query Parameters:**
- `searchTerm` (string, optional) - Search by name or email
- `departmentId` (guid, optional) - Filter by department
- `isActive` (boolean, optional) - Filter by active status
- `roleId` (guid, optional) - Filter by role
- `pageNumber` (integer, default: 1) - Page number
- `pageSize` (integer, default: 10, max: 100) - Items per page

**Response (200 OK):**
```json
{
  "items": [
    {
      "id": "123e4567-e89b-12d3-a456-426614174000",
      "email": "john.doe@example.com",
      "firstName": "John",
      "lastName": "Doe",
      "profilePhotoUrl": "https://cdn.example.com/photos/user123.jpg",
      "departmentId": "456e7890-e89b-12d3-a456-426614174001",
      "departmentName": "Engineering",
      "isActive": true,
      "lastLoginAt": "2026-02-27T14:30:00Z",
      "createdAt": "2026-01-15T10:00:00Z",
      "roles": ["Manager", "Developer"]
    }
  ],
  "pageNumber": 1,
  "totalPages": 5,
  "totalCount": 47,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

**cURL Example:**
```bash
curl -X GET "http://localhost:5034/api/users?searchTerm=john&pageNumber=1&pageSize=20" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

---

### GET /api/users/{id}

Get user by ID.

**Authorization:** Required
**Permission:** `User.Read`

**Path Parameters:**
- `id` (guid) - User ID

**Response (200 OK):**
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "email": "john.doe@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "profilePhotoUrl": "https://cdn.example.com/photos/user123.jpg",
  "departmentId": "456e7890-e89b-12d3-a456-426614174001",
  "departmentName": "Engineering",
  "isActive": true,
  "lastLoginAt": "2026-02-27T14:30:00Z",
  "createdAt": "2026-01-15T10:00:00Z",
  "modifiedAt": "2026-02-20T09:15:00Z",
  "roles": [
    {
      "id": "789e0123-e89b-12d3-a456-426614174002",
      "name": "Manager"
    }
  ],
  "permissions": [
    "User.Read",
    "Report.Export"
  ]
}
```

**Error Responses:**
- **404 Not Found** - User not found

**cURL Example:**
```bash
curl -X GET "http://localhost:5034/api/users/123e4567-e89b-12d3-a456-426614174000" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

---

### POST /api/users

Create new user.

**Authorization:** Required
**Permission:** `User.Create`

**Request Body:**
```json
{
  "email": "jane.smith@example.com",
  "firstName": "Jane",
  "lastName": "Smith",
  "password": "Password@123",
  "departmentId": "456e7890-e89b-12d3-a456-426614174001",
  "isActive": true
}
```

**Response (200 OK):**
```json
"123e4567-e89b-12d3-a456-426614174000"
```

**Error Responses:**
- **400 Bad Request** - Validation error
  ```json
  {
    "type": "ValidationError",
    "errors": {
      "Email": ["Email already exists"],
      "Password": ["Password must be at least 8 characters"]
    }
  }
  ```

**cURL Example:**
```bash
curl -X POST "http://localhost:5034/api/users" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "jane.smith@example.com",
    "firstName": "Jane",
    "lastName": "Smith",
    "password": "Password@123",
    "isActive": true
  }'
```

---

### PUT /api/users/{id}

Update user.

**Authorization:** Required
**Permission:** `User.Update`

**Path Parameters:**
- `id` (guid) - User ID

**Request Body:**
```json
{
  "firstName": "Jane",
  "lastName": "Smith-Johnson",
  "departmentId": "789e0123-e89b-12d3-a456-426614174002",
  "isActive": true
}
```

**Response (200 OK):**
```json
{
  "message": "User updated successfully"
}
```

**Error Responses:**
- **404 Not Found** - User not found

**cURL Example:**
```bash
curl -X PUT "http://localhost:5034/api/users/123e4567-e89b-12d3-a456-426614174000" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Jane",
    "lastName": "Smith-Johnson",
    "isActive": true
  }'
```

---

### DELETE /api/users/{id}

Delete user (soft delete).

**Authorization:** Required
**Permission:** `User.Delete`

**Path Parameters:**
- `id` (guid) - User ID

**Response (200 OK):**
```json
{
  "message": "User deleted successfully"
}
```

**Error Responses:**
- **404 Not Found** - User not found
- **400 Bad Request** - Cannot delete yourself

**cURL Example:**
```bash
curl -X DELETE "http://localhost:5034/api/users/123e4567-e89b-12d3-a456-426614174000" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

---

### POST /api/users/{id}/assign-roles

Assign roles to user.

**Authorization:** Required
**Permission:** `User.AssignRoles`

**Path Parameters:**
- `id` (guid) - User ID

**Request Body:**
```json
{
  "roleIds": [
    "789e0123-e89b-12d3-a456-426614174002",
    "012e3456-e89b-12d3-a456-426614174003"
  ]
}
```

**Response (200 OK):**
```json
{
  "message": "Roles assigned successfully"
}
```

**Error Responses:**
- **404 Not Found** - User or role not found

**cURL Example:**
```bash
curl -X POST "http://localhost:5034/api/users/123e4567-e89b-12d3-a456-426614174000/assign-roles" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "roleIds": ["789e0123-e89b-12d3-a456-426614174002"]
  }'
```

---

### POST /api/users/{id}/upload-photo

Upload profile photo.

**Authorization:** Required (user can upload own photo)
**Permission:** None (users can update their own profile photo)

**Path Parameters:**
- `id` (guid) - User ID (must be current user or have User.Update permission)

**Request Body:** multipart/form-data
- `file` (file) - Image file (JPG, PNG, max 5MB)

**Response (200 OK):**
```json
{
  "photoUrl": "https://cdn.example.com/photos/user123.jpg"
}
```

**Error Responses:**
- **400 Bad Request** - Invalid file format or size
  ```json
  {
    "type": "ValidationError",
    "errors": {
      "File": ["File size must not exceed 5MB", "Only JPG and PNG files are allowed"]
    }
  }
  ```

**cURL Example:**
```bash
curl -X POST "http://localhost:5034/api/users/123e4567-e89b-12d3-a456-426614174000/upload-photo" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -F "file=@/path/to/photo.jpg"
```

---

### DELETE /api/users/{id}/photo

Delete profile photo.

**Authorization:** Required (user can delete own photo)
**Permission:** None (users can delete their own profile photo)

**Path Parameters:**
- `id` (guid) - User ID

**Response (200 OK):**
```json
{
  "message": "Photo deleted successfully"
}
```

**cURL Example:**
```bash
curl -X DELETE "http://localhost:5034/api/users/123e4567-e89b-12d3-a456-426614174000/photo" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

---

## Business Rules

### Email Validation
- Must be valid email format
- Must be unique within tenant
- Case-insensitive comparison

### Password Requirements
- Minimum 8 characters
- At least 1 uppercase letter
- At least 1 lowercase letter
- At least 1 digit
- At least 1 special character

### Profile Photos
- Allowed formats: JPG, PNG
- Maximum size: 5MB
- Automatically resized to 400x400px
- Stored in cloud storage (or local disk in development)

### Soft Delete
- Deleted users are marked with `IsDeleted=true`
- Email is preserved (cannot be reused)
- Can be restored by SuperAdmin (requires direct database access)

---

## Permissions

| Permission | Description |
|------------|-------------|
| User.Create | Create new users |
| User.Read | View user list and details |
| User.Update | Modify user information |
| User.Delete | Delete users |
| User.AssignRoles | Assign/remove roles from users |

---

**[← Back to API Reference](../API.md)**
