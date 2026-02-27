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
| 6 | **State Machine** | Phase 2 | Stateless library wrapped with persistence, audit trail, permission checks. Generic for any entity lifecycle. |
| 5 | **Workflow Engine** | Phase 2 | Elsa v3 with custom activities (Approval, Notify, StateTransition). Multi-level approvals, amount/role routing, SLA timers. |
| 4 | **Notifications** | Phase 2 | MassTransit v8 event bus + templates + channels (email via MailKit, in-app via SignalR, SMS pluggable). |
| 11 | **Background Jobs** | Phase 2 | Hangfire with SQL Server persistence. Tenant-aware jobs. Built-in cleanup and escalation jobs. |
| 15 | **Feature Flags** | Phase 2 | Microsoft.FeatureManagement with custom TenantFeatureFilter. Per-tenant module enable/disable. |
| 8 | **Configuration** | Phase 2 | DB-driven key-value config per tenant. Cached with invalidation. Categories: General, Approval, Notification, Security. |

### Phase 3 — Domain Proof + Integration
| # | Module | Status | Implementation |
|---|--------|--------|----------------|
| 7 | **File Management** | Phase 3 | IStorageProvider abstraction (local/Azure/S3). Metadata, access control, versioning. |
| 9 | **Reporting** | Phase 3 | Saved reports, CSV/Excel export (ClosedXML), dashboard widget data API. |
| 10 | **API Integration** | Phase 3 | API key auth, rate limiting (built-in ASP.NET Core), webhooks with HMAC signing and retry. |

### Phase 4 — Harden & Extend
| # | Module | Status | Implementation |
|---|--------|--------|----------------|
| 12 | **Localization** | Phase 4 | Microsoft.Extensions.Localization with DB provider. Currency formatting, timezone conversion (UTC storage). |
| 13 | **Theming** | Phase 4 | Per-tenant branding (logo, colors). CSS custom properties. Email template overrides. |
| 14 | **Security (advanced)** | Phase 4 | Field-level encryption (AES-256), activity monitoring, brute force protection, session management. |

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
├── pages/          # Dashboard, Users, Roles, Departments, AuditLogs, Tenants
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
