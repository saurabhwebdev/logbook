import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Table, Button, Modal, Form, Input, Select, DatePicker, Tag, Drawer, Tabs,
  Typography, Flex, Switch, message, Descriptions, InputNumber,
} from 'antd';
import { PlusOutlined, EyeOutlined, EditOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import { ventilationApi } from '../api/ventilationApi';
import { mineSitesApi } from '../api/mineSitesApi';
import type { VentilationReading, GasReading } from '../types';
import type {
  CreateVentilationReadingRequest, UpdateVentilationReadingRequest,
  CreateGasReadingRequest, UpdateGasReadingRequest,
} from '../api/ventilationApi';

const { Title, Text } = Typography;
const { TextArea } = Input;

const ventilationStatuses = ['Normal', 'Restricted', 'Inadequate', 'Critical'];
const doorStatuses = ['Open', 'Closed', 'PartiallyOpen'];
const fanStatuses = ['Running', 'Stopped', 'Fault'];
const gasTypes = ['Methane', 'CarbonMonoxide', 'CarbonDioxide', 'NitrogenDioxide', 'HydrogenSulfide', 'Oxygen', 'SulfurDioxide', 'Other'];
const gasUnits = ['ppm', 'percent', 'mgm3'];
const gasStatuses = ['Normal', 'Warning', 'Alarm', 'Evacuation'];

const ventStatusColors: Record<string, string> = {
  Normal: '#34c759', Restricted: '#ff9500', Inadequate: '#ff3b30', Critical: '#af52de',
};
const doorStatusColors: Record<string, string> = {
  Open: '#34c759', Closed: '#ff3b30', PartiallyOpen: '#ff9500',
};
const fanStatusColors: Record<string, string> = {
  Running: '#34c759', Stopped: '#ff3b30', Fault: '#ff9500',
};
const gasTypeColors: Record<string, string> = {
  Methane: '#ff9500', CarbonMonoxide: '#ff3b30', CarbonDioxide: '#86868b', NitrogenDioxide: '#af52de',
  HydrogenSulfide: '#8b6914', Oxygen: '#0071e3', SulfurDioxide: '#ff453a', Other: '#86868b',
};
const gasStatusColors: Record<string, string> = {
  Normal: '#34c759', Warning: '#ff9500', Alarm: '#ff3b30', Evacuation: '#af52de',
};

export default function VentilationPage() {
  const [activeTab, setActiveTab] = useState('ventilation');

  return (
    <div>
      <Flex justify="space-between" align="center" style={{ marginBottom: 24 }}>
        <div>
          <Title level={3} style={{ margin: 0, fontWeight: 700, letterSpacing: -0.5 }}>Ventilation & Gas Monitoring</Title>
          <Text style={{ color: '#86868b', fontSize: 13 }}>Monitor ventilation conditions and gas levels across mine sites</Text>
        </div>
      </Flex>

      <Tabs activeKey={activeTab} onChange={setActiveTab} items={[
        { key: 'ventilation', label: 'Ventilation Readings', children: <VentilationReadingsTab /> },
        { key: 'gas', label: 'Gas Readings', children: <GasReadingsTab /> },
      ]} />
    </div>
  );
}

// ===== Ventilation Readings Tab =====
function VentilationReadingsTab() {
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<VentilationReading | null>(null);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [selected, setSelected] = useState<VentilationReading | null>(null);
  const [filterStatus, setFilterStatus] = useState<string | undefined>();
  const [form] = Form.useForm();
  const queryClient = useQueryClient();

  const { data: readings = [], isLoading } = useQuery({
    queryKey: ['ventilationReadings', filterStatus],
    queryFn: () => ventilationApi.getVentilationReadings(undefined, filterStatus),
  });

  const { data: mineSites = [] } = useQuery({
    queryKey: ['mineSites'],
    queryFn: () => mineSitesApi.getMineSites(),
  });

  const createMutation = useMutation({
    mutationFn: (data: CreateVentilationReadingRequest) => ventilationApi.createVentilationReading(data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['ventilationReadings'] }); setModalOpen(false); form.resetFields(); message.success('Ventilation reading created'); },
  });

  const updateMutation = useMutation({
    mutationFn: (data: UpdateVentilationReadingRequest) => ventilationApi.updateVentilationReading(data.id, data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['ventilationReadings'] }); setModalOpen(false); setEditing(null); form.resetFields(); message.success('Ventilation reading updated'); },
  });

  const handleSubmit = async () => {
    const values = await form.validateFields();
    const payload = {
      ...values,
      readingDateTime: values.readingDateTime?.toISOString(),
    };
    if (editing) {
      updateMutation.mutate({ ...payload, id: editing.id });
    } else {
      createMutation.mutate(payload);
    }
  };

  const openEdit = (record: VentilationReading) => {
    setEditing(record);
    form.setFieldsValue({
      ...record,
      readingDateTime: dayjs(record.readingDateTime),
    });
    setModalOpen(true);
  };

  const columns = [
    { title: 'Reading #', dataIndex: 'readingNumber', key: 'readingNumber', width: 120 },
    { title: 'Location', dataIndex: 'locationDescription', key: 'locationDescription', width: 180, ellipsis: true },
    { title: 'Airflow Vel.', dataIndex: 'airflowVelocity', key: 'airflowVelocity', width: 110, render: (v: number | null) => v !== null ? `${v} m/s` : '-' },
    { title: 'Airflow Vol.', dataIndex: 'airflowVolume', key: 'airflowVolume', width: 110, render: (v: number | null) => v !== null ? `${v} m\u00B3/s` : '-' },
    { title: 'Temp', dataIndex: 'temperature', key: 'temperature', width: 80, render: (v: number | null) => v !== null ? `${v}\u00B0C` : '-' },
    { title: 'Mine Site', dataIndex: 'mineSiteName', key: 'mineSiteName', width: 150 },
    { title: 'Date/Time', dataIndex: 'readingDateTime', key: 'readingDateTime', width: 150, render: (d: string) => dayjs(d).format('DD/MM/YYYY HH:mm') },
    { title: 'Fan', dataIndex: 'fanStatus', key: 'fanStatus', width: 100, render: (s: string | null) => s ? <Tag color={fanStatusColors[s]}>{s}</Tag> : '-' },
    { title: 'Status', dataIndex: 'ventilationStatus', key: 'ventilationStatus', width: 120, render: (s: string) => <Tag color={ventStatusColors[s]}>{s}</Tag> },
    {
      title: 'Actions', key: 'actions', width: 100,
      render: (_: unknown, record: VentilationReading) => (
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
        <Select placeholder="Filter by status" allowClear style={{ width: 180 }} onChange={setFilterStatus} options={ventilationStatuses.map(s => ({ label: s, value: s }))} />
        <Button type="primary" icon={<PlusOutlined />} onClick={() => { setEditing(null); form.resetFields(); setModalOpen(true); }}>New Reading</Button>
      </Flex>

      <Table dataSource={readings} columns={columns} rowKey="id" loading={isLoading} size="middle"
        style={{ background: '#fff', borderRadius: 12, overflow: 'hidden' }}
        pagination={{ pageSize: 15, showSizeChanger: false }} />

      <Modal title={editing ? 'Edit Ventilation Reading' : 'New Ventilation Reading'} open={modalOpen} onCancel={() => { setModalOpen(false); setEditing(null); }}
        onOk={handleSubmit} width={720} okText={editing ? 'Update' : 'Create'}
        confirmLoading={createMutation.isPending || updateMutation.isPending}>
        <Form form={form} layout="vertical" style={{ maxHeight: 500, overflow: 'auto', paddingRight: 8 }}>
          <Flex gap={16}>
            <Form.Item name="mineSiteId" label="Mine Site" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select placeholder="Select mine site" disabled={!!editing} options={mineSites.map(m => ({ label: m.name, value: m.id }))} />
            </Form.Item>
            <Form.Item name="readingDateTime" label="Reading Date & Time" rules={[{ required: true }]} style={{ flex: 1 }}>
              <DatePicker showTime style={{ width: '100%' }} />
            </Form.Item>
          </Flex>
          <Form.Item name="locationDescription" label="Location Description" rules={[{ required: true }]}>
            <Input placeholder="e.g., Main Decline Level 3, Shaft 2 Intake" />
          </Form.Item>
          <Flex gap={16}>
            <Form.Item name="airflowVelocity" label="Airflow Velocity (m/s)" style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} step={0.01} />
            </Form.Item>
            <Form.Item name="airflowVolume" label="Airflow Volume (m\u00B3/s)" style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} step={0.01} />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="temperature" label="Temperature (\u00B0C)" style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} step={0.1} />
            </Form.Item>
            <Form.Item name="humidity" label="Humidity (%)" style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} step={0.1} min={0} max={100} />
            </Form.Item>
            <Form.Item name="barometricPressure" label="Barometric Pressure (hPa)" style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} step={0.1} />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="doorStatus" label="Door Status" style={{ flex: 1 }}>
              <Select allowClear options={doorStatuses.map(s => ({ label: s, value: s }))} />
            </Form.Item>
            <Form.Item name="fanStatus" label="Fan Status" style={{ flex: 1 }}>
              <Select allowClear options={fanStatuses.map(s => ({ label: s, value: s }))} />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="recordedBy" label="Recorded By" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Input />
            </Form.Item>
            <Form.Item name="instrumentUsed" label="Instrument Used" style={{ flex: 1 }}>
              <Input />
            </Form.Item>
          </Flex>
          <Form.Item name="notes" label="Notes">
            <TextArea rows={2} />
          </Form.Item>
          {editing && (
            <Form.Item name="ventilationStatus" label="Ventilation Status">
              <Select options={ventilationStatuses.map(s => ({ label: s, value: s }))} />
            </Form.Item>
          )}
        </Form>
      </Modal>

      <Drawer title={selected?.readingNumber} open={drawerOpen} onClose={() => { setDrawerOpen(false); setSelected(null); }} width={700}>
        {selected && (
          <Descriptions column={2} size="small" bordered>
            <Descriptions.Item label="Reading #">{selected.readingNumber}</Descriptions.Item>
            <Descriptions.Item label="Status"><Tag color={ventStatusColors[selected.ventilationStatus]}>{selected.ventilationStatus}</Tag></Descriptions.Item>
            <Descriptions.Item label="Mine Site">{selected.mineSiteName}</Descriptions.Item>
            {selected.mineAreaName && <Descriptions.Item label="Mine Area">{selected.mineAreaName}</Descriptions.Item>}
            <Descriptions.Item label="Location" span={2}>{selected.locationDescription}</Descriptions.Item>
            {selected.airflowVelocity !== null && <Descriptions.Item label="Airflow Velocity">{selected.airflowVelocity} m/s</Descriptions.Item>}
            {selected.airflowVolume !== null && <Descriptions.Item label="Airflow Volume">{selected.airflowVolume} m{'\u00B3'}/s</Descriptions.Item>}
            {selected.temperature !== null && <Descriptions.Item label="Temperature">{selected.temperature}{'\u00B0'}C</Descriptions.Item>}
            {selected.humidity !== null && <Descriptions.Item label="Humidity">{selected.humidity}%</Descriptions.Item>}
            {selected.barometricPressure !== null && <Descriptions.Item label="Barometric Pressure">{selected.barometricPressure} hPa</Descriptions.Item>}
            <Descriptions.Item label="Reading Date/Time">{dayjs(selected.readingDateTime).format('DD/MM/YYYY HH:mm')}</Descriptions.Item>
            <Descriptions.Item label="Recorded By">{selected.recordedBy}</Descriptions.Item>
            {selected.instrumentUsed && <Descriptions.Item label="Instrument Used">{selected.instrumentUsed}</Descriptions.Item>}
            {selected.doorStatus && <Descriptions.Item label="Door Status"><Tag color={doorStatusColors[selected.doorStatus]}>{selected.doorStatus}</Tag></Descriptions.Item>}
            {selected.fanStatus && <Descriptions.Item label="Fan Status"><Tag color={fanStatusColors[selected.fanStatus]}>{selected.fanStatus}</Tag></Descriptions.Item>}
            {selected.notes && <Descriptions.Item label="Notes" span={2}>{selected.notes}</Descriptions.Item>}
            <Descriptions.Item label="Created">{dayjs(selected.createdAt).format('DD/MM/YYYY HH:mm')}</Descriptions.Item>
          </Descriptions>
        )}
      </Drawer>
    </>
  );
}

// ===== Gas Readings Tab =====
function GasReadingsTab() {
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<GasReading | null>(null);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [selected, setSelected] = useState<GasReading | null>(null);
  const [filterGasType, setFilterGasType] = useState<string | undefined>();
  const [filterStatus, setFilterStatus] = useState<string | undefined>();
  const [form] = Form.useForm();
  const queryClient = useQueryClient();

  const { data: readings = [], isLoading } = useQuery({
    queryKey: ['gasReadings', filterGasType, filterStatus],
    queryFn: () => ventilationApi.getGasReadings(undefined, filterGasType, filterStatus),
  });

  const { data: mineSites = [] } = useQuery({
    queryKey: ['mineSites'],
    queryFn: () => mineSitesApi.getMineSites(),
  });

  const createMutation = useMutation({
    mutationFn: (data: CreateGasReadingRequest) => ventilationApi.createGasReading(data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['gasReadings'] }); setModalOpen(false); form.resetFields(); message.success('Gas reading created'); },
  });

  const updateMutation = useMutation({
    mutationFn: (data: UpdateGasReadingRequest) => ventilationApi.updateGasReading(data.id, data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['gasReadings'] }); setModalOpen(false); setEditing(null); form.resetFields(); message.success('Gas reading updated'); },
  });

  const handleSubmit = async () => {
    const values = await form.validateFields();
    const payload = {
      ...values,
      readingDateTime: values.readingDateTime?.toISOString(),
      calibrationDate: values.calibrationDate?.toISOString(),
    };
    if (editing) {
      updateMutation.mutate({ ...payload, id: editing.id });
    } else {
      createMutation.mutate(payload);
    }
  };

  const openEdit = (record: GasReading) => {
    setEditing(record);
    form.setFieldsValue({
      ...record,
      readingDateTime: dayjs(record.readingDateTime),
      calibrationDate: record.calibrationDate ? dayjs(record.calibrationDate) : undefined,
    });
    setModalOpen(true);
  };

  const columns = [
    { title: 'Reading #', dataIndex: 'readingNumber', key: 'readingNumber', width: 120 },
    { title: 'Gas Type', dataIndex: 'gasType', key: 'gasType', width: 150, render: (t: string) => <Tag color={gasTypeColors[t]}>{t}</Tag> },
    { title: 'Concentration', key: 'concentration', width: 130, render: (_: unknown, r: GasReading) => `${r.concentration} ${r.unit}` },
    { title: 'Location', dataIndex: 'locationDescription', key: 'locationDescription', width: 180, ellipsis: true },
    { title: 'Mine Site', dataIndex: 'mineSiteName', key: 'mineSiteName', width: 150 },
    { title: 'Date/Time', dataIndex: 'readingDateTime', key: 'readingDateTime', width: 150, render: (d: string) => dayjs(d).format('DD/MM/YYYY HH:mm') },
    { title: 'Recorded By', dataIndex: 'recordedBy', key: 'recordedBy', width: 130 },
    { title: 'Exceedance', dataIndex: 'isExceedance', key: 'isExceedance', width: 100, render: (v: boolean) => v ? <Tag color="red">Yes</Tag> : <Tag color="green">No</Tag> },
    { title: 'Status', dataIndex: 'status', key: 'status', width: 110, render: (s: string) => <Tag color={gasStatusColors[s]}>{s}</Tag> },
    {
      title: 'Actions', key: 'actions', width: 100,
      render: (_: unknown, record: GasReading) => (
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
        <Select placeholder="Filter by gas type" allowClear style={{ width: 180 }} onChange={setFilterGasType} options={gasTypes.map(t => ({ label: t, value: t }))} />
        <Select placeholder="Filter by status" allowClear style={{ width: 160 }} onChange={setFilterStatus} options={gasStatuses.map(s => ({ label: s, value: s }))} />
        <Button type="primary" icon={<PlusOutlined />} onClick={() => { setEditing(null); form.resetFields(); setModalOpen(true); }}>New Gas Reading</Button>
      </Flex>

      <Table dataSource={readings} columns={columns} rowKey="id" loading={isLoading} size="middle"
        style={{ background: '#fff', borderRadius: 12, overflow: 'hidden' }}
        pagination={{ pageSize: 15, showSizeChanger: false }} />

      <Modal title={editing ? 'Edit Gas Reading' : 'New Gas Reading'} open={modalOpen} onCancel={() => { setModalOpen(false); setEditing(null); }}
        onOk={handleSubmit} width={720} okText={editing ? 'Update' : 'Create'}
        confirmLoading={createMutation.isPending || updateMutation.isPending}>
        <Form form={form} layout="vertical" style={{ maxHeight: 500, overflow: 'auto', paddingRight: 8 }}>
          <Flex gap={16}>
            <Form.Item name="mineSiteId" label="Mine Site" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select placeholder="Select mine site" disabled={!!editing} options={mineSites.map(m => ({ label: m.name, value: m.id }))} />
            </Form.Item>
            <Form.Item name="gasType" label="Gas Type" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select options={gasTypes.map(t => ({ label: t, value: t }))} />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="concentration" label="Concentration" rules={[{ required: true }]} style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} step={0.01} />
            </Form.Item>
            <Form.Item name="unit" label="Unit" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select options={gasUnits.map(u => ({ label: u, value: u }))} />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="thresholdTWA" label="Threshold TWA" style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} step={0.01} />
            </Form.Item>
            <Form.Item name="thresholdSTEL" label="Threshold STEL" style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} step={0.01} />
            </Form.Item>
            <Form.Item name="thresholdCeiling" label="Threshold Ceiling" style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} step={0.01} />
            </Form.Item>
          </Flex>
          <Form.Item name="locationDescription" label="Location Description" rules={[{ required: true }]}>
            <Input placeholder="e.g., Main Decline Level 3, Stope 4B" />
          </Form.Item>
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
            <Form.Item name="instrumentId" label="Instrument ID" style={{ flex: 1 }}>
              <Input />
            </Form.Item>
          </Flex>
          <Form.Item name="calibrationDate" label="Calibration Date">
            <DatePicker style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item name="actionTaken" label="Action Taken">
            <TextArea rows={2} />
          </Form.Item>
          <Form.Item name="notes" label="Notes">
            <TextArea rows={2} />
          </Form.Item>
          {editing && (
            <Form.Item name="status" label="Status">
              <Select options={gasStatuses.map(s => ({ label: s, value: s }))} />
            </Form.Item>
          )}
        </Form>
      </Modal>

      <Drawer title={selected?.readingNumber} open={drawerOpen} onClose={() => { setDrawerOpen(false); setSelected(null); }} width={700}>
        {selected && (
          <Descriptions column={2} size="small" bordered>
            <Descriptions.Item label="Reading #">{selected.readingNumber}</Descriptions.Item>
            <Descriptions.Item label="Status"><Tag color={gasStatusColors[selected.status]}>{selected.status}</Tag></Descriptions.Item>
            <Descriptions.Item label="Gas Type"><Tag color={gasTypeColors[selected.gasType]}>{selected.gasType}</Tag></Descriptions.Item>
            <Descriptions.Item label="Concentration">{selected.concentration} {selected.unit}</Descriptions.Item>
            <Descriptions.Item label="Mine Site">{selected.mineSiteName}</Descriptions.Item>
            {selected.mineAreaName && <Descriptions.Item label="Mine Area">{selected.mineAreaName}</Descriptions.Item>}
            <Descriptions.Item label="Location" span={2}>{selected.locationDescription}</Descriptions.Item>
            <Descriptions.Item label="Exceedance">{selected.isExceedance ? 'Yes' : 'No'}</Descriptions.Item>
            {selected.thresholdTWA !== null && <Descriptions.Item label="Threshold TWA">{selected.thresholdTWA} {selected.unit}</Descriptions.Item>}
            {selected.thresholdSTEL !== null && <Descriptions.Item label="Threshold STEL">{selected.thresholdSTEL} {selected.unit}</Descriptions.Item>}
            {selected.thresholdCeiling !== null && <Descriptions.Item label="Threshold Ceiling">{selected.thresholdCeiling} {selected.unit}</Descriptions.Item>}
            <Descriptions.Item label="Reading Date/Time">{dayjs(selected.readingDateTime).format('DD/MM/YYYY HH:mm')}</Descriptions.Item>
            <Descriptions.Item label="Recorded By">{selected.recordedBy}</Descriptions.Item>
            {selected.instrumentId && <Descriptions.Item label="Instrument ID">{selected.instrumentId}</Descriptions.Item>}
            {selected.calibrationDate && <Descriptions.Item label="Calibration Date">{dayjs(selected.calibrationDate).format('DD/MM/YYYY')}</Descriptions.Item>}
            {selected.actionTaken && <Descriptions.Item label="Action Taken" span={2}>{selected.actionTaken}</Descriptions.Item>}
            {selected.notes && <Descriptions.Item label="Notes" span={2}>{selected.notes}</Descriptions.Item>}
            <Descriptions.Item label="Created">{dayjs(selected.createdAt).format('DD/MM/YYYY HH:mm')}</Descriptions.Item>
          </Descriptions>
        )}
      </Drawer>
    </>
  );
}
