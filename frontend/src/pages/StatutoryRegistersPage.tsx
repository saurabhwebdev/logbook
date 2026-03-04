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
  Drawer,
  Descriptions,
  Alert,
  Badge,
} from 'antd';
import {
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  ExclamationCircleOutlined,
  EyeOutlined,
  FileAddOutlined,
  BookOutlined,
  HistoryOutlined,
} from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AxiosError } from 'axios';
import dayjs from 'dayjs';
import { statutoryRegistersApi } from '../api/statutoryRegistersApi';
import type {
  CreateStatutoryRegisterRequest,
  UpdateStatutoryRegisterRequest,
  CreateRegisterEntryRequest,
  AmendRegisterEntryRequest,
} from '../api/statutoryRegistersApi';
import { mineSitesApi } from '../api/mineSitesApi';
import type { StatutoryRegister, RegisterEntry, MineSite } from '../types';
import PermissionGate from '../components/PermissionGate';

const { Text, Title } = Typography;
const { TextArea } = Input;

const REGISTER_TYPES = [
  'Accident',
  'DangerousOccurrence',
  'PersonEntry',
  'Explosives',
  'MachineBreakdown',
  'Inspection',
  'WorkmenPresence',
  'Ventilation',
  'TimberSupply',
  'Custom',
];

const JURISDICTIONS = [
  { value: 'MSHA', label: 'MSHA (USA)' },
  { value: 'DGMS', label: 'DGMS (India)' },
  { value: 'AU_QLD', label: 'Queensland (Australia)' },
  { value: 'AU_NSW', label: 'New South Wales (Australia)' },
  { value: 'AU_WA', label: 'Western Australia' },
  { value: 'SA_MHSA', label: 'MHSA (South Africa)' },
  { value: 'CANADA_BC', label: 'British Columbia (Canada)' },
  { value: 'CHILE_SERNAGEOMIN', label: 'SERNAGEOMIN (Chile)' },
  { value: 'PERU_OSINERGMIN', label: 'OSINERGMIN (Peru)' },
  { value: 'OTHER', label: 'Other' },
];

const ENTRY_STATUSES = ['Open', 'ActionRequired', 'Closed', 'Amended'];

const statusColors: Record<string, string> = {
  Open: 'blue',
  ActionRequired: 'orange',
  Closed: 'green',
  Amended: 'purple',
};

const typeColors: Record<string, string> = {
  Accident: 'red',
  DangerousOccurrence: 'volcano',
  PersonEntry: 'blue',
  Explosives: 'orange',
  MachineBreakdown: 'gold',
  Inspection: 'cyan',
  WorkmenPresence: 'geekblue',
  Ventilation: 'lime',
  TimberSupply: 'green',
  Custom: 'default',
};

// ===== REGISTERS LIST =====
function RegistersListTab({ mineSiteId }: { mineSiteId: string }) {
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<StatutoryRegister | null>(null);
  const [selectedRegister, setSelectedRegister] = useState<StatutoryRegister | null>(null);
  const [form] = Form.useForm();
  const queryClient = useQueryClient();

  const { data: registers = [], isLoading } = useQuery({
    queryKey: ['statutoryRegisters', mineSiteId],
    queryFn: () => statutoryRegistersApi.getRegisters(mineSiteId),
    enabled: !!mineSiteId,
  });

  const createMutation = useMutation({
    mutationFn: (data: CreateStatutoryRegisterRequest) => statutoryRegistersApi.createRegister(data),
    onSuccess: () => {
      message.success('Register created');
      queryClient.invalidateQueries({ queryKey: ['statutoryRegisters', mineSiteId] });
      setModalOpen(false);
      form.resetFields();
    },
    onError: (err: AxiosError<{ detail?: string }>) => {
      message.error(err.response?.data?.detail || 'Failed to create register');
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateStatutoryRegisterRequest }) =>
      statutoryRegistersApi.updateRegister(id, data),
    onSuccess: () => {
      message.success('Register updated');
      queryClient.invalidateQueries({ queryKey: ['statutoryRegisters', mineSiteId] });
      setModalOpen(false);
      setEditing(null);
      form.resetFields();
    },
    onError: (err: AxiosError<{ detail?: string }>) => {
      message.error(err.response?.data?.detail || 'Failed to update register');
    },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => statutoryRegistersApi.deleteRegister(id),
    onSuccess: () => {
      message.success('Register deleted');
      queryClient.invalidateQueries({ queryKey: ['statutoryRegisters', mineSiteId] });
    },
    onError: (err: AxiosError<{ detail?: string }>) => {
      message.error(err.response?.data?.detail || 'Failed to delete register');
    },
  });

  const openCreate = () => {
    setEditing(null);
    form.resetFields();
    form.setFieldsValue({ isRequired: true, retentionYears: 5, isActive: true, sortOrder: 0 });
    setModalOpen(true);
  };

  const openEdit = (record: StatutoryRegister) => {
    setEditing(record);
    form.setFieldsValue({
      name: record.name,
      code: record.code,
      registerType: record.registerType,
      description: record.description,
      jurisdiction: record.jurisdiction,
      isRequired: record.isRequired,
      retentionYears: record.retentionYears,
      isActive: record.isActive,
      sortOrder: record.sortOrder,
    });
    setModalOpen(true);
  };

  const handleSubmit = async () => {
    const values = await form.validateFields();
    if (editing) {
      updateMutation.mutate({
        id: editing.id,
        data: { id: editing.id, ...values },
      });
    } else {
      createMutation.mutate({ mineSiteId, ...values });
    }
  };

  const handleDelete = (record: StatutoryRegister) => {
    Modal.confirm({
      title: 'Delete Register?',
      icon: <ExclamationCircleOutlined />,
      content: `Are you sure you want to delete "${record.name}"? This cannot be undone.`,
      okText: 'Delete',
      okType: 'danger',
      onOk: () => deleteMutation.mutateAsync(record.id),
    });
  };

  const columns: ColumnsType<StatutoryRegister> = [
    {
      title: 'Name',
      dataIndex: 'name',
      key: 'name',
      render: (name: string, record) => (
        <Flex align="center" gap={8}>
          <BookOutlined />
          <div>
            <Text strong>{name}</Text>
            {record.code && <Text type="secondary" style={{ marginLeft: 6 }}>({record.code})</Text>}
          </div>
        </Flex>
      ),
    },
    {
      title: 'Type',
      dataIndex: 'registerType',
      key: 'registerType',
      width: 160,
      render: (type: string) => <Tag color={typeColors[type] || 'default'}>{type}</Tag>,
    },
    {
      title: 'Jurisdiction',
      dataIndex: 'jurisdiction',
      key: 'jurisdiction',
      width: 140,
    },
    {
      title: 'Required',
      dataIndex: 'isRequired',
      key: 'isRequired',
      width: 90,
      align: 'center',
      render: (v: boolean) => v ? <Tag color="red">Yes</Tag> : <Tag>No</Tag>,
    },
    {
      title: 'Retention',
      dataIndex: 'retentionYears',
      key: 'retentionYears',
      width: 100,
      align: 'center',
      render: (v: number) => `${v} yrs`,
    },
    {
      title: 'Entries',
      dataIndex: 'entryCount',
      key: 'entryCount',
      width: 80,
      align: 'center',
      render: (count: number) => <Badge count={count} showZero style={{ backgroundColor: count > 0 ? '#1890ff' : '#d9d9d9' }} />,
    },
    {
      title: 'Status',
      dataIndex: 'isActive',
      key: 'isActive',
      width: 90,
      render: (v: boolean) => <Tag color={v ? 'green' : 'default'}>{v ? 'Active' : 'Inactive'}</Tag>,
    },
    {
      title: 'Actions',
      key: 'actions',
      width: 150,
      render: (_, record) => (
        <Space size="small">
          <Button type="text" size="small" icon={<EyeOutlined />} onClick={() => setSelectedRegister(record)} />
          <PermissionGate permission="StatutoryRegister.Update">
            <Button type="text" size="small" icon={<EditOutlined />} onClick={() => openEdit(record)} />
          </PermissionGate>
          <PermissionGate permission="StatutoryRegister.Delete">
            <Button type="text" size="small" danger icon={<DeleteOutlined />} onClick={() => handleDelete(record)} />
          </PermissionGate>
        </Space>
      ),
    },
  ];

  return (
    <>
      <Flex justify="space-between" align="center" style={{ marginBottom: 16 }}>
        <Text type="secondary">{registers.length} register(s)</Text>
        <PermissionGate permission="StatutoryRegister.Create">
          <Button type="primary" icon={<PlusOutlined />} onClick={openCreate}>Add Register</Button>
        </PermissionGate>
      </Flex>

      <Table columns={columns} dataSource={registers} rowKey="id" loading={isLoading} size="middle" pagination={false} />

      <Modal
        title={editing ? 'Edit Register' : 'New Statutory Register'}
        open={modalOpen}
        onOk={handleSubmit}
        onCancel={() => { setModalOpen(false); setEditing(null); form.resetFields(); }}
        confirmLoading={createMutation.isPending || updateMutation.isPending}
        width={600}
      >
        <Form form={form} layout="vertical" style={{ marginTop: 16 }}>
          <Form.Item name="name" label="Register Name" rules={[{ required: true }]}>
            <Input placeholder="e.g., Accident Register" />
          </Form.Item>
          <Flex gap={16}>
            <Form.Item name="code" label="Code" style={{ flex: 1 }}>
              <Input placeholder="e.g., FORM_M" maxLength={50} />
            </Form.Item>
            <Form.Item name="registerType" label="Type" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select options={REGISTER_TYPES.map(t => ({ value: t, label: t }))} />
            </Form.Item>
          </Flex>
          <Form.Item name="jurisdiction" label="Jurisdiction" rules={[{ required: true }]}>
            <Select options={JURISDICTIONS} />
          </Form.Item>
          <Form.Item name="description" label="Description">
            <TextArea rows={2} />
          </Form.Item>
          <Flex gap={16}>
            <Form.Item name="isRequired" label="Legally Required" style={{ flex: 1 }}>
              <Select options={[{ value: true, label: 'Yes' }, { value: false, label: 'No' }]} />
            </Form.Item>
            <Form.Item name="retentionYears" label="Retention (years)" style={{ flex: 1 }}>
              <InputNumber min={1} max={100} style={{ width: '100%' }} />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="isActive" label="Active" style={{ flex: 1 }}>
              <Select options={[{ value: true, label: 'Active' }, { value: false, label: 'Inactive' }]} />
            </Form.Item>
            <Form.Item name="sortOrder" label="Sort Order" style={{ flex: 1 }}>
              <InputNumber min={0} style={{ width: '100%' }} />
            </Form.Item>
          </Flex>
        </Form>
      </Modal>

      <Drawer
        title="Register Details"
        open={!!selectedRegister}
        onClose={() => setSelectedRegister(null)}
        width={500}
      >
        {selectedRegister && (
          <Descriptions column={1} bordered size="small">
            <Descriptions.Item label="Name">{selectedRegister.name}</Descriptions.Item>
            <Descriptions.Item label="Code">{selectedRegister.code || '-'}</Descriptions.Item>
            <Descriptions.Item label="Type"><Tag color={typeColors[selectedRegister.registerType]}>{selectedRegister.registerType}</Tag></Descriptions.Item>
            <Descriptions.Item label="Jurisdiction">{selectedRegister.jurisdiction}</Descriptions.Item>
            <Descriptions.Item label="Description">{selectedRegister.description || '-'}</Descriptions.Item>
            <Descriptions.Item label="Required">{selectedRegister.isRequired ? 'Yes' : 'No'}</Descriptions.Item>
            <Descriptions.Item label="Retention">{selectedRegister.retentionYears} years</Descriptions.Item>
            <Descriptions.Item label="Entries">{selectedRegister.entryCount}</Descriptions.Item>
            <Descriptions.Item label="Active"><Tag color={selectedRegister.isActive ? 'green' : 'default'}>{selectedRegister.isActive ? 'Yes' : 'No'}</Tag></Descriptions.Item>
            <Descriptions.Item label="Created">{dayjs(selectedRegister.createdAt).format('DD MMM YYYY HH:mm')}</Descriptions.Item>
          </Descriptions>
        )}
      </Drawer>
    </>
  );
}

// ===== REGISTER ENTRIES TAB =====
function RegisterEntriesTab({ mineSiteId }: { mineSiteId: string }) {
  const [selectedRegisterId, setSelectedRegisterId] = useState<string>('');
  const [entryModalOpen, setEntryModalOpen] = useState(false);
  const [amendModalOpen, setAmendModalOpen] = useState(false);
  const [selectedEntry, setSelectedEntry] = useState<RegisterEntry | null>(null);
  const [amendingEntry, setAmendingEntry] = useState<RegisterEntry | null>(null);
  const [statusFilter, setStatusFilter] = useState<string>('');
  const [entryForm] = Form.useForm();
  const [amendForm] = Form.useForm();
  const queryClient = useQueryClient();

  const { data: registers = [] } = useQuery({
    queryKey: ['statutoryRegisters', mineSiteId],
    queryFn: () => statutoryRegistersApi.getRegisters(mineSiteId),
    enabled: !!mineSiteId,
  });

  const { data: entries = [], isLoading } = useQuery({
    queryKey: ['registerEntries', selectedRegisterId, statusFilter],
    queryFn: () => statutoryRegistersApi.getEntries(selectedRegisterId, statusFilter || undefined),
    enabled: !!selectedRegisterId,
  });

  const createEntryMutation = useMutation({
    mutationFn: (data: CreateRegisterEntryRequest) => statutoryRegistersApi.createEntry(data),
    onSuccess: () => {
      message.success('Entry recorded');
      queryClient.invalidateQueries({ queryKey: ['registerEntries', selectedRegisterId] });
      queryClient.invalidateQueries({ queryKey: ['statutoryRegisters', mineSiteId] });
      setEntryModalOpen(false);
      entryForm.resetFields();
    },
    onError: (err: AxiosError<{ detail?: string }>) => {
      message.error(err.response?.data?.detail || 'Failed to create entry');
    },
  });

  const amendEntryMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: AmendRegisterEntryRequest }) =>
      statutoryRegistersApi.amendEntry(id, data),
    onSuccess: () => {
      message.success('Amendment recorded');
      queryClient.invalidateQueries({ queryKey: ['registerEntries', selectedRegisterId] });
      setAmendModalOpen(false);
      setAmendingEntry(null);
      amendForm.resetFields();
    },
    onError: (err: AxiosError<{ detail?: string }>) => {
      message.error(err.response?.data?.detail || 'Failed to amend entry');
    },
  });

  const openCreateEntry = () => {
    entryForm.resetFields();
    entryForm.setFieldsValue({ entryDate: dayjs() });
    setEntryModalOpen(true);
  };

  const openAmend = (record: RegisterEntry) => {
    setAmendingEntry(record);
    amendForm.setFieldsValue({
      subject: record.subject,
      details: record.details,
      reportedBy: record.reportedBy,
      witnessName: record.witnessName,
      actionTaken: record.actionTaken,
      entryDate: dayjs(),
    });
    setAmendModalOpen(true);
  };

  const handleCreateEntry = async () => {
    const values = await entryForm.validateFields();
    createEntryMutation.mutate({
      statutoryRegisterId: selectedRegisterId,
      mineSiteId,
      entryDate: dayjs(values.entryDate).toISOString(),
      subject: values.subject,
      details: values.details,
      reportedBy: values.reportedBy,
      witnessName: values.witnessName,
      actionTaken: values.actionTaken,
      actionDueDate: values.actionDueDate ? dayjs(values.actionDueDate).toISOString() : undefined,
    });
  };

  const handleAmendEntry = async () => {
    if (!amendingEntry) return;
    const values = await amendForm.validateFields();
    amendEntryMutation.mutate({
      id: amendingEntry.id,
      data: {
        originalEntryId: amendingEntry.id,
        statutoryRegisterId: selectedRegisterId,
        mineSiteId,
        entryDate: dayjs(values.entryDate).toISOString(),
        subject: values.subject,
        details: values.details,
        reportedBy: values.reportedBy,
        witnessName: values.witnessName,
        actionTaken: values.actionTaken,
        actionDueDate: values.actionDueDate ? dayjs(values.actionDueDate).toISOString() : undefined,
        amendmentReason: values.amendmentReason,
      },
    });
  };

  const columns: ColumnsType<RegisterEntry> = [
    {
      title: '#',
      dataIndex: 'entryNumber',
      key: 'entryNumber',
      width: 60,
      align: 'center',
      render: (n: number) => <Text strong>#{n}</Text>,
    },
    {
      title: 'Date',
      dataIndex: 'entryDate',
      key: 'entryDate',
      width: 130,
      render: (d: string) => dayjs(d).format('DD MMM YYYY'),
      sorter: (a, b) => dayjs(a.entryDate).unix() - dayjs(b.entryDate).unix(),
      defaultSortOrder: 'descend',
    },
    { title: 'Subject', dataIndex: 'subject', key: 'subject', ellipsis: true },
    { title: 'Reported By', dataIndex: 'reportedBy', key: 'reportedBy', width: 150 },
    {
      title: 'Area',
      dataIndex: 'mineAreaName',
      key: 'mineAreaName',
      width: 130,
      render: (v: string | null) => v || '-',
    },
    {
      title: 'Status',
      dataIndex: 'status',
      key: 'status',
      width: 130,
      render: (s: string) => <Tag color={statusColors[s] || 'default'}>{s}</Tag>,
    },
    {
      title: 'Amendments',
      dataIndex: 'amendmentCount',
      key: 'amendmentCount',
      width: 110,
      align: 'center',
      render: (count: number, record) => (
        <Flex align="center" gap={4}>
          {count > 0 && <HistoryOutlined />}
          {count}
          {record.amendmentOfEntryId && <Tag color="purple" style={{ marginLeft: 4 }}>Amendment</Tag>}
        </Flex>
      ),
    },
    {
      title: 'Actions',
      key: 'actions',
      width: 100,
      render: (_, record) => (
        <Space size="small">
          <Button type="text" size="small" icon={<EyeOutlined />} onClick={() => setSelectedEntry(record)} />
          <PermissionGate permission="RegisterEntry.Amend">
            <Button type="text" size="small" icon={<FileAddOutlined />} onClick={() => openAmend(record)} title="Amend" />
          </PermissionGate>
        </Space>
      ),
    },
  ];

  return (
    <>
      <Alert
        message="Tamper-Proof Register"
        description="Entries in statutory registers are append-only and cannot be edited or deleted. To correct an entry, use the Amend function which creates a new linked amendment entry."
        type="info"
        showIcon
        style={{ marginBottom: 16 }}
      />

      <Flex justify="space-between" align="center" style={{ marginBottom: 16 }} wrap="wrap" gap={12}>
        <Flex align="center" gap={12}>
          <Select
            placeholder="Select Register"
            style={{ width: 280 }}
            value={selectedRegisterId || undefined}
            onChange={setSelectedRegisterId}
            options={registers.map(r => ({ value: r.id, label: `${r.name}${r.code ? ` (${r.code})` : ''}` }))}
            showSearch
            optionFilterProp="label"
          />
          <Select
            placeholder="Status"
            allowClear
            style={{ width: 150 }}
            value={statusFilter || undefined}
            onChange={(v) => setStatusFilter(v || '')}
            options={ENTRY_STATUSES.map(s => ({ value: s, label: s }))}
          />
          <Text type="secondary">{entries.length} entries</Text>
        </Flex>
        {selectedRegisterId && (
          <PermissionGate permission="RegisterEntry.Create">
            <Button type="primary" icon={<PlusOutlined />} onClick={openCreateEntry}>New Entry</Button>
          </PermissionGate>
        )}
      </Flex>

      {!selectedRegisterId ? (
        <Flex justify="center" align="center" style={{ minHeight: 200 }}>
          <Text type="secondary">Select a register to view entries</Text>
        </Flex>
      ) : (
        <Table columns={columns} dataSource={entries} rowKey="id" loading={isLoading} size="middle" pagination={{ pageSize: 25 }} />
      )}

      {/* Create Entry Modal */}
      <Modal
        title="New Register Entry"
        open={entryModalOpen}
        onOk={handleCreateEntry}
        onCancel={() => { setEntryModalOpen(false); entryForm.resetFields(); }}
        confirmLoading={createEntryMutation.isPending}
        width={650}
      >
        <Form form={entryForm} layout="vertical" style={{ marginTop: 16 }}>
          <Form.Item name="entryDate" label="Entry Date" rules={[{ required: true }]}>
            <DatePicker showTime style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item name="subject" label="Subject" rules={[{ required: true, max: 500 }]}>
            <Input placeholder="Brief description of the entry" />
          </Form.Item>
          <Form.Item name="details" label="Details" rules={[{ required: true, max: 4000 }]}>
            <TextArea rows={4} placeholder="Full details..." />
          </Form.Item>
          <Flex gap={16}>
            <Form.Item name="reportedBy" label="Reported By" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Input placeholder="Name of person reporting" />
            </Form.Item>
            <Form.Item name="witnessName" label="Witness" style={{ flex: 1 }}>
              <Input placeholder="Witness name (if applicable)" />
            </Form.Item>
          </Flex>
          <Form.Item name="actionTaken" label="Action Taken">
            <TextArea rows={2} placeholder="Actions taken..." />
          </Form.Item>
          <Form.Item name="actionDueDate" label="Action Due Date">
            <DatePicker style={{ width: '100%' }} />
          </Form.Item>
        </Form>
      </Modal>

      {/* Amend Entry Modal */}
      <Modal
        title={`Amend Entry #${amendingEntry?.entryNumber || ''}`}
        open={amendModalOpen}
        onOk={handleAmendEntry}
        onCancel={() => { setAmendModalOpen(false); setAmendingEntry(null); amendForm.resetFields(); }}
        confirmLoading={amendEntryMutation.isPending}
        width={650}
      >
        <Alert
          message="This will create a new amendment entry linked to the original. The original entry remains unchanged."
          type="warning"
          showIcon
          style={{ marginBottom: 16 }}
        />
        <Form form={amendForm} layout="vertical">
          <Form.Item name="amendmentReason" label="Reason for Amendment" rules={[{ required: true, message: 'Amendment reason is required' }]}>
            <TextArea rows={2} placeholder="Why is this entry being amended?" />
          </Form.Item>
          <Form.Item name="entryDate" label="Amendment Date" rules={[{ required: true }]}>
            <DatePicker showTime style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item name="subject" label="Corrected Subject" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item name="details" label="Corrected Details" rules={[{ required: true }]}>
            <TextArea rows={4} />
          </Form.Item>
          <Flex gap={16}>
            <Form.Item name="reportedBy" label="Reported By" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Input />
            </Form.Item>
            <Form.Item name="witnessName" label="Witness" style={{ flex: 1 }}>
              <Input />
            </Form.Item>
          </Flex>
          <Form.Item name="actionTaken" label="Action Taken">
            <TextArea rows={2} />
          </Form.Item>
          <Form.Item name="actionDueDate" label="Action Due Date">
            <DatePicker style={{ width: '100%' }} />
          </Form.Item>
        </Form>
      </Modal>

      {/* Entry Detail Drawer */}
      <Drawer
        title={`Entry #${selectedEntry?.entryNumber || ''}`}
        open={!!selectedEntry}
        onClose={() => setSelectedEntry(null)}
        width={600}
      >
        {selectedEntry && (
          <>
            {selectedEntry.amendmentOfEntryId && (
              <Alert
                message="This is an amendment entry"
                description={`Amendment reason: ${selectedEntry.amendmentReason || 'Not specified'}`}
                type="info"
                showIcon
                style={{ marginBottom: 16 }}
              />
            )}
            <Descriptions column={1} bordered size="small">
              <Descriptions.Item label="Entry #">{selectedEntry.entryNumber}</Descriptions.Item>
              <Descriptions.Item label="Date">{dayjs(selectedEntry.entryDate).format('DD MMM YYYY HH:mm')}</Descriptions.Item>
              <Descriptions.Item label="Register">{selectedEntry.registerName}</Descriptions.Item>
              <Descriptions.Item label="Status"><Tag color={statusColors[selectedEntry.status]}>{selectedEntry.status}</Tag></Descriptions.Item>
              <Descriptions.Item label="Subject">{selectedEntry.subject}</Descriptions.Item>
              <Descriptions.Item label="Details">{selectedEntry.details}</Descriptions.Item>
              <Descriptions.Item label="Reported By">{selectedEntry.reportedBy}</Descriptions.Item>
              <Descriptions.Item label="Witness">{selectedEntry.witnessName || '-'}</Descriptions.Item>
              <Descriptions.Item label="Mine Area">{selectedEntry.mineAreaName || '-'}</Descriptions.Item>
              <Descriptions.Item label="Action Taken">{selectedEntry.actionTaken || '-'}</Descriptions.Item>
              <Descriptions.Item label="Action Due">{selectedEntry.actionDueDate ? dayjs(selectedEntry.actionDueDate).format('DD MMM YYYY') : '-'}</Descriptions.Item>
              <Descriptions.Item label="Action Completed">{selectedEntry.actionCompletedDate ? dayjs(selectedEntry.actionCompletedDate).format('DD MMM YYYY') : '-'}</Descriptions.Item>
              <Descriptions.Item label="Amendments">{selectedEntry.amendmentCount}</Descriptions.Item>
              <Descriptions.Item label="Recorded">{dayjs(selectedEntry.createdAt).format('DD MMM YYYY HH:mm')}</Descriptions.Item>
            </Descriptions>
          </>
        )}
      </Drawer>
    </>
  );
}

// ===== MAIN PAGE =====
export default function StatutoryRegistersPage() {
  const [selectedSiteId, setSelectedSiteId] = useState<string>('');

  const { data: sites = [], isLoading: sitesLoading } = useQuery({
    queryKey: ['mineSites'],
    queryFn: mineSitesApi.getMineSites,
  });

  return (
    <div>
      <Flex justify="space-between" align="center" style={{ marginBottom: 24 }}>
        <Title level={3} style={{ margin: 0 }}>Statutory Registers</Title>
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
            <BookOutlined style={{ marginRight: 8 }} />
            Select a mine site to manage statutory registers
          </Text>
        </Flex>
      ) : (
        <Tabs
          defaultActiveKey="registers"
          items={[
            {
              key: 'registers',
              label: 'Registers',
              children: <RegistersListTab mineSiteId={selectedSiteId} />,
            },
            {
              key: 'entries',
              label: 'Register Entries',
              children: <RegisterEntriesTab mineSiteId={selectedSiteId} />,
            },
          ]}
        />
      )}
    </div>
  );
}
