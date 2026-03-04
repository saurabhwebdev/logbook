import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Table, Button, Modal, Form, Input, Select, DatePicker, Tag, Drawer,
  Typography, Flex, InputNumber, message, Popconfirm, Descriptions, Tabs, Badge,
} from 'antd';
import { PlusOutlined, EyeOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import { equipmentApi } from '../api/equipmentApi';
import { mineSitesApi } from '../api/mineSitesApi';
import type { EquipmentItem, MaintenanceRecord } from '../types';
import type {
  CreateEquipmentRequest, UpdateEquipmentRequest,
  CreateMaintenanceRequest, UpdateMaintenanceRequest,
} from '../api/equipmentApi';

const { Title, Text } = Typography;
const { TextArea } = Input;

const categories = ['Vehicle', 'HeavyMachinery', 'Drill', 'Crusher', 'Conveyor', 'Pump', 'Electrical', 'Ventilation', 'Safety', 'Other'];
const statuses = ['Operational', 'UnderMaintenance', 'Breakdown', 'Decommissioned', 'Standby'];
const maintenanceTypes = ['Preventive', 'Corrective', 'Predictive', 'Emergency', 'Overhaul', 'Inspection'];
const priorities = ['Critical', 'High', 'Medium', 'Low'];
const maintenanceStatuses = ['Scheduled', 'InProgress', 'Completed', 'Cancelled', 'Overdue'];

const statusColors: Record<string, string> = { Operational: '#34c759', UnderMaintenance: '#ff9500', Breakdown: '#ff3b30', Decommissioned: '#86868b', Standby: '#0071e3' };
const priorityColors: Record<string, string> = { Critical: '#ff3b30', High: '#ff9500', Medium: '#ffcc00', Low: '#34c759' };
const mStatusColors: Record<string, string> = { Scheduled: '#0071e3', InProgress: '#ff9500', Completed: '#34c759', Cancelled: '#86868b', Overdue: '#ff3b30' };

export default function EquipmentPage() {
  const [activeTab, setActiveTab] = useState('equipment');
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<EquipmentItem | null>(null);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [selected, setSelected] = useState<EquipmentItem | null>(null);
  const [mModalOpen, setMModalOpen] = useState(false);
  const [editingM, setEditingM] = useState<MaintenanceRecord | null>(null);
  const [filterStatus, setFilterStatus] = useState<string | undefined>();
  const [form] = Form.useForm();
  const [mForm] = Form.useForm();
  const qc = useQueryClient();

  const { data: equipment = [], isLoading } = useQuery({
    queryKey: ['equipment', filterStatus],
    queryFn: () => equipmentApi.getAll(undefined, undefined, filterStatus),
  });

  const { data: mineSites = [] } = useQuery({
    queryKey: ['mineSites'],
    queryFn: () => mineSitesApi.getMineSites(),
  });

  const { data: maintenanceRecords = [] } = useQuery({
    queryKey: ['maintenanceRecords', selected?.id],
    queryFn: () => equipmentApi.getMaintenanceRecords(selected!.id),
    enabled: !!selected,
  });

  const { data: allMaintenance = [], isLoading: mLoading } = useQuery({
    queryKey: ['allMaintenance'],
    queryFn: () => equipmentApi.getMaintenanceRecords(),
  });

  const createMut = useMutation({
    mutationFn: (d: CreateEquipmentRequest) => equipmentApi.create(d),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['equipment'] }); setModalOpen(false); form.resetFields(); message.success('Equipment added'); },
  });
  const updateMut = useMutation({
    mutationFn: (d: UpdateEquipmentRequest) => equipmentApi.update(d.id, d),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['equipment'] }); setModalOpen(false); setEditing(null); form.resetFields(); message.success('Equipment updated'); },
  });
  const deleteMut = useMutation({
    mutationFn: (id: string) => equipmentApi.delete(id),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['equipment'] }); message.success('Equipment deleted'); },
  });

  const createMMut = useMutation({
    mutationFn: (d: CreateMaintenanceRequest) => equipmentApi.createMaintenance(d.equipmentId, d),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['maintenanceRecords'] }); qc.invalidateQueries({ queryKey: ['allMaintenance'] }); qc.invalidateQueries({ queryKey: ['equipment'] }); setMModalOpen(false); mForm.resetFields(); message.success('Work order created'); },
  });
  const updateMMut = useMutation({
    mutationFn: (d: UpdateMaintenanceRequest) => equipmentApi.updateMaintenance(d.id, d),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['maintenanceRecords'] }); qc.invalidateQueries({ queryKey: ['allMaintenance'] }); setMModalOpen(false); setEditingM(null); mForm.resetFields(); message.success('Work order updated'); },
  });

  const handleSubmit = async () => {
    const values = await form.validateFields();
    const payload = {
      ...values,
      purchaseDate: values.purchaseDate?.toISOString(),
      nextServiceDate: values.nextServiceDate?.toISOString(),
      lastServiceDate: values.lastServiceDate?.toISOString(),
    };
    if (editing) updateMut.mutate({ ...payload, id: editing.id });
    else createMut.mutate(payload);
  };

  const handleMSubmit = async () => {
    const values = await mForm.validateFields();
    const payload = {
      ...values,
      scheduledDate: values.scheduledDate?.toISOString(),
      startedAt: values.startedAt?.toISOString(),
      completedAt: values.completedAt?.toISOString(),
    };
    if (editingM) updateMMut.mutate({ ...payload, id: editingM.id });
    else createMMut.mutate({ ...payload, equipmentId: selected!.id });
  };

  const openEdit = (record: EquipmentItem) => {
    setEditing(record);
    form.setFieldsValue({
      ...record,
      purchaseDate: record.purchaseDate ? dayjs(record.purchaseDate) : undefined,
      nextServiceDate: record.nextServiceDate ? dayjs(record.nextServiceDate) : undefined,
      lastServiceDate: record.lastServiceDate ? dayjs(record.lastServiceDate) : undefined,
    });
    setModalOpen(true);
  };

  const openEditM = (record: MaintenanceRecord) => {
    setEditingM(record);
    mForm.setFieldsValue({
      ...record,
      scheduledDate: dayjs(record.scheduledDate),
      startedAt: record.startedAt ? dayjs(record.startedAt) : undefined,
      completedAt: record.completedAt ? dayjs(record.completedAt) : undefined,
    });
    setMModalOpen(true);
  };

  const eqColumns = [
    { title: 'Asset #', dataIndex: 'assetNumber', key: 'assetNumber', width: 100 },
    { title: 'Name', dataIndex: 'name', key: 'name', ellipsis: true },
    { title: 'Category', dataIndex: 'category', key: 'category', width: 130 },
    { title: 'Make/Model', key: 'makeModel', width: 150, render: (_: unknown, r: EquipmentItem) => [r.make, r.model].filter(Boolean).join(' ') || '-' },
    { title: 'Mine Site', dataIndex: 'mineSiteName', key: 'mineSiteName', width: 130 },
    { title: 'Status', dataIndex: 'status', key: 'status', width: 130, render: (s: string) => <Tag color={statusColors[s]}>{s}</Tag> },
    { title: 'Hours', dataIndex: 'hoursOperated', key: 'hoursOperated', width: 80, render: (h: number | null) => h != null ? h.toLocaleString() : '-' },
    { title: 'WOs', dataIndex: 'maintenanceCount', key: 'maintenanceCount', width: 60, render: (c: number) => <Badge count={c} showZero color="#0071e3" /> },
    {
      title: 'Actions', key: 'actions', width: 130,
      render: (_: unknown, record: EquipmentItem) => (
        <Flex gap={4}>
          <Button type="text" size="small" icon={<EyeOutlined />} onClick={() => { setSelected(record); setDrawerOpen(true); }} />
          <Button type="text" size="small" icon={<EditOutlined />} onClick={() => openEdit(record)} />
          <Popconfirm title="Delete this equipment?" onConfirm={() => deleteMut.mutate(record.id)}>
            <Button type="text" size="small" danger icon={<DeleteOutlined />} />
          </Popconfirm>
        </Flex>
      ),
    },
  ];

  const woColumns = [
    { title: 'WO #', dataIndex: 'workOrderNumber', key: 'workOrderNumber', width: 100 },
    { title: 'Title', dataIndex: 'title', key: 'title', ellipsis: true },
    { title: 'Equipment', dataIndex: 'equipmentName', key: 'equipmentName', width: 150 },
    { title: 'Type', dataIndex: 'maintenanceType', key: 'maintenanceType', width: 110 },
    { title: 'Priority', dataIndex: 'priority', key: 'priority', width: 90, render: (p: string) => <Tag color={priorityColors[p]}>{p}</Tag> },
    { title: 'Scheduled', dataIndex: 'scheduledDate', key: 'scheduledDate', width: 110, render: (d: string) => dayjs(d).format('DD/MM/YYYY') },
    { title: 'Status', dataIndex: 'status', key: 'status', width: 110, render: (s: string) => <Tag color={mStatusColors[s]}>{s}</Tag> },
    { title: 'Performed By', dataIndex: 'performedBy', key: 'performedBy', width: 130 },
    {
      title: 'Actions', key: 'actions', width: 60,
      render: (_: unknown, record: MaintenanceRecord) => (
        <Button type="text" size="small" icon={<EditOutlined />} onClick={() => openEditM(record)} />
      ),
    },
  ];

  return (
    <div>
      <Flex justify="space-between" align="center" style={{ marginBottom: 24 }}>
        <div>
          <Title level={3} style={{ margin: 0, fontWeight: 700, letterSpacing: -0.5 }}>Equipment & Maintenance</Title>
          <Text style={{ color: '#86868b', fontSize: 13 }}>Manage equipment registry and maintenance work orders</Text>
        </div>
      </Flex>

      <Tabs activeKey={activeTab} onChange={setActiveTab} items={[
        {
          key: 'equipment', label: 'Equipment Registry',
          children: (
            <>
              <Flex gap={8} style={{ marginBottom: 16 }} justify="flex-end">
                <Select placeholder="Filter by status" allowClear style={{ width: 180 }} onChange={setFilterStatus} options={statuses.map(s => ({ label: s, value: s }))} />
                <Button type="primary" icon={<PlusOutlined />} onClick={() => { setEditing(null); form.resetFields(); setModalOpen(true); }}>Add Equipment</Button>
              </Flex>
              <Table dataSource={equipment} columns={eqColumns} rowKey="id" loading={isLoading} size="middle"
                style={{ background: '#fff', borderRadius: 12, overflow: 'hidden' }}
                pagination={{ pageSize: 15, showSizeChanger: false }} />
            </>
          ),
        },
        {
          key: 'workorders', label: 'Work Orders',
          children: (
            <Table dataSource={allMaintenance} columns={woColumns} rowKey="id" loading={mLoading} size="middle"
              style={{ background: '#fff', borderRadius: 12, overflow: 'hidden' }}
              pagination={{ pageSize: 15, showSizeChanger: false }} />
          ),
        },
      ]} />

      {/* Equipment Create/Edit Modal */}
      <Modal title={editing ? 'Edit Equipment' : 'Add Equipment'} open={modalOpen} onCancel={() => { setModalOpen(false); setEditing(null); }}
        onOk={handleSubmit} width={720} okText={editing ? 'Update' : 'Add'}
        confirmLoading={createMut.isPending || updateMut.isPending}>
        <Form form={form} layout="vertical" style={{ maxHeight: 500, overflow: 'auto', paddingRight: 8 }}>
          <Flex gap={16}>
            <Form.Item name="mineSiteId" label="Mine Site" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select placeholder="Select mine site" disabled={!!editing} options={mineSites.map(m => ({ label: m.name, value: m.id }))} />
            </Form.Item>
            <Form.Item name="category" label="Category" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select options={categories.map(c => ({ label: c, value: c }))} />
            </Form.Item>
          </Flex>
          <Form.Item name="name" label="Name" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Flex gap={16}>
            <Form.Item name="make" label="Make" style={{ flex: 1 }}><Input /></Form.Item>
            <Form.Item name="model" label="Model" style={{ flex: 1 }}><Input /></Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="serialNumber" label="Serial Number" style={{ flex: 1 }}><Input /></Form.Item>
            <Form.Item name="yearOfManufacture" label="Year" style={{ flex: 1 }}><InputNumber style={{ width: '100%' }} min={1900} max={2030} /></Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="location" label="Location" style={{ flex: 1 }}><Input /></Form.Item>
            <Form.Item name="operatorName" label="Operator" style={{ flex: 1 }}><Input /></Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="hoursOperated" label="Hours Operated" style={{ flex: 1 }}><InputNumber style={{ width: '100%' }} min={0} /></Form.Item>
            <Form.Item name="nextServiceHours" label="Next Service (hours)" style={{ flex: 1 }}><InputNumber style={{ width: '100%' }} min={0} /></Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="purchaseDate" label="Purchase Date" style={{ flex: 1 }}><DatePicker style={{ width: '100%' }} /></Form.Item>
            <Form.Item name="purchaseCost" label="Purchase Cost" style={{ flex: 1 }}><InputNumber style={{ width: '100%' }} min={0} prefix="$" /></Form.Item>
          </Flex>
          <Form.Item name="nextServiceDate" label="Next Service Date"><DatePicker style={{ width: '100%' }} /></Form.Item>
          <Form.Item name="warrantyInfo" label="Warranty Info"><Input /></Form.Item>
          <Form.Item name="notes" label="Notes"><TextArea rows={2} /></Form.Item>
          {editing && (
            <Form.Item name="status" label="Status">
              <Select options={statuses.map(s => ({ label: s, value: s }))} />
            </Form.Item>
          )}
        </Form>
      </Modal>

      {/* Equipment Detail Drawer */}
      <Drawer title={selected?.name} open={drawerOpen} onClose={() => { setDrawerOpen(false); setSelected(null); }} width={700}>
        {selected && (
          <Tabs items={[
            {
              key: 'details', label: 'Details',
              children: (
                <Descriptions column={2} size="small" bordered>
                  <Descriptions.Item label="Asset #">{selected.assetNumber}</Descriptions.Item>
                  <Descriptions.Item label="Status"><Tag color={statusColors[selected.status]}>{selected.status}</Tag></Descriptions.Item>
                  <Descriptions.Item label="Category">{selected.category}</Descriptions.Item>
                  <Descriptions.Item label="Mine Site">{selected.mineSiteName}</Descriptions.Item>
                  {selected.make && <Descriptions.Item label="Make">{selected.make}</Descriptions.Item>}
                  {selected.model && <Descriptions.Item label="Model">{selected.model}</Descriptions.Item>}
                  {selected.serialNumber && <Descriptions.Item label="Serial #">{selected.serialNumber}</Descriptions.Item>}
                  {selected.yearOfManufacture && <Descriptions.Item label="Year">{selected.yearOfManufacture}</Descriptions.Item>}
                  {selected.location && <Descriptions.Item label="Location">{selected.location}</Descriptions.Item>}
                  {selected.operatorName && <Descriptions.Item label="Operator">{selected.operatorName}</Descriptions.Item>}
                  {selected.hoursOperated != null && <Descriptions.Item label="Hours">{selected.hoursOperated.toLocaleString()}</Descriptions.Item>}
                  {selected.nextServiceDate && <Descriptions.Item label="Next Service">{dayjs(selected.nextServiceDate).format('DD/MM/YYYY')}</Descriptions.Item>}
                  {selected.purchaseCost != null && <Descriptions.Item label="Cost">${selected.purchaseCost.toLocaleString()}</Descriptions.Item>}
                  {selected.warrantyInfo && <Descriptions.Item label="Warranty" span={2}>{selected.warrantyInfo}</Descriptions.Item>}
                  {selected.notes && <Descriptions.Item label="Notes" span={2}>{selected.notes}</Descriptions.Item>}
                </Descriptions>
              ),
            },
            {
              key: 'maintenance', label: `Maintenance (${maintenanceRecords.length})`,
              children: (
                <div>
                  <Button type="primary" size="small" icon={<PlusOutlined />} style={{ marginBottom: 12 }}
                    onClick={() => { setEditingM(null); mForm.resetFields(); setMModalOpen(true); }}>Create Work Order</Button>
                  {maintenanceRecords.map((m: MaintenanceRecord) => (
                    <div key={m.id} style={{ background: '#f8f9fa', padding: 16, borderRadius: 8, marginBottom: 8 }}>
                      <Flex justify="space-between" align="center" style={{ marginBottom: 8 }}>
                        <Flex gap={8} align="center">
                          <Text strong>{m.workOrderNumber}</Text>
                          <Tag color={priorityColors[m.priority]}>{m.priority}</Tag>
                          <Tag>{m.maintenanceType}</Tag>
                        </Flex>
                        <Flex gap={4}>
                          <Tag color={mStatusColors[m.status]}>{m.status}</Tag>
                          <Button type="text" size="small" icon={<EditOutlined />} onClick={() => openEditM(m)} />
                        </Flex>
                      </Flex>
                      <Text>{m.title}</Text>
                      <div style={{ marginTop: 4 }}>
                        <Text style={{ fontSize: 12, color: '#86868b' }}>{dayjs(m.scheduledDate).format('DD/MM/YYYY')}{m.performedBy ? ` · ${m.performedBy}` : ''}</Text>
                      </div>
                      {m.downtimeHours != null && <div><Text style={{ fontSize: 12, color: '#ff3b30' }}>Downtime: {m.downtimeHours}h</Text></div>}
                    </div>
                  ))}
                </div>
              ),
            },
          ]} />
        )}
      </Drawer>

      {/* Maintenance Create/Edit Modal */}
      <Modal title={editingM ? 'Edit Work Order' : 'Create Work Order'} open={mModalOpen} onCancel={() => { setMModalOpen(false); setEditingM(null); }}
        onOk={handleMSubmit} width={600} confirmLoading={createMMut.isPending || updateMMut.isPending}>
        <Form form={mForm} layout="vertical" style={{ maxHeight: 500, overflow: 'auto', paddingRight: 8 }}>
          <Flex gap={16}>
            <Form.Item name="maintenanceType" label="Type" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select options={maintenanceTypes.map(t => ({ label: t, value: t }))} />
            </Form.Item>
            <Form.Item name="priority" label="Priority" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select options={priorities.map(p => ({ label: p, value: p }))} />
            </Form.Item>
          </Flex>
          <Form.Item name="title" label="Title" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item name="description" label="Description"><TextArea rows={2} /></Form.Item>
          <Flex gap={16}>
            <Form.Item name="scheduledDate" label="Scheduled Date" rules={[{ required: true }]} style={{ flex: 1 }}>
              <DatePicker style={{ width: '100%' }} />
            </Form.Item>
            <Form.Item name="performedBy" label="Performed By" style={{ flex: 1 }}>
              <Input />
            </Form.Item>
          </Flex>
          {editingM && (
            <>
              <Flex gap={16}>
                <Form.Item name="status" label="Status" style={{ flex: 1 }}>
                  <Select options={maintenanceStatuses.map(s => ({ label: s, value: s }))} />
                </Form.Item>
                <Form.Item name="downtimeHours" label="Downtime (hours)" style={{ flex: 1 }}>
                  <InputNumber style={{ width: '100%' }} min={0} />
                </Form.Item>
              </Flex>
              <Flex gap={16}>
                <Form.Item name="startedAt" label="Started At" style={{ flex: 1 }}><DatePicker showTime style={{ width: '100%' }} /></Form.Item>
                <Form.Item name="completedAt" label="Completed At" style={{ flex: 1 }}><DatePicker showTime style={{ width: '100%' }} /></Form.Item>
              </Flex>
              <Flex gap={16}>
                <Form.Item name="laborCost" label="Labor Cost ($)" style={{ flex: 1 }}><InputNumber style={{ width: '100%' }} min={0} /></Form.Item>
                <Form.Item name="partsCost" label="Parts Cost ($)" style={{ flex: 1 }}><InputNumber style={{ width: '100%' }} min={0} /></Form.Item>
              </Flex>
              <Form.Item name="partsUsed" label="Parts Used"><TextArea rows={2} /></Form.Item>
              <Form.Item name="findings" label="Findings"><TextArea rows={2} /></Form.Item>
              <Form.Item name="actionsTaken" label="Actions Taken"><TextArea rows={2} /></Form.Item>
            </>
          )}
          <Form.Item name="notes" label="Notes"><TextArea rows={2} /></Form.Item>
        </Form>
      </Modal>
    </div>
  );
}
