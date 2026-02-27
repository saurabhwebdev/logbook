import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Card,
  Table,
  Button,
  Tag,
  Modal,
  Input,
  Select,
  message,
  Badge,
  Space,
  Empty,
} from 'antd';
import { ClockCircleOutlined } from '@ant-design/icons';
import { workflowTasksApi } from '../api/workflowTasksApi';
import type { WorkflowTask } from '../types';
import { useTenantTheme } from '../contexts/ThemeContext';
import dayjs from 'dayjs';
import relativeTime from 'dayjs/plugin/relativeTime';

dayjs.extend(relativeTime);

const { TextArea } = Input;

export default function MyTasksPage() {
  const { theme } = useTenantTheme();
  const primaryColor = theme?.primaryColor || '#0071e3';
  const queryClient = useQueryClient();

  const [selectedTask, setSelectedTask] = useState<WorkflowTask | null>(null);
  const [taskModalVisible, setTaskModalVisible] = useState(false);
  const [comments, setComments] = useState('');
  const [statusFilter, setStatusFilter] = useState<string | undefined>(undefined);

  const { data, isLoading } = useQuery({
    queryKey: ['my-tasks', statusFilter],
    queryFn: () => workflowTasksApi.getMyTasks(statusFilter, 1, 50),
  });

  const completeTaskMutation = useMutation({
    mutationFn: ({ taskId, status, comments }: { taskId: string; status: string; comments?: string }) =>
      workflowTasksApi.complete(taskId, status, comments),
    onSuccess: () => {
      message.success('Task completed successfully');
      queryClient.invalidateQueries({ queryKey: ['my-tasks'] });
      setTaskModalVisible(false);
      setSelectedTask(null);
      setComments('');
    },
    onError: () => {
      message.error('Failed to complete task');
    },
  });

  const handleTaskClick = (task: WorkflowTask) => {
    setSelectedTask(task);
    setTaskModalVisible(true);
  };

  const handleApprove = () => {
    if (selectedTask) {
      completeTaskMutation.mutate({
        taskId: selectedTask.id,
        status: 'Approved',
        comments,
      });
    }
  };

  const handleReject = () => {
    if (selectedTask) {
      completeTaskMutation.mutate({
        taskId: selectedTask.id,
        status: 'Rejected',
        comments,
      });
    }
  };

  const getPriorityBadge = (priority: number) => {
    const colors = { 1: 'default', 2: 'blue', 3: 'red' };
    const labels = { 1: 'Low', 2: 'Medium', 3: 'High' };
    return <Badge color={colors[priority as keyof typeof colors]} text={labels[priority as keyof typeof labels]} />;
  };

  const getStatusTag = (status: string) => {
    const colors: Record<string, string> = {
      Pending: 'orange',
      Approved: 'green',
      Rejected: 'red',
      Completed: 'blue',
    };
    return <Tag color={colors[status] || 'default'}>{status}</Tag>;
  };

  const columns = [
    {
      title: 'TASK',
      dataIndex: 'taskName',
      key: 'taskName',
      render: (text: string, record: WorkflowTask) => (
        <div>
          <div style={{ fontWeight: 500, color: '#1d1d1f' }}>{text}</div>
          <div style={{ fontSize: 12, color: '#86868b' }}>{record.workflowDefinitionName}</div>
        </div>
      ),
    },
    {
      title: 'PRIORITY',
      dataIndex: 'priority',
      key: 'priority',
      width: 120,
      render: (priority: number) => getPriorityBadge(priority),
    },
    {
      title: 'STATUS',
      dataIndex: 'status',
      key: 'status',
      width: 120,
      render: (status: string) => getStatusTag(status),
    },
    {
      title: 'DUE DATE',
      dataIndex: 'dueDate',
      key: 'dueDate',
      width: 150,
      render: (date: string | undefined) =>
        date ? (
          <span style={{ color: '#6e6e73' }}>{dayjs(date).format('MMM D, YYYY')}</span>
        ) : (
          <span style={{ color: '#86868b' }}>No due date</span>
        ),
    },
    {
      title: 'CREATED',
      dataIndex: 'createdAt',
      key: 'createdAt',
      width: 130,
      render: (date: string) => (
        <span style={{ color: '#86868b', fontSize: 13 }}>{dayjs(date).fromNow()}</span>
      ),
    },
  ];

  return (
    <div style={{ padding: 24 }}>
      <div style={{ marginBottom: 24, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <div>
          <h1 style={{ margin: 0, fontSize: 28, fontWeight: 600, color: '#1d1d1f' }}>
            My Tasks
          </h1>
          <p style={{ margin: '4px 0 0', color: '#86868b', fontSize: 14 }}>
            Tasks assigned to you
          </p>
        </div>
        <Space>
          <Select
            placeholder="Filter by status"
            style={{ width: 150 }}
            allowClear
            value={statusFilter}
            onChange={setStatusFilter}
            options={[
              { label: 'Pending', value: 'Pending' },
              { label: 'Approved', value: 'Approved' },
              { label: 'Rejected', value: 'Rejected' },
            ]}
          />
        </Space>
      </div>

      <Card
        style={{
          borderRadius: 12,
          border: '1px solid #e5e5ea',
          boxShadow: 'none',
        }}
      >
        <Table
          dataSource={data?.items || []}
          columns={columns}
          rowKey="id"
          loading={isLoading}
          pagination={false}
          onRow={(record) => ({
            onClick: () => handleTaskClick(record),
            style: { cursor: 'pointer' },
          })}
          locale={{
            emptyText: (
              <Empty
                image={Empty.PRESENTED_IMAGE_SIMPLE}
                description={<span style={{ color: '#86868b' }}>No tasks assigned to you</span>}
              />
            ),
          }}
        />
      </Card>

      <Modal
        title={
          <div>
            <div style={{ fontSize: 18, fontWeight: 600, color: '#1d1d1f' }}>
              {selectedTask?.taskName}
            </div>
            <div style={{ fontSize: 13, color: '#86868b', marginTop: 4 }}>
              {selectedTask?.workflowDefinitionName}
            </div>
          </div>
        }
        open={taskModalVisible}
        onCancel={() => {
          setTaskModalVisible(false);
          setSelectedTask(null);
          setComments('');
        }}
        footer={
          selectedTask?.status === 'Pending' ? (
            <Space>
              <Button onClick={() => setTaskModalVisible(false)}>Cancel</Button>
              <Button
                danger
                onClick={handleReject}
                loading={completeTaskMutation.isPending}
              >
                Reject
              </Button>
              <Button
                type="primary"
                style={{ backgroundColor: primaryColor }}
                onClick={handleApprove}
                loading={completeTaskMutation.isPending}
              >
                Approve
              </Button>
            </Space>
          ) : null
        }
        width={600}
      >
        {selectedTask && (
          <div style={{ marginTop: 16 }}>
            <div style={{ marginBottom: 16 }}>
              <div style={{ fontSize: 12, color: '#86868b', marginBottom: 4 }}>Priority</div>
              {getPriorityBadge(selectedTask.priority)}
            </div>

            <div style={{ marginBottom: 16 }}>
              <div style={{ fontSize: 12, color: '#86868b', marginBottom: 4 }}>Status</div>
              {getStatusTag(selectedTask.status)}
            </div>

            {selectedTask.dueDate && (
              <div style={{ marginBottom: 16 }}>
                <div style={{ fontSize: 12, color: '#86868b', marginBottom: 4 }}>Due Date</div>
                <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                  <ClockCircleOutlined style={{ color: '#86868b' }} />
                  <span>{dayjs(selectedTask.dueDate).format('MMMM D, YYYY')}</span>
                </div>
              </div>
            )}

            <div style={{ marginBottom: 16 }}>
              <div style={{ fontSize: 12, color: '#86868b', marginBottom: 4 }}>Entity</div>
              <div>{selectedTask.entityType}: {selectedTask.entityId}</div>
            </div>

            {selectedTask.status === 'Pending' && (
              <div style={{ marginTop: 24 }}>
                <div style={{ fontSize: 12, color: '#86868b', marginBottom: 8 }}>Comments (optional)</div>
                <TextArea
                  rows={4}
                  value={comments}
                  onChange={(e) => setComments(e.target.value)}
                  placeholder="Add comments about your decision..."
                />
              </div>
            )}

            {selectedTask.status !== 'Pending' && selectedTask.comments && (
              <div style={{ marginTop: 24 }}>
                <div style={{ fontSize: 12, color: '#86868b', marginBottom: 8 }}>Comments</div>
                <div
                  style={{
                    padding: 12,
                    backgroundColor: '#f5f5f7',
                    borderRadius: 8,
                    color: '#1d1d1f',
                  }}
                >
                  {selectedTask.comments}
                </div>
              </div>
            )}

            {selectedTask.completedAt && (
              <div style={{ marginTop: 16, paddingTop: 16, borderTop: '1px solid #f2f2f7' }}>
                <div style={{ fontSize: 12, color: '#86868b' }}>
                  Completed by {selectedTask.completedByUserName} on{' '}
                  {dayjs(selectedTask.completedAt).format('MMMM D, YYYY h:mm A')}
                </div>
              </div>
            )}
          </div>
        )}
      </Modal>
    </div>
  );
}
