import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Table, Button, Modal, Form, Input, Select, DatePicker, Tag, Drawer,
  Typography, Flex, Switch, InputNumber, message, Popconfirm, Descriptions, Tabs,
} from 'antd';
import { PlusOutlined, EyeOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import { safetyIncidentApi } from '../api/safetyIncidentApi';
import { mineSitesApi } from '../api/mineSitesApi';
import type { SafetyIncident, IncidentInvestigation } from '../types';
import type { CreateSafetyIncidentRequest, UpdateSafetyIncidentRequest, CreateInvestigationRequest } from '../api/safetyIncidentApi';

const { Title, Text } = Typography;
const { TextArea } = Input;

const incidentTypes = ['Injury', 'NearMiss', 'PropertyDamage', 'EnvironmentalRelease', 'FireExplosion', 'ElectricalIncident', 'VehicleIncident', 'FallOfGround', 'Other'];
const severities = ['Critical', 'High', 'Medium', 'Low'];
const statuses = ['Open', 'UnderInvestigation', 'ActionRequired', 'Closed', 'ReopenedForReview'];
const injuryTypes = ['Fatality', 'LostTimeInjury', 'MedicalTreatment', 'FirstAid', 'NoInjury'];
const methodologies = ['FiveWhys', 'FishboneDiagram', 'TapRooT', 'BowTie', 'FaultTreeAnalysis', 'IncidentCauseAnalysis', 'Other'];

const severityColors: Record<string, string> = { Critical: '#ff3b30', High: '#ff9500', Medium: '#ffcc00', Low: '#34c759' };
const statusColors: Record<string, string> = { Open: '#0071e3', UnderInvestigation: '#ff9500', ActionRequired: '#ff3b30', Closed: '#34c759', ReopenedForReview: '#af52de' };

export default function SafetyIncidentsPage() {
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<SafetyIncident | null>(null);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [selected, setSelected] = useState<SafetyIncident | null>(null);
  const [invModalOpen, setInvModalOpen] = useState(false);
  const [filterStatus, setFilterStatus] = useState<string | undefined>();
  const [form] = Form.useForm();
  const [invForm] = Form.useForm();
  const queryClient = useQueryClient();

  const { data: incidents = [], isLoading } = useQuery({
    queryKey: ['safetyIncidents', filterStatus],
    queryFn: () => safetyIncidentApi.getAll(undefined, filterStatus),
  });

  const { data: mineSites = [] } = useQuery({
    queryKey: ['mineSites'],
    queryFn: () => mineSitesApi.getMineSites(),
  });

  const { data: investigations = [] } = useQuery({
    queryKey: ['investigations', selected?.id],
    queryFn: () => safetyIncidentApi.getInvestigations(selected!.id),
    enabled: !!selected,
  });

  const createMutation = useMutation({
    mutationFn: (data: CreateSafetyIncidentRequest) => safetyIncidentApi.create(data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['safetyIncidents'] }); setModalOpen(false); form.resetFields(); message.success('Incident reported'); },
  });

  const updateMutation = useMutation({
    mutationFn: (data: UpdateSafetyIncidentRequest) => safetyIncidentApi.update(data.id, data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['safetyIncidents'] }); setModalOpen(false); setEditing(null); form.resetFields(); message.success('Incident updated'); },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => safetyIncidentApi.delete(id),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['safetyIncidents'] }); message.success('Incident deleted'); },
  });

  const invMutation = useMutation({
    mutationFn: (data: CreateInvestigationRequest) => safetyIncidentApi.createInvestigation(data.safetyIncidentId, data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['investigations'] }); queryClient.invalidateQueries({ queryKey: ['safetyIncidents'] }); setInvModalOpen(false); invForm.resetFields(); message.success('Investigation added'); },
  });

  const handleSubmit = async () => {
    const values = await form.validateFields();
    const payload = {
      ...values,
      incidentDateTime: values.incidentDateTime?.toISOString(),
      correctiveActionDueDate: values.correctiveActionDueDate?.toISOString(),
      correctiveActionCompletedDate: values.correctiveActionCompletedDate?.toISOString(),
    };
    if (editing) {
      updateMutation.mutate({ ...payload, id: editing.id });
    } else {
      createMutation.mutate(payload);
    }
  };

  const handleInvSubmit = async () => {
    const values = await invForm.validateFields();
    invMutation.mutate({
      ...values,
      safetyIncidentId: selected!.id,
      investigationDate: values.investigationDate?.toISOString(),
    });
  };

  const openEdit = (record: SafetyIncident) => {
    setEditing(record);
    form.setFieldsValue({
      ...record,
      incidentDateTime: dayjs(record.incidentDateTime),
      correctiveActionDueDate: record.correctiveActionDueDate ? dayjs(record.correctiveActionDueDate) : undefined,
      correctiveActionCompletedDate: record.correctiveActionCompletedDate ? dayjs(record.correctiveActionCompletedDate) : undefined,
    });
    setModalOpen(true);
  };

  const columns = [
    { title: 'Incident #', dataIndex: 'incidentNumber', key: 'incidentNumber', width: 120 },
    { title: 'Title', dataIndex: 'title', key: 'title', ellipsis: true },
    { title: 'Type', dataIndex: 'incidentType', key: 'incidentType', width: 140 },
    { title: 'Severity', dataIndex: 'severity', key: 'severity', width: 100, render: (s: string) => <Tag color={severityColors[s]}>{s}</Tag> },
    { title: 'Mine Site', dataIndex: 'mineSiteName', key: 'mineSiteName', width: 150 },
    { title: 'Date', dataIndex: 'incidentDateTime', key: 'incidentDateTime', width: 110, render: (d: string) => dayjs(d).format('DD/MM/YYYY') },
    { title: 'Status', dataIndex: 'status', key: 'status', width: 140, render: (s: string) => <Tag color={statusColors[s]}>{s}</Tag> },
    { title: 'Reported By', dataIndex: 'reportedBy', key: 'reportedBy', width: 130 },
    {
      title: 'Actions', key: 'actions', width: 130,
      render: (_: unknown, record: SafetyIncident) => (
        <Flex gap={4}>
          <Button type="text" size="small" icon={<EyeOutlined />} onClick={() => { setSelected(record); setDrawerOpen(true); }} />
          <Button type="text" size="small" icon={<EditOutlined />} onClick={() => openEdit(record)} />
          <Popconfirm title="Delete this incident?" onConfirm={() => deleteMutation.mutate(record.id)}>
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
          <Title level={3} style={{ margin: 0, fontWeight: 700, letterSpacing: -0.5 }}>Safety & Incident Management</Title>
          <Text style={{ color: '#86868b', fontSize: 13 }}>Report, track, and investigate safety incidents</Text>
        </div>
        <Flex gap={8}>
          <Select placeholder="Filter by status" allowClear style={{ width: 180 }} onChange={setFilterStatus} options={statuses.map(s => ({ label: s, value: s }))} />
          <Button type="primary" icon={<PlusOutlined />} onClick={() => { setEditing(null); form.resetFields(); setModalOpen(true); }}>Report Incident</Button>
        </Flex>
      </Flex>

      <Table dataSource={incidents} columns={columns} rowKey="id" loading={isLoading} size="middle"
        style={{ background: '#fff', borderRadius: 12, overflow: 'hidden' }}
        pagination={{ pageSize: 15, showSizeChanger: false }} />

      {/* Create/Edit Modal */}
      <Modal title={editing ? 'Edit Incident' : 'Report New Incident'} open={modalOpen} onCancel={() => { setModalOpen(false); setEditing(null); }}
        onOk={handleSubmit} width={720} okText={editing ? 'Update' : 'Submit Report'}
        confirmLoading={createMutation.isPending || updateMutation.isPending}>
        <Form form={form} layout="vertical" style={{ maxHeight: 500, overflow: 'auto', paddingRight: 8 }}>
          <Flex gap={16}>
            <Form.Item name="mineSiteId" label="Mine Site" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select placeholder="Select mine site" disabled={!!editing} options={mineSites.map(m => ({ label: m.name, value: m.id }))} />
            </Form.Item>
            <Form.Item name="incidentType" label="Incident Type" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select options={incidentTypes.map(t => ({ label: t, value: t }))} />
            </Form.Item>
          </Flex>
          <Form.Item name="title" label="Title" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Flex gap={16}>
            <Form.Item name="severity" label="Severity" style={{ flex: 1 }}>
              <Select options={severities.map(s => ({ label: s, value: s }))} />
            </Form.Item>
            <Form.Item name="incidentDateTime" label="Date & Time" rules={[{ required: true }]} style={{ flex: 1 }}>
              <DatePicker showTime style={{ width: '100%' }} />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="location" label="Location" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Input />
            </Form.Item>
            <Form.Item name="reportedBy" label="Reported By" rules={[{ required: !editing }]} style={{ flex: 1 }}>
              <Input disabled={!!editing} />
            </Form.Item>
          </Flex>
          <Form.Item name="description" label="Description" rules={[{ required: true }]}>
            <TextArea rows={3} />
          </Form.Item>
          <Form.Item name="immediateActions" label="Immediate Actions Taken">
            <TextArea rows={2} />
          </Form.Item>
          <Flex gap={16}>
            <Form.Item name="injuredPersonName" label="Injured Person" style={{ flex: 1 }}>
              <Input />
            </Form.Item>
            <Form.Item name="injuryType" label="Injury Type" style={{ flex: 1 }}>
              <Select allowClear options={injuryTypes.map(t => ({ label: t, value: t }))} />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="lostTimeDays" label="Lost Time (days)" style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} min={0} />
            </Form.Item>
            <Form.Item name="isReportable" label="Reportable?" valuePropName="checked" style={{ flex: 1 }}>
              <Switch />
            </Form.Item>
          </Flex>
          <Form.Item name="rootCause" label="Root Cause">
            <TextArea rows={2} />
          </Form.Item>
          <Form.Item name="correctiveActions" label="Corrective Actions">
            <TextArea rows={2} />
          </Form.Item>
          {editing && (
            <Form.Item name="status" label="Status">
              <Select options={statuses.map(s => ({ label: s, value: s }))} />
            </Form.Item>
          )}
        </Form>
      </Modal>

      {/* Detail Drawer */}
      <Drawer title={selected?.title} open={drawerOpen} onClose={() => { setDrawerOpen(false); setSelected(null); }} width={700}>
        {selected && (
          <Tabs items={[
            {
              key: 'details', label: 'Details',
              children: (
                <Descriptions column={2} size="small" bordered>
                  <Descriptions.Item label="Incident #">{selected.incidentNumber}</Descriptions.Item>
                  <Descriptions.Item label="Status"><Tag color={statusColors[selected.status]}>{selected.status}</Tag></Descriptions.Item>
                  <Descriptions.Item label="Type">{selected.incidentType}</Descriptions.Item>
                  <Descriptions.Item label="Severity"><Tag color={severityColors[selected.severity]}>{selected.severity}</Tag></Descriptions.Item>
                  <Descriptions.Item label="Mine Site">{selected.mineSiteName}</Descriptions.Item>
                  <Descriptions.Item label="Location">{selected.location}</Descriptions.Item>
                  <Descriptions.Item label="Date">{dayjs(selected.incidentDateTime).format('DD/MM/YYYY HH:mm')}</Descriptions.Item>
                  <Descriptions.Item label="Reported By">{selected.reportedBy}</Descriptions.Item>
                  <Descriptions.Item label="Description" span={2}>{selected.description}</Descriptions.Item>
                  {selected.immediateActions && <Descriptions.Item label="Immediate Actions" span={2}>{selected.immediateActions}</Descriptions.Item>}
                  {selected.injuredPersonName && <Descriptions.Item label="Injured Person">{selected.injuredPersonName}</Descriptions.Item>}
                  {selected.injuryType && <Descriptions.Item label="Injury Type">{selected.injuryType}</Descriptions.Item>}
                  {selected.lostTimeDays != null && <Descriptions.Item label="Lost Time Days">{selected.lostTimeDays}</Descriptions.Item>}
                  <Descriptions.Item label="Reportable">{selected.isReportable ? 'Yes' : 'No'}</Descriptions.Item>
                  {selected.rootCause && <Descriptions.Item label="Root Cause" span={2}>{selected.rootCause}</Descriptions.Item>}
                  {selected.correctiveActions && <Descriptions.Item label="Corrective Actions" span={2}>{selected.correctiveActions}</Descriptions.Item>}
                </Descriptions>
              ),
            },
            {
              key: 'investigations', label: `Investigations (${investigations.length})`,
              children: (
                <div>
                  <Button type="primary" size="small" icon={<PlusOutlined />} style={{ marginBottom: 12 }}
                    onClick={() => { invForm.resetFields(); setInvModalOpen(true); }}>Add Investigation</Button>
                  {investigations.map((inv: IncidentInvestigation) => (
                    <div key={inv.id} style={{ background: '#f8f9fa', padding: 16, borderRadius: 8, marginBottom: 8 }}>
                      <Flex justify="space-between" align="center" style={{ marginBottom: 8 }}>
                        <Text strong>{inv.investigatorName}</Text>
                        <Tag>{inv.status}</Tag>
                      </Flex>
                      <Text style={{ fontSize: 12, color: '#86868b' }}>{dayjs(inv.investigationDate).format('DD/MM/YYYY')} &middot; {inv.methodology}</Text>
                      <div style={{ marginTop: 8 }}><Text>{inv.findings}</Text></div>
                      {inv.recommendations && <div style={{ marginTop: 4 }}><Text style={{ color: '#0071e3' }}>Recommendations: {inv.recommendations}</Text></div>}
                    </div>
                  ))}
                </div>
              ),
            },
          ]} />
        )}
      </Drawer>

      {/* Investigation Modal */}
      <Modal title="Add Investigation" open={invModalOpen} onCancel={() => setInvModalOpen(false)}
        onOk={handleInvSubmit} confirmLoading={invMutation.isPending}>
        <Form form={invForm} layout="vertical">
          <Form.Item name="investigatorName" label="Investigator Name" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item name="investigationDate" label="Investigation Date" rules={[{ required: true }]}>
            <DatePicker style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item name="methodology" label="Methodology" rules={[{ required: true }]}>
            <Select options={methodologies.map(m => ({ label: m, value: m }))} />
          </Form.Item>
          <Form.Item name="findings" label="Findings" rules={[{ required: true }]}>
            <TextArea rows={3} />
          </Form.Item>
          <Form.Item name="rootCauseAnalysis" label="Root Cause Analysis">
            <TextArea rows={2} />
          </Form.Item>
          <Form.Item name="recommendations" label="Recommendations">
            <TextArea rows={2} />
          </Form.Item>
          <Form.Item name="preventiveMeasures" label="Preventive Measures">
            <TextArea rows={2} />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}
