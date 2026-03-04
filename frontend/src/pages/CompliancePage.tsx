import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Table, Button, Modal, Form, Input, Select, DatePicker, Tag, Drawer, Tabs,
  Typography, Flex, Switch, message, Descriptions, Popconfirm,
} from 'antd';
import { PlusOutlined, EyeOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import { complianceApi } from '../api/complianceApi';
import { mineSitesApi } from '../api/mineSitesApi';
import type { ComplianceRequirement, ComplianceAudit } from '../types';
import type {
  CreateComplianceRequirementRequest, UpdateComplianceRequirementRequest,
  CreateComplianceAuditRequest,
} from '../api/complianceApi';

const { Title, Text } = Typography;
const { TextArea } = Input;

const categories = ['Safety', 'Environmental', 'Mining', 'Labor', 'Health', 'Emergency', 'Reporting', 'Other'];
const frequencies = ['Daily', 'Weekly', 'Monthly', 'Quarterly', 'SemiAnnually', 'Annually', 'AsRequired'];
const statuses = ['Compliant', 'NonCompliant', 'PartiallyCompliant', 'Pending', 'Overdue', 'NotApplicable'];
const priorities = ['Critical', 'High', 'Medium', 'Low'];
const auditTypes = ['Internal', 'External', 'Regulatory', 'SelfAssessment'];
const complianceStatuses = ['Compliant', 'NonCompliant', 'PartiallyCompliant', 'Observation'];
const auditStatuses = ['Open', 'InProgress', 'Closed'];

const statusColors: Record<string, string> = {
  Compliant: '#34c759', NonCompliant: '#ff3b30', PartiallyCompliant: '#ff9500',
  Pending: '#86868b', Overdue: '#af52de', NotApplicable: '#8e8e93',
};
const priorityColors: Record<string, string> = {
  Critical: '#ff3b30', High: '#ff9500', Medium: '#0071e3', Low: '#34c759',
};
const categoryColors: Record<string, string> = {
  Safety: '#ff3b30', Environmental: '#34c759', Mining: '#8b6914', Labor: '#0071e3',
  Health: '#af52de', Emergency: '#ff453a', Reporting: '#86868b', Other: '#8e8e93',
};
const auditTypeColors: Record<string, string> = {
  Internal: '#0071e3', External: '#ff9500', Regulatory: '#ff3b30', SelfAssessment: '#34c759',
};
const complianceStatusColors: Record<string, string> = {
  Compliant: '#34c759', NonCompliant: '#ff3b30', PartiallyCompliant: '#ff9500', Observation: '#86868b',
};
const auditStatusColors: Record<string, string> = {
  Open: '#0071e3', InProgress: '#ff9500', Closed: '#34c759',
};

export default function CompliancePage() {
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [selected, setSelected] = useState<ComplianceRequirement | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<ComplianceRequirement | null>(null);
  const [filterStatus, setFilterStatus] = useState<string | undefined>();
  const [filterCategory, setFilterCategory] = useState<string | undefined>();
  const [form] = Form.useForm();
  const queryClient = useQueryClient();

  const { data: requirements = [], isLoading } = useQuery({
    queryKey: ['complianceRequirements', filterStatus, filterCategory],
    queryFn: () => complianceApi.getRequirements(undefined, filterStatus, filterCategory),
  });

  const { data: mineSites = [] } = useQuery({
    queryKey: ['mineSites'],
    queryFn: () => mineSitesApi.getMineSites(),
  });

  const createMutation = useMutation({
    mutationFn: (data: CreateComplianceRequirementRequest) => complianceApi.createRequirement(data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['complianceRequirements'] }); setModalOpen(false); form.resetFields(); message.success('Requirement created'); },
  });

  const updateMutation = useMutation({
    mutationFn: (data: UpdateComplianceRequirementRequest) => complianceApi.updateRequirement(data.id, data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['complianceRequirements'] }); setModalOpen(false); setEditing(null); form.resetFields(); message.success('Requirement updated'); },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => complianceApi.deleteRequirement(id),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['complianceRequirements'] }); message.success('Requirement deleted'); },
  });

  const handleSubmit = async () => {
    const values = await form.validateFields();
    const payload = {
      ...values,
      dueDate: values.dueDate?.toISOString(),
      nextDueDate: values.nextDueDate?.toISOString(),
      lastCompletedDate: values.lastCompletedDate?.toISOString(),
    };
    if (editing) {
      updateMutation.mutate({ ...payload, id: editing.id });
    } else {
      createMutation.mutate(payload);
    }
  };

  const openEdit = (record: ComplianceRequirement) => {
    setEditing(record);
    form.setFieldsValue({
      ...record,
      dueDate: record.dueDate ? dayjs(record.dueDate) : undefined,
      nextDueDate: record.nextDueDate ? dayjs(record.nextDueDate) : undefined,
      lastCompletedDate: record.lastCompletedDate ? dayjs(record.lastCompletedDate) : undefined,
    });
    setModalOpen(true);
  };

  const columns = [
    { title: 'Code', dataIndex: 'code', key: 'code', width: 120 },
    { title: 'Title', dataIndex: 'title', key: 'title', width: 220, ellipsis: true },
    { title: 'Category', dataIndex: 'category', key: 'category', width: 130, render: (c: string) => <Tag color={categoryColors[c]}>{c}</Tag> },
    { title: 'Jurisdiction', dataIndex: 'jurisdiction', key: 'jurisdiction', width: 140, ellipsis: true },
    { title: 'Mine Site', dataIndex: 'mineSiteName', key: 'mineSiteName', width: 140 },
    { title: 'Frequency', dataIndex: 'frequency', key: 'frequency', width: 110 },
    { title: 'Priority', dataIndex: 'priority', key: 'priority', width: 100, render: (p: string) => <Tag color={priorityColors[p]}>{p}</Tag> },
    { title: 'Status', dataIndex: 'status', key: 'status', width: 140, render: (s: string) => <Tag color={statusColors[s]}>{s}</Tag> },
    { title: 'Audits', dataIndex: 'auditCount', key: 'auditCount', width: 70 },
    {
      title: 'Actions', key: 'actions', width: 130,
      render: (_: unknown, record: ComplianceRequirement) => (
        <Flex gap={4}>
          <Button type="text" size="small" icon={<EyeOutlined />} onClick={() => { setSelected(record); setDrawerOpen(true); }} />
          <Button type="text" size="small" icon={<EditOutlined />} onClick={() => openEdit(record)} />
          <Popconfirm title="Delete this requirement?" onConfirm={() => deleteMutation.mutate(record.id)} okText="Yes" cancelText="No">
            <Button type="text" size="small" danger icon={<DeleteOutlined />} />
          </Popconfirm>
        </Flex>
      ),
    },
  ];

  return (
    <div>
      <Flex justify="space-between" align="center" style={{ marginBottom: 24 }}>
        <div>
          <Title level={3} style={{ margin: 0, fontWeight: 700, letterSpacing: -0.5 }}>Compliance & Regulatory</Title>
          <Text style={{ color: '#86868b', fontSize: 13 }}>Manage compliance requirements, audits, and regulatory obligations</Text>
        </div>
      </Flex>

      <Flex justify="flex-end" gap={8} style={{ marginBottom: 16 }}>
        <Select placeholder="Filter by category" allowClear style={{ width: 180 }} onChange={setFilterCategory} options={categories.map(c => ({ label: c, value: c }))} />
        <Select placeholder="Filter by status" allowClear style={{ width: 180 }} onChange={setFilterStatus} options={statuses.map(s => ({ label: s, value: s }))} />
        <Button type="primary" icon={<PlusOutlined />} onClick={() => { setEditing(null); form.resetFields(); setModalOpen(true); }}>New Requirement</Button>
      </Flex>

      <Table dataSource={requirements} columns={columns} rowKey="id" loading={isLoading} size="middle"
        style={{ background: '#fff', borderRadius: 12, overflow: 'hidden' }}
        pagination={{ pageSize: 15, showSizeChanger: false }} />

      <Modal title={editing ? 'Edit Compliance Requirement' : 'New Compliance Requirement'} open={modalOpen} onCancel={() => { setModalOpen(false); setEditing(null); }}
        onOk={handleSubmit} width={720} okText={editing ? 'Update' : 'Create'}
        confirmLoading={createMutation.isPending || updateMutation.isPending}>
        <Form form={form} layout="vertical" style={{ maxHeight: 500, overflow: 'auto', paddingRight: 8 }}>
          <Flex gap={16}>
            <Form.Item name="mineSiteId" label="Mine Site" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select placeholder="Select mine site" disabled={!!editing} options={mineSites.map(m => ({ label: m.name, value: m.id }))} />
            </Form.Item>
            <Form.Item name="code" label="Code" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Input placeholder="e.g., MHSA-001" disabled={!!editing} />
            </Form.Item>
          </Flex>
          <Form.Item name="title" label="Title" rules={[{ required: true }]}>
            <Input placeholder="e.g., Monthly Safety Inspection Report" />
          </Form.Item>
          <Flex gap={16}>
            <Form.Item name="jurisdiction" label="Jurisdiction" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Input placeholder="e.g., Federal, State, Local" />
            </Form.Item>
            <Form.Item name="category" label="Category" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select options={categories.map(c => ({ label: c, value: c }))} />
            </Form.Item>
          </Flex>
          <Form.Item name="description" label="Description" rules={[{ required: true }]}>
            <TextArea rows={3} />
          </Form.Item>
          <Flex gap={16}>
            <Form.Item name="regulatoryBody" label="Regulatory Body" style={{ flex: 1 }}>
              <Input placeholder="e.g., MSHA, OSHA, EPA" />
            </Form.Item>
            <Form.Item name="referenceDocument" label="Reference Document" style={{ flex: 1 }}>
              <Input placeholder="e.g., 30 CFR Part 46" />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="frequency" label="Frequency" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select options={frequencies.map(f => ({ label: f, value: f }))} />
            </Form.Item>
            <Form.Item name="priority" label="Priority" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select options={priorities.map(p => ({ label: p, value: p }))} />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="dueDate" label="Due Date" style={{ flex: 1 }}>
              <DatePicker style={{ width: '100%' }} />
            </Form.Item>
            <Form.Item name="nextDueDate" label="Next Due Date" style={{ flex: 1 }}>
              <DatePicker style={{ width: '100%' }} />
            </Form.Item>
          </Flex>
          <Form.Item name="responsibleRole" label="Responsible Role">
            <Input placeholder="e.g., Safety Manager, Environmental Officer" />
          </Form.Item>
          <Form.Item name="penaltyForNonCompliance" label="Penalty for Non-Compliance">
            <TextArea rows={2} />
          </Form.Item>
          <Form.Item name="notes" label="Notes">
            <TextArea rows={2} />
          </Form.Item>
          {editing && (
            <>
              <Flex gap={16}>
                <Form.Item name="status" label="Status" style={{ flex: 1 }}>
                  <Select options={statuses.map(s => ({ label: s, value: s }))} />
                </Form.Item>
                <Form.Item name="isActive" label="Active" valuePropName="checked" style={{ flex: 1 }}>
                  <Switch />
                </Form.Item>
              </Flex>
              <Form.Item name="lastCompletedDate" label="Last Completed Date">
                <DatePicker style={{ width: '100%' }} />
              </Form.Item>
            </>
          )}
        </Form>
      </Modal>

      <Drawer title={selected?.title} open={drawerOpen} onClose={() => { setDrawerOpen(false); setSelected(null); }} width={800}>
        {selected && (
          <Tabs items={[
            { key: 'details', label: 'Details', children: <RequirementDetails requirement={selected} /> },
            { key: 'audits', label: `Audits (${selected.auditCount})`, children: <AuditsTab requirementId={selected.id} /> },
          ]} />
        )}
      </Drawer>
    </div>
  );
}

function RequirementDetails({ requirement }: { requirement: ComplianceRequirement }) {
  return (
    <Descriptions column={2} size="small" bordered>
      <Descriptions.Item label="Code">{requirement.code}</Descriptions.Item>
      <Descriptions.Item label="Status"><Tag color={statusColors[requirement.status]}>{requirement.status}</Tag></Descriptions.Item>
      <Descriptions.Item label="Title" span={2}>{requirement.title}</Descriptions.Item>
      <Descriptions.Item label="Category"><Tag color={categoryColors[requirement.category]}>{requirement.category}</Tag></Descriptions.Item>
      <Descriptions.Item label="Priority"><Tag color={priorityColors[requirement.priority]}>{requirement.priority}</Tag></Descriptions.Item>
      <Descriptions.Item label="Jurisdiction">{requirement.jurisdiction}</Descriptions.Item>
      <Descriptions.Item label="Mine Site">{requirement.mineSiteName}</Descriptions.Item>
      <Descriptions.Item label="Description" span={2}>{requirement.description}</Descriptions.Item>
      {requirement.regulatoryBody && <Descriptions.Item label="Regulatory Body">{requirement.regulatoryBody}</Descriptions.Item>}
      {requirement.referenceDocument && <Descriptions.Item label="Reference Document">{requirement.referenceDocument}</Descriptions.Item>}
      <Descriptions.Item label="Frequency">{requirement.frequency}</Descriptions.Item>
      {requirement.responsibleRole && <Descriptions.Item label="Responsible Role">{requirement.responsibleRole}</Descriptions.Item>}
      {requirement.dueDate && <Descriptions.Item label="Due Date">{dayjs(requirement.dueDate).format('DD/MM/YYYY')}</Descriptions.Item>}
      {requirement.nextDueDate && <Descriptions.Item label="Next Due Date">{dayjs(requirement.nextDueDate).format('DD/MM/YYYY')}</Descriptions.Item>}
      {requirement.lastCompletedDate && <Descriptions.Item label="Last Completed">{dayjs(requirement.lastCompletedDate).format('DD/MM/YYYY')}</Descriptions.Item>}
      <Descriptions.Item label="Active">{requirement.isActive ? 'Yes' : 'No'}</Descriptions.Item>
      {requirement.penaltyForNonCompliance && <Descriptions.Item label="Penalty" span={2}>{requirement.penaltyForNonCompliance}</Descriptions.Item>}
      {requirement.notes && <Descriptions.Item label="Notes" span={2}>{requirement.notes}</Descriptions.Item>}
      <Descriptions.Item label="Created">{dayjs(requirement.createdAt).format('DD/MM/YYYY HH:mm')}</Descriptions.Item>
    </Descriptions>
  );
}

function AuditsTab({ requirementId }: { requirementId: string }) {
  const [modalOpen, setModalOpen] = useState(false);
  const [form] = Form.useForm();
  const queryClient = useQueryClient();

  const { data: audits = [], isLoading } = useQuery({
    queryKey: ['complianceAudits', requirementId],
    queryFn: () => complianceApi.getAudits(requirementId),
  });

  const createMutation = useMutation({
    mutationFn: (data: CreateComplianceAuditRequest) => complianceApi.createAudit(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['complianceAudits', requirementId] });
      queryClient.invalidateQueries({ queryKey: ['complianceRequirements'] });
      setModalOpen(false);
      form.resetFields();
      message.success('Audit created');
    },
  });

  const handleSubmit = async () => {
    const values = await form.validateFields();
    createMutation.mutate({
      ...values,
      complianceRequirementId: requirementId,
      auditDate: values.auditDate?.toISOString(),
      actionDueDate: values.actionDueDate?.toISOString(),
    });
  };

  const columns = [
    { title: 'Audit #', dataIndex: 'auditNumber', key: 'auditNumber', width: 110 },
    { title: 'Date', dataIndex: 'auditDate', key: 'auditDate', width: 110, render: (d: string) => dayjs(d).format('DD/MM/YYYY') },
    { title: 'Auditor', dataIndex: 'auditorName', key: 'auditorName', width: 140 },
    { title: 'Type', dataIndex: 'auditType', key: 'auditType', width: 120, render: (t: string) => <Tag color={auditTypeColors[t]}>{t}</Tag> },
    { title: 'Compliance', dataIndex: 'complianceStatus', key: 'complianceStatus', width: 140, render: (s: string) => <Tag color={complianceStatusColors[s]}>{s}</Tag> },
    { title: 'Status', dataIndex: 'status', key: 'status', width: 100, render: (s: string) => <Tag color={auditStatusColors[s]}>{s}</Tag> },
    { title: 'Findings', dataIndex: 'findings', key: 'findings', ellipsis: true },
  ];

  return (
    <>
      <Flex justify="flex-end" style={{ marginBottom: 16 }}>
        <Button type="primary" icon={<PlusOutlined />} onClick={() => { form.resetFields(); setModalOpen(true); }}>New Audit</Button>
      </Flex>

      <Table dataSource={audits} columns={columns} rowKey="id" loading={isLoading} size="small"
        pagination={{ pageSize: 10, showSizeChanger: false }} />

      <Modal title="New Compliance Audit" open={modalOpen} onCancel={() => setModalOpen(false)}
        onOk={handleSubmit} width={640} okText="Create" confirmLoading={createMutation.isPending}>
        <Form form={form} layout="vertical" style={{ maxHeight: 450, overflow: 'auto', paddingRight: 8 }}>
          <Flex gap={16}>
            <Form.Item name="auditDate" label="Audit Date" rules={[{ required: true }]} style={{ flex: 1 }}>
              <DatePicker style={{ width: '100%' }} />
            </Form.Item>
            <Form.Item name="auditType" label="Audit Type" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select options={auditTypes.map(t => ({ label: t, value: t }))} />
            </Form.Item>
          </Flex>
          <Form.Item name="auditorName" label="Auditor Name" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item name="findings" label="Findings" rules={[{ required: true }]}>
            <TextArea rows={3} />
          </Form.Item>
          <Form.Item name="complianceStatus" label="Compliance Status" rules={[{ required: true }]}>
            <Select options={complianceStatuses.map(s => ({ label: s, value: s }))} />
          </Form.Item>
          <Form.Item name="correctiveActions" label="Corrective Actions">
            <TextArea rows={2} />
          </Form.Item>
          <Form.Item name="actionDueDate" label="Action Due Date">
            <DatePicker style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item name="evidenceReferences" label="Evidence References">
            <Input placeholder="e.g., DOC-001, PHOTO-002" />
          </Form.Item>
          <Form.Item name="notes" label="Notes">
            <TextArea rows={2} />
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
}
