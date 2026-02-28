# 🔄 Custom Workflow Engine Module

Multi-level approval workflows with task assignment and auto-advancement.

---

## 📋 Overview

The Custom Workflow Engine provides a lightweight, JSON-based workflow system for approval processes. Unlike heavyweight workflow engines (Elsa, WWF), this module focuses on simplicity and ease of use while providing powerful multi-step approval capabilities.

---

## ✨ Key Features

- ✅ **JSON-based workflow definitions** - Easy to create and modify
- ✅ **Multi-level approvals** - Manager → Director → CFO chains
- ✅ **Task assignment** - Assign to specific users
- ✅ **Priority levels** - Low (1), Medium (2), High (3)
- ✅ **Due dates** - Track overdue tasks
- ✅ **Auto-advancement** - Moves to next step on approval
- ✅ **Rejection handling** - Cancels workflow on rejection
- ✅ **Comments support** - Approval/rejection reasons
- ✅ **Workflow versioning** - Track definition versions

---

## 🗄️ Entities

### WorkflowDefinition

```csharp
public class WorkflowDefinition : TenantScopedEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }  // "Approval", "Notification", "DataProcessing"
    public string ConfigurationJson { get; set; }  // Workflow steps as JSON
    public bool IsActive { get; set; }
    public int Version { get; set; }
}
```

**ConfigurationJson Structure:**
```json
{
  "steps": [
    {
      "name": "Manager Approval",
      "type": "Approval",
      "assignTo": "role:Manager",
      "onApprove": "next",
      "onReject": "cancel"
    },
    {
      "name": "Director Approval",
      "type": "Approval",
      "assignTo": "role:Director",
      "onApprove": "next",
      "onReject": "cancel"
    },
    {
      "name": "Complete",
      "type": "End"
    }
  ]
}
```

### WorkflowInstance

```csharp
public class WorkflowInstance : TenantScopedEntity
{
    public Guid WorkflowDefinitionId { get; set; }
    public string EntityType { get; set; }  // "PurchaseOrder", "LeaveRequest"
    public string EntityId { get; set; }
    public string Status { get; set; }  // "Running", "Completed", "Cancelled"
    public string CurrentStepName { get; set; }
    public string ContextJson { get; set; }  // Workflow variables
    public DateTime? CompletedAt { get; set; }

    public WorkflowDefinition WorkflowDefinition { get; set; }
}
```

### WorkflowTask

```csharp
public class WorkflowTask : TenantScopedEntity
{
    public Guid WorkflowInstanceId { get; set; }
    public string TaskName { get; set; }
    public string TaskType { get; set; }  // "Approval", "Review", "Notification"
    public Guid AssignedToUserId { get; set; }
    public string Status { get; set; }  // "Pending", "Approved", "Rejected"
    public int Priority { get; set; }  // 1=Low, 2=Medium, 3=High
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Guid? CompletedByUserId { get; set; }
    public string? Comments { get; set; }
    public string DataJson { get; set; }  // Task-specific data

    public WorkflowInstance WorkflowInstance { get; set; }
    public User AssignedToUser { get; set; }
    public User? CompletedByUser { get; set; }
}
```

---

## 🔄 Workflow Flow Diagram

```
┌─────────────────────────────────────────────────────────────┐
│  Purchase Order Created ($5,000)                            │
└───────────────────────┬─────────────────────────────────────┘
                        │
                        ↓
        ┌───────────────────────────────┐
        │  Start Workflow               │
        │  (Purchase Order Approval)    │
        └───────────────┬───────────────┘
                        │
                        ↓
        ┌────────────────────────────────────┐
        │  Step 1: Manager Approval          │
        │  → Create WorkflowTask             │
        │  → Assign to: John (Manager)       │
        │  → Priority: High                  │
        └────────┬───────────────────────────┘
                 │
        ┌────────┴────────┐
        │   Approved?     │
        └────┬────────────┴┐
             │ Yes         │ No
             ↓             ↓
  ┌──────────────────┐  ┌────────────────┐
  │ Step 2: Director │  │ Cancel         │
  │ Approval         │  │ Workflow       │
  │ → Assign to:     │  │ Status:        │
  │   Sarah          │  │ "Cancelled"    │
  └────┬─────────────┘  └────────────────┘
       │
       ↓ Approved
  ┌──────────────────┐
  │ Complete         │
  │ Status:          │
  │ "Completed"      │
  └──────────────────┘
```

---

## 🌐 API Endpoints

### Workflow Definitions

| Method | Endpoint | Description | Permission |
|--------|----------|-------------|------------|
| GET | `/api/workflow-definitions` | List definitions | WorkflowDefinition.Read |
| POST | `/api/workflow-definitions` | Create definition | WorkflowDefinition.Create |

### Workflow Instances

| Method | Endpoint | Description | Permission |
|--------|----------|-------------|------------|
| GET | `/api/workflow-instances` | List instances | Workflow.View |
| GET | `/api/workflow-instances/statistics` | Get stats | Workflow.View |
| POST | `/api/workflow-instances/start` | Start workflow | Workflow.Start |
| POST | `/api/workflow-instances/{id}/cancel` | Cancel workflow | Workflow.Cancel |

### Workflow Tasks

| Method | Endpoint | Description | Permission |
|--------|----------|-------------|------------|
| GET | `/api/workflow-tasks/my-tasks` | My pending tasks | WorkflowTask.View |
| GET | `/api/workflow-tasks/{id}` | Get task details | WorkflowTask.View |
| POST | `/api/workflow-tasks/{id}/complete` | Approve/reject | WorkflowTask.Complete |
| POST | `/api/workflow-tasks/{id}/reassign` | Reassign task | WorkflowTask.Reassign |

---

## 💻 Usage Examples

### Start a Workflow

```bash
curl -X POST https://api.coreengine.com/api/workflow-instances/start \
  -H "Authorization: Bearer TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "workflowDefinitionId": "123e4567-e89b-12d3-a456-426614174000",
    "entityType": "PurchaseOrder",
    "entityId": "po-2024-001",
    "contextJson": "{\"amount\": 5000, \"vendor\": \"Acme Corp\"}"
  }'
```

### Approve a Task

```bash
curl -X POST https://api.coreengine.com/api/workflow-tasks/{taskId}/complete \
  -H "Authorization: Bearer TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "isApproved": true,
    "comments": "Approved - within budget"
  }'
```

### Reject a Task

```bash
curl -X POST https://api.coreengine.com/api/workflow-tasks/{taskId}/complete \
  -H "Authorization: Bearer TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "isApproved": false,
    "comments": "Insufficient justification"
  }'
```

---

## 🎨 Frontend Pages

### 1. My Tasks Page (`/my-tasks`)

**Features:**
- List all pending tasks assigned to current user
- Priority badges (Low=blue, Medium=orange, High=red)
- Due date highlighting (overdue=red)
- Task details modal
- Approve/Reject buttons with comments

**Screenshot:**
```
┌─────────────────────────────────────────────────────────┐
│  My Tasks                                    🔔 3       │
├─────────────────────────────────────────────────────────┤
│                                                          │
│  ┌──────────────────────────────────────────────────┐  │
│  │ 🔴 HIGH │ Purchase Order Approval            │  │
│  │ Workflow: PO-2024-001                         │  │
│  │ Due: Today                                    │  │
│  │ [View Details] [✓ Approve] [✗ Reject]       │  │
│  └──────────────────────────────────────────────────┘  │
│                                                          │
│  ┌──────────────────────────────────────────────────┐  │
│  │ 🟠 MEDIUM │ Leave Request Approval           │  │
│  │ Workflow: LR-2024-045                         │  │
│  │ Due: Tomorrow                                 │  │
│  │ [View Details] [✓ Approve] [✗ Reject]       │  │
│  └──────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
```

### 2. Workflows Page (`/workflows`)

**Features:**
- Statistics cards (Total Definitions, Active Instances, Completed Today, Pending Tasks)
- List of workflow instances
- Filter by status (Running, Completed, Cancelled)
- View workflow history

### 3. Workflow Definitions Page (`/workflow-definitions`)

**Features:**
- List all workflow definitions
- Create new definition with JSON editor
- Activate/deactivate definitions
- Version management

---

## ⚙️ Configuration

### Seeded Workflows

**1. Purchase Order Approval:**
```json
{
  "name": "Purchase Order Approval",
  "category": "Approval",
  "configurationJson": {
    "steps": [
      {"name": "Manager Approval", "type": "Approval", "assignTo": "role:Manager"},
      {"name": "Director Approval", "type": "Approval", "assignTo": "role:Director"},
      {"name": "Complete", "type": "End"}
    ]
  }
}
```

**2. Leave Request Approval:**
```json
{
  "name": "Leave Request Approval",
  "category": "Approval",
  "configurationJson": {
    "steps": [
      {"name": "Manager Approval", "type": "Approval", "assignTo": "role:Manager"},
      {"name": "HR Approval", "type": "Approval", "assignTo": "role:HR"},
      {"name": "Complete", "type": "End"}
    ]
  }
}
```

---

## 🔐 Permissions

- **WorkflowDefinition.Create** - Create workflow definitions
- **WorkflowDefinition.Read** - View workflow definitions
- **WorkflowDefinition.Update** - Modify workflow definitions
- **WorkflowDefinition.Delete** - Delete workflow definitions
- **Workflow.Start** - Start new workflow instances
- **Workflow.View** - View workflow instances
- **Workflow.Cancel** - Cancel running workflows
- **WorkflowTask.View** - View assigned tasks
- **WorkflowTask.Complete** - Approve/reject tasks
- **WorkflowTask.Reassign** - Reassign tasks to others

---

## 📚 Related Documentation

- [My Tasks Module](my-tasks.md) - User task interface
- [Notifications Module](notifications.md) - Task assignment notifications
- [Email Module](email-templates.md) - Email notifications

---

**[← Back to Modules](../MODULES.md)** | **[API Reference →](../api/workflows.md)**
