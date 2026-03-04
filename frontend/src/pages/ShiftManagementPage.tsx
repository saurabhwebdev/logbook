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
  InputNumber,
  message,
  Flex,
  Tag,
  Tabs,
  DatePicker,
  TimePicker,
  Drawer,
  Descriptions,
  ColorPicker,
} from 'antd';
import {
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  ExclamationCircleOutlined,
  EyeOutlined,
  CheckCircleOutlined,
  SwapOutlined,
} from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AxiosError } from 'axios';
import dayjs from 'dayjs';
import { shiftsApi } from '../api/shiftsApi';
import type {
  CreateShiftDefinitionRequest,
  UpdateShiftDefinitionRequest,
  CreateShiftInstanceRequest,
  UpdateShiftInstanceRequest,
  CreateShiftHandoverRequest,
  UpdateShiftHandoverRequest,
} from '../api/shiftsApi';
import { mineSitesApi } from '../api/mineSitesApi';
import type { ShiftDefinition, ShiftInstance, ShiftHandover, MineSite } from '../types';
import PermissionGate from '../components/PermissionGate';

const { Text, Title } = Typography;
const { TextArea } = Input;
const { RangePicker } = DatePicker;

const SHIFT_STATUSES = ['Scheduled', 'InProgress', 'Completed', 'Cancelled'];
const HANDOVER_STATUSES = ['Draft', 'Submitted', 'Acknowledged'];

const statusColors: Record<string, string> = {
  Scheduled: 'blue',
  InProgress: 'orange',
  Completed: 'green',
  Cancelled: 'red',
  Draft: 'default',
  Submitted: 'processing',
  Acknowledged: 'success',
};

// ===== SHIFT DEFINITIONS TAB =====
function ShiftDefinitionsTab({ mineSiteId }: { mineSiteId: string }) {
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<ShiftDefinition | null>(null);
  const [form] = Form.useForm();
  const queryClient = useQueryClient();

  const { data: definitions = [], isLoading } = useQuery({
    queryKey: ['shiftDefinitions', mineSiteId],
    queryFn: () => shiftsApi.getShiftDefinitions(mineSiteId),
    enabled: !!mineSiteId,
  });

  const createMutation = useMutation({
    mutationFn: (data: CreateShiftDefinitionRequest) => shiftsApi.createShiftDefinition(data),
    onSuccess: () => {
      message.success('Shift definition created');
      queryClient.invalidateQueries({ queryKey: ['shiftDefinitions', mineSiteId] });
      setModalOpen(false);
      form.resetFields();
    },
    onError: (err: AxiosError<{ detail?: string }>) => {
      message.error(err.response?.data?.detail || 'Failed to create shift definition');
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateShiftDefinitionRequest }) =>
      shiftsApi.updateShiftDefinition(id, data),
    onSuccess: () => {
      message.success('Shift definition updated');
      queryClient.invalidateQueries({ queryKey: ['shiftDefinitions', mineSiteId] });
      setModalOpen(false);
      setEditing(null);
      form.resetFields();
    },
    onError: (err: AxiosError<{ detail?: string }>) => {
      message.error(err.response?.data?.detail || 'Failed to update shift definition');
    },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => shiftsApi.deleteShiftDefinition(id),
    onSuccess: () => {
      message.success('Shift definition deleted');
      queryClient.invalidateQueries({ queryKey: ['shiftDefinitions', mineSiteId] });
    },
    onError: (err: AxiosError<{ detail?: string }>) => {
      message.error(err.response?.data?.detail || 'Failed to delete shift definition');
    },
  });

  const openCreate = () => {
    setEditing(null);
    form.resetFields();
    form.setFieldsValue({ isActive: true, shiftOrder: 1 });
    setModalOpen(true);
  };

  const openEdit = (record: ShiftDefinition) => {
    setEditing(record);
    form.setFieldsValue({
      name: record.name,
      code: record.code,
      startTime: record.startTime ? dayjs(record.startTime, 'HH:mm') : null,
      endTime: record.endTime ? dayjs(record.endTime, 'HH:mm') : null,
      shiftOrder: record.shiftOrder,
      color: record.color,
      isActive: record.isActive,
    });
    setModalOpen(true);
  };

  const handleSubmit = async () => {
    const values = await form.validateFields();
    const startTime = values.startTime ? dayjs(values.startTime).format('HH:mm') : '';
    const endTime = values.endTime ? dayjs(values.endTime).format('HH:mm') : '';
    const color = typeof values.color === 'string' ? values.color : values.color?.toHexString?.() || null;

    if (editing) {
      updateMutation.mutate({
        id: editing.id,
        data: { id: editing.id, name: values.name, code: values.code, startTime, endTime, shiftOrder: values.shiftOrder, color, isActive: values.isActive },
      });
    } else {
      createMutation.mutate({ mineSiteId, name: values.name, code: values.code, startTime, endTime, shiftOrder: values.shiftOrder, color, isActive: values.isActive });
    }
  };

  const handleDelete = (record: ShiftDefinition) => {
    Modal.confirm({
      title: 'Delete Shift Definition?',
      icon: <ExclamationCircleOutlined />,
      content: `Are you sure you want to delete "${record.name}"? This cannot be undone.`,
      okText: 'Delete',
      okType: 'danger',
      onOk: () => deleteMutation.mutateAsync(record.id),
    });
  };

  const columns: ColumnsType<ShiftDefinition> = [
    {
      title: 'Name',
      dataIndex: 'name',
      key: 'name',
      render: (name: string, record) => (
        <Flex align="center" gap={8}>
          {record.color && <div style={{ width: 12, height: 12, borderRadius: 3, background: record.color }} />}
          <Text strong>{name}</Text>
          {record.code && <Text type="secondary">({record.code})</Text>}
        </Flex>
      ),
    },
    { title: 'Start', dataIndex: 'startTime', key: 'startTime', width: 100 },
    { title: 'End', dataIndex: 'endTime', key: 'endTime', width: 100 },
    { title: 'Order', dataIndex: 'shiftOrder', key: 'shiftOrder', width: 80, align: 'center' },
    {
      title: 'Status',
      dataIndex: 'isActive',
      key: 'isActive',
      width: 100,
      render: (isActive: boolean) => <Tag color={isActive ? 'green' : 'default'}>{isActive ? 'Active' : 'Inactive'}</Tag>,
    },
    { title: 'Instances', dataIndex: 'instanceCount', key: 'instanceCount', width: 100, align: 'center' },
    {
      title: 'Actions',
      key: 'actions',
      width: 120,
      render: (_, record) => (
        <Space size="small">
          <PermissionGate permission="ShiftDefinition.Update">
            <Button type="text" size="small" icon={<EditOutlined />} onClick={() => openEdit(record)} />
          </PermissionGate>
          <PermissionGate permission="ShiftDefinition.Delete">
            <Button type="text" size="small" danger icon={<DeleteOutlined />} onClick={() => handleDelete(record)} />
          </PermissionGate>
        </Space>
      ),
    },
  ];

  return (
    <>
      <Flex justify="space-between" align="center" style={{ marginBottom: 16 }}>
        <Text type="secondary">{definitions.length} shift definition(s)</Text>
        <PermissionGate permission="ShiftDefinition.Create">
          <Button type="primary" icon={<PlusOutlined />} onClick={openCreate}>Add Definition</Button>
        </PermissionGate>
      </Flex>

      <Table columns={columns} dataSource={definitions} rowKey="id" loading={isLoading} size="middle" pagination={false} />

      <Modal
        title={editing ? 'Edit Shift Definition' : 'New Shift Definition'}
        open={modalOpen}
        onOk={handleSubmit}
        onCancel={() => { setModalOpen(false); setEditing(null); form.resetFields(); }}
        confirmLoading={createMutation.isPending || updateMutation.isPending}
        width={500}
      >
        <Form form={form} layout="vertical" style={{ marginTop: 16 }}>
          <Form.Item name="name" label="Name" rules={[{ required: true, message: 'Name is required' }]}>
            <Input placeholder="e.g., Day Shift" />
          </Form.Item>
          <Form.Item name="code" label="Code">
            <Input placeholder="e.g., DS" maxLength={20} />
          </Form.Item>
          <Flex gap={16}>
            <Form.Item name="startTime" label="Start Time" rules={[{ required: true, message: 'Required' }]} style={{ flex: 1 }}>
              <TimePicker format="HH:mm" style={{ width: '100%' }} />
            </Form.Item>
            <Form.Item name="endTime" label="End Time" rules={[{ required: true, message: 'Required' }]} style={{ flex: 1 }}>
              <TimePicker format="HH:mm" style={{ width: '100%' }} />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="shiftOrder" label="Shift Order" style={{ flex: 1 }}>
              <InputNumber min={1} max={10} style={{ width: '100%' }} />
            </Form.Item>
            <Form.Item name="color" label="Color" style={{ flex: 1 }}>
              <ColorPicker />
            </Form.Item>
          </Flex>
          <Form.Item name="isActive" label="Active" valuePropName="checked" initialValue={true}>
            <Select options={[{ value: true, label: 'Active' }, { value: false, label: 'Inactive' }]} />
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
}

// ===== SHIFT INSTANCES TAB =====
function ShiftInstancesTab({ mineSiteId }: { mineSiteId: string }) {
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<ShiftInstance | null>(null);
  const [detailDrawer, setDetailDrawer] = useState<ShiftInstance | null>(null);
  const [dateRange, setDateRange] = useState<[string?, string?]>([]);
  const [form] = Form.useForm();
  const queryClient = useQueryClient();

  const { data: instances = [], isLoading } = useQuery({
    queryKey: ['shiftInstances', mineSiteId, dateRange[0], dateRange[1]],
    queryFn: () => shiftsApi.getShiftInstances(mineSiteId, dateRange[0], dateRange[1]),
    enabled: !!mineSiteId,
  });

  const { data: definitions = [] } = useQuery({
    queryKey: ['shiftDefinitions', mineSiteId],
    queryFn: () => shiftsApi.getShiftDefinitions(mineSiteId),
    enabled: !!mineSiteId,
  });

  const createMutation = useMutation({
    mutationFn: (data: CreateShiftInstanceRequest) => shiftsApi.createShiftInstance(data),
    onSuccess: () => {
      message.success('Shift instance created');
      queryClient.invalidateQueries({ queryKey: ['shiftInstances', mineSiteId] });
      setModalOpen(false);
      form.resetFields();
    },
    onError: (err: AxiosError<{ detail?: string }>) => {
      message.error(err.response?.data?.detail || 'Failed to create shift instance');
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateShiftInstanceRequest }) =>
      shiftsApi.updateShiftInstance(id, data),
    onSuccess: () => {
      message.success('Shift instance updated');
      queryClient.invalidateQueries({ queryKey: ['shiftInstances', mineSiteId] });
      setModalOpen(false);
      setEditing(null);
      form.resetFields();
    },
    onError: (err: AxiosError<{ detail?: string }>) => {
      message.error(err.response?.data?.detail || 'Failed to update shift instance');
    },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => shiftsApi.deleteShiftInstance(id),
    onSuccess: () => {
      message.success('Shift instance deleted');
      queryClient.invalidateQueries({ queryKey: ['shiftInstances', mineSiteId] });
    },
    onError: (err: AxiosError<{ detail?: string }>) => {
      message.error(err.response?.data?.detail || 'Failed to delete shift instance');
    },
  });

  const openCreate = () => {
    setEditing(null);
    form.resetFields();
    form.setFieldsValue({ status: 'Scheduled', date: dayjs() });
    setModalOpen(true);
  };

  const openEdit = (record: ShiftInstance) => {
    setEditing(record);
    form.setFieldsValue({
      shiftDefinitionId: record.shiftDefinitionId,
      date: dayjs(record.date),
      supervisorName: record.supervisorName,
      status: record.status,
      actualStartTime: record.actualStartTime ? dayjs(record.actualStartTime) : null,
      actualEndTime: record.actualEndTime ? dayjs(record.actualEndTime) : null,
      personnelCount: record.personnelCount,
      weatherConditions: record.weatherConditions,
      notes: record.notes,
    });
    setModalOpen(true);
  };

  const handleSubmit = async () => {
    const values = await form.validateFields();
    const date = dayjs(values.date).format('YYYY-MM-DD');
    const actualStartTime = values.actualStartTime ? dayjs(values.actualStartTime).toISOString() : undefined;
    const actualEndTime = values.actualEndTime ? dayjs(values.actualEndTime).toISOString() : undefined;

    if (editing) {
      updateMutation.mutate({
        id: editing.id,
        data: {
          id: editing.id,
          shiftDefinitionId: values.shiftDefinitionId,
          mineSiteId,
          date,
          supervisorName: values.supervisorName,
          status: values.status,
          actualStartTime,
          actualEndTime,
          personnelCount: values.personnelCount,
          weatherConditions: values.weatherConditions,
          notes: values.notes,
        },
      });
    } else {
      createMutation.mutate({
        shiftDefinitionId: values.shiftDefinitionId,
        mineSiteId,
        date,
        supervisorName: values.supervisorName,
        status: values.status,
        actualStartTime,
        actualEndTime,
        personnelCount: values.personnelCount,
        weatherConditions: values.weatherConditions,
        notes: values.notes,
      });
    }
  };

  const handleDelete = (record: ShiftInstance) => {
    Modal.confirm({
      title: 'Delete Shift Instance?',
      icon: <ExclamationCircleOutlined />,
      content: 'This cannot be undone.',
      okText: 'Delete',
      okType: 'danger',
      onOk: () => deleteMutation.mutateAsync(record.id),
    });
  };

  const columns: ColumnsType<ShiftInstance> = [
    {
      title: 'Date',
      dataIndex: 'date',
      key: 'date',
      width: 120,
      render: (d: string) => dayjs(d).format('DD MMM YYYY'),
      sorter: (a, b) => dayjs(a.date).unix() - dayjs(b.date).unix(),
      defaultSortOrder: 'descend',
    },
    { title: 'Shift', dataIndex: 'shiftDefinitionName', key: 'shiftDefinitionName' },
    { title: 'Supervisor', dataIndex: 'supervisorName', key: 'supervisorName', render: (v: string | null) => v || '-' },
    {
      title: 'Status',
      dataIndex: 'status',
      key: 'status',
      width: 120,
      render: (s: string) => <Tag color={statusColors[s] || 'default'}>{s}</Tag>,
    },
    { title: 'Personnel', dataIndex: 'personnelCount', key: 'personnelCount', width: 100, align: 'center', render: (v: number | null) => v ?? '-' },
    { title: 'Handovers', dataIndex: 'handoverCount', key: 'handoverCount', width: 100, align: 'center' },
    {
      title: 'Actions',
      key: 'actions',
      width: 140,
      render: (_, record) => (
        <Space size="small">
          <Button type="text" size="small" icon={<EyeOutlined />} onClick={() => setDetailDrawer(record)} />
          <PermissionGate permission="ShiftInstance.Update">
            <Button type="text" size="small" icon={<EditOutlined />} onClick={() => openEdit(record)} />
          </PermissionGate>
          <PermissionGate permission="ShiftInstance.Delete">
            <Button type="text" size="small" danger icon={<DeleteOutlined />} onClick={() => handleDelete(record)} />
          </PermissionGate>
        </Space>
      ),
    },
  ];

  return (
    <>
      <Flex justify="space-between" align="center" style={{ marginBottom: 16 }} wrap="wrap" gap={12}>
        <Flex align="center" gap={12}>
          <Text type="secondary">{instances.length} shift(s)</Text>
          <RangePicker
            size="small"
            onChange={(dates) => {
              if (dates && dates[0] && dates[1]) {
                setDateRange([dates[0].format('YYYY-MM-DD'), dates[1].format('YYYY-MM-DD')]);
              } else {
                setDateRange([]);
              }
            }}
          />
        </Flex>
        <PermissionGate permission="ShiftInstance.Create">
          <Button type="primary" icon={<PlusOutlined />} onClick={openCreate}>Add Shift</Button>
        </PermissionGate>
      </Flex>

      <Table columns={columns} dataSource={instances} rowKey="id" loading={isLoading} size="middle" pagination={{ pageSize: 20 }} />

      <Modal
        title={editing ? 'Edit Shift Instance' : 'New Shift Instance'}
        open={modalOpen}
        onOk={handleSubmit}
        onCancel={() => { setModalOpen(false); setEditing(null); form.resetFields(); }}
        confirmLoading={createMutation.isPending || updateMutation.isPending}
        width={600}
      >
        <Form form={form} layout="vertical" style={{ marginTop: 16 }}>
          <Flex gap={16}>
            <Form.Item name="shiftDefinitionId" label="Shift Definition" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select
                placeholder="Select shift"
                options={definitions.filter(d => d.isActive).map(d => ({ value: d.id, label: `${d.name} (${d.startTime}-${d.endTime})` }))}
              />
            </Form.Item>
            <Form.Item name="date" label="Date" rules={[{ required: true }]} style={{ flex: 1 }}>
              <DatePicker style={{ width: '100%' }} />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="supervisorName" label="Supervisor" style={{ flex: 1 }}>
              <Input placeholder="Supervisor name" />
            </Form.Item>
            <Form.Item name="status" label="Status" style={{ flex: 1 }}>
              <Select options={SHIFT_STATUSES.map(s => ({ value: s, label: s }))} />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="actualStartTime" label="Actual Start" style={{ flex: 1 }}>
              <DatePicker showTime style={{ width: '100%' }} />
            </Form.Item>
            <Form.Item name="actualEndTime" label="Actual End" style={{ flex: 1 }}>
              <DatePicker showTime style={{ width: '100%' }} />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="personnelCount" label="Personnel Count" style={{ flex: 1 }}>
              <InputNumber min={0} style={{ width: '100%' }} />
            </Form.Item>
            <Form.Item name="weatherConditions" label="Weather" style={{ flex: 1 }}>
              <Input placeholder="Clear, Rain, etc." />
            </Form.Item>
          </Flex>
          <Form.Item name="notes" label="Notes">
            <TextArea rows={3} placeholder="Shift notes..." />
          </Form.Item>
        </Form>
      </Modal>

      <Drawer
        title="Shift Details"
        open={!!detailDrawer}
        onClose={() => setDetailDrawer(null)}
        width={500}
      >
        {detailDrawer && (
          <Descriptions column={1} bordered size="small">
            <Descriptions.Item label="Shift">{detailDrawer.shiftDefinitionName}</Descriptions.Item>
            <Descriptions.Item label="Date">{dayjs(detailDrawer.date).format('DD MMM YYYY')}</Descriptions.Item>
            <Descriptions.Item label="Status"><Tag color={statusColors[detailDrawer.status]}>{detailDrawer.status}</Tag></Descriptions.Item>
            <Descriptions.Item label="Supervisor">{detailDrawer.supervisorName || '-'}</Descriptions.Item>
            <Descriptions.Item label="Personnel">{detailDrawer.personnelCount ?? '-'}</Descriptions.Item>
            <Descriptions.Item label="Actual Start">{detailDrawer.actualStartTime ? dayjs(detailDrawer.actualStartTime).format('DD MMM HH:mm') : '-'}</Descriptions.Item>
            <Descriptions.Item label="Actual End">{detailDrawer.actualEndTime ? dayjs(detailDrawer.actualEndTime).format('DD MMM HH:mm') : '-'}</Descriptions.Item>
            <Descriptions.Item label="Weather">{detailDrawer.weatherConditions || '-'}</Descriptions.Item>
            <Descriptions.Item label="Notes">{detailDrawer.notes || '-'}</Descriptions.Item>
            <Descriptions.Item label="Mine Site">{detailDrawer.mineSiteName}</Descriptions.Item>
            <Descriptions.Item label="Created">{dayjs(detailDrawer.createdAt).format('DD MMM YYYY HH:mm')}</Descriptions.Item>
          </Descriptions>
        )}
      </Drawer>
    </>
  );
}

// ===== SHIFT HANDOVERS TAB =====
function ShiftHandoversTab({ mineSiteId }: { mineSiteId: string }) {
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<ShiftHandover | null>(null);
  const [detailDrawer, setDetailDrawer] = useState<ShiftHandover | null>(null);
  const [dateRange, setDateRange] = useState<[string?, string?]>([]);
  const [form] = Form.useForm();
  const queryClient = useQueryClient();

  const { data: handovers = [], isLoading } = useQuery({
    queryKey: ['shiftHandovers', mineSiteId, dateRange[0], dateRange[1]],
    queryFn: () => shiftsApi.getShiftHandovers(mineSiteId, dateRange[0], dateRange[1]),
    enabled: !!mineSiteId,
  });

  const { data: instances = [] } = useQuery({
    queryKey: ['shiftInstances', mineSiteId],
    queryFn: () => shiftsApi.getShiftInstances(mineSiteId),
    enabled: !!mineSiteId,
  });

  const createMutation = useMutation({
    mutationFn: (data: CreateShiftHandoverRequest) => shiftsApi.createShiftHandover(data),
    onSuccess: () => {
      message.success('Handover created');
      queryClient.invalidateQueries({ queryKey: ['shiftHandovers', mineSiteId] });
      setModalOpen(false);
      form.resetFields();
    },
    onError: (err: AxiosError<{ detail?: string }>) => {
      message.error(err.response?.data?.detail || 'Failed to create handover');
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateShiftHandoverRequest }) =>
      shiftsApi.updateShiftHandover(id, data),
    onSuccess: () => {
      message.success('Handover updated');
      queryClient.invalidateQueries({ queryKey: ['shiftHandovers', mineSiteId] });
      setModalOpen(false);
      setEditing(null);
      form.resetFields();
    },
    onError: (err: AxiosError<{ detail?: string }>) => {
      message.error(err.response?.data?.detail || 'Failed to update handover');
    },
  });

  const acknowledgeMutation = useMutation({
    mutationFn: (id: string) => shiftsApi.acknowledgeShiftHandover(id),
    onSuccess: () => {
      message.success('Handover acknowledged');
      queryClient.invalidateQueries({ queryKey: ['shiftHandovers', mineSiteId] });
      setDetailDrawer(null);
    },
    onError: (err: AxiosError<{ detail?: string }>) => {
      message.error(err.response?.data?.detail || 'Failed to acknowledge');
    },
  });

  const openCreate = () => {
    setEditing(null);
    form.resetFields();
    form.setFieldsValue({ handoverDateTime: dayjs() });
    setModalOpen(true);
  };

  const openEdit = (record: ShiftHandover) => {
    setEditing(record);
    form.setFieldsValue({
      outgoingShiftInstanceId: record.outgoingShiftInstanceId,
      incomingShiftInstanceId: record.incomingShiftInstanceId,
      handoverDateTime: dayjs(record.handoverDateTime),
      safetyIssues: record.safetyIssues,
      ongoingOperations: record.ongoingOperations,
      pendingTasks: record.pendingTasks,
      equipmentStatus: record.equipmentStatus,
      environmentalConditions: record.environmentalConditions,
      generalRemarks: record.generalRemarks,
      handedOverBy: record.handedOverBy,
      receivedBy: record.receivedBy,
      status: record.status,
    });
    setModalOpen(true);
  };

  const handleSubmit = async () => {
    const values = await form.validateFields();
    const handoverDateTime = dayjs(values.handoverDateTime).toISOString();

    if (editing) {
      updateMutation.mutate({
        id: editing.id,
        data: {
          id: editing.id,
          outgoingShiftInstanceId: values.outgoingShiftInstanceId,
          incomingShiftInstanceId: values.incomingShiftInstanceId || undefined,
          mineSiteId,
          handoverDateTime,
          safetyIssues: values.safetyIssues,
          ongoingOperations: values.ongoingOperations,
          pendingTasks: values.pendingTasks,
          equipmentStatus: values.equipmentStatus,
          environmentalConditions: values.environmentalConditions,
          generalRemarks: values.generalRemarks,
          handedOverBy: values.handedOverBy,
          receivedBy: values.receivedBy,
          status: values.status,
        },
      });
    } else {
      createMutation.mutate({
        outgoingShiftInstanceId: values.outgoingShiftInstanceId,
        incomingShiftInstanceId: values.incomingShiftInstanceId || undefined,
        mineSiteId,
        handoverDateTime,
        safetyIssues: values.safetyIssues,
        ongoingOperations: values.ongoingOperations,
        pendingTasks: values.pendingTasks,
        equipmentStatus: values.equipmentStatus,
        environmentalConditions: values.environmentalConditions,
        generalRemarks: values.generalRemarks,
        handedOverBy: values.handedOverBy,
        receivedBy: values.receivedBy,
      });
    }
  };

  const shiftInstanceOptions = instances.map(i => ({
    value: i.id,
    label: `${i.shiftDefinitionName} - ${dayjs(i.date).format('DD MMM YYYY')} (${i.status})`,
  }));

  const columns: ColumnsType<ShiftHandover> = [
    {
      title: 'Date/Time',
      dataIndex: 'handoverDateTime',
      key: 'handoverDateTime',
      width: 160,
      render: (d: string) => dayjs(d).format('DD MMM YYYY HH:mm'),
      sorter: (a, b) => dayjs(a.handoverDateTime).unix() - dayjs(b.handoverDateTime).unix(),
      defaultSortOrder: 'descend',
    },
    { title: 'Outgoing Shift', dataIndex: 'outgoingShiftName', key: 'outgoingShiftName' },
    { title: 'Incoming Shift', dataIndex: 'incomingShiftName', key: 'incomingShiftName', render: (v: string | null) => v || '-' },
    { title: 'Handed Over By', dataIndex: 'handedOverBy', key: 'handedOverBy', render: (v: string | null) => v || '-' },
    { title: 'Received By', dataIndex: 'receivedBy', key: 'receivedBy', render: (v: string | null) => v || '-' },
    {
      title: 'Status',
      dataIndex: 'status',
      key: 'status',
      width: 130,
      render: (s: string) => <Tag color={statusColors[s] || 'default'}>{s}</Tag>,
    },
    {
      title: 'Actions',
      key: 'actions',
      width: 140,
      render: (_, record) => (
        <Space size="small">
          <Button type="text" size="small" icon={<EyeOutlined />} onClick={() => setDetailDrawer(record)} />
          <PermissionGate permission="ShiftHandover.Update">
            <Button type="text" size="small" icon={<EditOutlined />} onClick={() => openEdit(record)} />
          </PermissionGate>
        </Space>
      ),
    },
  ];

  return (
    <>
      <Flex justify="space-between" align="center" style={{ marginBottom: 16 }} wrap="wrap" gap={12}>
        <Flex align="center" gap={12}>
          <Text type="secondary">{handovers.length} handover(s)</Text>
          <RangePicker
            size="small"
            onChange={(dates) => {
              if (dates && dates[0] && dates[1]) {
                setDateRange([dates[0].format('YYYY-MM-DD'), dates[1].format('YYYY-MM-DD')]);
              } else {
                setDateRange([]);
              }
            }}
          />
        </Flex>
        <PermissionGate permission="ShiftHandover.Create">
          <Button type="primary" icon={<PlusOutlined />} onClick={openCreate}>New Handover</Button>
        </PermissionGate>
      </Flex>

      <Table columns={columns} dataSource={handovers} rowKey="id" loading={isLoading} size="middle" pagination={{ pageSize: 20 }} />

      <Modal
        title={editing ? 'Edit Handover' : 'New Shift Handover'}
        open={modalOpen}
        onOk={handleSubmit}
        onCancel={() => { setModalOpen(false); setEditing(null); form.resetFields(); }}
        confirmLoading={createMutation.isPending || updateMutation.isPending}
        width={700}
      >
        <Form form={form} layout="vertical" style={{ marginTop: 16 }}>
          <Flex gap={16}>
            <Form.Item name="outgoingShiftInstanceId" label="Outgoing Shift" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select placeholder="Select outgoing shift" options={shiftInstanceOptions} showSearch optionFilterProp="label" />
            </Form.Item>
            <Form.Item name="incomingShiftInstanceId" label="Incoming Shift" style={{ flex: 1 }}>
              <Select placeholder="Select incoming shift (optional)" options={shiftInstanceOptions} showSearch optionFilterProp="label" allowClear />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="handoverDateTime" label="Handover Date/Time" rules={[{ required: true }]} style={{ flex: 1 }}>
              <DatePicker showTime style={{ width: '100%' }} />
            </Form.Item>
            {editing && (
              <Form.Item name="status" label="Status" style={{ flex: 1 }}>
                <Select options={HANDOVER_STATUSES.map(s => ({ value: s, label: s }))} />
              </Form.Item>
            )}
          </Flex>
          <Flex gap={16}>
            <Form.Item name="handedOverBy" label="Handed Over By" style={{ flex: 1 }}>
              <Input placeholder="Name" />
            </Form.Item>
            <Form.Item name="receivedBy" label="Received By" style={{ flex: 1 }}>
              <Input placeholder="Name" />
            </Form.Item>
          </Flex>
          <Form.Item name="safetyIssues" label="Safety Issues">
            <TextArea rows={2} placeholder="Any safety concerns to pass on..." />
          </Form.Item>
          <Form.Item name="ongoingOperations" label="Ongoing Operations">
            <TextArea rows={2} placeholder="Current operations in progress..." />
          </Form.Item>
          <Form.Item name="pendingTasks" label="Pending Tasks">
            <TextArea rows={2} placeholder="Tasks that need completion..." />
          </Form.Item>
          <Form.Item name="equipmentStatus" label="Equipment Status">
            <TextArea rows={2} placeholder="Equipment notes..." />
          </Form.Item>
          <Form.Item name="environmentalConditions" label="Environmental Conditions">
            <Input placeholder="Weather, ventilation, etc." />
          </Form.Item>
          <Form.Item name="generalRemarks" label="General Remarks">
            <TextArea rows={2} placeholder="Any other notes..." />
          </Form.Item>
        </Form>
      </Modal>

      <Drawer
        title="Handover Details"
        open={!!detailDrawer}
        onClose={() => setDetailDrawer(null)}
        width={600}
        extra={
          detailDrawer?.status === 'Submitted' && (
            <PermissionGate permission="ShiftHandover.Update">
              <Button
                type="primary"
                icon={<CheckCircleOutlined />}
                onClick={() => acknowledgeMutation.mutate(detailDrawer.id)}
                loading={acknowledgeMutation.isPending}
              >
                Acknowledge
              </Button>
            </PermissionGate>
          )
        }
      >
        {detailDrawer && (
          <Descriptions column={1} bordered size="small">
            <Descriptions.Item label="Status"><Tag color={statusColors[detailDrawer.status]}>{detailDrawer.status}</Tag></Descriptions.Item>
            <Descriptions.Item label="Handover Time">{dayjs(detailDrawer.handoverDateTime).format('DD MMM YYYY HH:mm')}</Descriptions.Item>
            <Descriptions.Item label="Outgoing Shift">{detailDrawer.outgoingShiftName}</Descriptions.Item>
            <Descriptions.Item label="Incoming Shift">{detailDrawer.incomingShiftName || '-'}</Descriptions.Item>
            <Descriptions.Item label="Handed Over By">{detailDrawer.handedOverBy || '-'}</Descriptions.Item>
            <Descriptions.Item label="Received By">{detailDrawer.receivedBy || '-'}</Descriptions.Item>
            <Descriptions.Item label="Safety Issues">{detailDrawer.safetyIssues || '-'}</Descriptions.Item>
            <Descriptions.Item label="Ongoing Operations">{detailDrawer.ongoingOperations || '-'}</Descriptions.Item>
            <Descriptions.Item label="Pending Tasks">{detailDrawer.pendingTasks || '-'}</Descriptions.Item>
            <Descriptions.Item label="Equipment Status">{detailDrawer.equipmentStatus || '-'}</Descriptions.Item>
            <Descriptions.Item label="Environmental">{detailDrawer.environmentalConditions || '-'}</Descriptions.Item>
            <Descriptions.Item label="General Remarks">{detailDrawer.generalRemarks || '-'}</Descriptions.Item>
            {detailDrawer.acknowledgedAt && (
              <Descriptions.Item label="Acknowledged At">{dayjs(detailDrawer.acknowledgedAt).format('DD MMM YYYY HH:mm')}</Descriptions.Item>
            )}
            <Descriptions.Item label="Mine Site">{detailDrawer.mineSiteName}</Descriptions.Item>
            <Descriptions.Item label="Created">{dayjs(detailDrawer.createdAt).format('DD MMM YYYY HH:mm')}</Descriptions.Item>
          </Descriptions>
        )}
      </Drawer>
    </>
  );
}

// ===== MAIN PAGE =====
export default function ShiftManagementPage() {
  const [selectedSiteId, setSelectedSiteId] = useState<string>('');

  const { data: sites = [], isLoading: sitesLoading } = useQuery({
    queryKey: ['mineSites'],
    queryFn: mineSitesApi.getMineSites,
  });

  return (
    <div>
      <Flex justify="space-between" align="center" style={{ marginBottom: 24 }}>
        <Title level={3} style={{ margin: 0 }}>Shift Management</Title>
        <Select
          placeholder="Select Mine Site"
          style={{ width: 300 }}
          loading={sitesLoading}
          value={selectedSiteId || undefined}
          onChange={setSelectedSiteId}
          options={sites.map((s: MineSite) => ({ value: s.id, label: `${s.name}${s.code ? ` (${s.code})` : ''}` }))}
          showSearch
          optionFilterProp="label"
        />
      </Flex>

      {!selectedSiteId ? (
        <Flex justify="center" align="center" style={{ minHeight: 300 }}>
          <Text type="secondary" style={{ fontSize: 16 }}>
            <SwapOutlined style={{ marginRight: 8 }} />
            Select a mine site to manage shifts
          </Text>
        </Flex>
      ) : (
        <Tabs
          defaultActiveKey="definitions"
          items={[
            {
              key: 'definitions',
              label: 'Shift Definitions',
              children: <ShiftDefinitionsTab mineSiteId={selectedSiteId} />,
            },
            {
              key: 'instances',
              label: 'Shift Log',
              children: <ShiftInstancesTab mineSiteId={selectedSiteId} />,
            },
            {
              key: 'handovers',
              label: 'Handovers',
              children: <ShiftHandoversTab mineSiteId={selectedSiteId} />,
            },
          ]}
        />
      )}
    </div>
  );
}
