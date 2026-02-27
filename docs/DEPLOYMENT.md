# Deployment Guide

This guide covers deploying CoreEngine to production environments.

---

## 📋 Table of Contents

1. [Deployment Overview](#deployment-overview)
2. [Environment Configuration](#environment-configuration)
3. [Database Deployment](#database-deployment)
4. [Backend Deployment](#backend-deployment)
5. [Frontend Deployment](#frontend-deployment)
6. [IIS Deployment](#iis-deployment)
7. [Docker Deployment](#docker-deployment)
8. [Azure Deployment](#azure-deployment)
9. [Security Checklist](#security-checklist)
10. [Monitoring & Logging](#monitoring--logging)

---

## 🌍 Deployment Overview

### Deployment Architecture

```
┌──────────────────────────────────────────────────────────────┐
│                      🌐 Internet                              │
└────────────────────────┬─────────────────────────────────────┘
                         │
                         ↓
┌────────────────────────────────────────────────────────────┐
│                  🔒 Firewall / WAF                          │
│             (Azure Firewall / Cloudflare)                   │
└────────────────────────┬───────────────────────────────────┘
                         │
        ┌────────────────┴────────────────┐
        │                                 │
        ↓                                 ↓
┌───────────────┐               ┌──────────────────┐
│  CDN / Static │               │  Load Balancer   │
│    Content    │               │  (Azure LB/Nginx)│
│               │               └────────┬─────────┘
│  - React SPA  │                        │
│  - Images     │          ┌─────────────┼─────────────┐
│  - CSS/JS     │          │             │             │
└───────────────┘          ↓             ↓             ↓
                   ┌──────────┐  ┌──────────┐  ┌──────────┐
                   │ API Svr 1│  │ API Svr 2│  │ API Svr 3│
                   │ (IIS/.NET│  │ (IIS/.NET│  │ (IIS/.NET│
                   │   Core)  │  │   Core)  │  │   Core)  │
                   └────┬─────┘  └────┬─────┘  └────┬─────┘
                        │             │             │
                        └─────────────┼─────────────┘
                                      ↓
                            ┌─────────────────┐
                            │  SQL Server     │
                            │  (Primary)      │
                            │                 │
                            │  Always On AG   │
                            └────────┬────────┘
                                     │
                            ┌────────┴────────┐
                            │  SQL Server     │
                            │  (Secondary)    │
                            │  (Read Replica) │
                            └─────────────────┘
```

### Deployment Options

| Option | Best For | Complexity | Cost |
|--------|----------|------------|------|
| 🖥️ **IIS + SQL Server** | Windows shops, on-premise | Medium | Hardware cost |
| 🐳 **Docker + Kubernetes** | Cloud-native, scalability | High | Pay-per-use |
| ☁️ **Azure App Service** | Quick deployment, PaaS | Low | Pay-per-use |
| 🌊 **AWS ECS/Fargate** | AWS ecosystem | Medium | Pay-per-use |

---

## ⚙️ Environment Configuration

### Environment Variables

Create separate config files for each environment:

**appsettings.Production.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-sql.example.com;Database=CoreEngineDb;User Id=coreengine_app;Password=***;Encrypt=True;"
  },
  "Jwt": {
    "Issuer": "https://api.coreengine.com",
    "Audience": "https://app.coreengine.com",
    "ExpiryMinutes": 30
  },
  "Cors": {
    "AllowedOrigins": ["https://app.coreengine.com", "https://www.coreengine.com"]
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Warning",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/coreengine-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30
        }
      },
      {
        "Name": "Seq",
        "Args": {
          "serverUrl": "https://seq.coreengine.com"
        }
      }
    ]
  },
  "Hangfire": {
    "DashboardEnabled": false,
    "WorkerCount": 10
  },
  "Email": {
    "SmtpHost": "smtp.sendgrid.net",
    "SmtpPort": 587,
    "SmtpUser": "apikey",
    "SmtpPassword": "***",
    "FromEmail": "noreply@coreengine.com",
    "FromName": "CoreEngine"
  }
}
```

### Secrets Management

**🔐 Never commit secrets to Git!**

**Option 1: Azure Key Vault**
```csharp
// Program.cs
builder.Configuration.AddAzureKeyVault(
    new Uri("https://coreengine-kv.vault.azure.net/"),
    new DefaultAzureCredential());
```

**Option 2: Environment Variables**
```bash
# Set via PowerShell (Windows)
$env:ConnectionStrings__DefaultConnection="Server=..."
$env:Jwt__Secret="your-secret-key"

# Set via bash (Linux)
export ConnectionStrings__DefaultConnection="Server=..."
export JWT__SECRET="your-secret-key"
```

**Option 3: User Secrets (Development)**
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=..."
dotnet user-secrets set "Jwt:Secret" "your-secret-key"
```

---

## 💾 Database Deployment

### Pre-Deployment Checklist

- [ ] Backup production database
- [ ] Test migrations in staging
- [ ] Review migration scripts for breaking changes
- [ ] Plan maintenance window
- [ ] Notify users of downtime

### Option 1: Automated Migration (Recommended for Dev/Staging)

```bash
# Generate migration script
dotnet ef migrations script --idempotent --output migration.sql

# Review the script
cat migration.sql

# Apply to database
sqlcmd -S prod-server -d CoreEngineDb -i migration.sql
```

### Option 2: Manual Deployment (Recommended for Production)

```bash
# 1. Generate migration bundle
cd src/CoreEngine.API
dotnet ef migrations bundle --self-contained -r win-x64

# 2. Copy efbundle.exe to production server

# 3. Run on production (with connection string)
efbundle.exe --connection "Server=prod-server;Database=CoreEngineDb;..."
```

### Database Security

```sql
-- Create application user with minimal permissions
CREATE USER [coreengine_app] WITH PASSWORD = 'StrongPassword123!';

-- Grant only necessary permissions
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO [coreengine_app];

-- Deny dangerous permissions
DENY ALTER, DROP, CREATE TO [coreengine_app];

-- Enable encryption (TDE)
CREATE DATABASE ENCRYPTION KEY
WITH ALGORITHM = AES_256
ENCRYPTION BY SERVER CERTIFICATE CoreEngineCert;

ALTER DATABASE CoreEngineDb SET ENCRYPTION ON;
```

---

## 🔧 Backend Deployment

### Build for Production

```bash
cd src/CoreEngine.API

# Publish self-contained (includes .NET runtime)
dotnet publish -c Release -r win-x64 --self-contained true -o ./publish

# Or framework-dependent (smaller, requires .NET runtime on server)
dotnet publish -c Release -o ./publish
```

### Deployment Package Structure

```
publish/
├── CoreEngine.API.exe          # Executable
├── CoreEngine.API.dll          # Main assembly
├── appsettings.json            # Base config
├── appsettings.Production.json # Production config
├── web.config                  # IIS configuration
├── wwwroot/                    # Static files (if any)
└── [Dependencies...]           # All DLL dependencies
```

### Performance Optimization

**web.config (for IIS):**
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore processPath="dotnet"
                  arguments=".\CoreEngine.API.dll"
                  stdoutLogEnabled="true"
                  stdoutLogFile=".\logs\stdout"
                  hostingModel="inprocess">
        <environmentVariables>
          <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
          <environmentVariable name="ASPNETCORE_HTTPS_PORT" value="443" />
        </environmentVariables>
      </aspNetCore>

      <!-- Enable response compression -->
      <urlCompression doDynamicCompression="true" doStaticCompression="true" />

      <!-- Enable response caching -->
      <httpProtocol>
        <customHeaders>
          <add name="Cache-Control" value="public, max-age=31536000" />
        </customHeaders>
      </httpProtocol>
    </system.webServer>
  </location>
</configuration>
```

---

## 🎨 Frontend Deployment

### Build for Production

```bash
cd frontend

# Install dependencies
npm ci --production=false

# Build optimized bundle
npm run build

# Output directory: dist/
```

### Build Output

```
dist/
├── index.html                  # Entry point
├── assets/
│   ├── index-[hash].js         # Bundled JavaScript (minified)
│   ├── index-[hash].css        # Bundled CSS (minified)
│   └── [images/fonts]          # Static assets
└── vite.svg                    # Favicon
```

### Deployment Options

#### Option 1: CDN (Recommended)

**Azure Blob Storage + CDN:**
```bash
# Upload to Azure Blob Storage
az storage blob upload-batch -d '$web' -s ./dist --account-name coreengine

# Configure CDN
az cdn endpoint create \
  --resource-group coreengine-rg \
  --profile-name coreengine-cdn \
  --name app \
  --origin coreengine.blob.core.windows.net
```

**Cloudflare Pages:**
```bash
# Install Wrangler CLI
npm install -g wrangler

# Deploy
wrangler pages publish dist --project-name coreengine
```

#### Option 2: IIS Static Hosting

```xml
<!-- web.config in dist/ -->
<?xml version="1.0"?>
<configuration>
  <system.webServer>
    <rewrite>
      <rules>
        <!-- Redirect HTTP to HTTPS -->
        <rule name="HTTPS Redirect" stopProcessing="true">
          <match url="(.*)" />
          <conditions>
            <add input="{HTTPS}" pattern="^OFF$" />
          </conditions>
          <action type="Redirect" url="https://{HTTP_HOST}/{R:1}" redirectType="Permanent" />
        </rule>

        <!-- SPA Routing -->
        <rule name="React Routes" stopProcessing="true">
          <match url=".*" />
          <conditions logicalGrouping="MatchAll">
            <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
            <add input="{REQUEST_FILENAME}" matchType="IsDirectory" negate="true" />
          </conditions>
          <action type="Rewrite" url="/" />
        </rule>
      </rules>
    </rewrite>

    <!-- Security Headers -->
    <httpProtocol>
      <customHeaders>
        <add name="X-Content-Type-Options" value="nosniff" />
        <add name="X-Frame-Options" value="DENY" />
        <add name="X-XSS-Protection" value="1; mode=block" />
        <add name="Referrer-Policy" value="no-referrer" />
        <add name="Permissions-Policy" value="geolocation=(), microphone=()" />
      </customHeaders>
    </httpProtocol>

    <!-- Compression -->
    <urlCompression doStaticCompression="true" doDynamicCompression="true" />

    <!-- Static file caching -->
    <staticContent>
      <clientCache cacheControlMode="UseMaxAge" cacheControlMaxAge="365.00:00:00" />
    </staticContent>
  </system.webServer>
</configuration>
```

---

## 🖥️ IIS Deployment

### Prerequisites

```powershell
# Install IIS with necessary features
Install-WindowsFeature -name Web-Server -IncludeManagementTools

# Install .NET Core Hosting Bundle
# Download from: https://dotnet.microsoft.com/download/dotnet/9.0
```

### Step-by-Step Deployment

**1. Create Application Pool:**
```powershell
Import-Module WebAdministration

New-WebAppPool -Name "CoreEngine" -Force
Set-ItemProperty IIS:\AppPools\CoreEngine -Name "managedRuntimeVersion" -Value ""
Set-ItemProperty IIS:\AppPools\CoreEngine -Name "processModel.identityType" -Value "ApplicationPoolIdentity"
```

**2. Create Website:**
```powershell
# Create website directory
New-Item -ItemType Directory -Path "C:\inetpub\coreengine" -Force

# Create IIS website
New-Website -Name "CoreEngine" `
            -PhysicalPath "C:\inetpub\coreengine" `
            -ApplicationPool "CoreEngine" `
            -Port 443 `
            -HostHeader "api.coreengine.com" `
            -Ssl
```

**3. Configure SSL Certificate:**
```powershell
# Import SSL certificate
$cert = Import-PfxCertificate -FilePath "coreengine.pfx" -CertStoreLocation Cert:\LocalMachine\My -Password (ConvertTo-SecureString -String "password" -AsPlainText -Force)

# Bind to website
New-WebBinding -Name "CoreEngine" -IP "*" -Port 443 -Protocol https
$binding = Get-WebBinding -Name "CoreEngine" -Protocol "https"
$binding.AddSslCertificate($cert.Thumbprint, "my")
```

**4. Set Permissions:**
```powershell
# Grant IIS_IUSRS read permission
icacls "C:\inetpub\coreengine" /grant "IIS_IUSRS:(OI)(CI)R" /T

# Grant write permission to logs folder
icacls "C:\inetpub\coreengine\logs" /grant "IIS_IUSRS:(OI)(CI)W" /T
```

**5. Configure Application:**
```powershell
# Set environment variable
Set-WebConfigurationProperty -Filter "/system.webServer/aspNetCore/environmentVariables/add[@name='ASPNETCORE_ENVIRONMENT']" `
                              -PSPath "IIS:\Sites\CoreEngine" `
                              -Name "value" `
                              -Value "Production"
```

### Health Check Monitoring

```powershell
# Enable Application Initialization
Set-WebConfigurationProperty -PSPath 'MACHINE/WEBROOT/APPHOST' `
                              -Filter "system.webServer/applicationInitialization" `
                              -Name "doAppInitAfterRestart" `
                              -Value "True"

# Configure warm-up request
Add-WebConfigurationProperty -PSPath 'MACHINE/WEBROOT/APPHOST' `
                              -Filter "system.webServer/applicationInitialization" `
                              -Name "." `
                              -Value @{initializationPage='/health'}
```

---

## 🐳 Docker Deployment

### Dockerfile (Backend)

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/CoreEngine.API/CoreEngine.API.csproj", "CoreEngine.API/"]
COPY ["src/CoreEngine.Application/CoreEngine.Application.csproj", "CoreEngine.Application/"]
COPY ["src/CoreEngine.Domain/CoreEngine.Domain.csproj", "CoreEngine.Domain/"]
COPY ["src/CoreEngine.Infrastructure/CoreEngine.Infrastructure.csproj", "CoreEngine.Infrastructure/"]
COPY ["src/CoreEngine.Shared/CoreEngine.Shared.csproj", "CoreEngine.Shared/"]

RUN dotnet restore "CoreEngine.API/CoreEngine.API.csproj"

# Copy everything else and build
COPY src/ .
WORKDIR "/src/CoreEngine.API"
RUN dotnet build "CoreEngine.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "CoreEngine.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Copy published app
COPY --from=publish /app/publish .

# Create non-root user
RUN groupadd -r coreengine && useradd -r -g coreengine coreengine
USER coreengine

ENTRYPOINT ["dotnet", "CoreEngine.API.dll"]
```

### Dockerfile (Frontend)

```dockerfile
# Build stage
FROM node:22-alpine AS build
WORKDIR /app

# Copy package files
COPY frontend/package*.json ./
RUN npm ci

# Copy source and build
COPY frontend/ ./
RUN npm run build

# Production stage with nginx
FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html

# Copy nginx config
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

### docker-compose.yml

```yaml
version: '3.8'

services:
  database:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      SA_PASSWORD: "YourStrong!Passw0rd"
      ACCEPT_EULA: "Y"
    ports:
      - "1433:1433"
    volumes:
      - sqldata:/var/opt/mssql
    networks:
      - coreengine

  backend:
    build:
      context: .
      dockerfile: Dockerfile.backend
    environment:
      ASPNETCORE_ENVIRONMENT: Production
      ConnectionStrings__DefaultConnection: "Server=database;Database=CoreEngineDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True"
      Jwt__Secret: "your-super-secret-jwt-key-minimum-32-characters"
    ports:
      - "5034:80"
    depends_on:
      - database
    networks:
      - coreengine

  frontend:
    build:
      context: .
      dockerfile: Dockerfile.frontend
    ports:
      - "5173:80"
    depends_on:
      - backend
    networks:
      - coreengine

volumes:
  sqldata:

networks:
  coreengine:
    driver: bridge
```

### Deploy to Docker

```bash
# Build images
docker-compose build

# Start services
docker-compose up -d

# View logs
docker-compose logs -f backend

# Stop services
docker-compose down
```

---

## ☁️ Azure Deployment

### Azure Resources Needed

```
Resource Group: coreengine-prod-rg
├── App Service Plan (P1V2 or higher)
├── App Service (Backend API)
├── Static Web App (Frontend)
├── SQL Server (Managed Instance)
├── SQL Database (CoreEngineDb)
├── Key Vault (Secrets)
├── Application Insights (Monitoring)
├── Storage Account (File uploads)
└── CDN Profile (Static content delivery)
```

### Deploy Backend to Azure App Service

```bash
# Login to Azure
az login

# Create resource group
az group create --name coreengine-prod-rg --location eastus

# Create App Service Plan
az appservice plan create \
  --name coreengine-plan \
  --resource-group coreengine-prod-rg \
  --sku P1V2 \
  --is-linux

# Create Web App
az webapp create \
  --name coreengine-api \
  --resource-group coreengine-prod-rg \
  --plan coreengine-plan \
  --runtime "DOTNET|9.0"

# Configure app settings
az webapp config appsettings set \
  --name coreengine-api \
  --resource-group coreengine-prod-rg \
  --settings ASPNETCORE_ENVIRONMENT=Production

# Deploy from local publish folder
cd src/CoreEngine.API
dotnet publish -c Release -o ./publish
cd publish
zip -r ../deploy.zip *
az webapp deployment source config-zip \
  --name coreengine-api \
  --resource-group coreengine-prod-rg \
  --src ../deploy.zip
```

### Deploy Frontend to Azure Static Web Apps

```bash
# Create Static Web App
az staticwebapp create \
  --name coreengine-app \
  --resource-group coreengine-prod-rg \
  --location eastus2 \
  --source https://github.com/yourusername/coreengine \
  --branch main \
  --app-location "/frontend" \
  --output-location "dist"

# Manual deployment
cd frontend
npm run build
az staticwebapp deploy \
  --name coreengine-app \
  --resource-group coreengine-prod-rg \
  --source ./dist
```

---

## 🔒 Security Checklist

### Pre-Deployment

- [ ] **Remove debug symbols** from published binaries
- [ ] **Set environment to Production** (`ASPNETCORE_ENVIRONMENT=Production`)
- [ ] **Disable Swagger** in production (`if (app.Environment.IsDevelopment())`)
- [ ] **Disable detailed errors** (no stack traces to users)
- [ ] **Enable HTTPS only** (redirect HTTP → HTTPS)
- [ ] **Configure CORS** (whitelist specific origins)
- [ ] **Rotate all secrets** (JWT, DB passwords, API keys)
- [ ] **Enable SQL encryption** (TDE for data at rest)
- [ ] **Use parameterized queries** (prevent SQL injection)
- [ ] **Implement rate limiting** (already configured in CoreEngine)
- [ ] **Enable account lockout** (already configured)
- [ ] **Use strong passwords** (BCrypt hashing already in place)

### Post-Deployment

- [ ] **Enable SSL/TLS 1.3** minimum
- [ ] **Configure CSP headers** (Content Security Policy)
- [ ] **Enable HSTS** (HTTP Strict Transport Security)
- [ ] **Implement WAF** (Web Application Firewall)
- [ ] **Set up DDoS protection**
- [ ] **Configure backup strategy**
- [ ] **Enable audit logging**
- [ ] **Set up intrusion detection**
- [ ] **Perform penetration testing**
- [ ] **Review dependency vulnerabilities** (`dotnet list package --vulnerable`)

---

## 📊 Monitoring & Logging

### Application Insights (Azure)

```csharp
// Program.cs
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
});
```

### Health Checks Dashboard

```
https://api.coreengine.com/health

Response:
{
  "status": "Healthy",
  "totalDuration": "00:00:00.0234567",
  "entries": {
    "Database": {
      "status": "Healthy",
      "duration": "00:00:00.0123456"
    },
    "Hangfire": {
      "status": "Healthy"
    },
    "SignalR": {
      "status": "Healthy"
    }
  }
}
```

### Logging Strategy

**Log Levels:**
- 🔴 **Critical**: System failures, data loss
- 🟠 **Error**: Operation failures, exceptions
- 🟡 **Warning**: Unexpected behavior, deprecations
- 🟢 **Information**: General flow, startup/shutdown
- 🔵 **Debug**: Detailed diagnostic info (dev only)

**Serilog Configuration:**
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.Hosting.Lifetime": "Information",
        "Hangfire": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/coreengine-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30,
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "Seq",
        "Args": {
          "serverUrl": "https://seq.coreengine.com",
          "apiKey": "***"
        }
      }
    ],
    "Enrich": ["FromLogContext", "WithMachineName", "WithThreadId"]
  }
}
```

---

## 🚀 Blue-Green Deployment

```
┌─────────────────────────────────────────┐
│          Load Balancer (ALB)            │
└──────────────┬──────────────────────────┘
               │
        ┌──────┴──────┐
        │             │
        ↓             ↓ (switch traffic here)
┌──────────────┐ ┌──────────────┐
│ BLUE         │ │ GREEN        │
│ (Current)    │ │ (New)        │
│ v1.0.0       │ │ v1.1.0       │
│              │ │              │
│ - API Svr 1  │ │ - API Svr 1  │
│ - API Svr 2  │ │ - API Svr 2  │
└──────────────┘ └──────────────┘
```

**Steps:**
1. Deploy new version to GREEN environment
2. Run smoke tests on GREEN
3. Switch 10% traffic to GREEN (canary)
4. Monitor metrics for 15 minutes
5. Gradually increase to 100%
6. Keep BLUE as rollback option for 24 hours

---

## 📋 Deployment Runbook

```
Pre-Deployment (1 week before):
☐ Create deployment plan
☐ Schedule maintenance window
☐ Notify stakeholders
☐ Prepare rollback plan
☐ Test in staging environment
☐ Review change list
☐ Update documentation

Deployment Day (T-1 hour):
☐ Backup production database
☐ Verify backup integrity
☐ Put application in maintenance mode
☐ Stop application pool / services

Deployment (T-0):
☐ Deploy database migrations
☐ Deploy backend application
☐ Deploy frontend application
☐ Update configuration
☐ Start application pool / services
☐ Run smoke tests
☐ Verify all services healthy

Post-Deployment (T+1 hour):
☐ Monitor error logs
☐ Check application metrics
☐ Verify key user flows
☐ Remove maintenance mode
☐ Notify stakeholders of success
☐ Document lessons learned

If Issues Detected:
☐ Execute rollback plan
☐ Restore database backup
☐ Deploy previous version
☐ Notify stakeholders
☐ Schedule post-mortem
```

---

## 🆘 Rollback Procedure

```bash
# 1. Stop current application
iisreset /stop

# 2. Restore database backup
sqlcmd -S prod-server -Q "
  ALTER DATABASE CoreEngineDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
  RESTORE DATABASE CoreEngineDb FROM DISK = 'C:\Backup\CoreEngineDb_20260227.bak' WITH REPLACE;
  ALTER DATABASE CoreEngineDb SET MULTI_USER;
"

# 3. Deploy previous version
cd C:\Deployments\v1.0.0
Copy-Item -Recurse -Force * C:\inetpub\coreengine\

# 4. Start application
iisreset /start

# 5. Verify health
curl https://api.coreengine.com/health
```

---

**Next:** [← Back to README](../README.md) | [Modules Documentation →](MODULES.md)
