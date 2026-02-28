# ⚙️ Workflow Instances API

Start and manage workflow executions.

---

## Endpoints

### GET /api/WorkflowInstances

List workflow instances with filtering.

**Authorization:** Required
**Permission:** `Workflow.View`

**Query Parameters:**
- `workflowDefinitionId` (guid, optional) - Filter by definition
- `entityType` (string, optional) - Filter by entity type (e.g., "PurchaseOrder")
- `status` (string, optional) - Filter by status: "Running", "Completed", "Cancelled"
- `pageNumber` (integer, default: 1)
- `pageSize` (integer, default: 10, max: 100)

**Response (200 OK):**
```json
{
  "items": [
    {
      "id": "123e4567-e89b-12d3-a456-426614174000",
      "workflowDefinitionId": "456e7890-e89b-12d3-a456-426614174001",
      "workflowDefinitionName": "Purchase Order Approval",
      "entityType": "PurchaseOrder",
      "entityId": "po-2024-001",
      "status": "Running",
      "currentStepName": "DirectorApproval",
      "contextJson": "{\"amount\": 5000, \"vendor\": \"Acme Corp\"}",
      "createdAt": "2026-02-27T10:00:00Z",
      "completedAt": null
    }
  ],
  "pageNumber": 1,
  "totalPages": 1,
  "totalCount": 1
}
```

**cURL Example:**
```bash
curl -X GET "http://localhost:5034/api/WorkflowInstances?status=Running&pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

---

### GET /api/WorkflowInstances/statistics

Get workflow statistics for current tenant.

**Authorization:** Required
**Permission:** `Workflow.View`

**Response (200 OK):**
```json
{
  "totalDefinitions": 5,
  "activeInstances": 12,
  "completedToday": 8,
  "pendingTasks": 15
}
```

**cURL Example:**
```bash
curl -X GET "http://localhost:5034/api/WorkflowInstances/statistics" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

---

### POST /api/WorkflowInstances

Start a new workflow instance.

**Authorization:** Required
**Permission:** `Workflow.Start`

**Request Body:**
```json
{
  "workflowDefinitionId": "456e7890-e89b-12d3-a456-426614174001",
  "entityType": "PurchaseOrder",
  "entityId": "po-2024-001",
  "contextJson": "{\"amount\": 5000, \"vendor\": \"Acme Corp\", \"requestedBy\": \"John Doe\"}"
}
```

**Field Descriptions:**
- `workflowDefinitionId` - ID of the workflow definition to execute
- `entityType` - Type of entity (e.g., "PurchaseOrder", "LeaveRequest", "ExpenseReport")
- `entityId` - Unique identifier for the entity being processed
- `contextJson` - JSON string with workflow variables (used in templates, notifications)

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
      "WorkflowDefinitionId": ["Workflow definition not found"],
      "ContextJson": ["Invalid JSON format"]
    }
  }
  ```

- **404 Not Found** - Workflow definition not found or inactive

**cURL Example:**
```bash
curl -X POST "http://localhost:5034/api/WorkflowInstances" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "workflowDefinitionId": "456e7890-e89b-12d3-a456-426614174001",
    "entityType": "PurchaseOrder",
    "entityId": "po-2024-001",
    "contextJson": "{\"amount\": 5000}"
  }'
```

---

### POST /api/WorkflowInstances/{id}/cancel

Cancel a running workflow instance.

**Authorization:** Required
**Permission:** `Workflow.Cancel`

**Path Parameters:**
- `id` (guid) - Workflow instance ID

**Request Body:**
```json
{
  "reason": "No longer needed"
}
```

**Response (200 OK):**
```json
{
  "message": "Workflow cancelled successfully"
}
```

**Error Responses:**
- **404 Not Found** - Workflow instance not found
- **400 Bad Request** - Workflow already completed or cancelled

**cURL Example:**
```bash
curl -X POST "http://localhost:5034/api/WorkflowInstances/123e4567-e89b-12d3-a456-426614174000/cancel" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "reason": "Request withdrawn"
  }'
```

---

## Workflow Execution Flow

```
┌─────────────────────────────────────────────────────────────┐
│  1. Start Workflow Instance                                  │
│     POST /api/WorkflowInstances                              │
│     → Creates instance with status "Running"                 │
│     → Sets current step to first step (Start)                │
└───────────────────────┬─────────────────────────────────────┘
                        │
                        ↓
        ┌───────────────────────────────┐
        │  2. Process Start Step        │
        │     → Move to next step       │
        └───────────────┬───────────────┘
                        │
                        ↓
        ┌────────────────────────────────────┐
        │  3. Approval Step                  │
        │     → Create WorkflowTask          │
        │     → Assign to user/role          │
        │     → Send notification            │
        └────────┬───────────────────────────┘
                 │
        ┌────────┴────────┐
        │   Approved?     │
        └────┬────────────┴┐
             │ Yes         │ No
             ↓             ↓
  ┌──────────────────┐  ┌────────────────┐
  │ Move to Next     │  │ Cancel         │
  │ Step             │  │ Workflow       │
  │ → Repeat Step 3  │  │ Status:        │
  │   if more steps  │  │ "Cancelled"    │
  └────┬─────────────┘  └────────────────┘
       │
       ↓ All approved
  ┌──────────────────┐
  │ Complete         │
  │ Status:          │
  │ "Completed"      │
  │ Set CompletedAt  │
  └──────────────────┘
```

---

## Workflow Status Values

| Status | Description |
|--------|-------------|
| Running | Workflow is actively executing |
| Completed | Workflow reached End step successfully |
| Cancelled | Workflow was cancelled (task rejected or manually cancelled) |

---

## Context JSON Usage

The `contextJson` field stores workflow variables that can be used in:

1. **Email Templates** - Variable substitution
   ```json
   {
     "amount": 5000,
     "vendor": "Acme Corp",
     "requestedBy": "John Doe"
   }
   ```

   Email template can use: `{{amount}}`, `{{vendor}}`, `{{requestedBy}}`

2. **Notifications** - Dynamic messages
   ```json
   {
     "taskName": "Approve Purchase Order",
     "amount": 5000
   }
   ```

3. **Conditional Logic** - Future enhancement for conditional routing
   ```json
   {
     "amount": 5000,
     "urgency": "high"
   }
   ```

---

## Permissions

| Permission | Description |
|------------|-------------|
| Workflow.Start | Start new workflow instances |
| Workflow.View | View workflow instances |
| Workflow.Cancel | Cancel running workflows |

---

## Business Rules

### Starting Workflows
- Workflow definition must be active
- EntityType + EntityId combination should be unique (one workflow per entity at a time)
- ContextJson must be valid JSON

### Cancellation
- Only running workflows can be cancelled
- Cancellation is permanent (cannot be restarted)
- All pending tasks are marked as cancelled

### Completion
- Workflow completes when End step is reached
- CompletedAt timestamp is set
- No further tasks can be created

---

## Integration Examples

### 1. Purchase Order Workflow

```bash
# 1. Create purchase order in your system
ORDER_ID="po-2024-001"

# 2. Start approval workflow
WORKFLOW_ID=$(curl -s -X POST "http://localhost:5034/api/WorkflowInstances" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d "{
    \"workflowDefinitionId\": \"$PURCHASE_ORDER_WORKFLOW_ID\",
    \"entityType\": \"PurchaseOrder\",
    \"entityId\": \"$ORDER_ID\",
    \"contextJson\": \"{\\\"amount\\\": 5000, \\\"vendor\\\": \\\"Acme Corp\\\"}\"
  }" | jq -r '.')

# 3. Monitor workflow status
curl -X GET "http://localhost:5034/api/WorkflowInstances/$WORKFLOW_ID" \
  -H "Authorization: Bearer $TOKEN"
```

### 2. Leave Request Workflow

```bash
# Start leave request workflow
curl -X POST "http://localhost:5034/api/WorkflowInstances" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "workflowDefinitionId": "LEAVE_REQUEST_WORKFLOW_ID",
    "entityType": "LeaveRequest",
    "entityId": "lr-2024-045",
    "contextJson": "{\"employeeName\": \"John Doe\", \"startDate\": \"2026-03-15\", \"endDate\": \"2026-03-20\", \"type\": \"Vacation\"}"
  }'
```

---

**[← Back to API Reference](../API.md)** | **[Workflow Tasks API →](workflow-tasks.md)**
