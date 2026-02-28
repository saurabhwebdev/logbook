# 🔄 Workflow Definitions API

Manage workflow templates and definitions.

---

## Endpoints

### GET /api/WorkflowDefinitions

List workflow definitions with pagination and filtering.

**Authorization:** Required
**Permission:** `WorkflowDefinition.Read`

**Query Parameters:**
- `searchTerm` (string, optional) - Search by name or description
- `category` (string, optional) - Filter by category (e.g., "Approval", "Notification")
- `isActive` (boolean, optional) - Filter by active status
- `pageNumber` (integer, default: 1)
- `pageSize` (integer, default: 10, max: 100)

**Response (200 OK):**
```json
{
  "items": [
    {
      "id": "123e4567-e89b-12d3-a456-426614174000",
      "name": "Purchase Order Approval",
      "description": "Multi-level approval workflow for purchase orders",
      "category": "Approval",
      "configurationJson": "{\n  \"Steps\": [\n    { \"Name\": \"Submit\", \"Type\": \"Start\" },\n    { \"Name\": \"ManagerApproval\", \"Type\": \"Approval\" },\n    { \"Name\": \"DirectorApproval\", \"Type\": \"Approval\" },\n    { \"Name\": \"Complete\", \"Type\": \"End\" }\n  ]\n}",
      "isActive": true,
      "version": 1,
      "createdAt": "2026-02-01T10:00:00Z"
    },
    {
      "id": "456e7890-e89b-12d3-a456-426614174001",
      "name": "Leave Request Approval",
      "description": "Employee leave request approval workflow",
      "category": "Approval",
      "configurationJson": "{\n  \"Steps\": [\n    { \"Name\": \"Submit\", \"Type\": \"Start\" },\n    { \"Name\": \"ManagerApproval\", \"Type\": \"Approval\" },\n    { \"Name\": \"HRApproval\", \"Type\": \"Approval\" },\n    { \"Name\": \"Complete\", \"Type\": \"End\" }\n  ]\n}",
      "isActive": true,
      "version": 1,
      "createdAt": "2026-02-01T10:00:00Z"
    }
  ],
  "pageNumber": 1,
  "totalPages": 1,
  "totalCount": 2
}
```

**cURL Example:**
```bash
curl -X GET "http://localhost:5034/api/WorkflowDefinitions?category=Approval&pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

---

### POST /api/WorkflowDefinitions

Create new workflow definition.

**Authorization:** Required
**Permission:** `WorkflowDefinition.Create`

**Request Body:**
```json
{
  "name": "Expense Report Approval",
  "description": "Multi-step approval for expense reports over $500",
  "category": "Approval",
  "configurationJson": "{\n  \"Steps\": [\n    {\n      \"Name\": \"SubmitExpense\",\n      \"Type\": \"Start\"\n    },\n    {\n      \"Name\": \"ManagerApproval\",\n      \"Type\": \"Approval\",\n      \"AssignTo\": \"role:Manager\",\n      \"OnApprove\": \"next\",\n      \"OnReject\": \"cancel\"\n    },\n    {\n      \"Name\": \"FinanceApproval\",\n      \"Type\": \"Approval\",\n      \"AssignTo\": \"role:Finance\",\n      \"OnApprove\": \"next\",\n      \"OnReject\": \"cancel\"\n    },\n    {\n      \"Name\": \"Complete\",\n      \"Type\": \"End\"\n    }\n  ]\n}",
  "isActive": true
}
```

**ConfigurationJson Structure:**
```json
{
  "Steps": [
    {
      "Name": "StepName",
      "Type": "Start|Approval|Notification|End",
      "AssignTo": "role:RoleName|user:UserId",
      "OnApprove": "next|complete|cancel",
      "OnReject": "cancel|previous"
    }
  ]
}
```

**Response (200 OK):**
```json
"123e4567-e89b-12d3-a456-426614174000"
```

**Error Responses:**
- **400 Bad Request** - Validation error
  ```json
  {
    "type": "ValidationError",
    "errors": {
      "Name": ["Name is required"],
      "ConfigurationJson": ["Invalid JSON format"]
    }
  }
  ```

**cURL Example:**
```bash
curl -X POST "http://localhost:5034/api/WorkflowDefinitions" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Expense Report Approval",
    "description": "Expense approval workflow",
    "category": "Approval",
    "configurationJson": "{\"Steps\": [{\"Name\": \"Submit\", \"Type\": \"Start\"}, {\"Name\": \"Complete\", \"Type\": \"End\"}]}",
    "isActive": true
  }'
```

---

## Configuration JSON Schema

### Step Types

1. **Start** - Initial step (required, must be first)
   ```json
   {
     "Name": "Submit",
     "Type": "Start"
   }
   ```

2. **Approval** - Approval task assignment
   ```json
   {
     "Name": "ManagerApproval",
     "Type": "Approval",
     "AssignTo": "role:Manager",
     "OnApprove": "next",
     "OnReject": "cancel"
   }
   ```

3. **Notification** - Send notification
   ```json
   {
     "Name": "NotifySubmitter",
     "Type": "Notification",
     "Recipients": "submitter",
     "Message": "Your request has been approved"
   }
   ```

4. **End** - Final step (required, must be last)
   ```json
   {
     "Name": "Complete",
     "Type": "End"
   }
   ```

### Assignment Targets

- `"role:RoleName"` - Assign to first user with role
- `"user:UserId"` - Assign to specific user (GUID)
- `"department:DepartmentId"` - Assign to department manager

### Actions

- `"next"` - Move to next step
- `"complete"` - Complete workflow
- `"cancel"` - Cancel workflow
- `"previous"` - Go back to previous step

---

## Workflow Categories

| Category | Description | Example Use Cases |
|----------|-------------|-------------------|
| Approval | Multi-level approval processes | Purchase orders, leave requests, expense reports |
| Notification | Automated notifications | Welcome emails, deadline reminders |
| DataProcessing | Background data operations | Import/export, calculations |
| Custom | Custom workflows | Any business-specific process |

---

## Permissions

| Permission | Description |
|------------|-------------|
| WorkflowDefinition.Create | Create workflow definitions |
| WorkflowDefinition.Read | View workflow definitions |
| WorkflowDefinition.Update | Modify workflow definitions |
| WorkflowDefinition.Delete | Delete workflow definitions |

---

## Business Rules

### Validation Rules
- Name must be unique within tenant
- ConfigurationJson must be valid JSON
- Must have exactly one Start step
- Must have exactly one End step
- Step names must be unique within workflow
- Approval steps must have AssignTo specified

### Versioning
- Version number auto-increments on updates
- Previous versions are preserved (audit trail)
- Only latest version is active

### Activation
- Inactive workflows cannot be started
- Running instances continue even if definition is deactivated
- Reactivation does not affect existing instances

---

## Example Workflows

### Simple Two-Step Approval
```json
{
  "Steps": [
    {
      "Name": "Submit",
      "Type": "Start"
    },
    {
      "Name": "ManagerApproval",
      "Type": "Approval",
      "AssignTo": "role:Manager",
      "OnApprove": "next",
      "OnReject": "cancel"
    },
    {
      "Name": "Complete",
      "Type": "End"
    }
  ]
}
```

### Three-Level Approval Chain
```json
{
  "Steps": [
    {
      "Name": "Submit",
      "Type": "Start"
    },
    {
      "Name": "ManagerApproval",
      "Type": "Approval",
      "AssignTo": "role:Manager",
      "OnApprove": "next",
      "OnReject": "cancel"
    },
    {
      "Name": "DirectorApproval",
      "Type": "Approval",
      "AssignTo": "role:Director",
      "OnApprove": "next",
      "OnReject": "cancel"
    },
    {
      "Name": "CFOApproval",
      "Type": "Approval",
      "AssignTo": "role:CFO",
      "OnApprove": "next",
      "OnReject": "cancel"
    },
    {
      "Name": "Complete",
      "Type": "End"
    }
  ]
}
```

---

**[← Back to API Reference](../API.md)** | **[Workflow Instances API →](workflow-instances.md)**
