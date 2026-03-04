import { useState } from 'react';
import {
  Typography,
  Button,
  Table,
  Space,
  Modal,
  Form,
  Input,
  Select,
  InputNumber,
  message,
  Flex,
  Tag,
  Descriptions,
  Drawer,
} from 'antd';
import {
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  ExclamationCircleOutlined,
  EnvironmentOutlined,
  EyeOutlined,
} from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AxiosError } from 'axios';
import dayjs from 'dayjs';
import { mineSitesApi } from '../api/mineSitesApi';
import type { CreateMineSiteRequest, UpdateMineSiteRequest } from '../api/mineSitesApi';
import type { MineSite } from '../types';
import PermissionGate from '../components/PermissionGate';

const { Text, Title } = Typography;

const MINE_TYPES = ['Underground', 'OpenPit', 'Mixed'];
const JURISDICTIONS = [
  { value: 'MSHA', label: 'MSHA (USA)' },
  { value: 'DGMS', label: 'DGMS (India)' },
  { value: 'AU_QLD', label: 'Queensland (Australia)' },
  { value: 'AU_NSW', label: 'New South Wales (Australia)' },
  { value: 'AU_WA', label: 'Western Australia' },
  { value: 'SA_MHSA', label: 'MHSA (South Africa)' },
  { value: 'CANADA_BC', label: 'British Columbia (Canada)' },
  { value: 'CANADA_ON', label: 'Ontario (Canada)' },
  { value: 'CHILE_SERNAGEOMIN', label: 'SERNAGEOMIN (Chile)' },
  { value: 'PERU_OSINERGMIN', label: 'OSINERGMIN (Peru)' },
  { value: 'OTHER', label: 'Other' },
];
const STATUSES = ['Active', 'Suspended', 'Closed', 'UnderConstruction'];
const UNIT_SYSTEMS = ['Metric', 'Imperial'];

interface MineSiteFormValues {
  name: string;
  code?: string;
  mineType: string;
  jurisdiction: string;
  jurisdictionDetails?: string;
  latitude?: number;
  longitude?: number;
  address?: string;
  country?: string;
  state?: string;
  mineralsMined?: string;
  operatingCompany?: string;
  miningLicenseNumber?: string;
  licenseExpiryDate?: string;
  operationalSince?: string;
  status?: string;
  emergencyContactName?: string;
  emergencyContactPhone?: string;
  nearestHospital?: string;
  nearestHospitalPhone?: string;
  nearestHospitalDistanceKm?: number;
  unitSystem?: string;
  timeZone?: string;
  shiftsPerDay?: number;
  shiftPattern?: string;
}

export default function MineSitesPage() {
  const queryClient = useQueryClient();
  const [modalOpen, setModalOpen] = useState(false);
  const [editingSite, setEditingSite] = useState<MineSite | null>(null);
  const [detailSite, setDetailSite] = useState<MineSite | null>(null);
  const [form] = Form.useForm<MineSiteFormValues>();

  const sitesQuery = useQuery({
    queryKey: ['mineSites'],
    queryFn: () => mineSitesApi.getMineSites(),
  });

  const createMutation = useMutation({
    mutationFn: (data: CreateMineSiteRequest) => mineSitesApi.createMineSite(data),
    onSuccess: () => {
      message.success('Mine site created');
      queryClient.invalidateQueries({ queryKey: ['mineSites'] });
      closeModal();
    },
    onError: (error: AxiosError<{ message?: string; title?: string }>) => {
      message.error(error.response?.data?.message || error.response?.data?.title || 'Failed to create mine site');
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateMineSiteRequest }) =>
      mineSitesApi.updateMineSite(id, data),
    onSuccess: () => {
      message.success('Mine site updated');
      queryClient.invalidateQueries({ queryKey: ['mineSites'] });
      closeModal();
    },
    onError: (error: AxiosError<{ message?: string; title?: string }>) => {
      message.error(error.response?.data?.message || error.response?.data?.title || 'Failed to update mine site');
    },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => mineSitesApi.deleteMineSite(id),
    onSuccess: () => {
      message.success('Mine site deleted');
      queryClient.invalidateQueries({ queryKey: ['mineSites'] });
    },
    onError: (error: AxiosError<{ message?: string }>) => {
      message.error(error.response?.data?.message || 'Failed to delete mine site');
    },
  });

  const openCreateModal = () => {
    setEditingSite(null);
    form.resetFields();
    form.setFieldsValue({ status: 'Active', unitSystem: 'Metric', timeZone: 'UTC', shiftsPerDay: 3 });
    setModalOpen(true);
  };

  const openEditModal = (site: MineSite) => {
    setEditingSite(site);
    form.setFieldsValue({
      name: site.name,
      code: site.code ?? undefined,
      mineType: site.mineType,
      jurisdiction: site.jurisdiction,
      jurisdictionDetails: site.jurisdictionDetails ?? undefined,
      latitude: site.latitude ?? undefined,
      longitude: site.longitude ?? undefined,
      address: site.address ?? undefined,
      country: site.country ?? undefined,
      state: site.state ?? undefined,
      mineralsMined: site.mineralsMined ?? undefined,
      operatingCompany: site.operatingCompany ?? undefined,
      miningLicenseNumber: site.miningLicenseNumber ?? undefined,
      status: site.status,
      emergencyContactName: site.emergencyContactName ?? undefined,
      emergencyContactPhone: site.emergencyContactPhone ?? undefined,
      nearestHospital: site.nearestHospital ?? undefined,
      nearestHospitalPhone: site.nearestHospitalPhone ?? undefined,
      nearestHospitalDistanceKm: site.nearestHospitalDistanceKm ?? undefined,
      unitSystem: site.unitSystem,
      timeZone: site.timeZone,
      shiftsPerDay: site.shiftsPerDay,
      shiftPattern: site.shiftPattern ?? undefined,
    });
    setModalOpen(true);
  };

  const closeModal = () => {
    setModalOpen(false);
    setEditingSite(null);
    form.resetFields();
  };

  const handleSubmit = async () => {
    const values = await form.validateFields();
    if (editingSite) {
      updateMutation.mutate({
        id: editingSite.id,
        data: {
          id: editingSite.id,
          ...values,
          status: values.status || 'Active',
          unitSystem: values.unitSystem || 'Metric',
          timeZone: values.timeZone || 'UTC',
          shiftsPerDay: values.shiftsPerDay || 3,
        } as UpdateMineSiteRequest,
      });
    } else {
      createMutation.mutate(values as CreateMineSiteRequest);
    }
  };

  const confirmDelete = (id: string, name: string) => {
    Modal.confirm({
      title: 'Delete Mine Site',
      icon: <ExclamationCircleOutlined />,
      content: `Are you sure you want to delete "${name}"?`,
      okText: 'Delete',
      okType: 'danger',
      onOk: () => deleteMutation.mutate(id),
    });
  };

  const statusColor = (status: string) => {
    switch (status) {
      case 'Active': return 'green';
      case 'Suspended': return 'orange';
      case 'Closed': return 'red';
      case 'UnderConstruction': return 'blue';
      default: return 'default';
    }
  };

  const mineTypeColor = (type: string) => {
    switch (type) {
      case 'Underground': return 'purple';
      case 'OpenPit': return 'cyan';
      case 'Mixed': return 'geekblue';
      default: return 'default';
    }
  };

  const columns: ColumnsType<MineSite> = [
    {
      title: 'NAME',
      dataIndex: 'name',
      key: 'name',
      render: (text: string, record: MineSite) => (
        <div>
          <Text strong style={{ fontSize: 13 }}>{text}</Text>
          {record.code && <Text type="secondary" style={{ fontSize: 12, display: 'block' }}>{record.code}</Text>}
        </div>
      ),
    },
    {
      title: 'TYPE',
      dataIndex: 'mineType',
      key: 'mineType',
      render: (type: string) => <Tag color={mineTypeColor(type)}>{type}</Tag>,
    },
    {
      title: 'JURISDICTION',
      dataIndex: 'jurisdiction',
      key: 'jurisdiction',
      render: (j: string) => {
        const found = JURISDICTIONS.find(x => x.value === j);
        return <Text style={{ fontSize: 13 }}>{found?.label || j}</Text>;
      },
    },
    {
      title: 'LOCATION',
      key: 'location',
      render: (_: unknown, record: MineSite) => (
        <Text style={{ fontSize: 13 }}>
          {[record.country, record.state].filter(Boolean).join(', ') || '-'}
        </Text>
      ),
    },
    {
      title: 'STATUS',
      dataIndex: 'status',
      key: 'status',
      render: (status: string) => <Tag color={statusColor(status)}>{status}</Tag>,
    },
    {
      title: 'AREAS',
      dataIndex: 'areaCount',
      key: 'areaCount',
      render: (count: number) => <Text style={{ fontSize: 13 }}>{count}</Text>,
    },
    {
      title: 'CREATED',
      dataIndex: 'createdAt',
      key: 'createdAt',
      render: (date: string) => <Text style={{ fontSize: 12, color: '#86868b' }}>{dayjs(date).format('DD MMM YYYY')}</Text>,
    },
    {
      title: '',
      key: 'actions',
      width: 140,
      render: (_: unknown, record: MineSite) => (
        <Space size="small">
          <Button type="text" size="small" icon={<EyeOutlined />} onClick={() => setDetailSite(record)} />
          <PermissionGate permission="MineSite.Update">
            <Button type="text" size="small" icon={<EditOutlined />} onClick={() => openEditModal(record)} />
          </PermissionGate>
          <PermissionGate permission="MineSite.Delete">
            <Button type="text" size="small" danger icon={<DeleteOutlined />} onClick={() => confirmDelete(record.id, record.name)} />
          </PermissionGate>
        </Space>
      ),
    },
  ];

  return (
    <>
      <Flex justify="space-between" align="center" style={{ marginBottom: 24 }}>
        <div>
          <Title level={4} style={{ margin: 0, fontWeight: 700 }}>Mine Sites</Title>
          <Text type="secondary" style={{ fontSize: 13 }}>Manage mine sites, locations, and jurisdictions</Text>
        </div>
        <PermissionGate permission="MineSite.Create">
          <Button type="primary" icon={<PlusOutlined />} onClick={openCreateModal}>
            Add Mine Site
          </Button>
        </PermissionGate>
      </Flex>

      <Table
        columns={columns}
        dataSource={sitesQuery.data}
        rowKey="id"
        loading={sitesQuery.isLoading}
        pagination={{ pageSize: 10, showSizeChanger: false }}
        size="middle"
        style={{ background: '#fff', borderRadius: 12, border: '1px solid #e5e5ea' }}
      />

      {/* Create/Edit Modal */}
      <Modal
        title={editingSite ? 'Edit Mine Site' : 'New Mine Site'}
        open={modalOpen}
        onCancel={closeModal}
        onOk={handleSubmit}
        confirmLoading={createMutation.isPending || updateMutation.isPending}
        width={720}
        okText={editingSite ? 'Save Changes' : 'Create'}
      >
        <Form form={form} layout="vertical" style={{ marginTop: 16 }}>
          <Flex gap={16}>
            <Form.Item name="name" label="Mine Site Name" rules={[{ required: true }]} style={{ flex: 2 }}>
              <Input placeholder="e.g. Kalgoorlie Gold Mine" />
            </Form.Item>
            <Form.Item name="code" label="Code" style={{ flex: 1 }}>
              <Input placeholder="e.g. KGM-01" />
            </Form.Item>
          </Flex>

          <Flex gap={16}>
            <Form.Item name="mineType" label="Mine Type" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select options={MINE_TYPES.map(t => ({ label: t, value: t }))} />
            </Form.Item>
            <Form.Item name="jurisdiction" label="Jurisdiction" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select options={JURISDICTIONS} />
            </Form.Item>
            <Form.Item name="status" label="Status" style={{ flex: 1 }}>
              <Select options={STATUSES.map(s => ({ label: s, value: s }))} />
            </Form.Item>
          </Flex>

          <Flex gap={16}>
            <Form.Item name="operatingCompany" label="Operating Company" style={{ flex: 1 }}>
              <Input placeholder="e.g. BHP Group" />
            </Form.Item>
            <Form.Item name="miningLicenseNumber" label="Mining License #" style={{ flex: 1 }}>
              <Input />
            </Form.Item>
          </Flex>

          <Flex gap={16}>
            <Form.Item name="country" label="Country" style={{ flex: 1 }}>
              <Input placeholder="e.g. Australia" />
            </Form.Item>
            <Form.Item name="state" label="State/Province" style={{ flex: 1 }}>
              <Input placeholder="e.g. Western Australia" />
            </Form.Item>
            <Form.Item name="address" label="Address" style={{ flex: 2 }}>
              <Input />
            </Form.Item>
          </Flex>

          <Flex gap={16}>
            <Form.Item name="latitude" label="Latitude" style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} min={-90} max={90} step={0.0001} />
            </Form.Item>
            <Form.Item name="longitude" label="Longitude" style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} min={-180} max={180} step={0.0001} />
            </Form.Item>
            <Form.Item name="mineralsMined" label="Minerals Mined" style={{ flex: 2 }}>
              <Input placeholder="e.g. Gold, Copper, Zinc" />
            </Form.Item>
          </Flex>

          <Flex gap={16}>
            <Form.Item name="unitSystem" label="Unit System" style={{ flex: 1 }}>
              <Select options={UNIT_SYSTEMS.map(u => ({ label: u, value: u }))} />
            </Form.Item>
            <Form.Item name="timeZone" label="Time Zone" style={{ flex: 1 }}>
              <Input placeholder="e.g. Australia/Perth" />
            </Form.Item>
            <Form.Item name="shiftsPerDay" label="Shifts/Day" style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} min={1} max={4} />
            </Form.Item>
            <Form.Item name="shiftPattern" label="Shift Pattern" style={{ flex: 1 }}>
              <Input placeholder="e.g. 2x12, 3x8" />
            </Form.Item>
          </Flex>

          <Text strong style={{ display: 'block', marginBottom: 8, marginTop: 8 }}>Emergency Information</Text>
          <Flex gap={16}>
            <Form.Item name="emergencyContactName" label="Emergency Contact" style={{ flex: 1 }}>
              <Input />
            </Form.Item>
            <Form.Item name="emergencyContactPhone" label="Emergency Phone" style={{ flex: 1 }}>
              <Input />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="nearestHospital" label="Nearest Hospital" style={{ flex: 2 }}>
              <Input />
            </Form.Item>
            <Form.Item name="nearestHospitalPhone" label="Hospital Phone" style={{ flex: 1 }}>
              <Input />
            </Form.Item>
            <Form.Item name="nearestHospitalDistanceKm" label="Distance (km)" style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} min={0} step={0.1} />
            </Form.Item>
          </Flex>
        </Form>
      </Modal>

      {/* Detail Drawer */}
      <Drawer
        title={detailSite?.name || 'Mine Site Details'}
        open={!!detailSite}
        onClose={() => setDetailSite(null)}
        width={600}
      >
        {detailSite && (
          <Descriptions column={2} bordered size="small">
            <Descriptions.Item label="Name" span={2}>{detailSite.name}</Descriptions.Item>
            <Descriptions.Item label="Code">{detailSite.code || '-'}</Descriptions.Item>
            <Descriptions.Item label="Status"><Tag color={statusColor(detailSite.status)}>{detailSite.status}</Tag></Descriptions.Item>
            <Descriptions.Item label="Mine Type"><Tag color={mineTypeColor(detailSite.mineType)}>{detailSite.mineType}</Tag></Descriptions.Item>
            <Descriptions.Item label="Jurisdiction">{JURISDICTIONS.find(j => j.value === detailSite.jurisdiction)?.label || detailSite.jurisdiction}</Descriptions.Item>
            <Descriptions.Item label="Operating Company" span={2}>{detailSite.operatingCompany || '-'}</Descriptions.Item>
            <Descriptions.Item label="Country">{detailSite.country || '-'}</Descriptions.Item>
            <Descriptions.Item label="State">{detailSite.state || '-'}</Descriptions.Item>
            <Descriptions.Item label="Address" span={2}>{detailSite.address || '-'}</Descriptions.Item>
            {detailSite.latitude && detailSite.longitude && (
              <Descriptions.Item label="Coordinates" span={2}>
                <EnvironmentOutlined style={{ marginRight: 4 }} />
                {detailSite.latitude}, {detailSite.longitude}
              </Descriptions.Item>
            )}
            <Descriptions.Item label="Minerals Mined" span={2}>{detailSite.mineralsMined || '-'}</Descriptions.Item>
            <Descriptions.Item label="Mining License #">{detailSite.miningLicenseNumber || '-'}</Descriptions.Item>
            <Descriptions.Item label="License Expiry">{detailSite.licenseExpiryDate ? dayjs(detailSite.licenseExpiryDate).format('DD MMM YYYY') : '-'}</Descriptions.Item>
            <Descriptions.Item label="Unit System">{detailSite.unitSystem}</Descriptions.Item>
            <Descriptions.Item label="Time Zone">{detailSite.timeZone}</Descriptions.Item>
            <Descriptions.Item label="Shifts/Day">{detailSite.shiftsPerDay}</Descriptions.Item>
            <Descriptions.Item label="Shift Pattern">{detailSite.shiftPattern || '-'}</Descriptions.Item>
            <Descriptions.Item label="Areas">{detailSite.areaCount}</Descriptions.Item>
            <Descriptions.Item label="Operational Since">{detailSite.operationalSince ? dayjs(detailSite.operationalSince).format('DD MMM YYYY') : '-'}</Descriptions.Item>
            <Descriptions.Item label="Emergency Contact" span={2}>
              {detailSite.emergencyContactName ? `${detailSite.emergencyContactName} - ${detailSite.emergencyContactPhone}` : '-'}
            </Descriptions.Item>
            <Descriptions.Item label="Nearest Hospital" span={2}>
              {detailSite.nearestHospital ? `${detailSite.nearestHospital} (${detailSite.nearestHospitalDistanceKm ?? '?'} km) - ${detailSite.nearestHospitalPhone}` : '-'}
            </Descriptions.Item>
          </Descriptions>
        )}
      </Drawer>
    </>
  );
}
