import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Table, Button, Modal, Form, Input, Select, DatePicker, Tag, Drawer,
  Typography, Flex, message, Popconfirm, Descriptions, Tabs, Badge,
} from 'antd';
import { PlusOutlined, EyeOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import { personnelApi } from '../api/personnelApi';
import { mineSitesApi } from '../api/mineSitesApi';
import type { PersonnelRecord, PersonnelCertification } from '../types';
import type { CreatePersonnelRequest, UpdatePersonnelRequest, CreateCertificationRequest } from '../api/personnelApi';

const { Title, Text } = Typography;
const { TextArea } = Input;

const roles = ['Miner', 'Operator', 'Supervisor', 'Engineer', 'Geologist', 'Electrician', 'Mechanic', 'Safety', 'Blaster', 'Manager', 'Contractor', 'Other'];
const employmentTypes = ['Permanent', 'Contract', 'Casual', 'Apprentice'];
const statuses = ['Active', 'OnLeave', 'Suspended', 'Terminated', 'Retired'];
const certCategories = ['Safety', 'Blasting', 'Electrical', 'FirstAid', 'Equipment', 'Statutory', 'Other'];

const statusColors: Record<string, string> = { Active: '#34c759', OnLeave: '#ff9500', Suspended: '#ff3b30', Terminated: '#86868b', Retired: '#0071e3' };
const certStatusColors: Record<string, string> = { Valid: '#34c759', Expired: '#ff3b30', Revoked: '#86868b', Pending: '#ff9500' };

export default function PersonnelPage() {
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<PersonnelRecord | null>(null);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [selected, setSelected] = useState<PersonnelRecord | null>(null);
  const [certModalOpen, setCertModalOpen] = useState(false);
  const [filterStatus, setFilterStatus] = useState<string | undefined>();
  const [form] = Form.useForm();
  const [certForm] = Form.useForm();
  const qc = useQueryClient();

  const { data: personnel = [], isLoading } = useQuery({
    queryKey: ['personnel', filterStatus],
    queryFn: () => personnelApi.getAll(undefined, filterStatus),
  });

  const { data: mineSites = [] } = useQuery({
    queryKey: ['mineSites'],
    queryFn: () => mineSitesApi.getMineSites(),
  });

  const { data: certifications = [] } = useQuery({
    queryKey: ['certifications', selected?.id],
    queryFn: () => personnelApi.getCertifications(selected!.id),
    enabled: !!selected,
  });

  const createMut = useMutation({
    mutationFn: (d: CreatePersonnelRequest) => personnelApi.create(d),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['personnel'] }); setModalOpen(false); form.resetFields(); message.success('Personnel added'); },
  });
  const updateMut = useMutation({
    mutationFn: (d: UpdatePersonnelRequest) => personnelApi.update(d.id, d),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['personnel'] }); setModalOpen(false); setEditing(null); form.resetFields(); message.success('Personnel updated'); },
  });
  const deleteMut = useMutation({
    mutationFn: (id: string) => personnelApi.delete(id),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['personnel'] }); message.success('Personnel deleted'); },
  });
  const certMut = useMutation({
    mutationFn: (d: CreateCertificationRequest) => personnelApi.createCertification(d.personnelId, d),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['certifications'] }); qc.invalidateQueries({ queryKey: ['personnel'] }); setCertModalOpen(false); certForm.resetFields(); message.success('Certification added'); },
  });

  const handleSubmit = async () => {
    const values = await form.validateFields();
    const payload = {
      ...values,
      dateOfJoining: values.dateOfJoining?.toISOString(),
      dateOfLeaving: values.dateOfLeaving?.toISOString(),
      medicalFitnessExpiry: values.medicalFitnessExpiry?.toISOString(),
    };
    if (editing) updateMut.mutate({ ...payload, id: editing.id });
    else createMut.mutate(payload);
  };

  const handleCertSubmit = async () => {
    const values = await certForm.validateFields();
    certMut.mutate({
      ...values,
      personnelId: selected!.id,
      issueDate: values.issueDate?.toISOString(),
      expiryDate: values.expiryDate?.toISOString(),
    });
  };

  const openEdit = (record: PersonnelRecord) => {
    setEditing(record);
    form.setFieldsValue({
      ...record,
      dateOfJoining: dayjs(record.dateOfJoining),
      dateOfLeaving: record.dateOfLeaving ? dayjs(record.dateOfLeaving) : undefined,
      medicalFitnessExpiry: record.medicalFitnessExpiry ? dayjs(record.medicalFitnessExpiry) : undefined,
    });
    setModalOpen(true);
  };

  const columns = [
    { title: 'Emp #', dataIndex: 'employeeNumber', key: 'employeeNumber', width: 100 },
    { title: 'Name', key: 'name', render: (_: unknown, r: PersonnelRecord) => `${r.firstName} ${r.lastName}` },
    { title: 'Role', dataIndex: 'role', key: 'role', width: 110 },
    { title: 'Mine Site', dataIndex: 'mineSiteName', key: 'mineSiteName', width: 130 },
    { title: 'Type', dataIndex: 'employmentType', key: 'employmentType', width: 100 },
    { title: 'Status', dataIndex: 'status', key: 'status', width: 110, render: (s: string) => <Tag color={statusColors[s]}>{s}</Tag> },
    { title: 'Joined', dataIndex: 'dateOfJoining', key: 'dateOfJoining', width: 100, render: (d: string) => dayjs(d).format('DD/MM/YYYY') },
    { title: 'Certs', dataIndex: 'certificationCount', key: 'certificationCount', width: 60, render: (c: number) => <Badge count={c} showZero color="#0071e3" /> },
    {
      title: 'Actions', key: 'actions', width: 130,
      render: (_: unknown, record: PersonnelRecord) => (
        <Flex gap={4}>
          <Button type="text" size="small" icon={<EyeOutlined />} onClick={() => { setSelected(record); setDrawerOpen(true); }} />
          <Button type="text" size="small" icon={<EditOutlined />} onClick={() => openEdit(record)} />
          <Popconfirm title="Delete this person?" onConfirm={() => deleteMut.mutate(record.id)}>
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
          <Title level={3} style={{ margin: 0, fontWeight: 700, letterSpacing: -0.5 }}>Personnel & Workforce</Title>
          <Text style={{ color: '#86868b', fontSize: 13 }}>Manage mine personnel, certifications, and competencies</Text>
        </div>
        <Flex gap={8}>
          <Select placeholder="Filter by status" allowClear style={{ width: 160 }} onChange={setFilterStatus} options={statuses.map(s => ({ label: s, value: s }))} />
          <Button type="primary" icon={<PlusOutlined />} onClick={() => { setEditing(null); form.resetFields(); setModalOpen(true); }}>Add Personnel</Button>
        </Flex>
      </Flex>

      <Table dataSource={personnel} columns={columns} rowKey="id" loading={isLoading} size="middle"
        style={{ background: '#fff', borderRadius: 12, overflow: 'hidden' }}
        pagination={{ pageSize: 15, showSizeChanger: false }} />

      <Modal title={editing ? 'Edit Personnel' : 'Add Personnel'} open={modalOpen} onCancel={() => { setModalOpen(false); setEditing(null); }}
        onOk={handleSubmit} width={720} okText={editing ? 'Update' : 'Add'}
        confirmLoading={createMut.isPending || updateMut.isPending}>
        <Form form={form} layout="vertical" style={{ maxHeight: 500, overflow: 'auto', paddingRight: 8 }}>
          <Flex gap={16}>
            <Form.Item name="mineSiteId" label="Mine Site" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select placeholder="Select mine site" disabled={!!editing} options={mineSites.map(m => ({ label: m.name, value: m.id }))} />
            </Form.Item>
            <Form.Item name="role" label="Role" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select options={roles.map(r => ({ label: r, value: r }))} />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="firstName" label="First Name" rules={[{ required: true }]} style={{ flex: 1 }}><Input /></Form.Item>
            <Form.Item name="lastName" label="Last Name" rules={[{ required: true }]} style={{ flex: 1 }}><Input /></Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="department" label="Department" style={{ flex: 1 }}><Input /></Form.Item>
            <Form.Item name="designation" label="Designation" style={{ flex: 1 }}><Input /></Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="employmentType" label="Employment Type" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select options={employmentTypes.map(t => ({ label: t, value: t }))} />
            </Form.Item>
            <Form.Item name="dateOfJoining" label="Date of Joining" rules={[{ required: true }]} style={{ flex: 1 }}>
              <DatePicker style={{ width: '100%' }} />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="contactPhone" label="Phone" style={{ flex: 1 }}><Input /></Form.Item>
            <Form.Item name="contactEmail" label="Email" style={{ flex: 1 }}><Input /></Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="emergencyContactName" label="Emergency Contact" style={{ flex: 1 }}><Input /></Form.Item>
            <Form.Item name="emergencyContactPhone" label="Emergency Phone" style={{ flex: 1 }}><Input /></Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="bloodGroup" label="Blood Group" style={{ flex: 1 }}><Input /></Form.Item>
            <Form.Item name="medicalFitnessExpiry" label="Medical Fitness Expiry" style={{ flex: 1 }}><DatePicker style={{ width: '100%' }} /></Form.Item>
          </Flex>
          <Form.Item name="notes" label="Notes"><TextArea rows={2} /></Form.Item>
          {editing && (
            <Flex gap={16}>
              <Form.Item name="status" label="Status" style={{ flex: 1 }}>
                <Select options={statuses.map(s => ({ label: s, value: s }))} />
              </Form.Item>
              <Form.Item name="dateOfLeaving" label="Date of Leaving" style={{ flex: 1 }}>
                <DatePicker style={{ width: '100%' }} />
              </Form.Item>
            </Flex>
          )}
        </Form>
      </Modal>

      <Drawer title={selected ? `${selected.firstName} ${selected.lastName}` : ''} open={drawerOpen} onClose={() => { setDrawerOpen(false); setSelected(null); }} width={700}>
        {selected && (
          <Tabs items={[
            {
              key: 'details', label: 'Details',
              children: (
                <Descriptions column={2} size="small" bordered>
                  <Descriptions.Item label="Employee #">{selected.employeeNumber}</Descriptions.Item>
                  <Descriptions.Item label="Status"><Tag color={statusColors[selected.status]}>{selected.status}</Tag></Descriptions.Item>
                  <Descriptions.Item label="Role">{selected.role}</Descriptions.Item>
                  <Descriptions.Item label="Mine Site">{selected.mineSiteName}</Descriptions.Item>
                  {selected.department && <Descriptions.Item label="Department">{selected.department}</Descriptions.Item>}
                  {selected.designation && <Descriptions.Item label="Designation">{selected.designation}</Descriptions.Item>}
                  <Descriptions.Item label="Employment">{selected.employmentType}</Descriptions.Item>
                  <Descriptions.Item label="Joined">{dayjs(selected.dateOfJoining).format('DD/MM/YYYY')}</Descriptions.Item>
                  {selected.contactPhone && <Descriptions.Item label="Phone">{selected.contactPhone}</Descriptions.Item>}
                  {selected.contactEmail && <Descriptions.Item label="Email">{selected.contactEmail}</Descriptions.Item>}
                  {selected.emergencyContactName && <Descriptions.Item label="Emergency">{selected.emergencyContactName} ({selected.emergencyContactPhone})</Descriptions.Item>}
                  {selected.bloodGroup && <Descriptions.Item label="Blood Group">{selected.bloodGroup}</Descriptions.Item>}
                  {selected.medicalFitnessExpiry && <Descriptions.Item label="Medical Expiry">{dayjs(selected.medicalFitnessExpiry).format('DD/MM/YYYY')}</Descriptions.Item>}
                  {selected.notes && <Descriptions.Item label="Notes" span={2}>{selected.notes}</Descriptions.Item>}
                </Descriptions>
              ),
            },
            {
              key: 'certifications', label: `Certifications (${certifications.length})`,
              children: (
                <div>
                  <Button type="primary" size="small" icon={<PlusOutlined />} style={{ marginBottom: 12 }}
                    onClick={() => { certForm.resetFields(); setCertModalOpen(true); }}>Add Certification</Button>
                  {certifications.map((c: PersonnelCertification) => (
                    <div key={c.id} style={{ background: '#f8f9fa', padding: 16, borderRadius: 8, marginBottom: 8 }}>
                      <Flex justify="space-between" align="center" style={{ marginBottom: 4 }}>
                        <Text strong>{c.certificationName}</Text>
                        <Tag color={certStatusColors[c.status]}>{c.status}</Tag>
                      </Flex>
                      <Text style={{ fontSize: 12, color: '#86868b' }}>
                        {c.issuingAuthority ? `${c.issuingAuthority} · ` : ''}
                        Issued: {dayjs(c.issueDate).format('DD/MM/YYYY')}
                        {c.expiryDate ? ` · Expires: ${dayjs(c.expiryDate).format('DD/MM/YYYY')}` : ''}
                      </Text>
                      {c.certificateNumber && <div><Text style={{ fontSize: 12 }}>#{c.certificateNumber}</Text></div>}
                    </div>
                  ))}
                </div>
              ),
            },
          ]} />
        )}
      </Drawer>

      <Modal title="Add Certification" open={certModalOpen} onCancel={() => setCertModalOpen(false)}
        onOk={handleCertSubmit} confirmLoading={certMut.isPending}>
        <Form form={certForm} layout="vertical">
          <Form.Item name="certificationName" label="Certification Name" rules={[{ required: true }]}><Input /></Form.Item>
          <Flex gap={16}>
            <Form.Item name="certificateNumber" label="Certificate #" style={{ flex: 1 }}><Input /></Form.Item>
            <Form.Item name="issuingAuthority" label="Issuing Authority" style={{ flex: 1 }}><Input /></Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="issueDate" label="Issue Date" rules={[{ required: true }]} style={{ flex: 1 }}><DatePicker style={{ width: '100%' }} /></Form.Item>
            <Form.Item name="expiryDate" label="Expiry Date" style={{ flex: 1 }}><DatePicker style={{ width: '100%' }} /></Form.Item>
          </Flex>
          <Form.Item name="category" label="Category">
            <Select allowClear options={certCategories.map(c => ({ label: c, value: c }))} />
          </Form.Item>
          <Form.Item name="notes" label="Notes"><TextArea rows={2} /></Form.Item>
        </Form>
      </Modal>
    </div>
  );
}
