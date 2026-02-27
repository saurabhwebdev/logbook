import { useState } from 'react';
import { Typography, Table, Flex, Button, Modal, Form, Input, Select, message, Popconfirm, Timeline, Dropdown } from 'antd';
import { PlusOutlined, DeleteOutlined, HistoryOutlined, ThunderboltOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import type { MenuProps } from 'antd';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import dayjs from 'dayjs';
import { demoTasksApi } from '../api/demoTasksApi';
import type { DemoTask, TransitionLog, StateTransitionDefinition } from '../types';

const { Text } = Typography;

const stateColors: Record<string, string> = {
  Draft: '#86868b',
  Open: '#0071e3',
  InProgress: '#ff9500',
  Review: '#af52de',
  Done: '#34c759',
  Cancelled: '#ff3b30',
};

const priorityColors: Record<string, string> = {
  Low: '#86868b',
  Medium: '#0071e3',
  High: '#ff9500',
  Critical: '#ff3b30',
};

export default function DemoTasksPage() {
  const [createOpen, setCreateOpen] = useState(false);
  const [historyTaskId, setHistoryTaskId] = useState<string | null>(null);
  const [form] = Form.useForm();
  const queryClient = useQueryClient();

  const { data, isLoading } = useQuery({ queryKey: ['demoTasks'], queryFn: demoTasksApi.getAll });
  const statesQuery = useQuery({ queryKey: ['demoTaskStates'], queryFn: demoTasksApi.getStates });

  const historyQuery = useQuery({
    queryKey: ['demoTaskHistory', historyTaskId],
    queryFn: () => demoTasksApi.getHistory(historyTaskId!),
    enabled: !!historyTaskId,
  });

  const createMutation = useMutation({
    mutationFn: demoTasksApi.create,
    onSuccess: () => {
      message.success('Task created');
      queryClient.invalidateQueries({ queryKey: ['demoTasks'] });
      setCreateOpen(false);
      form.resetFields();
    },
    onError: () => message.error('Failed to create task'),
  });

  const transitionMutation = useMutation({
    mutationFn: ({ id, triggerName }: { id: string; triggerName: string }) =>
      demoTasksApi.transition(id, triggerName),
    onSuccess: () => {
      message.success('State transitioned');
      queryClient.invalidateQueries({ queryKey: ['demoTasks'] });
      queryClient.invalidateQueries({ queryKey: ['demoTaskHistory'] });
    },
    onError: () => message.error('Transition failed'),
  });

  const deleteMutation = useMutation({
    mutationFn: demoTasksApi.delete,
    onSuccess: () => {
      message.success('Task deleted');
      queryClient.invalidateQueries({ queryKey: ['demoTasks'] });
    },
  });

  const getAvailableTransitions = (currentState: string): StateTransitionDefinition[] => {
    if (!statesQuery.data) return [];
    return statesQuery.data.transitions.filter((t) => t.fromState === currentState);
  };

  const columns: ColumnsType<DemoTask> = [
    {
      title: 'Task',
      key: 'title',
      render: (_, r) => (
        <div>
          <div style={{ fontWeight: 500, color: '#1d1d1f', fontSize: 13 }}>{r.title}</div>
          {r.description && <div style={{ fontSize: 12, color: '#86868b' }}>{r.description}</div>}
        </div>
      ),
      sorter: (a, b) => a.title.localeCompare(b.title),
    },
    {
      title: 'State',
      key: 'currentState',
      width: 130,
      render: (_, r) => (
        <span style={{ display: 'inline-flex', alignItems: 'center', gap: 6, fontSize: 12, fontWeight: 500, color: stateColors[r.currentState] || '#86868b' }}>
          <span style={{ width: 8, height: 8, borderRadius: '50%', background: stateColors[r.currentState] || '#86868b' }} />
          {r.currentState}
        </span>
      ),
      filters: Object.keys(stateColors).map((s) => ({ text: s, value: s })),
      onFilter: (value, r) => r.currentState === value,
    },
    {
      title: 'Priority',
      key: 'priority',
      width: 100,
      render: (_, r) => (
        <span style={{ fontSize: 12, fontWeight: 500, color: priorityColors[r.priority] || '#86868b' }}>{r.priority}</span>
      ),
    },
    {
      title: 'Assigned',
      dataIndex: 'assignedTo',
      key: 'assignedTo',
      width: 120,
      render: (v: string | null) => <Text style={{ fontSize: 13, color: '#6e6e73' }}>{v || '-'}</Text>,
    },
    {
      title: 'Created',
      dataIndex: 'createdAt',
      key: 'createdAt',
      width: 110,
      render: (v: string) => <Text style={{ fontSize: 13, color: '#86868b' }}>{dayjs(v).format('MMM D, YYYY')}</Text>,
    },
    {
      title: 'Actions',
      key: 'actions',
      width: 160,
      render: (_, r) => {
        const transitions = getAvailableTransitions(r.currentState);
        const menuItems: MenuProps['items'] = transitions.map((t) => ({
          key: t.triggerName,
          label: `${t.triggerName} → ${t.toState}`,
        }));

        return (
          <Flex gap={4}>
            {transitions.length > 0 && (
              <Dropdown
                menu={{
                  items: menuItems,
                  onClick: ({ key }) => transitionMutation.mutate({ id: r.id, triggerName: key }),
                }}
                trigger={['click']}
              >
                <Button type="text" size="small" icon={<ThunderboltOutlined />} style={{ color: '#0071e3' }} />
              </Dropdown>
            )}
            <Button type="text" size="small" icon={<HistoryOutlined />} onClick={() => setHistoryTaskId(r.id)} />
            <Popconfirm title="Delete?" onConfirm={() => deleteMutation.mutate(r.id)} okText="Delete" okButtonProps={{ danger: true }}>
              <Button type="text" size="small" icon={<DeleteOutlined />} danger />
            </Popconfirm>
          </Flex>
        );
      },
    },
  ];

  return (
    <div>
      <Flex align="center" justify="space-between" style={{ marginBottom: 24 }}>
        <div>
          <h2 style={{ fontSize: 22, fontWeight: 700, color: '#1d1d1f', margin: 0 }}>Tasks (Demo)</h2>
          <Text style={{ fontSize: 13, color: '#86868b' }}>State machine proof-of-concept. Transition tasks through lifecycle states.</Text>
        </div>
        <Button type="primary" icon={<PlusOutlined />} onClick={() => setCreateOpen(true)}>New Task</Button>
      </Flex>

      <div style={{ background: '#ffffff', borderRadius: 12, border: '1px solid #e5e5ea', overflow: 'hidden' }}>
        <Table<DemoTask> rowKey="id" columns={columns} dataSource={data ?? []} loading={isLoading} pagination={{ showSizeChanger: true, style: { padding: '0 16px' } }} />
      </div>

      {/* Create Modal */}
      <Modal title="New Task" open={createOpen} onCancel={() => setCreateOpen(false)} onOk={() => form.validateFields().then((v) => createMutation.mutate(v))} confirmLoading={createMutation.isPending} okText="Create">
        <Form form={form} layout="vertical" style={{ marginTop: 16 }}>
          <Form.Item name="title" label="Title" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="description" label="Description"><Input.TextArea rows={2} /></Form.Item>
          <Form.Item name="assignedTo" label="Assigned To"><Input placeholder="Name (optional)" /></Form.Item>
          <Form.Item name="priority" label="Priority" rules={[{ required: true }]} initialValue="Medium">
            <Select options={[
              { label: 'Low', value: 'Low' },
              { label: 'Medium', value: 'Medium' },
              { label: 'High', value: 'High' },
              { label: 'Critical', value: 'Critical' },
            ]} />
          </Form.Item>
        </Form>
      </Modal>

      {/* History Modal */}
      <Modal title="Transition History" open={!!historyTaskId} onCancel={() => setHistoryTaskId(null)} footer={null} width={500}>
        {historyQuery.isLoading ? (
          <div style={{ padding: 40, textAlign: 'center' }}><Text style={{ color: '#86868b' }}>Loading...</Text></div>
        ) : (historyQuery.data ?? []).length === 0 ? (
          <div style={{ padding: 40, textAlign: 'center' }}><Text style={{ color: '#86868b' }}>No transitions yet</Text></div>
        ) : (
          <Timeline style={{ marginTop: 24 }}
            items={(historyQuery.data ?? []).map((log: TransitionLog) => ({
              color: stateColors[log.toState] || '#86868b',
              children: (
                <div>
                  <Flex align="center" gap={6}>
                    <span style={{ fontWeight: 500, fontSize: 13, color: stateColors[log.fromState] || '#86868b' }}>{log.fromState}</span>
                    <span style={{ color: '#86868b' }}>→</span>
                    <span style={{ fontWeight: 500, fontSize: 13, color: stateColors[log.toState] || '#86868b' }}>{log.toState}</span>
                    <code style={{ fontSize: 11, background: '#f5f5f7', padding: '1px 6px', borderRadius: 4, marginLeft: 4 }}>{log.triggerName}</code>
                  </Flex>
                  {log.comments && <div style={{ fontSize: 12, color: '#6e6e73', marginTop: 2 }}>{log.comments}</div>}
                  <div style={{ fontSize: 11, color: '#86868b', marginTop: 2 }}>{dayjs(log.transitionedAt).format('MMM D, YYYY h:mm A')}</div>
                </div>
              ),
            }))}
          />
        )}
      </Modal>
    </div>
  );
}
