# 🔐 Authentication API

JWT-based authentication with refresh tokens.

---

## Endpoints

### POST /api/auth/login

Login with email and password to obtain access and refresh tokens.

**Authorization:** None (public endpoint)

**Request Body:**
```json
{
  "email": "admin@coreengine.local",
  "password": "Admin@123"
}
```

**Response (200 OK):**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "abc123def456...",
  "expiresAt": "2026-03-01T10:30:00Z",
  "user": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "email": "admin@coreengine.local",
    "firstName": "System",
    "lastName": "Administrator",
    "profilePhotoUrl": null,
    "tenantId": "00000000-0000-0000-0000-000000000001",
    "tenantName": "Default",
    "roles": ["SuperAdmin"],
    "permissions": ["User.Create", "User.Read", ...]
  }
}
```

**Error Responses:**

- **400 Bad Request** - Invalid request format
  ```json
  {
    "type": "ValidationError",
    "title": "Validation failed",
    "errors": {
      "Email": ["Email is required"]
    }
  }
  ```

- **401 Unauthorized** - Invalid credentials
  ```json
  {
    "type": "AuthenticationError",
    "title": "Authentication failed",
    "detail": "Invalid email or password"
  }
  ```

- **423 Locked** - Account locked
  ```json
  {
    "type": "AccountLockedError",
    "title": "Account locked",
    "detail": "Account is locked due to multiple failed login attempts. Try again in 15 minutes."
  }
  ```

**cURL Example:**
```bash
curl -X POST "http://localhost:5034/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@coreengine.local",
    "password": "Admin@123"
  }'
```

---

### POST /api/auth/refresh

Refresh access token using refresh token.

**Authorization:** None (public endpoint)

**Request Body:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "abc123def456..."
}
```

**Response (200 OK):**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "def456ghi789...",
  "expiresAt": "2026-03-01T11:30:00Z"
}
```

**Error Responses:**

- **401 Unauthorized** - Invalid or expired refresh token
  ```json
  {
    "type": "AuthenticationError",
    "title": "Invalid refresh token",
    "detail": "The refresh token is invalid or has expired"
  }
  ```

**cURL Example:**
```bash
curl -X POST "http://localhost:5034/api/auth/refresh" \
  -H "Content-Type: application/json" \
  -d '{
    "accessToken": "CURRENT_ACCESS_TOKEN",
    "refreshToken": "CURRENT_REFRESH_TOKEN"
  }'
```

---

### POST /api/auth/logout

Logout and invalidate refresh token.

**Authorization:** Required (Bearer token)

**Request Body:**
```json
{
  "refreshToken": "abc123def456..."
}
```

**Response (200 OK):**
```json
{
  "message": "Logged out successfully"
}
```

**cURL Example:**
```bash
curl -X POST "http://localhost:5034/api/auth/logout" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "refreshToken": "YOUR_REFRESH_TOKEN"
  }'
```

---

### POST /api/auth/change-password

Change current user's password.

**Authorization:** Required (Bearer token)

**Request Body:**
```json
{
  "currentPassword": "OldPassword@123",
  "newPassword": "NewPassword@456"
}
```

**Response (200 OK):**
```json
{
  "message": "Password changed successfully"
}
```

**Error Responses:**

- **400 Bad Request** - Password doesn't meet requirements
  ```json
  {
    "type": "ValidationError",
    "title": "Password validation failed",
    "errors": {
      "NewPassword": [
        "Password must be at least 8 characters",
        "Password must contain uppercase letter",
        "Password must contain special character"
      ]
    }
  }
  ```

- **401 Unauthorized** - Current password incorrect
  ```json
  {
    "type": "AuthenticationError",
    "title": "Invalid current password"
  }
  ```

**cURL Example:**
```bash
curl -X POST "http://localhost:5034/api/auth/change-password" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "currentPassword": "Admin@123",
    "newPassword": "NewPassword@456"
  }'
```

---

### POST /api/auth/forgot-password

Request password reset email.

**Authorization:** None (public endpoint)

**Request Body:**
```json
{
  "email": "user@example.com"
}
```

**Response (200 OK):**
```json
{
  "message": "If an account with that email exists, a password reset link has been sent"
}
```

**Note:** Always returns 200 OK even if email doesn't exist (security best practice).

**cURL Example:**
```bash
curl -X POST "http://localhost:5034/api/auth/forgot-password" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com"
  }'
```

---

### POST /api/auth/reset-password

Reset password using token from email.

**Authorization:** None (public endpoint)

**Request Body:**
```json
{
  "email": "user@example.com",
  "token": "abc123def456...",
  "newPassword": "NewPassword@123"
}
```

**Response (200 OK):**
```json
{
  "message": "Password reset successfully"
}
```

**Error Responses:**

- **400 Bad Request** - Invalid or expired token
  ```json
  {
    "type": "ValidationError",
    "title": "Invalid reset token",
    "detail": "The password reset token is invalid or has expired"
  }
  ```

**cURL Example:**
```bash
curl -X POST "http://localhost:5034/api/auth/reset-password" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "token": "TOKEN_FROM_EMAIL",
    "newPassword": "NewPassword@123"
  }'
```

---

## Security Features

### Password Requirements

- Minimum 8 characters
- At least 1 uppercase letter
- At least 1 lowercase letter
- At least 1 digit
- At least 1 special character (!@#$%^&*)

### Account Lockout

- **Trigger:** 5 failed login attempts
- **Duration:** 15 minutes
- **Notification:** Email sent on lockout

### Token Security

- **Access Token:** 1 hour expiration
- **Refresh Token:** 30 days expiration
- **Token Rotation:** Refresh tokens are rotated on use
- **Revocation:** Logout invalidates refresh token

### Rate Limiting

- **Login Endpoint:** 10 requests per minute per IP
- **Other Auth Endpoints:** 10 requests per minute per IP

---

## JWT Token Structure

### Access Token Claims

```json
{
  "sub": "123e4567-e89b-12d3-a456-426614174000",
  "email": "admin@coreengine.local",
  "given_name": "System",
  "family_name": "Administrator",
  "tenantId": "00000000-0000-0000-0000-000000000001",
  "permissions": [
    "User.Create",
    "User.Read",
    "User.Update",
    "User.Delete"
  ],
  "exp": 1709140800,
  "iss": "CoreEngine",
  "aud": "CoreEngineClient"
}
```

---

## Error Codes

| Code | Description |
|------|-------------|
| 400 | Validation error (invalid email format, password too weak) |
| 401 | Authentication failed (invalid credentials, expired token) |
| 423 | Account locked (too many failed attempts) |
| 429 | Rate limit exceeded |
| 500 | Internal server error |

---

**[← Back to API Reference](../API.md)**
