# ✅ Workflow Tasks API

Task assignment, approval, and rejection endpoints.

---

## Endpoints

### GET /api/WorkflowTasks/my-tasks

Get pending tasks assigned to current user.

**Authorization:** Required
**Permission:** `WorkflowTask.View`

**Query Parameters:**
- `status` (string, optional) - Filter by status: "Pending", "Approved", "Rejected"
- `priority` (integer, optional) - Filter by priority: 1 (Low), 2 (Medium), 3 (High)
- `pageNumber` (integer, default: 1)
- `pageSize` (integer, default: 10, max: 100)

**Response (200 OK):**
```json
{
  "items": [
    {
      "id": "123e4567-e89b-12d3-a456-426614174000",
      "workflowInstanceId": "456e7890-e89b-12d3-a456-426614174001",
      "taskName": "Manager Approval",
      "taskType": "Approval",
      "assignedToUserId": "789e0123-e89b-12d3-a456-426614174002",
      "assignedToUserName": "John Doe",
      "status": "Pending",
      "priority": 3,
      "dueDate": "2026-02-28T17:00:00Z",
      "completedAt": null,
      "completedByUserId": null,
      "comments": null,
      "dataJson": "{\"entityType\": \"PurchaseOrder\", \"entityId\": \"po-2024-001\", \"amount\": 5000}",
      "createdAt": "2026-02-27T10:00:00Z"
    }
  ],
  "pageNumber": 1,
  "totalPages": 1,
  "totalCount": 1
}
```

**cURL Example:**
```bash
curl -X GET "http://localhost:5034/api/WorkflowTasks/my-tasks?status=Pending&pageNumber=1&pageSize=20" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

---

### GET /api/WorkflowTasks/{id}

Get task details by ID.

**Authorization:** Required
**Permission:** `WorkflowTask.View`

**Path Parameters:**
- `id` (guid) - Task ID

**Response (200 OK):**
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "workflowInstanceId": "456e7890-e89b-12d3-a456-426614174001",
  "workflowInstance": {
    "id": "456e7890-e89b-12d3-a456-426614174001",
    "workflowDefinitionName": "Purchase Order Approval",
    "entityType": "PurchaseOrder",
    "entityId": "po-2024-001",
    "status": "Running"
  },
  "taskName": "Manager Approval",
  "taskType": "Approval",
  "assignedToUserId": "789e0123-e89b-12d3-a456-426614174002",
  "assignedToUserName": "John Doe",
  "status": "Pending",
  "priority": 3,
  "dueDate": "2026-02-28T17:00:00Z",
  "completedAt": null,
  "completedByUserId": null,
  "completedByUserName": null,
  "comments": null,
  "dataJson": "{\"entityType\": \"PurchaseOrder\", \"entityId\": \"po-2024-001\", \"amount\": 5000, \"vendor\": \"Acme Corp\"}",
  "createdAt": "2026-02-27T10:00:00Z"
}
```

**Error Responses:**
- **404 Not Found** - Task not found
- **403 Forbidden** - Task not assigned to current user

**cURL Example:**
```bash
curl -X GET "http://localhost:5034/api/WorkflowTasks/123e4567-e89b-12d3-a456-426614174000" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

---

### POST /api/WorkflowTasks/{id}/complete

Approve or reject a task.

**Authorization:** Required
**Permission:** `WorkflowTask.Complete`

**Path Parameters:**
- `id` (guid) - Task ID

**Request Body:**
```json
{
  "status": "Approved",
  "comments": "Approved - within budget"
}
```

**Field Descriptions:**
- `status` - Must be "Approved" or "Rejected"
- `comments` - Optional reason for approval/rejection

**Response (200 OK):**
```json
{
  "message": "Task completed successfully",
  "workflowStatus": "Running",
  "nextStepName": "DirectorApproval"
}
```

**Error Responses:**
- **400 Bad Request** - Invalid status or task already completed
  ```json
  {
    "type": "ValidationError",
    "errors": {
      "Status": ["Status must be 'Approved' or 'Rejected'"]
    }
  }
  ```

- **403 Forbidden** - Task not assigned to current user
- **404 Not Found** - Task not found

**cURL Example (Approve):**
```bash
curl -X POST "http://localhost:5034/api/WorkflowTasks/123e4567-e89b-12d3-a456-426614174000/complete" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "status": "Approved",
    "comments": "Looks good, approved"
  }'
```

**cURL Example (Reject):**
```bash
curl -X POST "http://localhost:5034/api/WorkflowTasks/123e4567-e89b-12d3-a456-426614174000/complete" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "status": "Rejected",
    "comments": "Insufficient budget justification"
  }'
```

---

### POST /api/WorkflowTasks/{id}/reassign

Reassign task to another user.

**Authorization:** Required
**Permission:** `WorkflowTask.Reassign`

**Path Parameters:**
- `id` (guid) - Task ID

**Request Body:**
```json
{
  "newAssigneeUserId": "789e0123-e89b-12d3-a456-426614174002",
  "reason": "Manager is on vacation"
}
```

**Response (200 OK):**
```json
{
  "message": "Task reassigned successfully"
}
```

**Error Responses:**
- **404 Not Found** - Task or new assignee not found
- **400 Bad Request** - Task already completed

**cURL Example:**
```bash
curl -X POST "http://localhost:5034/api/WorkflowTasks/123e4567-e89b-12d3-a456-426614174000/reassign" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "newAssigneeUserId": "789e0123-e89b-12d3-a456-426614174002",
    "reason": "Delegating to team lead"
  }'
```

---

## Task Priority Levels

| Priority | Value | Description | Color |
|----------|-------|-------------|-------|
| Low | 1 | Standard priority, no urgency | Blue |
| Medium | 2 | Needs attention soon | Orange |
| High | 3 | Urgent, requires immediate action | Red |

---

## Task Status Values

| Status | Description |
|--------|-------------|
| Pending | Awaiting user action |
| Approved | Task approved, workflow advanced |
| Rejected | Task rejected, workflow cancelled |

---

## Due Date Handling

**Due Date Calculation:**
- Set when task is created
- Default: 2 business days from creation
- Configurable per workflow definition

**Overdue Tasks:**
- Tasks past due date are highlighted in UI
- No automatic actions (workflow doesn't auto-cancel)
- Managers can see overdue tasks in reports

**Reminders:**
- Email sent 24 hours before due date
- Email sent on due date if still pending
- Email sent daily after due date until completed

---

## Task Data JSON

The `dataJson` field contains workflow context specific to this task:

```json
{
  "entityType": "PurchaseOrder",
  "entityId": "po-2024-001",
  "amount": 5000,
  "vendor": "Acme Corp",
  "requestedBy": "John Doe",
  "requestDate": "2026-02-27"
}
```

**Frontend Usage:**
- Display purchase order details
- Link to entity (e.g., `/purchase-orders/po-2024-001`)
- Show relevant context for decision-making

---

## Permissions

| Permission | Description |
|------------|-------------|
| WorkflowTask.View | View assigned tasks |
| WorkflowTask.Complete | Approve/reject tasks |
| WorkflowTask.Reassign | Reassign tasks to others |

---

## Business Rules

### Task Assignment
- Only one user can be assigned per task
- User must have appropriate role if task specifies role requirement
- Assignment triggers notification to user

### Task Completion
- Only assigned user can complete task (unless SuperAdmin)
- Comments are required for rejections (optional for approvals)
- Completing task advances or cancels workflow based on configuration

### Task Reassignment
- Only pending tasks can be reassigned
- Reassignment creates audit log entry
- New assignee receives notification

---

## Notification Triggers

### Task Created
- **Recipient:** Assigned user
- **Type:** In-app + Email
- **Content:** "You have been assigned: {taskName}"

### Task Completed
- **Recipient:** Workflow initiator
- **Type:** In-app
- **Content:** "{taskName} was {approved/rejected} by {userName}"

### Task Overdue
- **Recipient:** Assigned user
- **Type:** Email
- **Content:** "Task {taskName} is overdue"

### Task Reassigned
- **Recipients:** Old assignee + New assignee
- **Type:** In-app + Email
- **Content:** "Task reassigned from {oldUser} to {newUser}"

---

## Example User Flow

### Approving a Task

```bash
# 1. Get my pending tasks
curl -X GET "http://localhost:5034/api/WorkflowTasks/my-tasks?status=Pending" \
  -H "Authorization: Bearer $TOKEN"

# Response shows task ID: 123e4567-...

# 2. Get task details
curl -X GET "http://localhost:5034/api/WorkflowTasks/123e4567-..." \
  -H "Authorization: Bearer $TOKEN"

# 3. Approve the task
curl -X POST "http://localhost:5034/api/WorkflowTasks/123e4567-.../complete" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "status": "Approved",
    "comments": "Budget verified, approved"
  }'
```

### Rejecting a Task

```bash
# Reject with required comment
curl -X POST "http://localhost:5034/api/WorkflowTasks/123e4567-.../complete" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "status": "Rejected",
    "comments": "Amount exceeds department budget. Please reduce scope or get VP approval."
  }'
```

### Reassigning a Task

```bash
# Reassign to another user (e.g., delegate during vacation)
curl -X POST "http://localhost:5034/api/WorkflowTasks/123e4567-.../reassign" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "newAssigneeUserId": "789e0123-...",
    "reason": "Out of office until March 5"
  }'
```

---

## Real-time Updates

Tasks support real-time updates via SignalR:

```typescript
// Frontend: Listen for task updates
connection.on('TaskAssigned', (task) => {
  // Refresh my tasks list
  queryClient.invalidateQueries(['workflow-tasks', 'my-tasks']);

  // Show notification
  message.info(`New task assigned: ${task.taskName}`);
});

connection.on('TaskCompleted', (taskId, status) => {
  // Update UI
  queryClient.invalidateQueries(['workflow-tasks']);
});
```

---

**[← Back to API Reference](../API.md)** | **[← Workflow Instances API](workflow-instances.md)**
