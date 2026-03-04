import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Table, Button, Modal, Form, Input, Select, DatePicker, Tag, Drawer, Tabs,
  Typography, Flex, Switch, message, Popconfirm, Descriptions, InputNumber,
} from 'antd';
import { PlusOutlined, EyeOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import { environmentalApi } from '../api/environmentalApi';
import { mineSitesApi } from '../api/mineSitesApi';
import type { EnvironmentalReading, EnvironmentalIncident } from '../types';
import type {
  CreateEnvironmentalReadingRequest, UpdateEnvironmentalReadingRequest,
  CreateEnvironmentalIncidentRequest, UpdateEnvironmentalIncidentRequest,
} from '../api/environmentalApi';

const { Title, Text } = Typography;
const { TextArea } = Input;

const readingTypes = ['DustLevel', 'NoiseLevel', 'WaterQuality', 'AirQuality', 'GroundVibration', 'WaterLevel', 'SoilContamination', 'BlastVibration', 'Other'];
const readingStatuses = ['Normal', 'Warning', 'Critical', 'Exceedance'];
const incidentTypes = ['Spill', 'Emission', 'Discharge', 'Dust', 'Noise', 'WaterContamination', 'LandDegradation', 'Other'];
const incidentSeverities = ['Critical', 'High', 'Medium', 'Low'];
const incidentStatuses = ['Open', 'Investigating', 'Remediation', 'Closed'];

const readingStatusColors: Record<string, string> = {
  Normal: '#34c759', Warning: '#ff9500', Critical: '#ff3b30', Exceedance: '#af52de',
};
const readingTypeColors: Record<string, string> = {
  DustLevel: '#8b6914', NoiseLevel: '#ff9500', WaterQuality: '#0071e3', AirQuality: '#34c759',
  GroundVibration: '#af52de', WaterLevel: '#007aff', SoilContamination: '#ff3b30', BlastVibration: '#ff453a', Other: '#86868b',
};
const incidentStatusColors: Record<string, string> = {
  Open: '#ff3b30', Investigating: '#ff9500', Remediation: '#0071e3', Closed: '#34c759',
};
const severityColors: Record<string, string> = {
  Critical: '#ff3b30', High: '#ff9500', Medium: '#ffcc00', Low: '#34c759',
};

export default function EnvironmentalPage() {
  const [activeTab, setActiveTab] = useState('readings');

  return (
    <div>
      <Flex justify="space-between" align="center" style={{ marginBottom: 24 }}>
        <div>
          <Title level={3} style={{ margin: 0, fontWeight: 700, letterSpacing: -0.5 }}>Environmental Monitoring</Title>
          <Text style={{ color: '#86868b', fontSize: 13 }}>Track environmental readings, thresholds, and incidents across mine sites</Text>
        </div>
      </Flex>

      <Tabs activeKey={activeTab} onChange={setActiveTab} items={[
        { key: 'readings', label: 'Readings', children: <ReadingsTab /> },
        { key: 'incidents', label: 'Incidents', children: <IncidentsTab /> },
      ]} />
    </div>
  );
}

// ===== Readings Tab =====
function ReadingsTab() {
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<EnvironmentalReading | null>(null);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [selected, setSelected] = useState<EnvironmentalReading | null>(null);
  const [filterType, setFilterType] = useState<string | undefined>();
  const [filterStatus, setFilterStatus] = useState<string | undefined>();
  const [form] = Form.useForm();
  const queryClient = useQueryClient();

  const { data: readings = [], isLoading } = useQuery({
    queryKey: ['environmentalReadings', filterType, filterStatus],
    queryFn: () => environmentalApi.getReadings(undefined, filterType, filterStatus),
  });

  const { data: mineSites = [] } = useQuery({
    queryKey: ['mineSites'],
    queryFn: () => mineSitesApi.getMineSites(),
  });

  const createMutation = useMutation({
    mutationFn: (data: CreateEnvironmentalReadingRequest) => environmentalApi.createReading(data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['environmentalReadings'] }); setModalOpen(false); form.resetFields(); message.success('Reading created'); },
  });

  const updateMutation = useMutation({
    mutationFn: (data: UpdateEnvironmentalReadingRequest) => environmentalApi.updateReading(data.id, data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['environmentalReadings'] }); setModalOpen(false); setEditing(null); form.resetFields(); message.success('Reading updated'); },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => environmentalApi.deleteReading(id),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['environmentalReadings'] }); message.success('Reading deleted'); },
  });

  const handleSubmit = async () => {
    const values = await form.validateFields();
    const payload = {
      ...values,
      readingDateTime: values.readingDateTime?.toISOString(),
      calibratedDate: values.calibratedDate?.toISOString(),
    };
    if (editing) {
      updateMutation.mutate({ ...payload, id: editing.id });
    } else {
      createMutation.mutate(payload);
    }
  };

  const openEdit = (record: EnvironmentalReading) => {
    setEditing(record);
    form.setFieldsValue({
      ...record,
      readingDateTime: dayjs(record.readingDateTime),
      calibratedDate: record.calibratedDate ? dayjs(record.calibratedDate) : undefined,
    });
    setModalOpen(true);
  };

  const columns = [
    { title: 'Reading #', dataIndex: 'readingNumber', key: 'readingNumber', width: 120 },
    { title: 'Type', dataIndex: 'readingType', key: 'readingType', width: 140, render: (t: string) => <Tag color={readingTypeColors[t]}>{t}</Tag> },
    { title: 'Parameter', dataIndex: 'parameter', key: 'parameter', width: 150, ellipsis: true },
    { title: 'Value', key: 'value', width: 120, render: (_: unknown, r: EnvironmentalReading) => `${r.value} ${r.unit}` },
    { title: 'Mine Site', dataIndex: 'mineSiteName', key: 'mineSiteName', width: 150 },
    { title: 'Date/Time', dataIndex: 'readingDateTime', key: 'readingDateTime', width: 150, render: (d: string) => dayjs(d).format('DD/MM/YYYY HH:mm') },
    { title: 'Recorded By', dataIndex: 'recordedBy', key: 'recordedBy', width: 130 },
    { title: 'Exceedance', dataIndex: 'isExceedance', key: 'isExceedance', width: 100, render: (v: boolean) => v ? <Tag color="red">Yes</Tag> : <Tag color="green">No</Tag> },
    { title: 'Status', dataIndex: 'status', key: 'status', width: 110, render: (s: string) => <Tag color={readingStatusColors[s]}>{s}</Tag> },
    {
      title: 'Actions', key: 'actions', width: 130,
      render: (_: unknown, record: EnvironmentalReading) => (
        <Flex gap={4}>
          <Button type="text" size="small" icon={<EyeOutlined />} onClick={() => { setSelected(record); setDrawerOpen(true); }} />
          <Button type="text" size="small" icon={<EditOutlined />} onClick={() => openEdit(record)} />
          <Popconfirm title="Delete this reading?" onConfirm={() => deleteMutation.mutate(record.id)}>
            <Button type="text" size="small" danger icon={<DeleteOutlined />} />
          </Popconfirm>
        </Flex>
      ),
    },
  ];

  return (
    <>
      <Flex justify="flex-end" gap={8} style={{ marginBottom: 16 }}>
        <Select placeholder="Filter by type" allowClear style={{ width: 180 }} onChange={setFilterType} options={readingTypes.map(t => ({ label: t, value: t }))} />
        <Select placeholder="Filter by status" allowClear style={{ width: 160 }} onChange={setFilterStatus} options={readingStatuses.map(s => ({ label: s, value: s }))} />
        <Button type="primary" icon={<PlusOutlined />} onClick={() => { setEditing(null); form.resetFields(); setModalOpen(true); }}>New Reading</Button>
      </Flex>

      <Table dataSource={readings} columns={columns} rowKey="id" loading={isLoading} size="middle"
        style={{ background: '#fff', borderRadius: 12, overflow: 'hidden' }}
        pagination={{ pageSize: 15, showSizeChanger: false }} />

      <Modal title={editing ? 'Edit Reading' : 'New Environmental Reading'} open={modalOpen} onCancel={() => { setModalOpen(false); setEditing(null); }}
        onOk={handleSubmit} width={720} okText={editing ? 'Update' : 'Create'}
        confirmLoading={createMutation.isPending || updateMutation.isPending}>
        <Form form={form} layout="vertical" style={{ maxHeight: 500, overflow: 'auto', paddingRight: 8 }}>
          <Flex gap={16}>
            <Form.Item name="mineSiteId" label="Mine Site" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select placeholder="Select mine site" disabled={!!editing} options={mineSites.map(m => ({ label: m.name, value: m.id }))} />
            </Form.Item>
            <Form.Item name="readingType" label="Reading Type" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select options={readingTypes.map(t => ({ label: t, value: t }))} />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="parameter" label="Parameter" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Input placeholder="e.g., PM10, dB(A), pH" />
            </Form.Item>
            <Form.Item name="unit" label="Unit" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Input placeholder="e.g., mg/m3, dB, mg/L" />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="value" label="Value" rules={[{ required: true }]} style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} step={0.01} />
            </Form.Item>
            <Form.Item name="thresholdMin" label="Threshold Min" style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} step={0.01} />
            </Form.Item>
            <Form.Item name="thresholdMax" label="Threshold Max" style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} step={0.01} />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="readingDateTime" label="Reading Date & Time" rules={[{ required: true }]} style={{ flex: 1 }}>
              <DatePicker showTime style={{ width: '100%' }} />
            </Form.Item>
            <Form.Item name="isExceedance" label="Is Exceedance" valuePropName="checked" style={{ flex: 1 }}>
              <Switch />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="recordedBy" label="Recorded By" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Input />
            </Form.Item>
            <Form.Item name="monitoringStation" label="Monitoring Station" style={{ flex: 1 }}>
              <Input />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="instrumentUsed" label="Instrument Used" style={{ flex: 1 }}>
              <Input />
            </Form.Item>
            <Form.Item name="calibratedDate" label="Calibrated Date" style={{ flex: 1 }}>
              <DatePicker style={{ width: '100%' }} />
            </Form.Item>
          </Flex>
          <Form.Item name="weatherConditions" label="Weather Conditions">
            <Input />
          </Form.Item>
          <Form.Item name="notes" label="Notes">
            <TextArea rows={2} />
          </Form.Item>
          {editing && (
            <Form.Item name="status" label="Status">
              <Select options={readingStatuses.map(s => ({ label: s, value: s }))} />
            </Form.Item>
          )}
        </Form>
      </Modal>

      <Drawer title={selected?.readingNumber} open={drawerOpen} onClose={() => { setDrawerOpen(false); setSelected(null); }} width={700}>
        {selected && (
          <Descriptions column={2} size="small" bordered>
            <Descriptions.Item label="Reading #">{selected.readingNumber}</Descriptions.Item>
            <Descriptions.Item label="Status"><Tag color={readingStatusColors[selected.status]}>{selected.status}</Tag></Descriptions.Item>
            <Descriptions.Item label="Type"><Tag color={readingTypeColors[selected.readingType]}>{selected.readingType}</Tag></Descriptions.Item>
            <Descriptions.Item label="Mine Site">{selected.mineSiteName}</Descriptions.Item>
            {selected.mineAreaName && <Descriptions.Item label="Mine Area">{selected.mineAreaName}</Descriptions.Item>}
            <Descriptions.Item label="Parameter">{selected.parameter}</Descriptions.Item>
            <Descriptions.Item label="Value">{selected.value} {selected.unit}</Descriptions.Item>
            <Descriptions.Item label="Exceedance">{selected.isExceedance ? 'Yes' : 'No'}</Descriptions.Item>
            {selected.thresholdMin !== null && <Descriptions.Item label="Threshold Min">{selected.thresholdMin} {selected.unit}</Descriptions.Item>}
            {selected.thresholdMax !== null && <Descriptions.Item label="Threshold Max">{selected.thresholdMax} {selected.unit}</Descriptions.Item>}
            <Descriptions.Item label="Reading Date/Time">{dayjs(selected.readingDateTime).format('DD/MM/YYYY HH:mm')}</Descriptions.Item>
            <Descriptions.Item label="Recorded By">{selected.recordedBy}</Descriptions.Item>
            {selected.monitoringStation && <Descriptions.Item label="Monitoring Station">{selected.monitoringStation}</Descriptions.Item>}
            {selected.instrumentUsed && <Descriptions.Item label="Instrument Used">{selected.instrumentUsed}</Descriptions.Item>}
            {selected.calibratedDate && <Descriptions.Item label="Calibrated Date">{dayjs(selected.calibratedDate).format('DD/MM/YYYY')}</Descriptions.Item>}
            {selected.weatherConditions && <Descriptions.Item label="Weather Conditions" span={2}>{selected.weatherConditions}</Descriptions.Item>}
            {selected.notes && <Descriptions.Item label="Notes" span={2}>{selected.notes}</Descriptions.Item>}
            <Descriptions.Item label="Created">{dayjs(selected.createdAt).format('DD/MM/YYYY HH:mm')}</Descriptions.Item>
          </Descriptions>
        )}
      </Drawer>
    </>
  );
}

// ===== Incidents Tab =====
function IncidentsTab() {
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<EnvironmentalIncident | null>(null);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [selected, setSelected] = useState<EnvironmentalIncident | null>(null);
  const [filterStatus, setFilterStatus] = useState<string | undefined>();
  const [form] = Form.useForm();
  const queryClient = useQueryClient();

  const { data: incidents = [], isLoading } = useQuery({
    queryKey: ['environmentalIncidents', filterStatus],
    queryFn: () => environmentalApi.getIncidents(undefined, filterStatus),
  });

  const { data: mineSites = [] } = useQuery({
    queryKey: ['mineSites'],
    queryFn: () => mineSitesApi.getMineSites(),
  });

  const createMutation = useMutation({
    mutationFn: (data: CreateEnvironmentalIncidentRequest) => environmentalApi.createIncident(data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['environmentalIncidents'] }); setModalOpen(false); form.resetFields(); message.success('Incident created'); },
  });

  const updateMutation = useMutation({
    mutationFn: (data: UpdateEnvironmentalIncidentRequest) => environmentalApi.updateIncident(data.id, data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['environmentalIncidents'] }); setModalOpen(false); setEditing(null); form.resetFields(); message.success('Incident updated'); },
  });

  const handleSubmit = async () => {
    const values = await form.validateFields();
    const payload = {
      ...values,
      occurredAt: values.occurredAt?.toISOString(),
      closedAt: values.closedAt?.toISOString(),
    };
    if (editing) {
      updateMutation.mutate({ ...payload, id: editing.id });
    } else {
      createMutation.mutate(payload);
    }
  };

  const openEdit = (record: EnvironmentalIncident) => {
    setEditing(record);
    form.setFieldsValue({
      ...record,
      occurredAt: dayjs(record.occurredAt),
      closedAt: record.closedAt ? dayjs(record.closedAt) : undefined,
    });
    setModalOpen(true);
  };

  const columns = [
    { title: 'Incident #', dataIndex: 'incidentNumber', key: 'incidentNumber', width: 110 },
    { title: 'Title', dataIndex: 'title', key: 'title', ellipsis: true },
    { title: 'Type', dataIndex: 'incidentType', key: 'incidentType', width: 150, render: (t: string) => <Tag>{t}</Tag> },
    { title: 'Severity', dataIndex: 'severity', key: 'severity', width: 100, render: (s: string) => <Tag color={severityColors[s]}>{s}</Tag> },
    { title: 'Mine Site', dataIndex: 'mineSiteName', key: 'mineSiteName', width: 150 },
    { title: 'Occurred', dataIndex: 'occurredAt', key: 'occurredAt', width: 150, render: (d: string) => dayjs(d).format('DD/MM/YYYY HH:mm') },
    { title: 'Reported By', dataIndex: 'reportedBy', key: 'reportedBy', width: 130 },
    { title: 'Authority', dataIndex: 'notifiedAuthority', key: 'notifiedAuthority', width: 90, render: (v: boolean) => v ? <Tag color="blue">Yes</Tag> : <Tag>No</Tag> },
    { title: 'Status', dataIndex: 'status', key: 'status', width: 120, render: (s: string) => <Tag color={incidentStatusColors[s]}>{s}</Tag> },
    {
      title: 'Actions', key: 'actions', width: 100,
      render: (_: unknown, record: EnvironmentalIncident) => (
        <Flex gap={4}>
          <Button type="text" size="small" icon={<EyeOutlined />} onClick={() => { setSelected(record); setDrawerOpen(true); }} />
          <Button type="text" size="small" icon={<EditOutlined />} onClick={() => openEdit(record)} />
        </Flex>
      ),
    },
  ];

  return (
    <>
      <Flex justify="flex-end" gap={8} style={{ marginBottom: 16 }}>
        <Select placeholder="Filter by status" allowClear style={{ width: 160 }} onChange={setFilterStatus} options={incidentStatuses.map(s => ({ label: s, value: s }))} />
        <Button type="primary" icon={<PlusOutlined />} onClick={() => { setEditing(null); form.resetFields(); setModalOpen(true); }}>New Incident</Button>
      </Flex>

      <Table dataSource={incidents} columns={columns} rowKey="id" loading={isLoading} size="middle"
        style={{ background: '#fff', borderRadius: 12, overflow: 'hidden' }}
        pagination={{ pageSize: 15, showSizeChanger: false }} />

      <Modal title={editing ? 'Edit Incident' : 'New Environmental Incident'} open={modalOpen} onCancel={() => { setModalOpen(false); setEditing(null); }}
        onOk={handleSubmit} width={720} okText={editing ? 'Update' : 'Create'}
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
            <Form.Item name="severity" label="Severity" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select options={incidentSeverities.map(s => ({ label: s, value: s }))} />
            </Form.Item>
            <Form.Item name="occurredAt" label="Occurred At" rules={[{ required: true }]} style={{ flex: 1 }}>
              <DatePicker showTime style={{ width: '100%' }} />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="location" label="Location" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Input />
            </Form.Item>
            <Form.Item name="reportedBy" label="Reported By" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Input />
            </Form.Item>
          </Flex>
          <Form.Item name="description" label="Description" rules={[{ required: true }]}>
            <TextArea rows={3} />
          </Form.Item>
          <Form.Item name="impactAssessment" label="Impact Assessment">
            <TextArea rows={2} />
          </Form.Item>
          <Form.Item name="containmentActions" label="Containment Actions">
            <TextArea rows={2} />
          </Form.Item>
          <Form.Item name="remediationPlan" label="Remediation Plan">
            <TextArea rows={2} />
          </Form.Item>
          <Flex gap={16}>
            <Form.Item name="notifiedAuthority" label="Notified Authority" valuePropName="checked" style={{ flex: 1 }}>
              <Switch />
            </Form.Item>
            <Form.Item name="authorityReference" label="Authority Reference" style={{ flex: 1 }}>
              <Input />
            </Form.Item>
          </Flex>
          {editing && (
            <>
              <Flex gap={16}>
                <Form.Item name="status" label="Status" style={{ flex: 1 }}>
                  <Select options={incidentStatuses.map(s => ({ label: s, value: s }))} />
                </Form.Item>
                <Form.Item name="closedAt" label="Closed At" style={{ flex: 1 }}>
                  <DatePicker showTime style={{ width: '100%' }} />
                </Form.Item>
              </Flex>
              <Form.Item name="closureNotes" label="Closure Notes">
                <TextArea rows={2} />
              </Form.Item>
            </>
          )}
        </Form>
      </Modal>

      <Drawer title={selected?.title} open={drawerOpen} onClose={() => { setDrawerOpen(false); setSelected(null); }} width={700}>
        {selected && (
          <Descriptions column={2} size="small" bordered>
            <Descriptions.Item label="Incident #">{selected.incidentNumber}</Descriptions.Item>
            <Descriptions.Item label="Status"><Tag color={incidentStatusColors[selected.status]}>{selected.status}</Tag></Descriptions.Item>
            <Descriptions.Item label="Type">{selected.incidentType}</Descriptions.Item>
            <Descriptions.Item label="Severity"><Tag color={severityColors[selected.severity]}>{selected.severity}</Tag></Descriptions.Item>
            <Descriptions.Item label="Mine Site">{selected.mineSiteName}</Descriptions.Item>
            <Descriptions.Item label="Location">{selected.location}</Descriptions.Item>
            <Descriptions.Item label="Occurred At">{dayjs(selected.occurredAt).format('DD/MM/YYYY HH:mm')}</Descriptions.Item>
            <Descriptions.Item label="Reported By">{selected.reportedBy}</Descriptions.Item>
            <Descriptions.Item label="Description" span={2}>{selected.description}</Descriptions.Item>
            {selected.impactAssessment && <Descriptions.Item label="Impact Assessment" span={2}>{selected.impactAssessment}</Descriptions.Item>}
            {selected.containmentActions && <Descriptions.Item label="Containment Actions" span={2}>{selected.containmentActions}</Descriptions.Item>}
            {selected.remediationPlan && <Descriptions.Item label="Remediation Plan" span={2}>{selected.remediationPlan}</Descriptions.Item>}
            <Descriptions.Item label="Notified Authority">{selected.notifiedAuthority ? 'Yes' : 'No'}</Descriptions.Item>
            {selected.authorityReference && <Descriptions.Item label="Authority Reference">{selected.authorityReference}</Descriptions.Item>}
            {selected.closedAt && <Descriptions.Item label="Closed At">{dayjs(selected.closedAt).format('DD/MM/YYYY HH:mm')}</Descriptions.Item>}
            {selected.closureNotes && <Descriptions.Item label="Closure Notes" span={2}>{selected.closureNotes}</Descriptions.Item>}
            <Descriptions.Item label="Created">{dayjs(selected.createdAt).format('DD/MM/YYYY HH:mm')}</Descriptions.Item>
          </Descriptions>
        )}
      </Drawer>
    </>
  );
}
