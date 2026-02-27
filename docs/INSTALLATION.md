# Installation Guide

This guide will walk you through setting up CoreEngine on your local development machine or production server.

---

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Database Setup](#database-setup)
3. [Backend Setup](#backend-setup)
4. [Frontend Setup](#frontend-setup)
5. [First Run](#first-run)
6. [Verification](#verification)
7. [Troubleshooting](#troubleshooting)

---

## Prerequisites

Before installing CoreEngine, ensure you have the following installed:

### Required Software

| Software | Version | Download Link |
|----------|---------|---------------|
| **.NET SDK** | 9.0.305+ | [Download](https://dotnet.microsoft.com/download/dotnet/9.0) |
| **Node.js** | 22.19.0+ | [Download](https://nodejs.org/) |
| **npm** | 10.9.3+ | (Included with Node.js) |
| **SQL Server** | 2022 Express+ | [Download](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) |
| **Git** | Latest | [Download](https://git-scm.com/) |

### Optional Software

| Software | Purpose |
|----------|---------|
| **Visual Studio 2022** | Backend development IDE |
| **VS Code** | Frontend development IDE |
| **SQL Server Management Studio (SSMS)** | Database management |
| **Postman** | API testing |

### System Requirements

**Minimum:**
- OS: Windows 10/11, macOS 11+, or Linux
- RAM: 8 GB
- Disk Space: 2 GB free

**Recommended:**
- OS: Windows 11 or macOS 13+
- RAM: 16 GB
- Disk Space: 5 GB free
- SSD storage for better performance

---

## Database Setup

### Step 1: Install SQL Server

1. Download **SQL Server 2022 Express** (free)
2. Run the installer
3. Choose **Basic** installation
4. Accept defaults and install
5. Note the server name (usually `localhost` or `localhost\SQLEXPRESS`)

### Step 2: Verify SQL Server Installation

```bash
# Test connection using sqlcmd
sqlcmd -S localhost -E -Q "SELECT @@VERSION"
```

Expected output: SQL Server version information

### Step 3: Configure Windows Authentication

CoreEngine uses **Windows Authentication** by default. No additional configuration needed.

**Alternative: SQL Server Authentication**

If you prefer SQL Authentication:

1. Enable SQL Authentication in SQL Server Configuration Manager
2. Create a SQL user with `db_owner` permissions
3. Update connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CoreEngineDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True"
  }
}
```

---

## Backend Setup

### Step 1: Clone the Repository

```bash
git clone https://github.com/saurabhwebdev/coreengine.git
cd coreengine
```

### Step 2: Restore NuGet Packages

```bash
cd src/CoreEngine.API
dotnet restore
```

### Step 3: Configure JWT Secret

CoreEngine uses **.NET User Secrets** for the JWT signing key (never committed to Git):

```bash
cd src/CoreEngine.API
dotnet user-secrets set "Jwt:Secret" "your-super-secret-jwt-key-minimum-32-characters-long"
```

**Important:** Use a strong, random secret in production!

### Step 4: Update Configuration (Optional)

Edit `src/CoreEngine.API/appsettings.json` if needed:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CoreEngineDb;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Jwt": {
    "Issuer": "CoreEngine",
    "Audience": "CoreEngine",
    "ExpiryMinutes": 60
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:5173"]
  }
}
```

### Step 5: Create Database

```bash
# Make sure you're in src/CoreEngine.API directory
dotnet ef database update
```

This command will:
- Create the `CoreEngineDb` database
- Run all migrations
- Seed initial data (default tenant, admin user, roles, permissions)

**Expected output:**
```
Applying migration '20260227161938_InitialCreate'.
Applying migration '20260227175215_Phase2CoreEngines'.
...
Done.
```

### Step 6: Verify Database

```bash
sqlcmd -S localhost -E -d CoreEngineDb -Q "SELECT COUNT(*) FROM Users"
```

Expected: `(1 rows affected)` — Admin user created

### Step 7: Build Backend

```bash
dotnet build --no-incremental
```

Expected output: `Build succeeded. 0 Warning(s) 0 Error(s)`

---

## Frontend Setup

### Step 1: Navigate to Frontend Directory

```bash
cd ../../frontend
```

### Step 2: Install npm Packages

```bash
npm install
```

This will install:
- React 18
- TypeScript
- Vite
- Ant Design 5
- @tanstack/react-query
- axios
- @microsoft/signalr
- lottie-react
- And all other dependencies

### Step 3: Verify Installation

```bash
npm list --depth=0
```

Expected output: List of installed packages without errors

### Step 4: Build Frontend (Optional)

```bash
npm run build
```

Expected: `✓ built in XX.XXs`

---

## First Run

### Option 1: One-Click Start (Windows)

```bash
# From root directory
start.bat
```

This will:
1. Start the backend API
2. Start the frontend dev server
3. Open browser at http://localhost:5173

### Option 2: Manual Start

**Terminal 1 — Backend:**
```bash
cd src/CoreEngine.API
dotnet run
```

Backend will start at: `http://localhost:5034`

**Terminal 2 — Frontend:**
```bash
cd frontend
npm run dev
```

Frontend will start at: `http://localhost:5173`

### Step 2: Access the Application

1. Open browser: **http://localhost:5173**
2. Login with default credentials:
   - **Email:** `admin@coreengine.local`
   - **Password:** `Admin@123`

### Step 3: Explore the Dashboard

You should see:
- ✅ Dashboard with statistics cards
- ✅ Sidebar navigation with all modules
- ✅ Tenant switcher (Default tenant)
- ✅ User dropdown with profile options

---

## Verification

### Backend Health Checks

```bash
# Check if API is running
curl http://localhost:5034/health

# Check Swagger UI
# Open: http://localhost:5034/swagger
```

### Frontend Checks

```bash
# Verify build works
cd frontend
npm run build

# Check for TypeScript errors
npm run type-check
```

### Database Checks

```sql
-- Check all tables exist
SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'
-- Expected: 30+ tables

-- Check admin user
SELECT Email, EmailConfirmed FROM Users WHERE Email = 'admin@coreengine.local'
-- Expected: admin@coreengine.local, True

-- Check permissions seeded
SELECT COUNT(*) FROM Permissions
-- Expected: 60+ permissions

-- Check Hangfire tables
SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'HangFire'
-- Expected: 15+ Hangfire tables
```

### Hangfire Dashboard

1. Navigate to: **http://localhost:5034/hangfire**
2. You should see:
   - 2 recurring jobs (process-email-queue, cleanup-audit-logs)
   - Server status (20 workers)
   - Job statistics

---

## Troubleshooting

### Issue: "Unable to connect to SQL Server"

**Solution:**
1. Verify SQL Server is running:
   ```bash
   # Windows Services
   services.msc
   # Look for "SQL Server (SQLEXPRESS)" - should be "Running"
   ```
2. Test connection:
   ```bash
   sqlcmd -S localhost -E -Q "SELECT 1"
   ```
3. Update connection string with correct server name

### Issue: "Migration failed"

**Solution:**
```bash
# Drop database and recreate
sqlcmd -S localhost -E -Q "DROP DATABASE CoreEngineDb"
cd src/CoreEngine.API
dotnet ef database update
```

### Issue: "JWT Secret not configured"

**Solution:**
```bash
cd src/CoreEngine.API
dotnet user-secrets set "Jwt:Secret" "your-minimum-32-character-secret-key-here"
```

### Issue: "Port 5034 already in use"

**Solution:**
```bash
# Find process using port
netstat -ano | findstr :5034

# Kill process (Windows)
taskkill /PID <process_id> /F

# Or change port in launchSettings.json
```

### Issue: "npm install fails"

**Solution:**
```bash
# Clear cache and retry
npm cache clean --force
rm -rf node_modules package-lock.json
npm install
```

### Issue: "Frontend shows blank page"

**Solution:**
1. Check browser console for errors (F12)
2. Verify backend is running (`curl http://localhost:5034/health`)
3. Check CORS configuration in `appsettings.json`
4. Clear browser cache (Ctrl+Shift+Delete)

### Issue: "Hangfire dashboard is blank"

**Solution:**
```bash
# Restart backend to initialize Hangfire schema
cd src/CoreEngine.API
dotnet run
```

Hangfire creates tables on first run. Wait 15 seconds, then check dashboard.

### Issue: "SignalR connection fails"

**Solution:**
1. Check browser console for SignalR errors
2. Verify backend is running
3. Check SignalR hub configuration in `Program.cs`
4. Try accessing: `http://localhost:5034/hubs/notifications/negotiate`

---

## Next Steps

Now that CoreEngine is installed and running:

1. **Explore Modules** → Read [MODULES.md](MODULES.md)
2. **Test API** → Read [API.md](API.md)
3. **Build Features** → Read [DEVELOPMENT.md](DEVELOPMENT.md)
4. **Deploy** → Read [DEPLOYMENT.md](DEPLOYMENT.md)

---

## Quick Reference

### Default Credentials

| Account | Email | Password |
|---------|-------|----------|
| Super Admin | admin@coreengine.local | Admin@123 |

### Default URLs

| Service | URL |
|---------|-----|
| Frontend | http://localhost:5173 |
| Backend API | http://localhost:5034 |
| Swagger UI | http://localhost:5034/swagger |
| Hangfire Dashboard | http://localhost:5034/hangfire |

### Default Database

| Property | Value |
|----------|-------|
| Server | localhost |
| Database | CoreEngineDb |
| Authentication | Windows Authentication |

---

## Support

If you encounter issues not covered here:

1. Check [GitHub Issues](https://github.com/saurabhwebdev/coreengine/issues)
2. Review [ARCHITECTURE.md](../ARCHITECTURE.md)
3. Search error messages in project documentation

---

**Next:** [Modules Documentation →](MODULES.md)
