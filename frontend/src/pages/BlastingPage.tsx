import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Table, Button, Modal, Form, Input, Select, DatePicker, Tag, Drawer,
  Typography, Flex, Switch, InputNumber, message, Popconfirm, Descriptions, Tabs,
} from 'antd';
import { PlusOutlined, EyeOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import { blastingApi } from '../api/blastingApi';
import { mineSitesApi } from '../api/mineSitesApi';
import type { BlastEvent, ExplosiveUsage } from '../types';
import type { CreateBlastEventRequest, UpdateBlastEventRequest, CreateExplosiveUsageRequest } from '../api/blastingApi';

const { Title, Text } = Typography;
const { TextArea } = Input;

const blastTypes = ['Development', 'Production', 'Secondary', 'Presplit', 'TrimBlast', 'Other'];
const statuses = ['Planned', 'Approved', 'InProgress', 'Completed', 'Cancelled', 'Misfired'];
const explosiveTypes = ['Emulsion', 'ANFO', 'Dynamite', 'Detonator', 'Booster', 'Other'];
const units = ['kg', 'pieces', 'meters'];
const fragmentationQualities = ['Excellent', 'Good', 'Fair', 'Poor'];

const statusColors: Record<string, string> = {
  Planned: '#0071e3', Approved: '#34c759', InProgress: '#ff9500',
  Completed: '#86868b', Cancelled: '#ff3b30', Misfired: '#af52de',
};
const blastTypeColors: Record<string, string> = {
  Development: '#0071e3', Production: '#34c759', Secondary: '#ff9500',
  Presplit: '#af52de', TrimBlast: '#ffcc00', Other: '#86868b',
};
const fragColors: Record<string, string> = {
  Excellent: '#34c759', Good: '#0071e3', Fair: '#ff9500', Poor: '#ff3b30',
};

export default function BlastingPage() {
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<BlastEvent | null>(null);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [selected, setSelected] = useState<BlastEvent | null>(null);
  const [usageModalOpen, setUsageModalOpen] = useState(false);
  const [filterStatus, setFilterStatus] = useState<string | undefined>();
  const [form] = Form.useForm();
  const [usageForm] = Form.useForm();
  const queryClient = useQueryClient();

  const { data: blastEvents = [], isLoading } = useQuery({
    queryKey: ['blastEvents', filterStatus],
    queryFn: () => blastingApi.getAll(undefined, filterStatus),
  });

  const { data: mineSites = [] } = useQuery({
    queryKey: ['mineSites'],
    queryFn: () => mineSitesApi.getMineSites(),
  });

  const { data: usages = [] } = useQuery({
    queryKey: ['explosiveUsages', selected?.id],
    queryFn: () => blastingApi.getUsages(selected!.id),
    enabled: !!selected,
  });

  const createMutation = useMutation({
    mutationFn: (data: CreateBlastEventRequest) => blastingApi.create(data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['blastEvents'] }); setModalOpen(false); form.resetFields(); message.success('Blast event created'); },
  });

  const updateMutation = useMutation({
    mutationFn: (data: UpdateBlastEventRequest) => blastingApi.update(data.id, data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['blastEvents'] }); setModalOpen(false); setEditing(null); form.resetFields(); message.success('Blast event updated'); },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => blastingApi.delete(id),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['blastEvents'] }); message.success('Blast event deleted'); },
  });

  const usageMutation = useMutation({
    mutationFn: (data: CreateExplosiveUsageRequest) => blastingApi.createUsage(data.blastEventId, data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['explosiveUsages'] }); queryClient.invalidateQueries({ queryKey: ['blastEvents'] }); setUsageModalOpen(false); usageForm.resetFields(); message.success('Explosive usage recorded'); },
  });

  const handleSubmit = async () => {
    const values = await form.validateFields();
    const payload = {
      ...values,
      scheduledDateTime: values.scheduledDateTime?.toISOString(),
      actualDateTime: values.actualDateTime?.toISOString(),
    };
    if (editing) {
      updateMutation.mutate({ ...payload, id: editing.id });
    } else {
      createMutation.mutate(payload);
    }
  };

  const handleUsageSubmit = async () => {
    const values = await usageForm.validateFields();
    usageMutation.mutate({
      ...values,
      blastEventId: selected!.id,
    });
  };

  const openEdit = (record: BlastEvent) => {
    setEditing(record);
    form.setFieldsValue({
      ...record,
      scheduledDateTime: dayjs(record.scheduledDateTime),
      actualDateTime: record.actualDateTime ? dayjs(record.actualDateTime) : undefined,
    });
    setModalOpen(true);
  };

  const columns = [
    { title: 'Blast #', dataIndex: 'blastNumber', key: 'blastNumber', width: 110 },
    { title: 'Title', dataIndex: 'title', key: 'title', ellipsis: true },
    { title: 'Type', dataIndex: 'blastType', key: 'blastType', width: 120, render: (t: string) => <Tag color={blastTypeColors[t]}>{t}</Tag> },
    { title: 'Mine Site', dataIndex: 'mineSiteName', key: 'mineSiteName', width: 150 },
    { title: 'Scheduled', dataIndex: 'scheduledDateTime', key: 'scheduledDateTime', width: 130, render: (d: string) => dayjs(d).format('DD/MM/YYYY HH:mm') },
    { title: 'Status', dataIndex: 'status', key: 'status', width: 120, render: (s: string) => <Tag color={statusColors[s]}>{s}</Tag> },
    { title: 'Supervisor', dataIndex: 'supervisorName', key: 'supervisorName', width: 130 },
    { title: 'Blaster', dataIndex: 'licensedBlasterName', key: 'licensedBlasterName', width: 130 },
    {
      title: 'Actions', key: 'actions', width: 130,
      render: (_: unknown, record: BlastEvent) => (
        <Flex gap={4}>
          <Button type="text" size="small" icon={<EyeOutlined />} onClick={() => { setSelected(record); setDrawerOpen(true); }} />
          <Button type="text" size="small" icon={<EditOutlined />} onClick={() => openEdit(record)} />
          <Popconfirm title="Delete this blast event?" onConfirm={() => deleteMutation.mutate(record.id)}>
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
          <Title level={3} style={{ margin: 0, fontWeight: 700, letterSpacing: -0.5 }}>Blasting & Explosives Management</Title>
          <Text style={{ color: '#86868b', fontSize: 13 }}>Plan blast events, track explosive usage, and record safety confirmations</Text>
        </div>
        <Flex gap={8}>
          <Select placeholder="Filter by status" allowClear style={{ width: 180 }} onChange={setFilterStatus} options={statuses.map(s => ({ label: s, value: s }))} />
          <Button type="primary" icon={<PlusOutlined />} onClick={() => { setEditing(null); form.resetFields(); setModalOpen(true); }}>New Blast Event</Button>
        </Flex>
      </Flex>

      <Table dataSource={blastEvents} columns={columns} rowKey="id" loading={isLoading} size="middle"
        style={{ background: '#fff', borderRadius: 12, overflow: 'hidden' }}
        pagination={{ pageSize: 15, showSizeChanger: false }} />

      {/* Create/Edit Modal */}
      <Modal title={editing ? 'Edit Blast Event' : 'New Blast Event'} open={modalOpen} onCancel={() => { setModalOpen(false); setEditing(null); }}
        onOk={handleSubmit} width={720} okText={editing ? 'Update' : 'Create'}
        confirmLoading={createMutation.isPending || updateMutation.isPending}>
        <Form form={form} layout="vertical" style={{ maxHeight: 500, overflow: 'auto', paddingRight: 8 }}>
          <Flex gap={16}>
            <Form.Item name="mineSiteId" label="Mine Site" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select placeholder="Select mine site" disabled={!!editing} options={mineSites.map(m => ({ label: m.name, value: m.id }))} />
            </Form.Item>
            <Form.Item name="blastType" label="Blast Type" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select options={blastTypes.map(t => ({ label: t, value: t }))} />
            </Form.Item>
          </Flex>
          <Form.Item name="title" label="Title" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Flex gap={16}>
            <Form.Item name="scheduledDateTime" label="Scheduled Date & Time" rules={[{ required: true }]} style={{ flex: 1 }}>
              <DatePicker showTime style={{ width: '100%' }} />
            </Form.Item>
            <Form.Item name="location" label="Location" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Input />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="supervisorName" label="Supervisor" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Input />
            </Form.Item>
            <Form.Item name="licensedBlasterName" label="Licensed Blaster" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Input />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="drillingPattern" label="Drilling Pattern" style={{ flex: 1 }}>
              <Input />
            </Form.Item>
            <Form.Item name="numberOfHoles" label="Number of Holes" style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} min={0} />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="totalExplosivesKg" label="Total Explosives (kg)" style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} min={0} precision={2} />
            </Form.Item>
            <Form.Item name="explosiveType" label="Explosive Type" style={{ flex: 1 }}>
              <Input />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="detonatorType" label="Detonator Type" style={{ flex: 1 }}>
              <Input />
            </Form.Item>
            <Form.Item name="safetyRadius" label="Safety Radius (m)" style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} min={0} />
            </Form.Item>
          </Flex>
          <Form.Item name="blastDesignNotes" label="Blast Design Notes">
            <TextArea rows={2} />
          </Form.Item>
          {editing && (
            <>
              <Flex gap={16}>
                <Form.Item name="status" label="Status" style={{ flex: 1 }}>
                  <Select options={statuses.map(s => ({ label: s, value: s }))} />
                </Form.Item>
                <Form.Item name="actualDateTime" label="Actual Date & Time" style={{ flex: 1 }}>
                  <DatePicker showTime style={{ width: '100%' }} />
                </Form.Item>
              </Flex>
              <Flex gap={16}>
                <Form.Item name="evacuationConfirmed" label="Evacuation Confirmed" valuePropName="checked" style={{ flex: 1 }}>
                  <Switch />
                </Form.Item>
                <Form.Item name="sentryPostsConfirmed" label="Sentry Posts Confirmed" valuePropName="checked" style={{ flex: 1 }}>
                  <Switch />
                </Form.Item>
                <Form.Item name="preBlastWarningGiven" label="Pre-Blast Warning" valuePropName="checked" style={{ flex: 1 }}>
                  <Switch />
                </Form.Item>
              </Flex>
              <Flex gap={16}>
                <Form.Item name="vibrationReading" label="Vibration Reading" style={{ flex: 1 }}>
                  <InputNumber style={{ width: '100%' }} min={0} precision={2} />
                </Form.Item>
                <Form.Item name="airBlastReading" label="Air Blast Reading" style={{ flex: 1 }}>
                  <InputNumber style={{ width: '100%' }} min={0} precision={2} />
                </Form.Item>
              </Flex>
              <Flex gap={16}>
                <Form.Item name="fragmentationQuality" label="Fragmentation Quality" style={{ flex: 1 }}>
                  <Select allowClear options={fragmentationQualities.map(q => ({ label: q, value: q }))} />
                </Form.Item>
                <Form.Item name="misfireCount" label="Misfire Count" style={{ flex: 1 }}>
                  <InputNumber style={{ width: '100%' }} min={0} />
                </Form.Item>
              </Flex>
              <Form.Item name="postBlastInspection" label="Post-Blast Inspection">
                <TextArea rows={2} />
              </Form.Item>
              <Form.Item name="postBlastNotes" label="Post-Blast Notes">
                <TextArea rows={2} />
              </Form.Item>
            </>
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
                  <Descriptions.Item label="Blast #">{selected.blastNumber}</Descriptions.Item>
                  <Descriptions.Item label="Status"><Tag color={statusColors[selected.status]}>{selected.status}</Tag></Descriptions.Item>
                  <Descriptions.Item label="Type"><Tag color={blastTypeColors[selected.blastType]}>{selected.blastType}</Tag></Descriptions.Item>
                  <Descriptions.Item label="Mine Site">{selected.mineSiteName}</Descriptions.Item>
                  {selected.mineAreaName && <Descriptions.Item label="Mine Area">{selected.mineAreaName}</Descriptions.Item>}
                  <Descriptions.Item label="Location">{selected.location}</Descriptions.Item>
                  <Descriptions.Item label="Scheduled">{dayjs(selected.scheduledDateTime).format('DD/MM/YYYY HH:mm')}</Descriptions.Item>
                  {selected.actualDateTime && <Descriptions.Item label="Actual">{dayjs(selected.actualDateTime).format('DD/MM/YYYY HH:mm')}</Descriptions.Item>}
                  <Descriptions.Item label="Supervisor">{selected.supervisorName}</Descriptions.Item>
                  <Descriptions.Item label="Licensed Blaster">{selected.licensedBlasterName}</Descriptions.Item>
                  {selected.drillingPattern && <Descriptions.Item label="Drilling Pattern">{selected.drillingPattern}</Descriptions.Item>}
                  {selected.numberOfHoles != null && <Descriptions.Item label="Number of Holes">{selected.numberOfHoles}</Descriptions.Item>}
                  {selected.totalExplosivesKg != null && <Descriptions.Item label="Total Explosives">{selected.totalExplosivesKg} kg</Descriptions.Item>}
                  {selected.explosiveType && <Descriptions.Item label="Explosive Type">{selected.explosiveType}</Descriptions.Item>}
                  {selected.detonatorType && <Descriptions.Item label="Detonator Type">{selected.detonatorType}</Descriptions.Item>}
                  {selected.safetyRadius != null && <Descriptions.Item label="Safety Radius">{selected.safetyRadius} m</Descriptions.Item>}
                  <Descriptions.Item label="Evacuation Confirmed">{selected.evacuationConfirmed ? 'Yes' : 'No'}</Descriptions.Item>
                  <Descriptions.Item label="Sentry Posts Confirmed">{selected.sentryPostsConfirmed ? 'Yes' : 'No'}</Descriptions.Item>
                  <Descriptions.Item label="Pre-Blast Warning">{selected.preBlastWarningGiven ? 'Yes' : 'No'}</Descriptions.Item>
                  {selected.vibrationReading != null && <Descriptions.Item label="Vibration Reading">{selected.vibrationReading}</Descriptions.Item>}
                  {selected.airBlastReading != null && <Descriptions.Item label="Air Blast Reading">{selected.airBlastReading}</Descriptions.Item>}
                  {selected.fragmentationQuality && <Descriptions.Item label="Fragmentation"><Tag color={fragColors[selected.fragmentationQuality]}>{selected.fragmentationQuality}</Tag></Descriptions.Item>}
                  {selected.misfireCount > 0 && <Descriptions.Item label="Misfire Count"><Tag color="#ff3b30">{selected.misfireCount}</Tag></Descriptions.Item>}
                  {selected.blastDesignNotes && <Descriptions.Item label="Design Notes" span={2}>{selected.blastDesignNotes}</Descriptions.Item>}
                  {selected.postBlastInspection && <Descriptions.Item label="Post-Blast Inspection" span={2}>{selected.postBlastInspection}</Descriptions.Item>}
                  {selected.postBlastNotes && <Descriptions.Item label="Post-Blast Notes" span={2}>{selected.postBlastNotes}</Descriptions.Item>}
                </Descriptions>
              ),
            },
            {
              key: 'usages', label: `Explosive Usage (${usages.length})`,
              children: (
                <div>
                  <Button type="primary" size="small" icon={<PlusOutlined />} style={{ marginBottom: 12 }}
                    onClick={() => { usageForm.resetFields(); setUsageModalOpen(true); }}>Record Usage</Button>
                  {usages.map((u: ExplosiveUsage) => (
                    <div key={u.id} style={{ background: '#f8f9fa', padding: 16, borderRadius: 8, marginBottom: 8 }}>
                      <Flex justify="space-between" align="center" style={{ marginBottom: 8 }}>
                        <Text strong>{u.explosiveName}</Text>
                        <Tag>{u.type}</Tag>
                      </Flex>
                      <Text style={{ fontSize: 12, color: '#86868b' }}>
                        Issued: {u.quantityIssued} {u.unit} | Used: {u.quantityUsed} {u.unit} | Returned: {u.quantityReturned} {u.unit}
                      </Text>
                      {u.batchNumber && <div style={{ marginTop: 4 }}><Text style={{ fontSize: 12, color: '#86868b' }}>Batch: {u.batchNumber}</Text></div>}
                      {u.magazineSource && <div><Text style={{ fontSize: 12, color: '#86868b' }}>Magazine: {u.magazineSource}</Text></div>}
                      {u.issuedBy && <div><Text style={{ fontSize: 12, color: '#86868b' }}>Issued by: {u.issuedBy}</Text></div>}
                      {u.notes && <div style={{ marginTop: 4 }}><Text style={{ fontSize: 12 }}>{u.notes}</Text></div>}
                    </div>
                  ))}
                </div>
              ),
            },
          ]} />
        )}
      </Drawer>

      {/* Explosive Usage Modal */}
      <Modal title="Record Explosive Usage" open={usageModalOpen} onCancel={() => setUsageModalOpen(false)}
        onOk={handleUsageSubmit} confirmLoading={usageMutation.isPending}>
        <Form form={usageForm} layout="vertical">
          <Flex gap={16}>
            <Form.Item name="explosiveName" label="Explosive Name" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Input />
            </Form.Item>
            <Form.Item name="type" label="Type" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select options={explosiveTypes.map(t => ({ label: t, value: t }))} />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="quantityIssued" label="Qty Issued" rules={[{ required: true }]} style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} min={0} precision={3} />
            </Form.Item>
            <Form.Item name="quantityUsed" label="Qty Used" rules={[{ required: true }]} style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} min={0} precision={3} />
            </Form.Item>
            <Form.Item name="quantityReturned" label="Qty Returned" rules={[{ required: true }]} style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} min={0} precision={3} />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="unit" label="Unit" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select options={units.map(u => ({ label: u, value: u }))} />
            </Form.Item>
            <Form.Item name="batchNumber" label="Batch Number" style={{ flex: 1 }}>
              <Input />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="magazineSource" label="Magazine Source" style={{ flex: 1 }}>
              <Input />
            </Form.Item>
            <Form.Item name="issuedBy" label="Issued By" style={{ flex: 1 }}>
              <Input />
            </Form.Item>
          </Flex>
          <Form.Item name="receivedBy" label="Received By">
            <Input />
          </Form.Item>
          <Form.Item name="notes" label="Notes">
            <TextArea rows={2} />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}
