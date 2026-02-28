# Modules Documentation

Complete reference for all 23 modules in CoreEngine.

---

## 📂 Module Categories

### 🏗️ Core Infrastructure
- **[Multi-Tenancy](modules/multi-tenancy.md)** - Tenant isolation and subdomain routing
- **[IAM (Identity & Access Management)](modules/iam.md)** - Users, roles, departments, permissions
- **[Audit Logging](modules/audit.md)** - Automatic change tracking
- **[Security](modules/security.md)** - Lockout, rate limiting, encryption

### 📨 Communication & Workflows
- **[Notification Framework](modules/notifications.md)** - In-app notifications
- **[Email Templates](modules/email-templates.md)** - Variable substitution templates
- **[Email Queue](modules/email-queue.md)** - Retry logic and queue management
- **[Custom Workflow Engine](modules/workflows.md)** - Approval workflows and tasks
- **[Background Jobs (Hangfire)](modules/background-jobs.md)** - Scheduled and recurring jobs
- **[Real-time (SignalR)](modules/signalr.md)** - Live notifications and presence

### 📊 Data & State Management
- **[State Machine](modules/state-machine.md)** - Entity lifecycle management
- **[File Management](modules/files.md)** - Upload, download, storage
- **[Configuration Engine](modules/configuration.md)** - DB-driven settings
- **[Reporting](modules/reporting.md)** - Excel, CSV, PDF export

### 🔌 Integration & Features
- **[API Integration](modules/api-integration.md)** - API keys and webhooks
- **[Feature Flags](modules/feature-flags.md)** - Toggle features per tenant
- **[Theming](modules/theming.md)** - Per-tenant branding
- **[Help System](modules/help.md)** - Contextual help articles

### 👤 User Experience
- **[User Profiles](modules/user-profiles.md)** - Photos and activity feed
- **[Global Search](modules/search.md)** - Multi-entity search
- **[My Tasks](modules/my-tasks.md)** - Pending workflow approvals
- **[Dashboard](modules/dashboard.md)** - Statistics and charts

---

## 🎯 Quick Module Reference

| Module | Entities | API Endpoints | Frontend Pages | Key Features |
|--------|----------|---------------|----------------|--------------|
| Multi-Tenancy | Tenant | 5 | 2 | Subdomain routing, isolation |
| IAM | User, Role, Department | 15+ | 3 | RBAC, JWT, BCrypt |
| Workflows | WorkflowDefinition, WorkflowInstance, WorkflowTask | 8 | 3 | Multi-level approvals |
| Email | EmailTemplate, EmailQueue | 6 | 2 | Retry logic, templates |
| Files | FileMetadata | 4 | 1 | Storage abstraction |
| Reports | ReportDefinition | 5 | 1 | Excel, CSV, PDF |
| ... | ... | ... | ... | ... |

---

## 🚀 Getting Started with Modules

### 1. Understand the Architecture

All modules follow **Clean Architecture** with **CQRS pattern**:

```
Domain Entity → Application (CQRS) → API Controller → Frontend
```

### 2. Module Structure

Each module typically includes:

```
📦 Module (e.g., "Products")
├── 📁 Domain/
│   └── Entities/Product.cs
├── 📁 Application/
│   ├── Commands/
│   │   ├── CreateProduct/
│   │   ├── UpdateProduct/
│   │   └── DeleteProduct/
│   └── Queries/
│       ├── GetProducts/
│       └── GetProductById/
├── 📁 API/
│   └── Controllers/ProductsController.cs
└── 📁 Frontend/
    ├── api/productsApi.ts
    ├── pages/ProductsPage.tsx
    └── types/index.ts (Product interface)
```

### 3. Common Patterns

**All Entities:**
- Extend `TenantScopedEntity` for multi-tenancy
- Include audit fields (CreatedAt, ModifiedAt, IsDeleted)

**All Commands:**
- Have a `CommandHandler`
- Have a `CommandValidator` (FluentValidation)

**All Controllers:**
- Use `[Authorize]` attribute
- Use `[RequirePermission]` attribute
- Return standardized responses

**All Frontend Pages:**
- Use React Query for data fetching
- Follow Apple design system
- Include EmptyState components

---

## 📖 Module Deep Dives

Click on any module link above to see detailed documentation including:

- ✅ **Overview** - What the module does
- ✅ **Key Features** - Capabilities and highlights
- ✅ **Entities** - Database models with properties
- ✅ **API Endpoints** - Full REST API reference
- ✅ **Frontend Pages** - UI components and pages
- ✅ **Usage Examples** - Code snippets and curl commands
- ✅ **Configuration** - Settings and options
- ✅ **Permissions** - Required permissions list

---

## 🔗 Related Documentation

- **[API Reference](API.md)** - Complete REST API documentation
- **[Development Guide](DEVELOPMENT.md)** - How to add new modules
- **[Architecture](../ARCHITECTURE.md)** - System architecture overview

---

## 📊 Module Statistics

- **Total Modules**: 23
- **Total Entities**: 30+
- **Total API Endpoints**: 100+
- **Total Frontend Pages**: 25+
- **Total Permissions**: 60+

---

**Next:** Choose a module from the list above to explore its detailed documentation.
