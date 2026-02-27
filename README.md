# CoreEngine

> **Enterprise-grade reusable framework** — A production-ready core engine that any domain application can plug into.

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-18-61DAFB?logo=react)](https://reactjs.org/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.0-3178C6?logo=typescript)](https://www.typescriptlang.org/)
[![License](https://img.shields.io/badge/License-Private-red)](https://github.com/saurabhwebdev/coreengine)

CoreEngine is a **100% complete, production-ready enterprise framework** built with Clean Architecture, CQRS, and modern best practices. It provides all the infrastructure modules needed to build sophisticated domain-specific applications (CRM, ERP, HRMS, etc.) without reinventing the wheel.

---

## 🎯 **What is CoreEngine?**

CoreEngine is a **reusable foundation** for enterprise applications. Instead of building authentication, multi-tenancy, workflows, notifications, and reporting from scratch for every project, you plug your domain logic into CoreEngine and get all these features instantly.

**Think of it as:**
- A **complete SaaS boilerplate** with multi-tenancy out of the box
- An **enterprise application skeleton** with 23 pre-built modules
- A **Clean Architecture reference implementation** for .NET + React
- A **production-ready starting point** for any business application

---

## ✨ **Key Features**

### 🏢 **Multi-Tenancy**
- Tenant isolation via custom EF Core global query filters
- Per-tenant theming (logo, colors, branding)
- Tenant-scoped data with automatic filtering
- Subdomain-based tenant resolution

### 👥 **Identity & Access Management (IAM)**
- User management with ASP.NET Core Identity
- Role-based access control (RBAC)
- Flat permission catalog (`Module.Action`)
- Department organization
- JWT authentication with refresh tokens
- Account lockout (5 attempts / 15 min)
- Password hashing with BCrypt

### 📋 **Custom Workflow Engine**
- JSON-based workflow definitions
- Multi-level approval workflows
- Task assignment with priority & due dates
- Workflow instances with status tracking
- Auto-advance on approval/rejection
- My Tasks page for pending approvals
- Workflow statistics dashboard

### 📧 **Email & Notifications**
- In-app notification center
- Email templates with variable substitution
- Email queue with retry logic (max 3 attempts)
- MailKit SMTP integration
- Real-time notifications via SignalR

### 🔄 **Background Jobs (Hangfire)**
- SQL Server persistence
- Recurring job scheduling
- Email queue processing (every minute)
- Audit log cleanup (daily at 2 AM)
- Job monitoring dashboard
- 20 concurrent workers

### 🔴 **Real-time (SignalR)**
- Real-time notification delivery
- User presence tracking (online/offline)
- Auto-reconnect on disconnect
- Live dashboard updates
- Tenant-scoped SignalR hubs

### 🗂️ **State Machine**
- Stateless library integration
- DB-driven state definitions
- Configurable transitions
- State lifecycle management
- Demo task workflow included

### 📁 **File Management**
- `IStorageProvider` abstraction (pluggable storage)
- Local disk storage implementation
- File metadata tracking
- Upload/download with streaming
- 5MB file size validation

### 📊 **Reporting**
- Excel export (ClosedXML)
- CSV export
- PDF generation (QuestPDF)
- Configurable report definitions
- Dynamic column selection

### 🔌 **API Integration**
- API key authentication
- Webhook support with HMAC signing
- Webhook event subscriptions
- Rate limiting (10 req/min on auth endpoints)

### 🎨 **Per-Tenant Theming**
- Custom logo upload
- Primary color customization
- Sidebar color & text color
- Dynamic theme loading
- CSS variable injection

### 🔐 **Advanced Security**
- Account lockout after failed attempts
- Rate limiting on auth endpoints
- JWT with refresh token rotation
- Permission-based authorization
- CORS configuration

### 🚩 **Feature Flags**
- Microsoft.FeatureManagement integration
- Per-tenant feature toggles
- Enable/disable features dynamically

### 📖 **Help System**
- DB-driven markdown articles
- Contextual help drawer
- Category-based organization
- 13 seed help articles included

### 🔍 **Audit Logging**
- Automatic change tracking
- SaveChanges override + interceptor
- Tracks: Created, Modified, Deleted
- User and timestamp tracking
- Audit log viewer with filtering

### 🌍 **Global Search**
- Multi-entity search (7 types)
- Debounced autocomplete
- Entity-specific icons
- Jump to entity pages

### 📸 **User Profiles**
- Profile photo upload/delete
- Fallback to initials
- Activity feed (last 50 actions)
- Color-coded timeline

### ⚙️ **Configuration Engine**
- DB-driven key-value settings
- Categorized configurations
- Data type enforcement
- SMTP, storage, and system settings

---

## 🏗️ **Tech Stack**

### Backend
- **.NET 9.0** — Latest .NET with top-tier performance
- **EF Core 9** — Entity Framework for data access
- **MediatR** — CQRS pattern implementation
- **FluentValidation** — Command validation
- **Hangfire** — Background job processing
- **SignalR** — Real-time communication
- **MailKit** — Email sending
- **QuestPDF** — PDF generation
- **ClosedXML** — Excel export
- **Stateless** — State machine library
- **Serilog** — Structured logging
- **BCrypt.Net** — Password hashing

### Frontend
- **React 18** — UI library
- **TypeScript** — Type-safe JavaScript
- **Vite** — Fast build tool
- **Ant Design 5** — UI component library
- **React Query** — Server state management
- **React Router** — Client-side routing
- **Axios** — HTTP client
- **@microsoft/signalr** — Real-time client
- **lottie-react** — Animations for empty states
- **dayjs** — Date manipulation

### Database
- **SQL Server 2022 Express** — Relational database
- **Windows Authentication** — Integrated security

### Architecture
- **Clean Architecture** — Domain → Application → Infrastructure → API
- **CQRS** — Command Query Responsibility Segregation
- **Repository Pattern** — Data access abstraction
- **Unit of Work** — Transaction management
- **Dependency Injection** — Built-in .NET DI container

---

## 📚 **Documentation**

Comprehensive documentation is available in the `/docs` folder:

- **[Installation Guide](docs/INSTALLATION.md)** — Step-by-step setup instructions
- **[Modules Documentation](docs/MODULES.md)** — Detailed guide for all 23 modules
- **[API Reference](docs/API.md)** — REST API endpoints and examples
- **[Development Guide](docs/DEVELOPMENT.md)** — Adding new features and modules
- **[Deployment Guide](docs/DEPLOYMENT.md)** — Production deployment instructions
- **[Architecture Document](ARCHITECTURE.md)** — System architecture overview

---

## 🚀 **Quick Start**

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (version 9.0.305 or later)
- [Node.js](https://nodejs.org/) (version 22.19.0 or later)
- [SQL Server 2022 Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or higher
- Git (for cloning the repository)

### 1. Clone the Repository
```bash
git clone https://github.com/saurabhwebdev/coreengine.git
cd coreengine
```

### 2. Database Setup
```bash
# Update connection string in src/CoreEngine.API/appsettings.json if needed
# Default: Server=localhost;Database=CoreEngineDb;Trusted_Connection=True;TrustServerCertificate=True

# Create database and run migrations
cd src/CoreEngine.API
dotnet ef database update
```

### 3. Backend Setup
```bash
# Install dependencies and run
cd src/CoreEngine.API
dotnet restore
dotnet run
```

Backend will start at: `http://localhost:5034`

### 4. Frontend Setup
```bash
# Install dependencies and run (in new terminal)
cd frontend
npm install
npm run dev
```

Frontend will start at: `http://localhost:5173`

### 5. Login
- **URL:** http://localhost:5173
- **Email:** `admin@coreengine.local`
- **Password:** `Admin@123`

### One-Click Start (Windows)
```bash
# Run both backend + frontend + browser
start.bat
```

---

## 📂 **Project Structure**

```
coreengine/
├── src/
│   ├── CoreEngine.API/              # REST API (Controllers, Middleware, Hubs)
│   ├── CoreEngine.Application/      # CQRS Handlers, Interfaces, DTOs
│   ├── CoreEngine.Domain/           # Entities, Enums, Domain Logic
│   ├── CoreEngine.Infrastructure/   # EF Core, Services, Persistence
│   └── CoreEngine.Shared/           # Constants, Utilities, Exceptions
├── frontend/
│   ├── src/
│   │   ├── api/                     # Axios API clients
│   │   ├── components/              # Reusable React components
│   │   ├── contexts/                # React contexts (Auth, Theme, SignalR)
│   │   ├── layouts/                 # Layout components
│   │   ├── pages/                   # Page components
│   │   └── types/                   # TypeScript type definitions
│   ├── public/                      # Static assets
│   └── package.json                 # npm dependencies
├── docs/                            # Documentation
│   ├── INSTALLATION.md
│   ├── MODULES.md
│   ├── API.md
│   ├── DEVELOPMENT.md
│   └── DEPLOYMENT.md
├── ARCHITECTURE.md                  # Architecture overview
├── SPRINTS.md                       # Sprint planning history
├── README.md                        # This file
└── start.bat                        # One-click launcher
```

---

## 🎨 **Design System**

CoreEngine uses an **Apple-inspired design language**:

- **Font:** Inter (Google Fonts)
- **Palette:**
  - Text: `#1d1d1f` (near black)
  - Secondary: `#86868b` (gray)
  - Borders: `#e5e5ea` (light gray)
  - Background: `#ffffff` (white)
- **Primary Color:** `#0071e3` (blue) — tenant-customizable
- **Accent Colors:**
  - Success: `#34c759` (green)
  - Warning: `#ff9500` (orange)
  - Error: `#ff3b30` (red)
  - Info: `#af52de` (purple)
- **Cards:** 12px border-radius, 1px border, no shadows
- **Tables:** Uppercase 11px headers, text-only actions, dot status indicators

---

## 🔧 **Configuration**

### Backend Configuration
Edit `src/CoreEngine.API/appsettings.json`:

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
  "Serilog": {
    "MinimumLevel": "Information"
  }
}
```

**Important:** JWT secret is stored in .NET User Secrets (not in appsettings.json):
```bash
cd src/CoreEngine.API
dotnet user-secrets set "Jwt:Secret" "your-super-secret-key-min-32-chars"
```

### Frontend Configuration
Edit `frontend/src/api/client.ts` if backend URL changes:

```typescript
const apiClient = axios.create({
  baseURL: 'http://localhost:5034/api', // Change if needed
});
```

---

## 🧩 **All 23 Modules**

1. ✅ **Multi-Tenancy** — Tenant isolation, subdomain routing
2. ✅ **IAM** — Users, roles, departments, permissions
3. ✅ **Audit Logging** — Change tracking, audit viewer
4. ✅ **Notifications** — In-app + Email + SignalR real-time
5. ✅ **Custom Workflow Engine** — Approvals, tasks, instances
6. ✅ **State Machine** — Lifecycle management (Stateless)
7. ✅ **File Management** — Upload, download, storage abstraction
8. ✅ **Configuration Engine** — DB-driven settings
9. ✅ **Reporting** — Excel, CSV, PDF export
10. ✅ **API Integration** — API keys, webhooks, HMAC
11. ✅ **Background Jobs** — Hangfire with SQL persistence
12. ✅ **Theming** — Per-tenant logo & colors
13. ✅ **Security** — Lockout, rate limiting, JWT
14. ✅ **Feature Flags** — Microsoft.FeatureManagement
15. ✅ **Help System** — Markdown articles, contextual help
16. ✅ **User Profiles** — Photos, activity feed
17. ✅ **Global Search** — Multi-entity search
18. ✅ **Email Templates** — Variable substitution
19. ✅ **Email Queue** — Retry logic, queue management
20. ✅ **My Tasks** — Workflow approval tasks
21. ✅ **Workflows** — Definitions, instances, statistics
22. ✅ **Presence Tracking** — Online/offline via SignalR
23. ✅ **Dashboard** — Aggregated statistics, charts

---

## 🎯 **Use Cases**

CoreEngine is perfect for building:

- **CRM Systems** — Customer relationship management
- **ERP Systems** — Enterprise resource planning
- **HRMS** — Human resource management
- **Project Management** — Task tracking, workflows
- **Document Management** — File storage, approvals
- **Helpdesk Systems** — Ticket tracking, assignments
- **Procurement Systems** — Purchase orders, approvals
- **Any SaaS Application** — Multi-tenant ready

---

## 🤝 **Contributing**

This is a private repository. For contributions:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

**Please read [DEVELOPMENT.md](docs/DEVELOPMENT.md) for coding standards and patterns.**

---

## 📜 **License**

This project is private and proprietary. Unauthorized copying, distribution, or use is strictly prohibited.

---

## 👨‍💻 **Authors**

- **Saurabh Thakur** — [@saurabhwebdev](https://github.com/saurabhwebdev)
- **Claude Sonnet 4.5** — AI Development Assistant

---

## 🙏 **Acknowledgments**

- Clean Architecture by Robert C. Martin
- Microsoft .NET Team
- React & TypeScript communities
- All open-source library maintainers

---

## 📞 **Support**

- **Issues:** [GitHub Issues](https://github.com/saurabhwebdev/coreengine/issues)
- **Documentation:** [/docs](docs/)
- **Architecture:** [ARCHITECTURE.md](ARCHITECTURE.md)

---

<p align="center">
  <strong>Built with ❤️ using .NET 9, React 18, and Clean Architecture</strong>
</p>

<p align="center">
  <sub>CoreEngine — Your foundation for enterprise applications</sub>
</p>
