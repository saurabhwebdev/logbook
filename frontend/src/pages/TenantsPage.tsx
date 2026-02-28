import { useState } from 'react';
import {
  Typography,
  Button,
  Table,
  Space,
  Modal,
  Form,
  Input,
  Switch,
  message,
  Flex,
} from 'antd';
import { PlusOutlined, EditOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AxiosError } from 'axios';
import dayjs from 'dayjs';
import { tenantsApi } from '../api/tenantsApi';
import type { CreateTenantRequest, UpdateTenantRequest } from '../api/tenantsApi';
import type { Tenant } from '../types';
import PermissionGate from '../components/PermissionGate';

const { Text } = Typography;

interface TenantFormValues {
  name: string;
  subdomain: string;
  isActive: boolean;
}

export default function TenantsPage() {
  const queryClient = useQueryClient();
  const [modalOpen, setModalOpen] = useState(false);
  const [editingTenant, setEditingTenant] = useState<Tenant | null>(null);
  const [form] = Form.useForm<TenantFormValues>();

  const tenantsQuery = useQuery({
    queryKey: ['tenants'],
    queryFn: () => tenantsApi.getTenants(),
  });

  const createMutation = useMutation({
    mutationFn: (data: CreateTenantRequest) => tenantsApi.createTenant(data),
    onSuccess: () => {
      message.success('Tenant created');
      queryClient.invalidateQueries({ queryKey: ['tenants'] });
      closeModal();
    },
    onError: (error: AxiosError<{ message?: string; title?: string }>) => {
      message.error(error.response?.data?.message || error.response?.data?.title || 'Failed to create tenant');
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateTenantRequest }) =>
      tenantsApi.updateTenant(id, data),
    onSuccess: () => {
      message.success('Tenant updated');
      queryClient.invalidateQueries({ queryKey: ['tenants'] });
      closeModal();
    },
    onError: (error: AxiosError<{ message?: string; title?: string }>) => {
      message.error(error.response?.data?.message || error.response?.data?.title || 'Failed to update tenant');
    },
  });

  const openCreateModal = () => {
    setEditingTenant(null);
    form.resetFields();
    form.setFieldsValue({ isActive: true });
    setModalOpen(true);
  };

  const openEditModal = (tenant: Tenant) => {
    setEditingTenant(tenant);
    form.setFieldsValue({
      name: tenant.name,
      subdomain: tenant.subdomain,
      isActive: tenant.isActive,
    });
    setModalOpen(true);
  };

  const closeModal = () => {
    setModalOpen(false);
    setEditingTenant(null);
    form.resetFields();
  };

  const handleModalOk = async () => {
    try {
      const values = await form.validateFields();
      if (editingTenant) {
        updateMutation.mutate({
          id: editingTenant.id,
          data: { name: values.name, isActive: values.isActive },
        });
      } else {
        createMutation.mutate({
          name: values.name,
          subdomain: values.subdomain,
        });
      }
    } catch {
      // validation
    }
  };

  const columns: ColumnsType<Tenant> = [
    {
      title: 'Tenant',
      key: 'name',
      render: (_, record) => (
        <div>
          <div style={{ fontWeight: 500, color: '#1d1d1f', fontSize: 13 }}>{record.name}</div>
          <div style={{ fontSize: 12, color: '#86868b' }}>{record.subdomain}</div>
        </div>
      ),
      sorter: (a, b) => a.name.localeCompare(b.name),
    },
    {
      title: 'Status',
      dataIndex: 'isActive',
      key: 'isActive',
      width: 100,
      render: (value: boolean) =>
        value ? (
          <span style={{ display: 'inline-flex', alignItems: 'center', gap: 6, fontSize: 12, fontWeight: 500, color: '#34c759' }}>
            <span style={{ width: 6, height: 6, borderRadius: '50%', background: '#34c759' }} />
            Active
          </span>
        ) : (
          <span style={{ display: 'inline-flex', alignItems: 'center', gap: 6, fontSize: 12, fontWeight: 500, color: '#86868b' }}>
            <span style={{ width: 6, height: 6, borderRadius: '50%', background: '#86868b' }} />
            Inactive
          </span>
        ),
    },
    {
      title: 'Created',
      dataIndex: 'createdAt',
      key: 'createdAt',
      width: 130,
      render: (value: string) => (
        <Text style={{ fontSize: 13, color: '#86868b' }}>
          {dayjs(value).format('MMM D, YYYY')}
        </Text>
      ),
      sorter: (a, b) => dayjs(a.createdAt).unix() - dayjs(b.createdAt).unix(),
    },
    {
      title: '',
      key: 'actions',
      width: 60,
      align: 'right',
      render: (_, record) => (
        <Space size={0}>
          <PermissionGate permission="Tenant.Update">
            <Button type="text" size="small" icon={<EditOutlined />} onClick={() => openEditModal(record)} style={{ color: '#86868b' }} />
          </PermissionGate>
        </Space>
      ),
    },
  ];

  const isMutating = createMutation.isPending || updateMutation.isPending;

  return (
    <div>
      <Flex align="center" justify="space-between" style={{ marginBottom: 24 }}>
        <h2 style={{ fontSize: 22, fontWeight: 700, color: '#1d1d1f', margin: 0 }}>Tenants</h2>
        <PermissionGate permission="Tenant.Create">
          <Button type="primary" icon={<PlusOutlined />} onClick={openCreateModal}>
            Add tenant
          </Button>
        </PermissionGate>
      </Flex>

      <div style={{ background: '#ffffff', borderRadius: 12, border: '1px solid #e5e5ea', overflow: 'hidden' }}>
        <Table<Tenant>
          rowKey="id"
          columns={columns}
          dataSource={tenantsQuery.data ?? []}
          loading={tenantsQuery.isLoading}
          pagination={{ showSizeChanger: true, style: { padding: '0 16px' } }}
        />
      </div>

      <Modal
        title={editingTenant ? 'Edit tenant' : 'New tenant'}
        open={modalOpen}
        onOk={handleModalOk}
        onCancel={closeModal}
        confirmLoading={isMutating}
        destroyOnClose
        okText={editingTenant ? 'Save changes' : 'Create'}
      >
        <Form<TenantFormValues> form={form} layout="vertical" requiredMark={false}>
          <Form.Item
            name="name"
            label={<Text style={{ fontWeight: 500, fontSize: 13 }}>Name</Text>}
            rules={[{ required: true, message: 'Required' }]}
          >
            <Input placeholder="e.g. Acme Corp" />
          </Form.Item>
          <Form.Item
            name="subdomain"
            label={<Text style={{ fontWeight: 500, fontSize: 13 }}>Subdomain</Text>}
            rules={[
              { required: true, message: 'Required' },
              { pattern: /^[a-z0-9]+(-[a-z0-9]+)*$/, message: 'Lowercase letters, numbers, and hyphens only' },
            ]}
          >
            <Input placeholder="e.g. acme" disabled={!!editingTenant} />
          </Form.Item>
          {editingTenant && (
            <Form.Item
              name="isActive"
              label={<Text style={{ fontWeight: 500, fontSize: 13 }}>Active</Text>}
              valuePropName="checked"
            >
              <Switch />
            </Form.Item>
          )}
        </Form>
      </Modal>
    </div>
  );
}
