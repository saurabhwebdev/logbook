import { useState } from 'react';
import {
  Typography,
  Button,
  Table,
  Switch,
  Space,
  Modal,
  Form,
  Input,
  message,
  Flex,
} from 'antd';
import {
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  ExclamationCircleOutlined,
} from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AxiosError } from 'axios';
import { featureFlagsApi } from '../api/featureFlagsApi';
import type {
  CreateFeatureFlagRequest,
  UpdateFeatureFlagRequest,
} from '../api/featureFlagsApi';
import type { FeatureFlag } from '../types';
import PermissionGate from '../components/PermissionGate';

const { Text } = Typography;

interface FeatureFlagFormValues {
  name: string;
  description: string;
  isEnabled: boolean;
}

export default function FeatureFlagsPage() {
  const queryClient = useQueryClient();
  const [modalOpen, setModalOpen] = useState(false);
  const [editingFlag, setEditingFlag] = useState<FeatureFlag | null>(null);
  const [form] = Form.useForm<FeatureFlagFormValues>();

  const { data, isLoading } = useQuery({
    queryKey: ['featureFlags'],
    queryFn: () => featureFlagsApi.getAll(),
  });

  const createMutation = useMutation({
    mutationFn: (data: CreateFeatureFlagRequest) => featureFlagsApi.create(data),
    onSuccess: () => {
      message.success('Feature flag created');
      queryClient.invalidateQueries({ queryKey: ['featureFlags'] });
      closeModal();
    },
    onError: (error: AxiosError<{ message?: string; title?: string }>) => {
      message.error(error.response?.data?.message || error.response?.data?.title || 'Failed to create feature flag');
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateFeatureFlagRequest }) =>
      featureFlagsApi.update(id, data),
    onSuccess: () => {
      message.success('Feature flag updated');
      queryClient.invalidateQueries({ queryKey: ['featureFlags'] });
      closeModal();
    },
    onError: (error: AxiosError<{ message?: string; title?: string }>) => {
      message.error(error.response?.data?.message || error.response?.data?.title || 'Failed to update feature flag');
    },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => featureFlagsApi.delete(id),
    onSuccess: () => {
      message.success('Feature flag deleted');
      queryClient.invalidateQueries({ queryKey: ['featureFlags'] });
    },
    onError: () => {
      message.error('Failed to delete feature flag');
    },
  });

  const toggleMutation = useMutation({
    mutationFn: ({ id, isEnabled }: { id: string; isEnabled: boolean }) =>
      featureFlagsApi.toggle(id, isEnabled),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['featureFlags'] });
    },
    onError: () => message.error('Failed to toggle feature flag'),
  });

  const openCreateModal = () => {
    setEditingFlag(null);
    form.resetFields();
    form.setFieldsValue({ isEnabled: false });
    setModalOpen(true);
  };

  const openEditModal = (flag: FeatureFlag) => {
    setEditingFlag(flag);
    form.setFieldsValue({
      name: flag.name,
      description: flag.description ?? '',
      isEnabled: flag.isEnabled,
    });
    setModalOpen(true);
  };

  const closeModal = () => {
    setModalOpen(false);
    setEditingFlag(null);
    form.resetFields();
  };

  const handleModalOk = async () => {
    try {
      const values = await form.validateFields();
      if (editingFlag) {
        updateMutation.mutate({
          id: editingFlag.id,
          data: { name: values.name, description: values.description || undefined },
        });
      } else {
        createMutation.mutate({
          name: values.name,
          description: values.description || undefined,
          isEnabled: values.isEnabled,
        });
      }
    } catch {
      // validation
    }
  };

  const handleDelete = (flag: FeatureFlag) => {
    Modal.confirm({
      title: 'Delete feature flag',
      icon: <ExclamationCircleOutlined />,
      content: `This will permanently delete "${flag.name}". This action cannot be undone.`,
      okText: 'Delete',
      okType: 'danger',
      onOk: () => deleteMutation.mutateAsync(flag.id),
    });
  };

  const handleToggle = (id: string, checked: boolean) => {
    toggleMutation.mutate({ id, isEnabled: checked });
  };

  const columns: ColumnsType<FeatureFlag> = [
    {
      title: 'Feature',
      key: 'name',
      render: (_, record) => (
        <div>
          <div style={{ fontWeight: 500, color: '#1d1d1f', fontSize: 13 }}>{record.name}</div>
          {record.description && (
            <div style={{ fontSize: 12, color: '#86868b' }}>{record.description}</div>
          )}
        </div>
      ),
      sorter: (a, b) => a.name.localeCompare(b.name),
    },
    {
      title: 'Status',
      key: 'isEnabled',
      width: 120,
      render: (_, record) => (
        <Flex align="center" gap={8}>
          <span
            style={{
              display: 'inline-flex',
              alignItems: 'center',
              gap: 6,
              fontSize: 12,
              fontWeight: 500,
              color: record.isEnabled ? '#34c759' : '#86868b',
            }}
          >
            <span
              style={{
                width: 6,
                height: 6,
                borderRadius: '50%',
                background: record.isEnabled ? '#34c759' : '#86868b',
              }}
            />
            {record.isEnabled ? 'Enabled' : 'Disabled'}
          </span>
        </Flex>
      ),
    },
    {
      title: '',
      key: 'toggle',
      width: 80,
      render: (_, record) => (
        <Switch
          checked={record.isEnabled}
          onChange={(checked) => handleToggle(record.id, checked)}
          loading={toggleMutation.isPending}
          size="small"
        />
      ),
    },
    {
      title: '',
      key: 'actions',
      width: 100,
      align: 'right',
      render: (_, record) => (
        <Space size={0}>
          <PermissionGate permission="FeatureFlag.Update">
            <Button type="text" size="small" icon={<EditOutlined />} onClick={() => openEditModal(record)} style={{ color: '#86868b' }} />
          </PermissionGate>
          <PermissionGate permission="FeatureFlag.Delete">
            <Button type="text" size="small" danger icon={<DeleteOutlined />} onClick={() => handleDelete(record)} />
          </PermissionGate>
        </Space>
      ),
    },
  ];

  const isMutating = createMutation.isPending || updateMutation.isPending;

  return (
    <div>
      <Flex align="center" justify="space-between" style={{ marginBottom: 24 }}>
        <div>
          <h2 style={{ fontSize: 22, fontWeight: 700, color: '#1d1d1f', margin: 0 }}>
            Feature Flags
          </h2>
          <Text style={{ fontSize: 13, color: '#86868b' }}>
            Toggle features on or off per tenant.
          </Text>
        </div>
        <PermissionGate permission="FeatureFlag.Create">
          <Button type="primary" icon={<PlusOutlined />} onClick={openCreateModal}>
            New feature flag
          </Button>
        </PermissionGate>
      </Flex>

      <div
        style={{
          background: '#ffffff',
          borderRadius: 12,
          border: '1px solid #e5e5ea',
          overflow: 'hidden',
        }}
      >
        <Table<FeatureFlag>
          rowKey="id"
          columns={columns}
          dataSource={data ?? []}
          loading={isLoading}
          pagination={{ showSizeChanger: true, style: { padding: '0 16px' } }}
        />
      </div>

      <Modal
        title={editingFlag ? 'Edit feature flag' : 'New feature flag'}
        open={modalOpen}
        onOk={handleModalOk}
        onCancel={closeModal}
        confirmLoading={isMutating}
        destroyOnClose
        okText={editingFlag ? 'Save changes' : 'Create'}
      >
        <Form<FeatureFlagFormValues> form={form} layout="vertical" requiredMark={false}>
          <Form.Item name="name" label={<Text style={{ fontWeight: 500, fontSize: 13 }}>Name</Text>} rules={[{ required: true, message: 'Required' }]}>
            <Input placeholder="e.g. EnableAdvancedReporting" />
          </Form.Item>
          <Form.Item name="description" label={<Text style={{ fontWeight: 500, fontSize: 13 }}>Description</Text>}>
            <Input.TextArea rows={3} placeholder="What does this feature flag control?" />
          </Form.Item>
          {!editingFlag && (
            <Form.Item name="isEnabled" label={<Text style={{ fontWeight: 500, fontSize: 13 }}>Initial Status</Text>} valuePropName="checked">
              <Switch checkedChildren="Enabled" unCheckedChildren="Disabled" />
            </Form.Item>
          )}
        </Form>
      </Modal>
    </div>
  );
}
