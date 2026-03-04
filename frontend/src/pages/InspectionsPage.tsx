import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Table, Button, Modal, Form, Input, Select, DatePicker, Tag, Drawer,
  Typography, Flex, InputNumber, message, Popconfirm, Descriptions, Tabs, Badge,
} from 'antd';
import {
  PlusOutlined, EyeOutlined, EditOutlined, DeleteOutlined,
} from '@ant-design/icons';
import dayjs from 'dayjs';
import { inspectionApi } from '../api/inspectionApi';
import { mineSitesApi } from '../api/mineSitesApi';
import type { InspectionTemplate, Inspection, InspectionFinding } from '../types';
import type {
  CreateInspectionTemplateRequest, UpdateInspectionTemplateRequest,
  CreateInspectionRequest, UpdateInspectionRequest,
  CreateFindingRequest, UpdateFindingRequest,
} from '../api/inspectionApi';

const { Title, Text } = Typography;
const { TextArea } = Input;

const categories = ['Safety', 'Environmental', 'Equipment', 'Workplace', 'Regulatory', 'Electrical', 'Ventilation', 'Other'];
const frequencies = ['Daily', 'Weekly', 'Fortnightly', 'Monthly', 'Quarterly', 'Annually', 'AdHoc'];
const inspStatuses = ['Scheduled', 'InProgress', 'Completed', 'Overdue', 'Cancelled'];
const ratings = ['Compliant', 'PartiallyCompliant', 'NonCompliant', 'NotApplicable'];
const findingSeverities = ['Critical', 'High', 'Medium', 'Low'];
const findingCategories = ['Safety', 'Environmental', 'Equipment', 'Housekeeping', 'PPE', 'Procedure', 'Structural', 'Other'];
const findingStatuses = ['Open', 'InProgress', 'Completed', 'Overdue', 'Closed'];

const statusColors: Record<string, string> = { Scheduled: '#0071e3', InProgress: '#ff9500', Completed: '#34c759', Overdue: '#ff3b30', Cancelled: '#86868b' };
const severityColors: Record<string, string> = { Critical: '#ff3b30', High: '#ff9500', Medium: '#ffcc00', Low: '#34c759' };
const ratingColors: Record<string, string> = { Compliant: '#34c759', PartiallyCompliant: '#ff9500', NonCompliant: '#ff3b30', NotApplicable: '#86868b' };

export default function InspectionsPage() {
  const [activeTab, setActiveTab] = useState('inspections');
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<Inspection | null>(null);
  const [tplModalOpen, setTplModalOpen] = useState(false);
  const [editingTpl, setEditingTpl] = useState<InspectionTemplate | null>(null);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [selected, setSelected] = useState<Inspection | null>(null);
  const [findingModalOpen, setFindingModalOpen] = useState(false);
  const [editingFinding, setEditingFinding] = useState<InspectionFinding | null>(null);
  const [filterStatus, setFilterStatus] = useState<string | undefined>();
  const [form] = Form.useForm();
  const [tplForm] = Form.useForm();
  const [findingForm] = Form.useForm();
  const qc = useQueryClient();

  const { data: inspections = [], isLoading } = useQuery({
    queryKey: ['inspections', filterStatus],
    queryFn: () => inspectionApi.getAll(undefined, undefined, filterStatus),
  });

  const { data: templates = [], isLoading: tplLoading } = useQuery({
    queryKey: ['inspectionTemplates'],
    queryFn: () => inspectionApi.getTemplates(),
  });

  const { data: mineSites = [] } = useQuery({
    queryKey: ['mineSites'],
    queryFn: () => mineSitesApi.getMineSites(),
  });

  const { data: findings = [] } = useQuery({
    queryKey: ['inspectionFindings', selected?.id],
    queryFn: () => inspectionApi.getFindings(selected!.id),
    enabled: !!selected,
  });

  // Template mutations
  const createTplMut = useMutation({
    mutationFn: (d: CreateInspectionTemplateRequest) => inspectionApi.createTemplate(d),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['inspectionTemplates'] }); setTplModalOpen(false); tplForm.resetFields(); message.success('Template created'); },
  });
  const updateTplMut = useMutation({
    mutationFn: (d: UpdateInspectionTemplateRequest) => inspectionApi.updateTemplate(d.id, d),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['inspectionTemplates'] }); setTplModalOpen(false); setEditingTpl(null); tplForm.resetFields(); message.success('Template updated'); },
  });
  const deleteTplMut = useMutation({
    mutationFn: (id: string) => inspectionApi.deleteTemplate(id),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['inspectionTemplates'] }); message.success('Template deleted'); },
  });

  // Inspection mutations
  const createMut = useMutation({
    mutationFn: (d: CreateInspectionRequest) => inspectionApi.create(d),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['inspections'] }); setModalOpen(false); form.resetFields(); message.success('Inspection created'); },
  });
  const updateMut = useMutation({
    mutationFn: (d: UpdateInspectionRequest) => inspectionApi.update(d.id, d),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['inspections'] }); setModalOpen(false); setEditing(null); form.resetFields(); message.success('Inspection updated'); },
  });
  const deleteMut = useMutation({
    mutationFn: (id: string) => inspectionApi.delete(id),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['inspections'] }); message.success('Inspection deleted'); },
  });

  // Finding mutations
  const createFindingMut = useMutation({
    mutationFn: (d: CreateFindingRequest) => inspectionApi.createFinding(d.inspectionId, d),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['inspectionFindings'] }); qc.invalidateQueries({ queryKey: ['inspections'] }); setFindingModalOpen(false); findingForm.resetFields(); message.success('Finding added'); },
  });
  const updateFindingMut = useMutation({
    mutationFn: (d: UpdateFindingRequest) => inspectionApi.updateFinding(d.id, d),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['inspectionFindings'] }); setFindingModalOpen(false); setEditingFinding(null); findingForm.resetFields(); message.success('Finding updated'); },
  });

  const handleSubmit = async () => {
    const values = await form.validateFields();
    const payload = {
      ...values,
      scheduledDate: values.scheduledDate?.toISOString(),
      completedDate: values.completedDate?.toISOString(),
      signedOffAt: values.signedOffAt?.toISOString(),
    };
    if (editing) {
      updateMut.mutate({ ...payload, id: editing.id });
    } else {
      createMut.mutate(payload);
    }
  };

  const handleTplSubmit = async () => {
    const values = await tplForm.validateFields();
    if (editingTpl) {
      updateTplMut.mutate({ ...values, id: editingTpl.id });
    } else {
      createTplMut.mutate(values);
    }
  };

  const handleFindingSubmit = async () => {
    const values = await findingForm.validateFields();
    const payload = {
      ...values,
      actionDueDate: values.actionDueDate?.toISOString(),
      actionCompletedDate: values.actionCompletedDate?.toISOString(),
    };
    if (editingFinding) {
      updateFindingMut.mutate({ ...payload, id: editingFinding.id });
    } else {
      createFindingMut.mutate({ ...payload, inspectionId: selected!.id });
    }
  };

  const openEdit = (record: Inspection) => {
    setEditing(record);
    form.setFieldsValue({
      ...record,
      scheduledDate: dayjs(record.scheduledDate),
      completedDate: record.completedDate ? dayjs(record.completedDate) : undefined,
      signedOffAt: record.signedOffAt ? dayjs(record.signedOffAt) : undefined,
    });
    setModalOpen(true);
  };

  const openEditTpl = (record: InspectionTemplate) => {
    setEditingTpl(record);
    tplForm.setFieldsValue(record);
    setTplModalOpen(true);
  };

  const openEditFinding = (record: InspectionFinding) => {
    setEditingFinding(record);
    findingForm.setFieldsValue({
      ...record,
      actionDueDate: record.actionDueDate ? dayjs(record.actionDueDate) : undefined,
      actionCompletedDate: record.actionCompletedDate ? dayjs(record.actionCompletedDate) : undefined,
    });
    setFindingModalOpen(true);
  };

  const inspColumns = [
    { title: 'Inspection #', dataIndex: 'inspectionNumber', key: 'inspectionNumber', width: 120 },
    { title: 'Title', dataIndex: 'title', key: 'title', ellipsis: true },
    { title: 'Template', dataIndex: 'templateName', key: 'templateName', width: 140 },
    { title: 'Mine Site', dataIndex: 'mineSiteName', key: 'mineSiteName', width: 140 },
    { title: 'Scheduled', dataIndex: 'scheduledDate', key: 'scheduledDate', width: 110, render: (d: string) => dayjs(d).format('DD/MM/YYYY') },
    { title: 'Inspector', dataIndex: 'inspectorName', key: 'inspectorName', width: 130 },
    { title: 'Status', dataIndex: 'status', key: 'status', width: 120, render: (s: string) => <Tag color={statusColors[s]}>{s}</Tag> },
    { title: 'Rating', dataIndex: 'overallRating', key: 'overallRating', width: 130, render: (r: string | null) => r ? <Tag color={ratingColors[r]}>{r}</Tag> : '-' },
    { title: 'Findings', dataIndex: 'findingCount', key: 'findingCount', width: 80, render: (c: number) => <Badge count={c} showZero color="#0071e3" /> },
    {
      title: 'Actions', key: 'actions', width: 130,
      render: (_: unknown, record: Inspection) => (
        <Flex gap={4}>
          <Button type="text" size="small" icon={<EyeOutlined />} onClick={() => { setSelected(record); setDrawerOpen(true); }} />
          <Button type="text" size="small" icon={<EditOutlined />} onClick={() => openEdit(record)} />
          <Popconfirm title="Delete this inspection?" onConfirm={() => deleteMut.mutate(record.id)}>
            <Button type="text" size="small" danger icon={<DeleteOutlined />} />
          </Popconfirm>
        </Flex>
      ),
    },
  ];

  const tplColumns = [
    { title: 'Name', dataIndex: 'name', key: 'name' },
    { title: 'Code', dataIndex: 'code', key: 'code', width: 100 },
    { title: 'Category', dataIndex: 'category', key: 'category', width: 120 },
    { title: 'Frequency', dataIndex: 'frequency', key: 'frequency', width: 110 },
    { title: 'Active', dataIndex: 'isActive', key: 'isActive', width: 80, render: (v: boolean) => <Tag color={v ? '#34c759' : '#ff3b30'}>{v ? 'Yes' : 'No'}</Tag> },
    { title: 'Inspections', dataIndex: 'inspectionCount', key: 'inspectionCount', width: 100 },
    {
      title: 'Actions', key: 'actions', width: 130,
      render: (_: unknown, record: InspectionTemplate) => (
        <Flex gap={4}>
          <Button type="text" size="small" icon={<EditOutlined />} onClick={() => openEditTpl(record)} />
          <Popconfirm title="Delete this template?" onConfirm={() => deleteTplMut.mutate(record.id)}>
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
          <Title level={3} style={{ margin: 0, fontWeight: 700, letterSpacing: -0.5 }}>Inspection & Audit Management</Title>
          <Text style={{ color: '#86868b', fontSize: 13 }}>Schedule, conduct, and track inspections with findings</Text>
        </div>
      </Flex>

      <Tabs activeKey={activeTab} onChange={setActiveTab} items={[
        {
          key: 'inspections', label: 'Inspections',
          children: (
            <>
              <Flex gap={8} style={{ marginBottom: 16 }} justify="flex-end">
                <Select placeholder="Filter by status" allowClear style={{ width: 180 }} onChange={setFilterStatus} options={inspStatuses.map(s => ({ label: s, value: s }))} />
                <Button type="primary" icon={<PlusOutlined />} onClick={() => { setEditing(null); form.resetFields(); setModalOpen(true); }}>New Inspection</Button>
              </Flex>
              <Table dataSource={inspections} columns={inspColumns} rowKey="id" loading={isLoading} size="middle"
                style={{ background: '#fff', borderRadius: 12, overflow: 'hidden' }}
                pagination={{ pageSize: 15, showSizeChanger: false }} />
            </>
          ),
        },
        {
          key: 'templates', label: 'Templates',
          children: (
            <>
              <Flex gap={8} style={{ marginBottom: 16 }} justify="flex-end">
                <Button type="primary" icon={<PlusOutlined />} onClick={() => { setEditingTpl(null); tplForm.resetFields(); setTplModalOpen(true); }}>New Template</Button>
              </Flex>
              <Table dataSource={templates} columns={tplColumns} rowKey="id" loading={tplLoading} size="middle"
                style={{ background: '#fff', borderRadius: 12, overflow: 'hidden' }}
                pagination={{ pageSize: 15, showSizeChanger: false }} />
            </>
          ),
        },
      ]} />

      {/* Inspection Create/Edit Modal */}
      <Modal title={editing ? 'Edit Inspection' : 'Schedule New Inspection'} open={modalOpen} onCancel={() => { setModalOpen(false); setEditing(null); }}
        onOk={handleSubmit} width={720} okText={editing ? 'Update' : 'Create'}
        confirmLoading={createMut.isPending || updateMut.isPending}>
        <Form form={form} layout="vertical" style={{ maxHeight: 500, overflow: 'auto', paddingRight: 8 }}>
          <Flex gap={16}>
            <Form.Item name="inspectionTemplateId" label="Template" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select placeholder="Select template" disabled={!!editing} options={templates.map(t => ({ label: `${t.name} (${t.code})`, value: t.id }))} />
            </Form.Item>
            <Form.Item name="mineSiteId" label="Mine Site" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select placeholder="Select mine site" disabled={!!editing} options={mineSites.map(m => ({ label: m.name, value: m.id }))} />
            </Form.Item>
          </Flex>
          <Form.Item name="title" label="Title" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Flex gap={16}>
            <Form.Item name="scheduledDate" label="Scheduled Date" rules={[{ required: true }]} style={{ flex: 1 }}>
              <DatePicker style={{ width: '100%' }} />
            </Form.Item>
            <Form.Item name="inspectorName" label="Inspector" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Input />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="inspectorRole" label="Inspector Role" style={{ flex: 1 }}>
              <Input />
            </Form.Item>
            <Form.Item name="personnelPresent" label="Personnel Present" style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} min={0} />
            </Form.Item>
          </Flex>
          <Form.Item name="weatherConditions" label="Weather Conditions">
            <Input />
          </Form.Item>
          {editing && (
            <>
              <Flex gap={16}>
                <Form.Item name="status" label="Status" style={{ flex: 1 }}>
                  <Select options={inspStatuses.map(s => ({ label: s, value: s }))} />
                </Form.Item>
                <Form.Item name="overallRating" label="Overall Rating" style={{ flex: 1 }}>
                  <Select allowClear options={ratings.map(r => ({ label: r, value: r }))} />
                </Form.Item>
              </Flex>
              <Form.Item name="summary" label="Summary">
                <TextArea rows={3} />
              </Form.Item>
              <Flex gap={16}>
                <Form.Item name="completedDate" label="Completed Date" style={{ flex: 1 }}>
                  <DatePicker style={{ width: '100%' }} />
                </Form.Item>
                <Form.Item name="signedOffBy" label="Signed Off By" style={{ flex: 1 }}>
                  <Input />
                </Form.Item>
              </Flex>
            </>
          )}
        </Form>
      </Modal>

      {/* Template Create/Edit Modal */}
      <Modal title={editingTpl ? 'Edit Template' : 'Create Template'} open={tplModalOpen} onCancel={() => { setTplModalOpen(false); setEditingTpl(null); }}
        onOk={handleTplSubmit} width={600} okText={editingTpl ? 'Update' : 'Create'}
        confirmLoading={createTplMut.isPending || updateTplMut.isPending}>
        <Form form={tplForm} layout="vertical">
          <Flex gap={16}>
            <Form.Item name="name" label="Name" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Input />
            </Form.Item>
            <Form.Item name="code" label="Code" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Input />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="category" label="Category" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select options={categories.map(c => ({ label: c, value: c }))} />
            </Form.Item>
            <Form.Item name="frequency" label="Frequency" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select options={frequencies.map(f => ({ label: f, value: f }))} />
            </Form.Item>
          </Flex>
          <Form.Item name="description" label="Description">
            <TextArea rows={2} />
          </Form.Item>
          <Form.Item name="sortOrder" label="Sort Order" initialValue={0}>
            <InputNumber style={{ width: '100%' }} min={0} />
          </Form.Item>
          {editingTpl && (
            <Form.Item name="isActive" label="Active" valuePropName="checked" initialValue={true}>
              <Select options={[{ label: 'Yes', value: true }, { label: 'No', value: false }]} />
            </Form.Item>
          )}
        </Form>
      </Modal>

      {/* Inspection Detail Drawer */}
      <Drawer title={selected?.title} open={drawerOpen} onClose={() => { setDrawerOpen(false); setSelected(null); }} width={700}>
        {selected && (
          <Tabs items={[
            {
              key: 'details', label: 'Details',
              children: (
                <Descriptions column={2} size="small" bordered>
                  <Descriptions.Item label="Inspection #">{selected.inspectionNumber}</Descriptions.Item>
                  <Descriptions.Item label="Status"><Tag color={statusColors[selected.status]}>{selected.status}</Tag></Descriptions.Item>
                  <Descriptions.Item label="Template">{selected.templateName}</Descriptions.Item>
                  <Descriptions.Item label="Mine Site">{selected.mineSiteName}</Descriptions.Item>
                  {selected.mineAreaName && <Descriptions.Item label="Mine Area">{selected.mineAreaName}</Descriptions.Item>}
                  <Descriptions.Item label="Scheduled">{dayjs(selected.scheduledDate).format('DD/MM/YYYY')}</Descriptions.Item>
                  {selected.completedDate && <Descriptions.Item label="Completed">{dayjs(selected.completedDate).format('DD/MM/YYYY')}</Descriptions.Item>}
                  <Descriptions.Item label="Inspector">{selected.inspectorName}</Descriptions.Item>
                  {selected.overallRating && <Descriptions.Item label="Rating"><Tag color={ratingColors[selected.overallRating]}>{selected.overallRating}</Tag></Descriptions.Item>}
                  {selected.weatherConditions && <Descriptions.Item label="Weather">{selected.weatherConditions}</Descriptions.Item>}
                  {selected.personnelPresent != null && <Descriptions.Item label="Personnel">{selected.personnelPresent}</Descriptions.Item>}
                  {selected.summary && <Descriptions.Item label="Summary" span={2}>{selected.summary}</Descriptions.Item>}
                  {selected.signedOffBy && <Descriptions.Item label="Signed Off By">{selected.signedOffBy}</Descriptions.Item>}
                </Descriptions>
              ),
            },
            {
              key: 'findings', label: `Findings (${findings.length})`,
              children: (
                <div>
                  <Button type="primary" size="small" icon={<PlusOutlined />} style={{ marginBottom: 12 }}
                    onClick={() => { setEditingFinding(null); findingForm.resetFields(); setFindingModalOpen(true); }}>Add Finding</Button>
                  {findings.map((f: InspectionFinding) => (
                    <div key={f.id} style={{ background: '#f8f9fa', padding: 16, borderRadius: 8, marginBottom: 8 }}>
                      <Flex justify="space-between" align="center" style={{ marginBottom: 8 }}>
                        <Flex gap={8} align="center">
                          <Text strong>{f.findingNumber}</Text>
                          <Tag color={severityColors[f.severity]}>{f.severity}</Tag>
                          <Tag>{f.category}</Tag>
                        </Flex>
                        <Flex gap={4}>
                          <Tag color={statusColors[f.status] || '#86868b'}>{f.status}</Tag>
                          <Button type="text" size="small" icon={<EditOutlined />} onClick={() => openEditFinding(f)} />
                        </Flex>
                      </Flex>
                      <Text>{f.description}</Text>
                      {f.assignedTo && <div style={{ marginTop: 4 }}><Text style={{ fontSize: 12, color: '#86868b' }}>Assigned to: {f.assignedTo}</Text></div>}
                      {f.recommendedAction && <div style={{ marginTop: 4 }}><Text style={{ fontSize: 12, color: '#0071e3' }}>Action: {f.recommendedAction}</Text></div>}
                    </div>
                  ))}
                </div>
              ),
            },
          ]} />
        )}
      </Drawer>

      {/* Finding Create/Edit Modal */}
      <Modal title={editingFinding ? 'Edit Finding' : 'Add Finding'} open={findingModalOpen} onCancel={() => { setFindingModalOpen(false); setEditingFinding(null); }}
        onOk={handleFindingSubmit} confirmLoading={createFindingMut.isPending || updateFindingMut.isPending}>
        <Form form={findingForm} layout="vertical">
          <Flex gap={16}>
            <Form.Item name="category" label="Category" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select options={findingCategories.map(c => ({ label: c, value: c }))} />
            </Form.Item>
            <Form.Item name="severity" label="Severity" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select options={findingSeverities.map(s => ({ label: s, value: s }))} />
            </Form.Item>
          </Flex>
          <Form.Item name="description" label="Description" rules={[{ required: true }]}>
            <TextArea rows={3} />
          </Form.Item>
          <Form.Item name="location" label="Location">
            <Input />
          </Form.Item>
          <Form.Item name="recommendedAction" label="Recommended Action">
            <TextArea rows={2} />
          </Form.Item>
          <Flex gap={16}>
            <Form.Item name="assignedTo" label="Assigned To" style={{ flex: 1 }}>
              <Input />
            </Form.Item>
            <Form.Item name="actionDueDate" label="Action Due Date" style={{ flex: 1 }}>
              <DatePicker style={{ width: '100%' }} />
            </Form.Item>
          </Flex>
          {editingFinding && (
            <>
              <Flex gap={16}>
                <Form.Item name="status" label="Status" style={{ flex: 1 }}>
                  <Select options={findingStatuses.map(s => ({ label: s, value: s }))} />
                </Form.Item>
                <Form.Item name="actionCompletedDate" label="Completed Date" style={{ flex: 1 }}>
                  <DatePicker style={{ width: '100%' }} />
                </Form.Item>
              </Flex>
              <Form.Item name="closureNotes" label="Closure Notes">
                <TextArea rows={2} />
              </Form.Item>
            </>
          )}
        </Form>
      </Modal>
    </div>
  );
}
