# CoreEngine Development Sprints

## Sprint Planning Overview

**Goal:** Complete the enterprise core engine with all essential modules for production readiness.

**Current Completion:** ~70% (Foundation + Quick Wins complete)

**Target:** 100% production-ready enterprise framework

---

## ✅ **Completed Work (Before Sprints)**

### Phase 1 - Foundation (COMPLETE)
- Multi-Tenancy (Custom EF Core global query filters)
- IAM (Users, Roles, Departments, Permissions)
- Audit Logging (SaveChanges override + interceptor)
- React Frontend (Apple-inspired design, Ant Design 5)
- JWT Authentication + Refresh Tokens

### Phase 2 - Core Engines (COMPLETE)
- Configuration Engine (DB-driven settings)
- Feature Flags (Microsoft.FeatureManagement)
- Notifications (In-app, MassTransit ready)
- State Machine (Stateless library, DB-driven definitions)

### Phase 3 - Domain Proof (COMPLETE)
- File Management (IStorageProvider abstraction)
- Reporting (Excel/CSV/PDF export with QuestPDF)
- API Integration (API Keys, Webhooks)
- State Machine Demo (DemoTasks lifecycle)

### Phase 4 - Harden & Extend (COMPLETE)
- Advanced Security (Lockout, Rate Limiting)
- Per-Tenant Theming (Logo, Colors)
- Real Dashboard (Aggregated stats)
- Help Module (Markdown articles, contextual help)

### Quick Wins (COMPLETE)
- User Profile Photos (Upload/delete with validation)
- Activity Feed (Real-time user action timeline)
- Global Search (7 entity types, debounced)
- Lottie Animations (Professional empty states)

---

## 🚀 **Sprint 1: Background Jobs + Email Notifications** (Week 1-2)

### Objectives
- Add Hangfire for background job processing
- Integrate MailKit for email sending
- Create email template system
- Build email queue with retry logic
- Add job monitoring dashboard

### Backend Tasks
1. **Install Hangfire** (SQL Server storage)
   - Add Hangfire.AspNetCore + Hangfire.SqlServer NuGet packages
   - Configure in Program.cs with tenant-aware job filters
   - Create BackgroundJobsController with permissions

2. **Create Email Infrastructure**
   - EmailTemplate entity (Name, Subject, HtmlBody, PlainTextBody)
   - EmailQueue entity (To, Subject, Body, Status, SentAt, FailureReason)
   - EmailConfiguration (SMTP settings in SystemConfiguration)
   - IEmailService interface (SendEmailAsync, SendTemplatedEmailAsync)
   - SmtpEmailService implementation using MailKit

3. **CQRS Handlers**
   - SendEmailCommand (immediate send)
   - QueueEmailCommand (background send)
   - GetEmailTemplatesQuery
   - CreateEmailTemplateCommand

4. **Hangfire Jobs**
   - ProcessEmailQueueJob (recurring every 1 minute)
   - SendScheduledReportsJob
   - CleanupOldAuditLogsJob
   - TenantAwareJobFilter (inject ITenantContext)

### Frontend Tasks
1. **Email Templates Page** (/email-templates)
   - List templates with search
   - Create/Edit template modal with HTML editor
   - Template variables guide ({{userName}}, {{link}}, etc.)
   - Preview functionality

2. **Background Jobs Page** (/background-jobs)
   - Embed Hangfire Dashboard (iframe)
   - Job statistics cards
   - Recent jobs list
   - Manual job trigger buttons (with permission)

3. **Email Queue Page** (/email-queue)
   - List queued/sent/failed emails
   - Retry failed emails
   - Filter by status/date
   - Email preview modal

### Testing
- Send test email to verify SMTP
- Queue 100 emails, verify batch processing
- Test template rendering with variables
- Verify tenant isolation in jobs
- Test job retry on failure

### Deliverables
- ✅ Hangfire integrated with dashboard at /hangfire
- ✅ Email sending functional with MailKit
- ✅ Email templates CRUD complete
- ✅ Background job examples (cleanup, scheduled reports)
- ✅ All pages accessible from sidebar
- ✅ Unit tests for email service
- ✅ Migration applied, seed data added
- ✅ Committed to GitHub

---

## 🚀 **Sprint 2: Workflow Engine (Elsa v3) - Foundation** (Week 3-4)

### Objectives
- Integrate Elsa Workflows v3
- Create workflow definition storage
- Build basic approval workflow
- Add workflow instance tracking
- Create custom activities

### Backend Tasks
1. **Install Elsa Workflows v3**
   - Add Elsa.* NuGet packages (Designer, Core, EntityFramework)
   - Configure in Program.cs
   - Set up EF Core persistence

2. **Workflow Entities**
   - WorkflowDefinition (stored in Elsa tables)
   - WorkflowInstance tracking
   - WorkflowTask (approval tasks, user assignments)

3. **Custom Elsa Activities**
   - SendNotificationActivity
   - SendEmailActivity
   - StateTransitionActivity
   - AssignApprovalActivity
   - CheckPermissionActivity

4. **CQRS Handlers**
   - StartWorkflowCommand
   - CompleteTaskCommand (approve/reject)
   - GetPendingTasksQuery
   - GetWorkflowHistoryQuery

5. **Approval Workflow Example**
   - Purchase Order Approval (amount-based routing)
   - Leave Request Approval (manager → HR)
   - Document Approval (multi-level)

### Frontend Tasks
1. **My Tasks Page** (/my-tasks)
   - List pending approval tasks
   - Task details modal
   - Approve/Reject with comments
   - Task history timeline

2. **Workflow Admin Page** (/workflows) [Admin only]
   - List workflow definitions
   - Workflow statistics
   - Active instances
   - Workflow version history

3. **Workflow Integration**
   - Add "Submit for Approval" buttons to relevant entities
   - Show workflow status badges
   - Display approval history on entity pages

### Testing
- Create sample Purchase Order workflow
- Test approval routing (Manager → Director → CFO)
- Test rejection flow
- Verify SLA tracking
- Test parallel approvals

### Deliverables
- ✅ Elsa v3 integrated with EF Core persistence
- ✅ 3 custom activities working
- ✅ Approval workflow template created
- ✅ My Tasks page functional
- ✅ Workflow tracking UI complete
- ✅ Migration applied
- ✅ Committed to GitHub

---

## 🚀 **Sprint 3: Workflow Designer + Real-time (SignalR)** (Week 5-6)

### Objectives
- Add Elsa Designer UI
- Implement SignalR for real-time notifications
- Connect workflows to notifications
- Add presence indicators
- Build real-time dashboard updates

### Backend Tasks
1. **Elsa Designer API**
   - Enable Elsa Studio (workflow designer UI)
   - Custom activity descriptor for UI
   - Workflow validation API

2. **SignalR Infrastructure**
   - Add SignalR NuGet package
   - NotificationHub (SendNotification, BroadcastToTenant)
   - UserPresenceHub (Online/Offline tracking)
   - Tenant-scoped hub filters

3. **Real-time Integration**
   - Trigger SignalR on workflow task assignment
   - Live notification push
   - Dashboard stat updates
   - Audit log streaming

### Frontend Tasks
1. **Workflow Designer Integration**
   - Embed Elsa Designer (/workflow-designer)
   - Workflow canvas with drag-drop activities
   - Activity configuration panels
   - Workflow testing/debugging

2. **SignalR Client**
   - Add @microsoft/signalr package
   - SignalRContext with auto-reconnect
   - Real-time notification bell updates
   - Toast notifications on events

3. **Presence Indicators**
   - Green dot for online users
   - "Last seen" timestamps
   - Active users widget on dashboard

### Testing
- Create workflow visually in designer
- Test real-time notification delivery
- Verify multi-tab presence sync
- Load test: 100 concurrent SignalR connections
- Test reconnection after network drop

### Deliverables
- ✅ Elsa Designer embedded and functional
- ✅ SignalR real-time notifications working
- ✅ Presence tracking complete
- ✅ Dashboard live updates
- ✅ Workflow designer permissions enforced
- ✅ Committed to GitHub

---

## 🎯 **Success Criteria (All Sprints)**

### Code Quality
- ✅ All code follows Clean Architecture
- ✅ CQRS pattern maintained (MediatR)
- ✅ FluentValidation on all commands
- ✅ Permission-based access control
- ✅ Tenant isolation verified
- ✅ No TypeScript errors (strict mode)
- ✅ Backend builds with 0 warnings

### Testing
- ✅ Manual testing of all features
- ✅ API endpoints tested with Postman/curl
- ✅ Frontend UI verified in Chrome
- ✅ Database migrations applied cleanly
- ✅ Seed data runs successfully

### Documentation
- ✅ ARCHITECTURE.md updated with new modules
- ✅ MEMORY.md updated with implementation notes
- ✅ README.md updated with new features
- ✅ Inline code comments for complex logic
- ✅ API endpoints documented

### Git Hygiene
- ✅ Meaningful commit messages
- ✅ One commit per sprint (with detailed message)
- ✅ No committed secrets or temp files
- ✅ .gitignore properly configured
- ✅ Clean git history

---

## 📋 **Post-Sprint Backlog** (Future Enhancements)

1. **Caching (Redis)** - Distributed caching for performance
2. **Full-Text Search (Elasticsearch)** - Advanced search capabilities
3. **Mobile API** - Mobile-optimized endpoints + push notifications
4. **OAuth Provider** - Allow 3rd party integrations
5. **2FA** - TOTP, SMS authentication
6. **Localization** - Multi-language support
7. **Advanced Reporting** - Charts, pivot tables, SQL queries
8. **Document Versioning** - Track file changes over time
9. **Data Import** - Bulk import from Excel/CSV
10. **Performance Monitoring** - Application Insights integration

---

## 🚨 **Critical Rules for Implementation**

1. **Never break existing functionality** - Run full build + test after every change
2. **Follow existing patterns** - Match code style, naming, structure
3. **Tenant isolation is sacred** - Every entity must respect TenantId
4. **Permission-gate everything** - No bypassing RBAC
5. **Maintain Apple design** - Match existing UI aesthetics
6. **No shortcuts** - Proper CQRS, validation, error handling
7. **Test before commit** - Verify backend builds, frontend compiles, DB migrates
8. **Document as you go** - Update ARCHITECTURE.md, add comments
9. **One sprint at a time** - Complete, test, commit before next sprint
10. **Keep calm and code clean** - Quality over speed

---

**Last Updated:** 2026-02-27
**Status:** Sprint 1 Ready to Start
