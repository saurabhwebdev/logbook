# 📧 Email Templates Module

Template-based email system with variable substitution.

---

## 📋 Overview

The Email Templates module provides a flexible email templating system. Templates are stored in the database and support variable substitution using Handlebars-like syntax. This allows non-technical users to customize email content without code changes.

---

## ✨ Key Features

- ✅ **Database-stored templates** - Edit templates via UI
- ✅ **Variable substitution** - `{{userName}}`, `{{taskName}}` placeholders
- ✅ **HTML & Plain Text** - Dual format support
- ✅ **Preview functionality** - Test templates before sending
- ✅ **Multi-tenant scoped** - Templates per tenant
- ✅ **Template categories** - Organize by purpose
- ✅ **Version control** - Track template changes
- ✅ **Default templates** - Pre-built common templates
- ✅ **Validation** - Ensure required variables are present

---

## 🗄️ Entities

### EmailTemplate

**File:** `src/CoreEngine.Domain/Entities/EmailTemplate.cs`

```csharp
public class EmailTemplate : TenantScopedEntity
{
    public string Name { get; set; }
    public string Subject { get; set; }
    public string BodyHtml { get; set; }
    public string? BodyPlainText { get; set; }
    public string Category { get; set; }  // "Workflow", "Account", "Notification"
    public string? Description { get; set; }
    public string? RequiredVariables { get; set; }  // JSON array of required vars
    public bool IsActive { get; set; }
}
```

**Example Template:**
```json
{
  "name": "TaskAssignedTemplate",
  "subject": "New Task Assigned: {{taskName}}",
  "bodyHtml": "<h2>Hello {{userName}},</h2><p>You have been assigned a new task: <strong>{{taskName}}</strong></p><p>Due Date: {{dueDate}}</p><p><a href='{{taskLink}}'>View Task</a></p>",
  "requiredVariables": "[\"userName\", \"taskName\", \"dueDate\", \"taskLink\"]"
}
```

---

## 🌐 API Endpoints

| Method | Endpoint | Description | Permission |
|--------|----------|-------------|------------|
| GET | `/api/email-templates` | List templates (paginated) | EmailTemplate.Read |
| GET | `/api/email-templates/{id}` | Get template by ID | EmailTemplate.Read |
| POST | `/api/email-templates` | Create new template | EmailTemplate.Create |
| PUT | `/api/email-templates/{id}` | Update template | EmailTemplate.Update |
| DELETE | `/api/email-templates/{id}` | Delete template | EmailTemplate.Delete |
| POST | `/api/email-templates/{id}/preview` | Preview template with sample data | EmailTemplate.Read |

---

## 💻 Template Service

### Render Template

**File:** `src/CoreEngine.Application/Services/EmailTemplateService.cs`

```csharp
public class EmailTemplateService : IEmailTemplateService
{
    private readonly IApplicationDbContext _context;

    public async Task<string> RenderTemplateAsync(string templateName, Dictionary<string, string> variables)
    {
        var template = await _context.EmailTemplates
            .Where(t => t.Name == templateName && t.IsActive)
            .FirstOrDefaultAsync();

        if (template == null)
            throw new NotFoundException($"Email template '{templateName}' not found");

        // Simple variable replacement
        var rendered = template.BodyHtml;
        foreach (var variable in variables)
        {
            rendered = rendered.Replace($"{{{{{variable.Key}}}}}", variable.Value);
        }

        return rendered;
    }

    public async Task<EmailMessage> CreateEmailFromTemplateAsync(
        string templateName,
        string toEmail,
        Dictionary<string, string> variables)
    {
        var template = await _context.EmailTemplates
            .Where(t => t.Name == templateName && t.IsActive)
            .FirstOrDefaultAsync();

        if (template == null)
            throw new NotFoundException($"Email template '{templateName}' not found");

        // Render subject and body
        var subject = RenderVariables(template.Subject, variables);
        var bodyHtml = RenderVariables(template.BodyHtml, variables);
        var bodyPlainText = template.BodyPlainText != null
            ? RenderVariables(template.BodyPlainText, variables)
            : null;

        return new EmailMessage
        {
            To = toEmail,
            Subject = subject,
            BodyHtml = bodyHtml,
            BodyPlainText = bodyPlainText
        };
    }

    private string RenderVariables(string content, Dictionary<string, string> variables)
    {
        foreach (var variable in variables)
        {
            content = content.Replace($"{{{{{variable.Key}}}}}", variable.Value);
        }
        return content;
    }
}
```

---

## 💻 Usage Examples

### Send Email Using Template

```csharp
public class AssignTaskCommand : IRequest
{
    public Guid TaskId { get; set; }
    public Guid UserId { get; set; }
}

public class AssignTaskCommandHandler : IRequestHandler<AssignTaskCommand>
{
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly IEmailService _emailService;

    public async Task Handle(AssignTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.WorkflowTasks.FindAsync(request.TaskId);
        var user = await _context.Users.FindAsync(request.UserId);

        // Create email from template
        var variables = new Dictionary<string, string>
        {
            { "userName", user.FirstName },
            { "taskName", task.TaskName },
            { "dueDate", task.DueDate?.ToString("MMM dd, yyyy") ?? "Not set" },
            { "taskLink", $"https://app.coreengine.com/my-tasks/{task.Id}" }
        };

        var emailMessage = await _emailTemplateService.CreateEmailFromTemplateAsync(
            "TaskAssignedTemplate",
            user.Email,
            variables
        );

        // Queue email for sending
        await _emailService.QueueEmailAsync(emailMessage);
    }
}
```

---

## 🎨 Frontend Pages

### Email Templates Page (`/email-templates`)

**Features:**
- List all email templates
- Filter by category
- Create/edit template with rich text editor
- Variable picker (insert `{{variableName}}`)
- Preview template with sample data
- Activate/deactivate templates
- Duplicate template

**Screenshot:**
```
┌─────────────────────────────────────────────────────────────┐
│  Email Templates                           [+ Create New]   │
├─────────────────────────────────────────────────────────────┤
│  Category: [All ▼] [Workflow ▼] [Account ▼]   Search...    │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ ✅ Task Assigned Template                           │  │
│  │ Category: Workflow                                   │  │
│  │ Subject: New Task Assigned: {{taskName}}            │  │
│  │ Variables: userName, taskName, dueDate, taskLink    │  │
│  │ [Edit] [Preview] [Duplicate] [Deactivate]          │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ ✅ Password Reset Template                          │  │
│  │ Category: Account                                    │  │
│  │ Subject: Reset Your Password                        │  │
│  │ Variables: userName, resetLink                      │  │
│  │ [Edit] [Preview] [Duplicate] [Deactivate]          │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

### Template Editor Modal

**Features:**
- Rich text editor for HTML body
- Plain text editor
- Subject line editor
- Variable picker dropdown
- Preview pane (live preview with sample data)
- Validation (ensure required variables are present)

---

## 📧 Default Templates

### 1. Task Assigned Template

```html
<div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
  <h2 style="color: #1d1d1f;">Hello {{userName}},</h2>

  <p>You have been assigned a new task:</p>

  <div style="background: #f6f9ff; padding: 20px; border-radius: 8px; margin: 20px 0;">
    <h3 style="margin: 0 0 10px 0;">{{taskName}}</h3>
    <p style="margin: 0; color: #86868b;">Due Date: {{dueDate}}</p>
  </div>

  <a href="{{taskLink}}" style="display: inline-block; background: #0071e3; color: white; padding: 12px 24px; border-radius: 8px; text-decoration: none; margin: 20px 0;">
    View Task
  </a>

  <p style="color: #86868b; font-size: 12px; margin-top: 40px;">
    This is an automated email from CoreEngine. Please do not reply.
  </p>
</div>
```

**Required Variables:**
- `userName` - User's first name
- `taskName` - Task name
- `dueDate` - Task due date
- `taskLink` - Link to task details

### 2. Password Reset Template

```html
<div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
  <h2 style="color: #1d1d1f;">Hello {{userName}},</h2>

  <p>We received a request to reset your password. Click the button below to reset it:</p>

  <a href="{{resetLink}}" style="display: inline-block; background: #0071e3; color: white; padding: 12px 24px; border-radius: 8px; text-decoration: none; margin: 20px 0;">
    Reset Password
  </a>

  <p style="color: #86868b;">This link will expire in 24 hours.</p>

  <p>If you didn't request this, please ignore this email.</p>
</div>
```

**Required Variables:**
- `userName` - User's first name
- `resetLink` - Password reset link with token

### 3. Workflow Approved Template

```html
<div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
  <h2 style="color: #34c759;">✓ Workflow Approved</h2>

  <p>Hello {{userName}},</p>

  <p>Your {{workflowType}} has been approved by {{approverName}}.</p>

  <div style="background: #f0fff4; padding: 20px; border-radius: 8px; margin: 20px 0; border-left: 4px solid #34c759;">
    <p style="margin: 0;"><strong>{{workflowType}}:</strong> {{workflowName}}</p>
    <p style="margin: 10px 0 0 0; color: #86868b;">Approved at: {{approvedAt}}</p>
  </div>

  <a href="{{workflowLink}}" style="display: inline-block; background: #0071e3; color: white; padding: 12px 24px; border-radius: 8px; text-decoration: none; margin: 20px 0;">
    View Details
  </a>
</div>
```

**Required Variables:**
- `userName` - Workflow creator's name
- `workflowType` - Type (Purchase Order, Leave Request)
- `workflowName` - Workflow instance name
- `approverName` - Who approved it
- `approvedAt` - Timestamp
- `workflowLink` - Link to workflow details

### 4. Workflow Rejected Template

```html
<div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
  <h2 style="color: #ff3b30;">✗ Workflow Rejected</h2>

  <p>Hello {{userName}},</p>

  <p>Your {{workflowType}} has been rejected by {{rejectorName}}.</p>

  <div style="background: #fff5f5; padding: 20px; border-radius: 8px; margin: 20px 0; border-left: 4px solid #ff3b30;">
    <p style="margin: 0;"><strong>{{workflowType}}:</strong> {{workflowName}}</p>
    <p style="margin: 10px 0 0 0;"><strong>Reason:</strong> {{rejectionReason}}</p>
  </div>

  <a href="{{workflowLink}}" style="display: inline-block; background: #0071e3; color: white; padding: 12px 24px; border-radius: 8px; text-decoration: none; margin: 20px 0;">
    View Details
  </a>
</div>
```

**Required Variables:**
- `userName` - Workflow creator's name
- `workflowType` - Type
- `workflowName` - Instance name
- `rejectorName` - Who rejected it
- `rejectionReason` - Rejection comment
- `workflowLink` - Link to workflow details

---

## ⚙️ Configuration

### Template Settings

**File:** `appsettings.json`

```json
{
  "EmailTemplateSettings": {
    "EnableTemplateCache": true,
    "CacheDurationMinutes": 30,
    "MaxTemplateSizeKB": 100,
    "AllowHtmlTemplates": true,
    "RequirePlainTextFallback": false
  }
}
```

---

## 🔐 Permissions

- **EmailTemplate.Create** - Create email templates
- **EmailTemplate.Read** - View email templates
- **EmailTemplate.Update** - Modify email templates
- **EmailTemplate.Delete** - Delete email templates

**Default Roles:**
- **SuperAdmin**: All permissions
- **Admin**: EmailTemplate.Read, EmailTemplate.Update
- **User**: None

---

## 🧪 Testing Templates

### Unit Test

```csharp
[Fact]
public async Task RenderTemplate_ShouldReplaceVariables()
{
    // Arrange
    var template = new EmailTemplate
    {
        Name = "TestTemplate",
        Subject = "Hello {{userName}}",
        BodyHtml = "<p>Welcome {{userName}}, your email is {{userEmail}}</p>"
    };

    var variables = new Dictionary<string, string>
    {
        { "userName", "John Doe" },
        { "userEmail", "john@example.com" }
    };

    // Act
    var rendered = _templateService.RenderVariables(template.BodyHtml, variables);

    // Assert
    Assert.Equal("<p>Welcome John Doe, your email is john@example.com</p>", rendered);
}
```

---

## 🚨 Important Notes

1. **Variable Validation** - Always check that required variables are provided before rendering
2. **XSS Protection** - Sanitize user-provided variable values to prevent XSS attacks
3. **Fallback Templates** - Always have default system templates as fallback
4. **Cache Templates** - Cache frequently used templates for performance
5. **Plain Text** - Provide plain text version for email clients that don't support HTML

---

## 📚 Related Documentation

- [Email Queue Module](email-queue.md) - Email sending queue
- [Notifications Module](notifications.md) - In-app notifications
- [Background Jobs Module](background-jobs.md) - Email sending jobs

---

**[← Back to Modules](../MODULES.md)** | **[Next: Email Queue →](email-queue.md)**
