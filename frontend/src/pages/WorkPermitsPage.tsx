import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Table, Button, Modal, Form, Input, Select, DatePicker, Tag, Drawer,
  Typography, Flex, Switch, message, Popconfirm, Descriptions,
} from 'antd';
import { PlusOutlined, EyeOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import { workPermitApi } from '../api/workPermitApi';
import { mineSitesApi } from '../api/mineSitesApi';
import type { WorkPermit } from '../types';
import type { CreateWorkPermitRequest, UpdateWorkPermitRequest } from '../api/workPermitApi';

const { Title, Text } = Typography;
const { TextArea } = Input;

const permitTypes = ['HotWork', 'ConfinedSpace', 'WorkingAtHeight', 'Electrical', 'Excavation', 'CraneLifting', 'Demolition', 'Other'];
const statuses = ['Draft', 'Pending', 'Approved', 'Active', 'Completed', 'Rejected', 'Expired', 'Cancelled'];

const statusColors: Record<string, string> = {
  Draft: '#86868b', Pending: '#ff9500', Approved: '#34c759', Active: '#0071e3',
  Completed: '#6e6e73', Rejected: '#ff3b30', Expired: '#af52de', Cancelled: '#ff453a',
};
const permitTypeColors: Record<string, string> = {
  HotWork: '#ff3b30', ConfinedSpace: '#ff9500', WorkingAtHeight: '#af52de', Electrical: '#ffcc00',
  Excavation: '#8b6914', CraneLifting: '#0071e3', Demolition: '#86868b', Other: '#6e6e73',
};

export default function WorkPermitsPage() {
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<WorkPermit | null>(null);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [selected, setSelected] = useState<WorkPermit | null>(null);
  const [filterStatus, setFilterStatus] = useState<string | undefined>();
  const [filterType, setFilterType] = useState<string | undefined>();
  const [form] = Form.useForm();
  const queryClient = useQueryClient();

  const { data: workPermits = [], isLoading } = useQuery({
    queryKey: ['workPermits', filterStatus, filterType],
    queryFn: () => workPermitApi.getAll(undefined, filterStatus, filterType),
  });

  const { data: mineSites = [] } = useQuery({
    queryKey: ['mineSites'],
    queryFn: () => mineSitesApi.getMineSites(),
  });

  const createMutation = useMutation({
    mutationFn: (data: CreateWorkPermitRequest) => workPermitApi.create(data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['workPermits'] }); setModalOpen(false); form.resetFields(); message.success('Work permit created'); },
  });

  const updateMutation = useMutation({
    mutationFn: (data: UpdateWorkPermitRequest) => workPermitApi.update(data.id, data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['workPermits'] }); setModalOpen(false); setEditing(null); form.resetFields(); message.success('Work permit updated'); },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => workPermitApi.delete(id),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['workPermits'] }); message.success('Work permit deleted'); },
  });

  const handleSubmit = async () => {
    const values = await form.validateFields();
    const payload = {
      ...values,
      requestDate: values.requestDate?.toISOString(),
      startDateTime: values.startDateTime?.toISOString(),
      endDateTime: values.endDateTime?.toISOString(),
      approvedAt: values.approvedAt?.toISOString(),
      closedAt: values.closedAt?.toISOString(),
    };
    if (editing) {
      updateMutation.mutate({ ...payload, id: editing.id });
    } else {
      createMutation.mutate(payload);
    }
  };

  const openEdit = (record: WorkPermit) => {
    setEditing(record);
    form.setFieldsValue({
      ...record,
      requestDate: dayjs(record.requestDate),
      startDateTime: dayjs(record.startDateTime),
      endDateTime: dayjs(record.endDateTime),
      approvedAt: record.approvedAt ? dayjs(record.approvedAt) : undefined,
      closedAt: record.closedAt ? dayjs(record.closedAt) : undefined,
    });
    setModalOpen(true);
  };

  const columns = [
    { title: 'Permit #', dataIndex: 'permitNumber', key: 'permitNumber', width: 120 },
    { title: 'Title', dataIndex: 'title', key: 'title', ellipsis: true },
    { title: 'Type', dataIndex: 'permitType', key: 'permitType', width: 140, render: (t: string) => <Tag color={permitTypeColors[t]}>{t}</Tag> },
    { title: 'Mine Site', dataIndex: 'mineSiteName', key: 'mineSiteName', width: 150 },
    { title: 'Requested By', dataIndex: 'requestedBy', key: 'requestedBy', width: 140 },
    { title: 'Start', dataIndex: 'startDateTime', key: 'startDateTime', width: 130, render: (d: string) => dayjs(d).format('DD/MM/YYYY HH:mm') },
    { title: 'End', dataIndex: 'endDateTime', key: 'endDateTime', width: 130, render: (d: string) => dayjs(d).format('DD/MM/YYYY HH:mm') },
    { title: 'Status', dataIndex: 'status', key: 'status', width: 110, render: (s: string) => <Tag color={statusColors[s]}>{s}</Tag> },
    {
      title: 'Actions', key: 'actions', width: 130,
      render: (_: unknown, record: WorkPermit) => (
        <Flex gap={4}>
          <Button type="text" size="small" icon={<EyeOutlined />} onClick={() => { setSelected(record); setDrawerOpen(true); }} />
          <Button type="text" size="small" icon={<EditOutlined />} onClick={() => openEdit(record)} />
          <Popconfirm title="Delete this work permit?" onConfirm={() => deleteMutation.mutate(record.id)}>
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
          <Title level={3} style={{ margin: 0, fontWeight: 700, letterSpacing: -0.5 }}>Permit to Work</Title>
          <Text style={{ color: '#86868b', fontSize: 13 }}>Manage work permits, approvals, and safety controls for hazardous activities</Text>
        </div>
        <Flex gap={8}>
          <Select placeholder="Filter by status" allowClear style={{ width: 160 }} onChange={setFilterStatus} options={statuses.map(s => ({ label: s, value: s }))} />
          <Select placeholder="Filter by type" allowClear style={{ width: 180 }} onChange={setFilterType} options={permitTypes.map(t => ({ label: t, value: t }))} />
          <Button type="primary" icon={<PlusOutlined />} onClick={() => { setEditing(null); form.resetFields(); setModalOpen(true); }}>New Permit</Button>
        </Flex>
      </Flex>

      <Table dataSource={workPermits} columns={columns} rowKey="id" loading={isLoading} size="middle"
        style={{ background: '#fff', borderRadius: 12, overflow: 'hidden' }}
        pagination={{ pageSize: 15, showSizeChanger: false }} />

      {/* Create/Edit Modal */}
      <Modal title={editing ? 'Edit Work Permit' : 'New Work Permit'} open={modalOpen} onCancel={() => { setModalOpen(false); setEditing(null); }}
        onOk={handleSubmit} width={720} okText={editing ? 'Update' : 'Create'}
        confirmLoading={createMutation.isPending || updateMutation.isPending}>
        <Form form={form} layout="vertical" style={{ maxHeight: 500, overflow: 'auto', paddingRight: 8 }}>
          <Flex gap={16}>
            <Form.Item name="mineSiteId" label="Mine Site" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select placeholder="Select mine site" disabled={!!editing} options={mineSites.map(m => ({ label: m.name, value: m.id }))} />
            </Form.Item>
            <Form.Item name="permitType" label="Permit Type" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select options={permitTypes.map(t => ({ label: t, value: t }))} />
            </Form.Item>
          </Flex>
          <Form.Item name="title" label="Title" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Flex gap={16}>
            <Form.Item name="requestedBy" label="Requested By" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Input />
            </Form.Item>
            <Form.Item name="requestDate" label="Request Date" rules={[{ required: true }]} style={{ flex: 1 }}>
              <DatePicker style={{ width: '100%' }} />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="startDateTime" label="Start Date & Time" rules={[{ required: true }]} style={{ flex: 1 }}>
              <DatePicker showTime style={{ width: '100%' }} />
            </Form.Item>
            <Form.Item name="endDateTime" label="End Date & Time" rules={[{ required: true }]} style={{ flex: 1 }}>
              <DatePicker showTime style={{ width: '100%' }} />
            </Form.Item>
          </Flex>
          <Form.Item name="location" label="Location" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item name="workDescription" label="Work Description" rules={[{ required: true }]}>
            <TextArea rows={3} />
          </Form.Item>
          <Form.Item name="hazardsIdentified" label="Hazards Identified">
            <TextArea rows={2} />
          </Form.Item>
          <Form.Item name="controlMeasures" label="Control Measures">
            <TextArea rows={2} />
          </Form.Item>
          <Flex gap={16}>
            <Form.Item name="ppeRequired" label="PPE Required" style={{ flex: 1 }}>
              <Input />
            </Form.Item>
            <Form.Item name="gasTestRequired" label="Gas Test Required" valuePropName="checked" style={{ flex: 1 }}>
              <Switch />
            </Form.Item>
          </Flex>
          <Form.Item name="gasTestResults" label="Gas Test Results">
            <Input />
          </Form.Item>
          <Form.Item name="emergencyProcedures" label="Emergency Procedures">
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
                <Form.Item name="approvedBy" label="Approved By" style={{ flex: 1 }}>
                  <Input />
                </Form.Item>
              </Flex>
              <Flex gap={16}>
                <Form.Item name="approvedAt" label="Approved At" style={{ flex: 1 }}>
                  <DatePicker showTime style={{ width: '100%' }} />
                </Form.Item>
                <Form.Item name="closedBy" label="Closed By" style={{ flex: 1 }}>
                  <Input />
                </Form.Item>
              </Flex>
              <Flex gap={16}>
                <Form.Item name="closedAt" label="Closed At" style={{ flex: 1 }}>
                  <DatePicker showTime style={{ width: '100%' }} />
                </Form.Item>
                <Form.Item name="rejectionReason" label="Rejection Reason" style={{ flex: 1 }}>
                  <Input />
                </Form.Item>
              </Flex>
            </>
          )}
        </Form>
      </Modal>

      {/* Detail Drawer */}
      <Drawer title={selected?.title} open={drawerOpen} onClose={() => { setDrawerOpen(false); setSelected(null); }} width={700}>
        {selected && (
          <Descriptions column={2} size="small" bordered>
            <Descriptions.Item label="Permit #">{selected.permitNumber}</Descriptions.Item>
            <Descriptions.Item label="Status"><Tag color={statusColors[selected.status]}>{selected.status}</Tag></Descriptions.Item>
            <Descriptions.Item label="Type"><Tag color={permitTypeColors[selected.permitType]}>{selected.permitType}</Tag></Descriptions.Item>
            <Descriptions.Item label="Mine Site">{selected.mineSiteName}</Descriptions.Item>
            {selected.mineAreaName && <Descriptions.Item label="Mine Area">{selected.mineAreaName}</Descriptions.Item>}
            <Descriptions.Item label="Requested By">{selected.requestedBy}</Descriptions.Item>
            <Descriptions.Item label="Request Date">{dayjs(selected.requestDate).format('DD/MM/YYYY')}</Descriptions.Item>
            <Descriptions.Item label="Location">{selected.location}</Descriptions.Item>
            <Descriptions.Item label="Start">{dayjs(selected.startDateTime).format('DD/MM/YYYY HH:mm')}</Descriptions.Item>
            <Descriptions.Item label="End">{dayjs(selected.endDateTime).format('DD/MM/YYYY HH:mm')}</Descriptions.Item>
            <Descriptions.Item label="Work Description" span={2}>{selected.workDescription}</Descriptions.Item>
            {selected.hazardsIdentified && <Descriptions.Item label="Hazards Identified" span={2}>{selected.hazardsIdentified}</Descriptions.Item>}
            {selected.controlMeasures && <Descriptions.Item label="Control Measures" span={2}>{selected.controlMeasures}</Descriptions.Item>}
            {selected.ppeRequired && <Descriptions.Item label="PPE Required">{selected.ppeRequired}</Descriptions.Item>}
            <Descriptions.Item label="Gas Test Required">{selected.gasTestRequired ? 'Yes' : 'No'}</Descriptions.Item>
            {selected.gasTestResults && <Descriptions.Item label="Gas Test Results">{selected.gasTestResults}</Descriptions.Item>}
            {selected.emergencyProcedures && <Descriptions.Item label="Emergency Procedures" span={2}>{selected.emergencyProcedures}</Descriptions.Item>}
            {selected.approvedBy && <Descriptions.Item label="Approved By">{selected.approvedBy}</Descriptions.Item>}
            {selected.approvedAt && <Descriptions.Item label="Approved At">{dayjs(selected.approvedAt).format('DD/MM/YYYY HH:mm')}</Descriptions.Item>}
            {selected.closedBy && <Descriptions.Item label="Closed By">{selected.closedBy}</Descriptions.Item>}
            {selected.closedAt && <Descriptions.Item label="Closed At">{dayjs(selected.closedAt).format('DD/MM/YYYY HH:mm')}</Descriptions.Item>}
            {selected.rejectionReason && <Descriptions.Item label="Rejection Reason" span={2}>{selected.rejectionReason}</Descriptions.Item>}
            {selected.notes && <Descriptions.Item label="Notes" span={2}>{selected.notes}</Descriptions.Item>}
            <Descriptions.Item label="Created">{dayjs(selected.createdAt).format('DD/MM/YYYY HH:mm')}</Descriptions.Item>
          </Descriptions>
        )}
      </Drawer>
    </div>
  );
}
