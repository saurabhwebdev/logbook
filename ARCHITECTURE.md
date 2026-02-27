# CoreEngine — Enterprise Base Framework Architecture

## Vision
A reusable enterprise core engine that serves as the foundation for any enterprise application. Domain modules (procurement, CRM, clinic management, mining ops, etc.) plug into this core. The core never changes — only domain logic varies.

## Tech Stack
| Layer | Technology |
|-------|-----------|
| Backend | .NET 9, C# 13 |
| ORM | Entity Framework Core 9 |
| Database | SQL Server 2022 Express |
| Frontend | React 18 + TypeScript + Vite |
| UI Library | Ant Design 5 |
| Auth | JWT + Refresh Tokens |

## Architecture Pattern
**Clean Architecture** with strict dependency rules:
```
API → Infrastructure → Application → Domain → Shared
```
Domain has zero dependencies on frameworks. Application defines interfaces. Infrastructure implements them.

## Solution Structure
```
CoreEngine.sln
├── src/
│   ├── CoreEngine.Domain/          # Entities, enums, interfaces (pure C#)
│   ├── CoreEngine.Application/     # CQRS handlers (MediatR), DTOs, validators
│   ├── CoreEngine.Infrastructure/  # EF Core, JWT, BCrypt, repositories
│   ├── CoreEngine.API/             # ASP.NET Core API, middleware, controllers
│   └── CoreEngine.Shared/          # Constants, exceptions, helpers
├── frontend/                       # React + TypeScript + Ant Design
└── tests/                          # xUnit test projects
```

## Library Strategy (Compose Best-of-Breed)
Instead of building everything from scratch or coupling to a single framework (like ABP), we compose proven libraries:

| Need | Library | License |
|------|---------|---------|
| Multi-Tenancy | Custom EF Core Global Query Filters | N/A |
| State Machine | Stateless | Apache-2.0 |
| Workflow Engine | Elsa Workflows v3 | MIT |
| Background Jobs | Hangfire | LGPL-3.0 |
| Event Bus | MassTransit v8 | Apache-2.0 |
| Feature Flags | Microsoft.FeatureManagement | MIT |
| Excel Export | ClosedXML | MIT |
| Email | MailKit | MIT |
| Localization | Microsoft.Extensions.Localization | MIT |

---

## All 15 Core Modules

### Phase 1 — Foundation
| # | Module | Status | Implementation |
|---|--------|--------|----------------|
| 1 | **Multi-Tenancy** | DONE | Custom EF Core global query filters + TenantResolutionMiddleware. TenantId on every scoped entity. X-Tenant-Id header strategy. |
| 2 | **IAM** | DONE | Custom Permission entity (Module.Action format). JWT with permission claims. Users, Roles, Departments CRUD. |
| 3 | **Audit Logging** | DONE | SaveChangesAsync override (audit fields) + AuditableEntityInterceptor (old/new values). Immutable AuditLog table. |
| 14 | **Security (core)** | DONE | RBAC middleware, soft delete pattern, login tracking, JWT session management. |

### Phase 2 — Core Engines
| # | Module | Status | Implementation |
|---|--------|--------|----------------|
| 6 | **State Machine** | DONE | DB-driven state definitions + transition definitions per entity type. Transition log for audit trail. Permission-gated triggers. Sample "Task" lifecycle seeded (Draft → Open → InProgress → Review → Done/Cancelled). |
| 5 | **Workflow Engine** | PLANNED | Elsa v3 with custom activities (Approval, Notify, StateTransition). Multi-level approvals, amount/role routing, SLA timers. |
| 4 | **Notifications** | DONE | In-app notification entity with recipient, type (Info/Success/Warning/Error), read tracking. CQRS handlers for send, mark-read, mark-all-read. API + frontend bell icon page. |
| 11 | **Background Jobs** | PLANNED | Hangfire with SQL Server persistence. Tenant-aware jobs. Permissions defined (BackgroundJob.Read/Manage). |
| 15 | **Feature Flags** | DONE | DB-driven per-tenant feature flags with toggle API. Seeded defaults: Notifications.Email, Notifications.InApp, AuditLog.DetailedDiff, StateMachine.Enabled, BackgroundJobs.Enabled. Frontend toggle UI. |
| 8 | **Configuration** | DONE | DB-driven key-value config per tenant. Categories (General, Email, Security). Seeded defaults: App.Name, App.PageSize, Email.FromAddress, Email.FromName, Session.TimeoutMinutes, Password.MinLength. Frontend settings page with edit modal. |

### Phase 3 — Domain Proof + Integration
| # | Module | Status | Implementation |
|---|--------|--------|----------------|
| 7 | **File Management** | DONE | IFileStorageService abstraction (local disk now, pluggable to Azure/S3). Upload/download/delete with metadata (category, description). Multipart form upload with 50MB limit. |
| 9 | **Reporting** | DONE | Report definition entity with entity type, columns JSON, export format (Excel/CSV). CRUD API. ClosedXML NuGet added for export (Phase 4 will add actual export execution). |
| 10 | **API Integration** | DONE | API key management (BCrypt-hashed, prefix-only stored, raw shown once). Webhook subscriptions with HMAC signing secret (shown once). Event type filtering, failure tracking. |
| — | **State Machine Demo** | DONE | DemoTask entity as proof-of-concept. Full lifecycle: Draft → Open → InProgress → Review → Done/Cancelled. Uses Phase 2 state machine definitions. Transition dropdown, history timeline. |

### Phase 4 — Harden & Extend
| # | Module | Status | Implementation |
|---|--------|--------|----------------|
| 13 | **Theming** | DONE | Per-tenant branding: LogoUrl, PrimaryColor, SidebarColor on Tenant entity. AllowAnonymous GET theme endpoint. Dynamic sidebar colors and logo in MainLayout. Theming admin page with color pickers. |
| 14 | **Security (advanced)** | DONE | Account lockout (5 failed attempts → 15 min lock). ASP.NET Core fixed-window rate limiting (10 req/min on auth endpoints, 429 response). FailedLoginAttempts + LockoutEndAt fields on User. |
| — | **Dashboard (real data)** | DONE | Single aggregation query counting all modules (Users, Roles, Departments, AuditLogs, Files, Reports, DemoTasks, FeatureFlags, ApiKeys). 9 stat cards + recent activity timeline. |
| — | **Report Export** | DONE | Excel export via ClosedXML (.xlsx) and CSV export. Entity-type-based data querying (User, Department, Role, AuditLog, DemoTask). Column selection from report definition. |
| 12 | **Localization** | SKIPPED | Not needed for initial framework — teams start English-only. Can be added later. |

### Help Module
| # | Module | Status | Implementation |
|---|--------|--------|----------------|
| — | **Help Center** | DONE | DB-driven per-tenant help articles with markdown content. Full CRUD (HelpArticle entity with Title, Slug, ModuleKey, Content, Category, SortOrder, IsPublished, Tags). Contextual HelpDrawer triggered by `?` icon in top bar — auto-resolves module from current route. Help Center page with search/filter, card grid, article viewer with markdown rendering. Admin form with live markdown preview. 13 seed articles covering all modules. Feature-flag toggleable (`Help.Enabled`). Uses `react-markdown` for rendering. |

---

## Key Patterns
- **CQRS** via MediatR (commands and queries separated)
- **Repository + Unit of Work** (generic, EF Core backed)
- **Pipeline Behaviors** (validation, logging, exception handling)
- **Global Query Filters** (tenant isolation + soft delete via custom EF Core filters)
- **Permission-based Auth** (flat catalog: "User.Create", "Role.Delete", etc.)
- **Domain Events** (MassTransit pub/sub for loose coupling between modules)
- **State Pattern** (Stateless library for entity lifecycle management)

## Database
- **Server**: localhost (default instance), Windows Authentication
- **Database**: CoreEngineDb
- **Connection String**: Configured in appsettings.json

## Seed Data
- Default tenant: "Default" (subdomain: "default")
- Admin user: admin@coreengine.local / Admin@123
- Roles: SuperAdmin (all permissions), Admin (subset), User (basic)
- Permissions: Auto-generated from PermissionConstants via reflection

## Frontend Architecture
```
frontend/src/
├── api/            # Axios client + per-entity API modules
├── components/     # PermissionGate, ProtectedRoute
├── contexts/       # AuthContext (JWT + permissions)
├── layouts/        # AuthLayout (split-panel login), MainLayout (sidebar + topbar)
├── pages/          # Dashboard, Users, Roles, Departments, AuditLogs, Tenants, Settings, FeatureFlags, Notifications, StateMachine, Files, Reports, ApiIntegration, DemoTasks, Theming, Help
└── types/          # TypeScript interfaces for all domain models
```

### Design System
- **Font**: Inter (Google Fonts)
- **Palette**: Apple-inspired neutrals (#1d1d1f text, #86868b secondary, #e5e5ea borders)
- **Primary**: #0071e3 (blue), accents: #34c759 (green), #ff9500 (orange), #af52de (purple)
- **Cards**: 12px border-radius, 1px #e5e5ea border, no box-shadow
- **Tables**: Uppercase 11px headers, text-only action buttons, dot-status indicators
- **Theme**: Ant Design ConfigProvider overrides in App.tsx
- **State**: React Query for server state, React Context for auth/tenant
- **HTTP**: Axios with JWT + tenant header interceptors, auto token refresh
- **Routing**: react-router-dom with ProtectedRoute guard
- **Real-time**: SignalR for notifications (Phase 2)
- **i18n**: react-i18next (Phase 4)

## How Domain Modules Plug In
Any new domain module (procurement, CRM, etc.) follows this pattern:
1. Create entities extending `TenantScopedEntity` → instant multi-tenancy + soft delete + audit
2. Add permissions to `PermissionConstants` → instant RBAC enforcement
3. Create Commands/Queries following MediatR pattern → automatic validation + logging
4. Define state machine transitions → lifecycle management via Stateless
5. Register workflow definitions → approval routing via Elsa
6. Subscribe to domain events → notifications via MassTransit
7. Add frontend module in `src/modules/` → pluggable UI
8. Toggle via feature flag → per-tenant enable/disable
