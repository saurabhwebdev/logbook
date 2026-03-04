import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Table, Button, Modal, Form, Input, Select, DatePicker, Tag, Drawer,
  Typography, Flex, InputNumber, message, Popconfirm, Descriptions, Tabs,
} from 'antd';
import { PlusOutlined, EyeOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import { productionApi } from '../api/productionApi';
import { mineSitesApi } from '../api/mineSitesApi';
import type { ProductionLog, DispatchRecord } from '../types';
import type {
  CreateProductionLogRequest, UpdateProductionLogRequest,
  CreateDispatchRecordRequest, UpdateDispatchRecordRequest,
} from '../api/productionApi';

const { Title, Text } = Typography;
const { TextArea } = Input;

const materials = ['Ore', 'Waste', 'Topsoil', 'Overburden', 'Coal', 'Other'];
const prodStatuses = ['Draft', 'Submitted', 'Verified', 'Approved'];
const dispatchStatuses = ['Loading', 'InTransit', 'Delivered', 'Cancelled'];
const units = ['Tonnes', 'CubicMeters'];

const prodStatusColors: Record<string, string> = { Draft: '#86868b', Submitted: '#0071e3', Verified: '#ff9500', Approved: '#34c759' };
const dispatchStatusColors: Record<string, string> = { Loading: '#ff9500', InTransit: '#0071e3', Delivered: '#34c759', Cancelled: '#86868b' };

export default function ProductionPage() {
  const [activeTab, setActiveTab] = useState('production');
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<ProductionLog | null>(null);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [selected, setSelected] = useState<ProductionLog | null>(null);
  const [dModalOpen, setDModalOpen] = useState(false);
  const [editingD, setEditingD] = useState<DispatchRecord | null>(null);
  const [dDrawerOpen, setDDrawerOpen] = useState(false);
  const [selectedD, setSelectedD] = useState<DispatchRecord | null>(null);
  const [filterMaterial, setFilterMaterial] = useState<string | undefined>();
  const [filterDStatus, setFilterDStatus] = useState<string | undefined>();
  const [form] = Form.useForm();
  const [dForm] = Form.useForm();
  const qc = useQueryClient();

  const { data: logs = [], isLoading } = useQuery({
    queryKey: ['productionLogs', filterMaterial],
    queryFn: () => productionApi.getLogs(undefined, undefined, undefined, filterMaterial),
  });

  const { data: dispatches = [], isLoading: dLoading } = useQuery({
    queryKey: ['dispatchRecords', filterDStatus],
    queryFn: () => productionApi.getDispatch(undefined, filterDStatus),
  });

  const { data: mineSites = [] } = useQuery({
    queryKey: ['mineSites'],
    queryFn: () => mineSitesApi.getMineSites(),
  });

  // Production mutations
  const createMut = useMutation({
    mutationFn: (d: CreateProductionLogRequest) => productionApi.createLog(d),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['productionLogs'] }); setModalOpen(false); form.resetFields(); message.success('Production log created'); },
  });
  const updateMut = useMutation({
    mutationFn: (d: UpdateProductionLogRequest) => productionApi.updateLog(d.id, d),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['productionLogs'] }); setModalOpen(false); setEditing(null); form.resetFields(); message.success('Production log updated'); },
  });
  const deleteMut = useMutation({
    mutationFn: (id: string) => productionApi.deleteLog(id),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['productionLogs'] }); message.success('Production log deleted'); },
  });

  // Dispatch mutations
  const createDMut = useMutation({
    mutationFn: (d: CreateDispatchRecordRequest) => productionApi.createDispatch(d),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['dispatchRecords'] }); setDModalOpen(false); dForm.resetFields(); message.success('Dispatch record created'); },
  });
  const updateDMut = useMutation({
    mutationFn: (d: UpdateDispatchRecordRequest) => productionApi.updateDispatch(d.id, d),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['dispatchRecords'] }); setDModalOpen(false); setEditingD(null); dForm.resetFields(); message.success('Dispatch record updated'); },
  });
  const deleteDMut = useMutation({
    mutationFn: (id: string) => productionApi.deleteDispatch(id),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['dispatchRecords'] }); message.success('Dispatch record deleted'); },
  });

  const handleSubmit = async () => {
    const values = await form.validateFields();
    const payload = {
      ...values,
      date: values.date?.toISOString(),
      verifiedAt: values.verifiedAt?.toISOString(),
    };
    if (editing) updateMut.mutate({ ...payload, id: editing.id });
    else createMut.mutate(payload);
  };

  const handleDSubmit = async () => {
    const values = await dForm.validateFields();
    const payload = {
      ...values,
      date: values.date?.toISOString(),
      departureTime: values.departureTime?.toISOString(),
      arrivalTime: values.arrivalTime?.toISOString(),
    };
    if (editingD) updateDMut.mutate({ ...payload, id: editingD.id });
    else createDMut.mutate(payload);
  };

  const openEdit = (record: ProductionLog) => {
    setEditing(record);
    form.setFieldsValue({
      ...record,
      date: dayjs(record.date),
      verifiedAt: record.verifiedAt ? dayjs(record.verifiedAt) : undefined,
    });
    setModalOpen(true);
  };

  const openEditD = (record: DispatchRecord) => {
    setEditingD(record);
    dForm.setFieldsValue({
      ...record,
      date: dayjs(record.date),
      departureTime: record.departureTime ? dayjs(record.departureTime) : undefined,
      arrivalTime: record.arrivalTime ? dayjs(record.arrivalTime) : undefined,
    });
    setDModalOpen(true);
  };

  const logColumns = [
    { title: 'Log #', dataIndex: 'logNumber', key: 'logNumber', width: 100 },
    { title: 'Date', dataIndex: 'date', key: 'date', width: 110, render: (d: string) => dayjs(d).format('DD/MM/YYYY') },
    { title: 'Material', dataIndex: 'material', key: 'material', width: 100 },
    { title: 'Tonnes', dataIndex: 'quantityTonnes', key: 'quantityTonnes', width: 100, render: (v: number) => v.toLocaleString() },
    { title: 'Source', dataIndex: 'sourceLocation', key: 'sourceLocation', width: 130, ellipsis: true },
    { title: 'Destination', dataIndex: 'destinationLocation', key: 'destinationLocation', width: 130, ellipsis: true },
    { title: 'Mine Site', dataIndex: 'mineSiteName', key: 'mineSiteName', width: 130 },
    { title: 'Operator', dataIndex: 'operatorName', key: 'operatorName', width: 120 },
    { title: 'Status', dataIndex: 'status', key: 'status', width: 100, render: (s: string) => <Tag color={prodStatusColors[s]}>{s}</Tag> },
    {
      title: 'Actions', key: 'actions', width: 130,
      render: (_: unknown, record: ProductionLog) => (
        <Flex gap={4}>
          <Button type="text" size="small" icon={<EyeOutlined />} onClick={() => { setSelected(record); setDrawerOpen(true); }} />
          <Button type="text" size="small" icon={<EditOutlined />} onClick={() => openEdit(record)} />
          <Popconfirm title="Delete this production log?" onConfirm={() => deleteMut.mutate(record.id)}>
            <Button type="text" size="small" danger icon={<DeleteOutlined />} />
          </Popconfirm>
        </Flex>
      ),
    },
  ];

  const dispatchColumns = [
    { title: 'Dispatch #', dataIndex: 'dispatchNumber', key: 'dispatchNumber', width: 110 },
    { title: 'Date', dataIndex: 'date', key: 'date', width: 110, render: (d: string) => dayjs(d).format('DD/MM/YYYY') },
    { title: 'Vehicle', dataIndex: 'vehicleNumber', key: 'vehicleNumber', width: 110 },
    { title: 'Driver', dataIndex: 'driverName', key: 'driverName', width: 120 },
    { title: 'Material', dataIndex: 'material', key: 'material', width: 100 },
    { title: 'Source', dataIndex: 'sourceLocation', key: 'sourceLocation', width: 130, ellipsis: true },
    { title: 'Destination', dataIndex: 'destinationLocation', key: 'destinationLocation', width: 130, ellipsis: true },
    { title: 'Net Weight', key: 'netWeight', width: 110, render: (_: unknown, r: DispatchRecord) => r.netWeight != null ? `${r.netWeight.toLocaleString()} ${r.unit}` : '-' },
    { title: 'Mine Site', dataIndex: 'mineSiteName', key: 'mineSiteName', width: 130 },
    { title: 'Status', dataIndex: 'status', key: 'status', width: 110, render: (s: string) => <Tag color={dispatchStatusColors[s]}>{s}</Tag> },
    {
      title: 'Actions', key: 'actions', width: 130,
      render: (_: unknown, record: DispatchRecord) => (
        <Flex gap={4}>
          <Button type="text" size="small" icon={<EyeOutlined />} onClick={() => { setSelectedD(record); setDDrawerOpen(true); }} />
          <Button type="text" size="small" icon={<EditOutlined />} onClick={() => openEditD(record)} />
          <Popconfirm title="Delete this dispatch record?" onConfirm={() => deleteDMut.mutate(record.id)}>
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
          <Title level={3} style={{ margin: 0, fontWeight: 700, letterSpacing: -0.5 }}>Production & Dispatch</Title>
          <Text style={{ color: '#86868b', fontSize: 13 }}>Manage production logs and material dispatch records</Text>
        </div>
      </Flex>

      <Tabs activeKey={activeTab} onChange={setActiveTab} items={[
        {
          key: 'production', label: 'Production Logs',
          children: (
            <>
              <Flex gap={8} style={{ marginBottom: 16 }} justify="flex-end">
                <Select placeholder="Filter by material" allowClear style={{ width: 180 }} onChange={setFilterMaterial} options={materials.map(m => ({ label: m, value: m }))} />
                <Button type="primary" icon={<PlusOutlined />} onClick={() => { setEditing(null); form.resetFields(); setModalOpen(true); }}>Add Production Log</Button>
              </Flex>
              <Table dataSource={logs} columns={logColumns} rowKey="id" loading={isLoading} size="middle"
                style={{ background: '#fff', borderRadius: 12, overflow: 'hidden' }}
                pagination={{ pageSize: 15, showSizeChanger: false }} />
            </>
          ),
        },
        {
          key: 'dispatch', label: 'Dispatch Records',
          children: (
            <>
              <Flex gap={8} style={{ marginBottom: 16 }} justify="flex-end">
                <Select placeholder="Filter by status" allowClear style={{ width: 180 }} onChange={setFilterDStatus} options={dispatchStatuses.map(s => ({ label: s, value: s }))} />
                <Button type="primary" icon={<PlusOutlined />} onClick={() => { setEditingD(null); dForm.resetFields(); setDModalOpen(true); }}>Add Dispatch Record</Button>
              </Flex>
              <Table dataSource={dispatches} columns={dispatchColumns} rowKey="id" loading={dLoading} size="middle"
                style={{ background: '#fff', borderRadius: 12, overflow: 'hidden' }}
                pagination={{ pageSize: 15, showSizeChanger: false }} />
            </>
          ),
        },
      ]} />

      {/* Production Log Create/Edit Modal */}
      <Modal title={editing ? 'Edit Production Log' : 'Add Production Log'} open={modalOpen} onCancel={() => { setModalOpen(false); setEditing(null); }}
        onOk={handleSubmit} width={720} okText={editing ? 'Update' : 'Create'}
        confirmLoading={createMut.isPending || updateMut.isPending}>
        <Form form={form} layout="vertical" style={{ maxHeight: 500, overflow: 'auto', paddingRight: 8 }}>
          <Flex gap={16}>
            <Form.Item name="mineSiteId" label="Mine Site" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select placeholder="Select mine site" disabled={!!editing} options={mineSites.map(m => ({ label: m.name, value: m.id }))} />
            </Form.Item>
            <Form.Item name="material" label="Material" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select options={materials.map(m => ({ label: m, value: m }))} />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="date" label="Date" rules={[{ required: true }]} style={{ flex: 1 }}>
              <DatePicker style={{ width: '100%' }} />
            </Form.Item>
            <Form.Item name="shiftName" label="Shift" style={{ flex: 1 }}>
              <Input />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="sourceLocation" label="Source Location" style={{ flex: 1 }}><Input /></Form.Item>
            <Form.Item name="destinationLocation" label="Destination" style={{ flex: 1 }}><Input /></Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="quantityTonnes" label="Quantity (Tonnes)" rules={[{ required: true }]} style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} min={0} />
            </Form.Item>
            <Form.Item name="quantityBCM" label="Quantity (BCM)" style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} min={0} />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="equipmentUsed" label="Equipment Used" style={{ flex: 1 }}><Input /></Form.Item>
            <Form.Item name="operatorName" label="Operator" style={{ flex: 1 }}><Input /></Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="haulingDistance" label="Hauling Distance (km)" style={{ flex: 1 }}><InputNumber style={{ width: '100%' }} min={0} /></Form.Item>
            <Form.Item name="loadCount" label="Load Count" style={{ flex: 1 }}><InputNumber style={{ width: '100%' }} min={0} /></Form.Item>
          </Flex>
          {editing && (
            <>
              <Flex gap={16}>
                <Form.Item name="status" label="Status" style={{ flex: 1 }}>
                  <Select options={prodStatuses.map(s => ({ label: s, value: s }))} />
                </Form.Item>
                <Form.Item name="verifiedBy" label="Verified By" style={{ flex: 1 }}>
                  <Input />
                </Form.Item>
              </Flex>
              <Form.Item name="verifiedAt" label="Verified At">
                <DatePicker showTime style={{ width: '100%' }} />
              </Form.Item>
            </>
          )}
          <Form.Item name="notes" label="Notes"><TextArea rows={2} /></Form.Item>
        </Form>
      </Modal>

      {/* Production Log Detail Drawer */}
      <Drawer title={selected?.logNumber} open={drawerOpen} onClose={() => { setDrawerOpen(false); setSelected(null); }} width={600}>
        {selected && (
          <Descriptions column={2} size="small" bordered>
            <Descriptions.Item label="Log #">{selected.logNumber}</Descriptions.Item>
            <Descriptions.Item label="Status"><Tag color={prodStatusColors[selected.status]}>{selected.status}</Tag></Descriptions.Item>
            <Descriptions.Item label="Date">{dayjs(selected.date).format('DD/MM/YYYY')}</Descriptions.Item>
            <Descriptions.Item label="Material">{selected.material}</Descriptions.Item>
            <Descriptions.Item label="Mine Site">{selected.mineSiteName}</Descriptions.Item>
            {selected.mineAreaName && <Descriptions.Item label="Area">{selected.mineAreaName}</Descriptions.Item>}
            <Descriptions.Item label="Tonnes">{selected.quantityTonnes.toLocaleString()}</Descriptions.Item>
            {selected.quantityBCM != null && <Descriptions.Item label="BCM">{selected.quantityBCM.toLocaleString()}</Descriptions.Item>}
            {selected.sourceLocation && <Descriptions.Item label="Source">{selected.sourceLocation}</Descriptions.Item>}
            {selected.destinationLocation && <Descriptions.Item label="Destination">{selected.destinationLocation}</Descriptions.Item>}
            {selected.equipmentUsed && <Descriptions.Item label="Equipment">{selected.equipmentUsed}</Descriptions.Item>}
            {selected.operatorName && <Descriptions.Item label="Operator">{selected.operatorName}</Descriptions.Item>}
            {selected.haulingDistance != null && <Descriptions.Item label="Haul Distance">{selected.haulingDistance} km</Descriptions.Item>}
            {selected.loadCount != null && <Descriptions.Item label="Loads">{selected.loadCount}</Descriptions.Item>}
            {selected.shiftName && <Descriptions.Item label="Shift">{selected.shiftName}</Descriptions.Item>}
            {selected.verifiedBy && <Descriptions.Item label="Verified By">{selected.verifiedBy}</Descriptions.Item>}
            {selected.verifiedAt && <Descriptions.Item label="Verified At">{dayjs(selected.verifiedAt).format('DD/MM/YYYY HH:mm')}</Descriptions.Item>}
            {selected.notes && <Descriptions.Item label="Notes" span={2}>{selected.notes}</Descriptions.Item>}
          </Descriptions>
        )}
      </Drawer>

      {/* Dispatch Create/Edit Modal */}
      <Modal title={editingD ? 'Edit Dispatch Record' : 'Add Dispatch Record'} open={dModalOpen} onCancel={() => { setDModalOpen(false); setEditingD(null); }}
        onOk={handleDSubmit} width={720} okText={editingD ? 'Update' : 'Create'}
        confirmLoading={createDMut.isPending || updateDMut.isPending}>
        <Form form={dForm} layout="vertical" style={{ maxHeight: 500, overflow: 'auto', paddingRight: 8 }}>
          <Flex gap={16}>
            <Form.Item name="mineSiteId" label="Mine Site" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select placeholder="Select mine site" disabled={!!editingD} options={mineSites.map(m => ({ label: m.name, value: m.id }))} />
            </Form.Item>
            <Form.Item name="date" label="Date" rules={[{ required: true }]} style={{ flex: 1 }}>
              <DatePicker style={{ width: '100%' }} />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="vehicleNumber" label="Vehicle Number" rules={[{ required: true }]} style={{ flex: 1 }}><Input /></Form.Item>
            <Form.Item name="driverName" label="Driver Name" style={{ flex: 1 }}><Input /></Form.Item>
          </Flex>
          <Form.Item name="material" label="Material" rules={[{ required: true }]}>
            <Select options={materials.map(m => ({ label: m, value: m }))} />
          </Form.Item>
          <Flex gap={16}>
            <Form.Item name="sourceLocation" label="Source Location" rules={[{ required: true }]} style={{ flex: 1 }}><Input /></Form.Item>
            <Form.Item name="destinationLocation" label="Destination" rules={[{ required: true }]} style={{ flex: 1 }}><Input /></Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="grossWeight" label="Gross Weight" style={{ flex: 1 }}><InputNumber style={{ width: '100%' }} min={0} /></Form.Item>
            <Form.Item name="tareWeight" label="Tare Weight" style={{ flex: 1 }}><InputNumber style={{ width: '100%' }} min={0} /></Form.Item>
            <Form.Item name="netWeight" label="Net Weight" style={{ flex: 1 }}><InputNumber style={{ width: '100%' }} min={0} /></Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="unit" label="Unit" style={{ flex: 1 }}>
              <Select options={units.map(u => ({ label: u, value: u }))} />
            </Form.Item>
            <Form.Item name="weighbridgeTicketNumber" label="Weighbridge Ticket #" style={{ flex: 1 }}><Input /></Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="departureTime" label="Departure Time" style={{ flex: 1 }}><DatePicker showTime style={{ width: '100%' }} /></Form.Item>
            <Form.Item name="arrivalTime" label="Arrival Time" style={{ flex: 1 }}><DatePicker showTime style={{ width: '100%' }} /></Form.Item>
          </Flex>
          {editingD && (
            <Form.Item name="status" label="Status">
              <Select options={dispatchStatuses.map(s => ({ label: s, value: s }))} />
            </Form.Item>
          )}
          <Form.Item name="notes" label="Notes"><TextArea rows={2} /></Form.Item>
        </Form>
      </Modal>

      {/* Dispatch Detail Drawer */}
      <Drawer title={selectedD?.dispatchNumber} open={dDrawerOpen} onClose={() => { setDDrawerOpen(false); setSelectedD(null); }} width={600}>
        {selectedD && (
          <Descriptions column={2} size="small" bordered>
            <Descriptions.Item label="Dispatch #">{selectedD.dispatchNumber}</Descriptions.Item>
            <Descriptions.Item label="Status"><Tag color={dispatchStatusColors[selectedD.status]}>{selectedD.status}</Tag></Descriptions.Item>
            <Descriptions.Item label="Date">{dayjs(selectedD.date).format('DD/MM/YYYY')}</Descriptions.Item>
            <Descriptions.Item label="Mine Site">{selectedD.mineSiteName}</Descriptions.Item>
            <Descriptions.Item label="Vehicle">{selectedD.vehicleNumber}</Descriptions.Item>
            {selectedD.driverName && <Descriptions.Item label="Driver">{selectedD.driverName}</Descriptions.Item>}
            <Descriptions.Item label="Material">{selectedD.material}</Descriptions.Item>
            <Descriptions.Item label="Unit">{selectedD.unit}</Descriptions.Item>
            <Descriptions.Item label="Source">{selectedD.sourceLocation}</Descriptions.Item>
            <Descriptions.Item label="Destination">{selectedD.destinationLocation}</Descriptions.Item>
            {selectedD.grossWeight != null && <Descriptions.Item label="Gross Weight">{selectedD.grossWeight.toLocaleString()}</Descriptions.Item>}
            {selectedD.tareWeight != null && <Descriptions.Item label="Tare Weight">{selectedD.tareWeight.toLocaleString()}</Descriptions.Item>}
            {selectedD.netWeight != null && <Descriptions.Item label="Net Weight">{selectedD.netWeight.toLocaleString()}</Descriptions.Item>}
            {selectedD.weighbridgeTicketNumber && <Descriptions.Item label="Ticket #">{selectedD.weighbridgeTicketNumber}</Descriptions.Item>}
            {selectedD.departureTime && <Descriptions.Item label="Departure">{dayjs(selectedD.departureTime).format('DD/MM/YYYY HH:mm')}</Descriptions.Item>}
            {selectedD.arrivalTime && <Descriptions.Item label="Arrival">{dayjs(selectedD.arrivalTime).format('DD/MM/YYYY HH:mm')}</Descriptions.Item>}
            {selectedD.notes && <Descriptions.Item label="Notes" span={2}>{selectedD.notes}</Descriptions.Item>}
          </Descriptions>
        )}
      </Drawer>
    </div>
  );
}
