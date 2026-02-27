import { useState } from 'react';
import {
  Typography,
  Button,
  Table,
  Space,
  Modal,
  Form,
  Input,
  Select,
  Checkbox,
  InputNumber,
  message,
  Flex,
  Tag,
} from 'antd';
import {
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  ExclamationCircleOutlined,
  ArrowRightOutlined,
} from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AxiosError } from 'axios';
import { stateMachineApi } from '../api/stateMachineApi';
import type {
  CreateStateRequest,
  UpdateStateRequest,
  CreateTransitionRequest,
  UpdateTransitionRequest,
} from '../api/stateMachineApi';
import type { StateDefinition, StateTransitionDefinition } from '../types';
import PermissionGate from '../components/PermissionGate';
import { useTenantTheme } from '../contexts/ThemeContext';

const { Text } = Typography;

const ENTITY_TYPES = [
  { label: 'Task', value: 'Task' },
  { label: 'Purchase Order', value: 'PurchaseOrder' },
  { label: 'Invoice', value: 'Invoice' },
  { label: 'Workflow', value: 'Workflow' },
];

const PRESET_COLORS = [
  '#0071e3',
  '#28cd41',
  '#ff9500',
  '#ff3b30',
  '#5856d6',
  '#af52de',
  '#ff2d55',
  '#007aff',
  '#34c759',
  '#86868b',
];

interface StateFormValues {
  stateName: string;
  isInitial: boolean;
  isFinal: boolean;
  color?: string;
  sortOrder: number;
}

interface TransitionFormValues {
  fromState: string;
  toState: string;
  triggerName: string;
  requiredPermission?: string;
  description?: string;
}

export default function StateMachinePage() {
  const queryClient = useQueryClient();
  const { theme } = useTenantTheme();
  const primaryColor = theme?.primaryColor || '#0071e3';

  const [selectedEntityType, setSelectedEntityType] = useState('Task');
  const [stateModalOpen, setStateModalOpen] = useState(false);
  const [transitionModalOpen, setTransitionModalOpen] = useState(false);
  const [editingState, setEditingState] = useState<StateDefinition | null>(null);
  const [editingTransition, setEditingTransition] = useState<StateTransitionDefinition | null>(null);
  const [stateForm] = Form.useForm<StateFormValues>();
  const [transitionForm] = Form.useForm<TransitionFormValues>();

  const definitionsQuery = useQuery({
    queryKey: ['statemachine', selectedEntityType],
    queryFn: () => stateMachineApi.getDefinitions(selectedEntityType),
  });

  const createStateMutation = useMutation({
    mutationFn: (data: CreateStateRequest) => stateMachineApi.createState(data),
    onSuccess: () => {
      message.success('State created');
      queryClient.invalidateQueries({ queryKey: ['statemachine', selectedEntityType] });
      closeStateModal();
    },
    onError: (error: AxiosError<{ message?: string; title?: string }>) => {
      message.error(error.response?.data?.message || error.response?.data?.title || 'Failed to create state');
    },
  });

  const updateStateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateStateRequest }) =>
      stateMachineApi.updateState(id, data),
    onSuccess: () => {
      message.success('State updated');
      queryClient.invalidateQueries({ queryKey: ['statemachine', selectedEntityType] });
      closeStateModal();
    },
    onError: (error: AxiosError<{ message?: string; title?: string }>) => {
      message.error(error.response?.data?.message || error.response?.data?.title || 'Failed to update state');
    },
  });

  const deleteStateMutation = useMutation({
    mutationFn: (id: string) => stateMachineApi.deleteState(id),
    onSuccess: () => {
      message.success('State deleted');
      queryClient.invalidateQueries({ queryKey: ['statemachine', selectedEntityType] });
    },
    onError: () => {
      message.error('Failed to delete state');
    },
  });

  const createTransitionMutation = useMutation({
    mutationFn: (data: CreateTransitionRequest) => stateMachineApi.createTransition(data),
    onSuccess: () => {
      message.success('Transition created');
      queryClient.invalidateQueries({ queryKey: ['statemachine', selectedEntityType] });
      closeTransitionModal();
    },
    onError: (error: AxiosError<{ message?: string; title?: string }>) => {
      message.error(error.response?.data?.message || error.response?.data?.title || 'Failed to create transition');
    },
  });

  const updateTransitionMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateTransitionRequest }) =>
      stateMachineApi.updateTransition(id, data),
    onSuccess: () => {
      message.success('Transition updated');
      queryClient.invalidateQueries({ queryKey: ['statemachine', selectedEntityType] });
      closeTransitionModal();
    },
    onError: (error: AxiosError<{ message?: string; title?: string }>) => {
      message.error(error.response?.data?.message || error.response?.data?.title || 'Failed to update transition');
    },
  });

  const deleteTransitionMutation = useMutation({
    mutationFn: (id: string) => stateMachineApi.deleteTransition(id),
    onSuccess: () => {
      message.success('Transition deleted');
      queryClient.invalidateQueries({ queryKey: ['statemachine', selectedEntityType] });
    },
    onError: () => {
      message.error('Failed to delete transition');
    },
  });

  const openCreateStateModal = () => {
    setEditingState(null);
    stateForm.resetFields();
    stateForm.setFieldsValue({
      isInitial: false,
      isFinal: false,
      sortOrder: (definitionsQuery.data?.states.length || 0) + 1,
      color: PRESET_COLORS[0],
    });
    setStateModalOpen(true);
  };

  const openEditStateModal = (state: StateDefinition) => {
    setEditingState(state);
    stateForm.setFieldsValue({
      stateName: state.stateName,
      isInitial: state.isInitial,
      isFinal: state.isFinal,
      color: state.color || undefined,
      sortOrder: state.sortOrder,
    });
    setStateModalOpen(true);
  };

  const closeStateModal = () => {
    setStateModalOpen(false);
    setEditingState(null);
    stateForm.resetFields();
  };

  const handleStateModalOk = async () => {
    try {
      const values = await stateForm.validateFields();
      if (editingState) {
        updateStateMutation.mutate({
          id: editingState.id,
          data: {
            stateName: values.stateName,
            isInitial: values.isInitial,
            isFinal: values.isFinal,
            color: values.color,
            sortOrder: values.sortOrder,
          },
        });
      } else {
        createStateMutation.mutate({
          entityType: selectedEntityType,
          stateName: values.stateName,
          isInitial: values.isInitial,
          isFinal: values.isFinal,
          color: values.color,
          sortOrder: values.sortOrder,
        });
      }
    } catch {
      // validation
    }
  };

  const handleDeleteState = (state: StateDefinition) => {
    Modal.confirm({
      title: 'Delete state',
      icon: <ExclamationCircleOutlined />,
      content: `This will permanently delete the state "${state.stateName}". This action cannot be undone.`,
      okText: 'Delete',
      okType: 'danger',
      onOk: () => deleteStateMutation.mutateAsync(state.id),
    });
  };

  const openCreateTransitionModal = () => {
    setEditingTransition(null);
    transitionForm.resetFields();
    setTransitionModalOpen(true);
  };

  const openEditTransitionModal = (transition: StateTransitionDefinition) => {
    setEditingTransition(transition);
    transitionForm.setFieldsValue({
      fromState: transition.fromState,
      toState: transition.toState,
      triggerName: transition.triggerName,
      requiredPermission: transition.requiredPermission || undefined,
      description: transition.description || undefined,
    });
    setTransitionModalOpen(true);
  };

  const closeTransitionModal = () => {
    setTransitionModalOpen(false);
    setEditingTransition(null);
    transitionForm.resetFields();
  };

  const handleTransitionModalOk = async () => {
    try {
      const values = await transitionForm.validateFields();
      if (editingTransition) {
        updateTransitionMutation.mutate({
          id: editingTransition.id,
          data: {
            fromState: values.fromState,
            toState: values.toState,
            triggerName: values.triggerName,
            requiredPermission: values.requiredPermission,
            description: values.description,
          },
        });
      } else {
        createTransitionMutation.mutate({
          entityType: selectedEntityType,
          fromState: values.fromState,
          toState: values.toState,
          triggerName: values.triggerName,
          requiredPermission: values.requiredPermission,
          description: values.description,
        });
      }
    } catch {
      // validation
    }
  };

  const handleDeleteTransition = (transition: StateTransitionDefinition) => {
    Modal.confirm({
      title: 'Delete transition',
      icon: <ExclamationCircleOutlined />,
      content: `This will permanently delete the transition "${transition.fromState} → ${transition.toState}". This action cannot be undone.`,
      okText: 'Delete',
      okType: 'danger',
      onOk: () => deleteTransitionMutation.mutateAsync(transition.id),
    });
  };

  const stateColumns: ColumnsType<StateDefinition> = [
    {
      title: 'State',
      key: 'stateName',
      render: (_, record) => (
        <Flex align="center" gap={8}>
          <span
            style={{
              width: 10,
              height: 10,
              borderRadius: '50%',
              background: record.color || '#86868b',
              flexShrink: 0,
            }}
          />
          <span style={{ fontWeight: 500, fontSize: 13, color: '#1d1d1f' }}>
            {record.stateName}
          </span>
          {record.isInitial && (
            <Tag
              color="blue"
              style={{ fontSize: 11, borderRadius: 10, lineHeight: '18px', margin: 0 }}
            >
              Initial
            </Tag>
          )}
          {record.isFinal && (
            <Tag
              color="default"
              style={{ fontSize: 11, borderRadius: 10, lineHeight: '18px', margin: 0 }}
            >
              Final
            </Tag>
          )}
        </Flex>
      ),
      sorter: (a, b) => a.sortOrder - b.sortOrder,
    },
    {
      title: 'Order',
      dataIndex: 'sortOrder',
      key: 'sortOrder',
      width: 80,
      render: (value: number) => (
        <Text style={{ fontSize: 13, color: '#86868b' }}>{value}</Text>
      ),
      sorter: (a, b) => a.sortOrder - b.sortOrder,
    },
    {
      title: '',
      key: 'actions',
      width: 100,
      align: 'right',
      render: (_, record) => (
        <Space size={0}>
          <PermissionGate permission="StateMachine.Manage">
            <Button
              type="text"
              size="small"
              icon={<EditOutlined />}
              onClick={() => openEditStateModal(record)}
              style={{ color: '#86868b' }}
            />
          </PermissionGate>
          <PermissionGate permission="StateMachine.Manage">
            <Button
              type="text"
              size="small"
              danger
              icon={<DeleteOutlined />}
              onClick={() => handleDeleteState(record)}
            />
          </PermissionGate>
        </Space>
      ),
    },
  ];

  const transitionColumns: ColumnsType<StateTransitionDefinition> = [
    {
      title: 'Transition',
      key: 'transition',
      render: (_, record) => (
        <Flex align="center" gap={8}>
          <code
            style={{
              fontSize: 12,
              background: '#f5f5f7',
              padding: '2px 8px',
              borderRadius: 4,
            }}
          >
            {record.fromState}
          </code>
          <ArrowRightOutlined style={{ fontSize: 11, color: '#86868b' }} />
          <code
            style={{
              fontSize: 12,
              background: '#f5f5f7',
              padding: '2px 8px',
              borderRadius: 4,
            }}
          >
            {record.toState}
          </code>
        </Flex>
      ),
    },
    {
      title: 'Trigger',
      dataIndex: 'triggerName',
      key: 'triggerName',
      width: 140,
      render: (value: string) => (
        <span style={{ fontWeight: 500, fontSize: 13, color: primaryColor }}>{value}</span>
      ),
    },
    {
      title: 'Description',
      dataIndex: 'description',
      key: 'description',
      render: (value: string | null) => (
        <Text style={{ fontSize: 13, color: '#6e6e73' }}>{value || '-'}</Text>
      ),
    },
    {
      title: 'Permission',
      dataIndex: 'requiredPermission',
      key: 'requiredPermission',
      width: 160,
      render: (value: string | null) =>
        value ? (
          <code style={{ fontSize: 11, background: '#f5f5f7', padding: '2px 6px', borderRadius: 4 }}>
            {value}
          </code>
        ) : (
          <Text style={{ fontSize: 12, color: '#86868b' }}>None</Text>
        ),
    },
    {
      title: '',
      key: 'actions',
      width: 100,
      align: 'right',
      render: (_, record) => (
        <Space size={0}>
          <PermissionGate permission="StateMachine.Manage">
            <Button
              type="text"
              size="small"
              icon={<EditOutlined />}
              onClick={() => openEditTransitionModal(record)}
              style={{ color: '#86868b' }}
            />
          </PermissionGate>
          <PermissionGate permission="StateMachine.Manage">
            <Button
              type="text"
              size="small"
              danger
              icon={<DeleteOutlined />}
              onClick={() => handleDeleteTransition(record)}
            />
          </PermissionGate>
        </Space>
      ),
    },
  ];

  const stateOptions = (definitionsQuery.data?.states || []).map((s) => ({
    label: s.stateName,
    value: s.stateName,
  }));

  const isStateMutating = createStateMutation.isPending || updateStateMutation.isPending;
  const isTransitionMutating = createTransitionMutation.isPending || updateTransitionMutation.isPending;

  return (
    <div>
      <Flex align="center" justify="space-between" style={{ marginBottom: 24 }}>
        <div>
          <h2 style={{ fontSize: 22, fontWeight: 700, color: '#1d1d1f', margin: 0 }}>
            State Machine
          </h2>
          <Text style={{ fontSize: 13, color: '#86868b' }}>
            Manage entity lifecycle states and transitions
          </Text>
        </div>
        <Select
          value={selectedEntityType}
          onChange={setSelectedEntityType}
          options={ENTITY_TYPES}
          style={{ width: 180 }}
        />
      </Flex>

      {/* States */}
      <div style={{ marginBottom: 24 }}>
        <Flex align="center" justify="space-between" style={{ marginBottom: 12 }}>
          <Text
            style={{
              fontSize: 14,
              fontWeight: 600,
              color: '#1d1d1f',
            }}
          >
            States
          </Text>
          <PermissionGate permission="StateMachine.Manage">
            <Button
              type="primary"
              size="small"
              icon={<PlusOutlined />}
              onClick={openCreateStateModal}
            >
              New state
            </Button>
          </PermissionGate>
        </Flex>
        <div
          style={{
            background: '#ffffff',
            borderRadius: 12,
            border: '1px solid #e5e5ea',
            overflow: 'hidden',
          }}
        >
          <Table<StateDefinition>
            rowKey="id"
            columns={stateColumns}
            dataSource={definitionsQuery.data?.states ?? []}
            loading={definitionsQuery.isLoading}
            pagination={false}
            size="small"
          />
        </div>
      </div>

      {/* Transitions */}
      <div>
        <Flex align="center" justify="space-between" style={{ marginBottom: 12 }}>
          <Text
            style={{
              fontSize: 14,
              fontWeight: 600,
              color: '#1d1d1f',
            }}
          >
            Transitions
          </Text>
          <PermissionGate permission="StateMachine.Manage">
            <Button
              type="primary"
              size="small"
              icon={<PlusOutlined />}
              onClick={openCreateTransitionModal}
            >
              New transition
            </Button>
          </PermissionGate>
        </Flex>
        <div
          style={{
            background: '#ffffff',
            borderRadius: 12,
            border: '1px solid #e5e5ea',
            overflow: 'hidden',
          }}
        >
          <Table<StateTransitionDefinition>
            rowKey="id"
            columns={transitionColumns}
            dataSource={definitionsQuery.data?.transitions ?? []}
            loading={definitionsQuery.isLoading}
            pagination={false}
            size="small"
          />
        </div>
      </div>

      {/* State Modal */}
      <Modal
        title={editingState ? 'Edit state' : 'New state'}
        open={stateModalOpen}
        onOk={handleStateModalOk}
        onCancel={closeStateModal}
        confirmLoading={isStateMutating}
        destroyOnClose
        okText={editingState ? 'Save changes' : 'Create'}
      >
        <Form<StateFormValues> form={stateForm} layout="vertical" requiredMark={false}>
          <Form.Item
            name="stateName"
            label={<Text style={{ fontWeight: 500, fontSize: 13 }}>Name</Text>}
            rules={[{ required: true, message: 'Required' }]}
          >
            <Input placeholder="e.g. Open, In Progress, Completed" />
          </Form.Item>

          <Form.Item
            name="color"
            label={<Text style={{ fontWeight: 500, fontSize: 13 }}>Color</Text>}
          >
            <Select
              placeholder="Select a color"
              options={PRESET_COLORS.map((c) => ({
                label: (
                  <Flex align="center" gap={8}>
                    <span
                      style={{
                        width: 16,
                        height: 16,
                        borderRadius: '50%',
                        background: c,
                        border: '1px solid #e5e5ea',
                      }}
                    />
                    <span>{c}</span>
                  </Flex>
                ),
                value: c,
              }))}
            />
          </Form.Item>

          <Form.Item
            name="sortOrder"
            label={<Text style={{ fontWeight: 500, fontSize: 13 }}>Sort order</Text>}
            rules={[{ required: true, message: 'Required' }]}
          >
            <InputNumber min={0} style={{ width: '100%' }} />
          </Form.Item>

          <Form.Item name="isInitial" valuePropName="checked">
            <Checkbox>Initial state</Checkbox>
          </Form.Item>

          <Form.Item name="isFinal" valuePropName="checked">
            <Checkbox>Final state</Checkbox>
          </Form.Item>
        </Form>
      </Modal>

      {/* Transition Modal */}
      <Modal
        title={editingTransition ? 'Edit transition' : 'New transition'}
        open={transitionModalOpen}
        onOk={handleTransitionModalOk}
        onCancel={closeTransitionModal}
        confirmLoading={isTransitionMutating}
        destroyOnClose
        okText={editingTransition ? 'Save changes' : 'Create'}
      >
        <Form<TransitionFormValues> form={transitionForm} layout="vertical" requiredMark={false}>
          <Form.Item
            name="fromState"
            label={<Text style={{ fontWeight: 500, fontSize: 13 }}>From state</Text>}
            rules={[{ required: true, message: 'Required' }]}
          >
            <Select
              placeholder="Select state"
              options={stateOptions}
              showSearch
              filterOption={(input, option) =>
                (option?.label ?? '').toLowerCase().includes(input.toLowerCase())
              }
            />
          </Form.Item>

          <Form.Item
            name="toState"
            label={<Text style={{ fontWeight: 500, fontSize: 13 }}>To state</Text>}
            rules={[{ required: true, message: 'Required' }]}
          >
            <Select
              placeholder="Select state"
              options={stateOptions}
              showSearch
              filterOption={(input, option) =>
                (option?.label ?? '').toLowerCase().includes(input.toLowerCase())
              }
            />
          </Form.Item>

          <Form.Item
            name="triggerName"
            label={<Text style={{ fontWeight: 500, fontSize: 13 }}>Trigger name</Text>}
            rules={[{ required: true, message: 'Required' }]}
          >
            <Input placeholder="e.g. Start, Complete, Reject" />
          </Form.Item>

          <Form.Item
            name="description"
            label={<Text style={{ fontWeight: 500, fontSize: 13 }}>Description</Text>}
          >
            <Input.TextArea rows={2} placeholder="Optional description" />
          </Form.Item>

          <Form.Item
            name="requiredPermission"
            label={<Text style={{ fontWeight: 500, fontSize: 13 }}>Required permission</Text>}
          >
            <Input placeholder="e.g. Task.Approve (optional)" />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}
