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
  Tag,
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
import { emailTemplatesApi } from '../api/emailTemplatesApi';
import type { EmailTemplate } from '../types';
import PermissionGate from '../components/PermissionGate';

const { Title, Text } = Typography;
const { TextArea } = Input;

export default function EmailTemplatesPage() {
  const queryClient = useQueryClient();
  const [modalOpen, setModalOpen] = useState(false);
  const [editingTemplate, setEditingTemplate] = useState<EmailTemplate | null>(null);
  const [form] = Form.useForm();

  const { data, isLoading } = useQuery({
    queryKey: ['emailTemplates'],
    queryFn: () => emailTemplatesApi.getAll(),
  });

  const createMutation = useMutation({
    mutationFn: (data: Partial<EmailTemplate>) => emailTemplatesApi.create(data),
    onSuccess: () => {
      message.success('Email template created');
      queryClient.invalidateQueries({ queryKey: ['emailTemplates'] });
      closeModal();
    },
    onError: (error: AxiosError<{ message?: string; title?: string }>) => {
      message.error(error.response?.data?.message || error.response?.data?.title || 'Failed to create template');
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: Partial<EmailTemplate> }) =>
      emailTemplatesApi.update(id, data),
    onSuccess: () => {
      message.success('Email template updated');
      queryClient.invalidateQueries({ queryKey: ['emailTemplates'] });
      closeModal();
    },
    onError: (error: AxiosError<{ message?: string; title?: string }>) => {
      message.error(error.response?.data?.message || error.response?.data?.title || 'Failed to update template');
    },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => emailTemplatesApi.delete(id),
    onSuccess: () => {
      message.success('Email template deleted');
      queryClient.invalidateQueries({ queryKey: ['emailTemplates'] });
    },
    onError: () => {
      message.error('Failed to delete template');
    },
  });

  const openCreateModal = () => {
    setEditingTemplate(null);
    form.resetFields();
    form.setFieldsValue({ isActive: true });
    setModalOpen(true);
  };

  const openEditModal = (template: EmailTemplate) => {
    setEditingTemplate(template);
    form.setFieldsValue(template);
    setModalOpen(true);
  };

  const closeModal = () => {
    setModalOpen(false);
    setEditingTemplate(null);
    form.resetFields();
  };

  const handleSubmit = async (values: any) => {
    if (editingTemplate) {
      updateMutation.mutate({ id: editingTemplate.id, data: values });
    } else {
      createMutation.mutate(values);
    }
  };

  const handleDelete = (id: string, name: string) => {
    Modal.confirm({
      title: 'Delete Email Template',
      icon: <ExclamationCircleOutlined />,
      content: `Are you sure you want to delete "${name}"?`,
      okText: 'Delete',
      okType: 'danger',
      onOk: () => deleteMutation.mutate(id),
    });
  };

  const columns: ColumnsType<EmailTemplate> = [
    {
      title: 'NAME',
      dataIndex: 'name',
      key: 'name',
      sorter: (a, b) => a.name.localeCompare(b.name),
    },
    {
      title: 'SUBJECT',
      dataIndex: 'subject',
      key: 'subject',
    },
    {
      title: 'STATUS',
      key: 'isActive',
      width: 120,
      render: (_, record) => (
        <Tag color={record.isActive ? 'green' : 'default'}>
          {record.isActive ? 'Active' : 'Inactive'}
        </Tag>
      ),
    },
    {
      title: 'CREATED',
      dataIndex: 'createdAt',
      key: 'createdAt',
      width: 180,
      render: (date: string) => new Date(date).toLocaleString(),
    },
    {
      title: 'ACTIONS',
      key: 'actions',
      width: 150,
      render: (_, record) => (
        <Space>
          <PermissionGate permissions={['EmailTemplate.Update']}>
            <Button
              type="text"
              icon={<EditOutlined />}
              onClick={() => openEditModal(record)}
            />
          </PermissionGate>
          <PermissionGate permissions={['EmailTemplate.Delete']}>
            <Button
              type="text"
              danger
              icon={<DeleteOutlined />}
              onClick={() => handleDelete(record.id, record.name)}
            />
          </PermissionGate>
        </Space>
      ),
    },
  ];

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 24 }}>
        <div>
          <Title level={2} style={{ margin: 0 }}>Email Templates</Title>
          <Text type="secondary">Manage reusable email templates with variable support</Text>
        </div>
        <PermissionGate permissions={['EmailTemplate.Create']}>
          <Button type="primary" icon={<PlusOutlined />} onClick={openCreateModal}>
            Create Template
          </Button>
        </PermissionGate>
      </div>

      <Table
        columns={columns}
        dataSource={data || []}
        loading={isLoading}
        rowKey="id"
        pagination={{ pageSize: 25 }}
      />

      <Modal
        title={editingTemplate ? 'Edit Email Template' : 'Create Email Template'}
        open={modalOpen}
        onCancel={closeModal}
        onOk={() => form.submit()}
        confirmLoading={createMutation.isPending || updateMutation.isPending}
        width={800}
      >
        <Form form={form} layout="vertical" onFinish={handleSubmit}>
          <Form.Item name="name" label="Template Name" rules={[{ required: true }]}>
            <Input placeholder="e.g., WelcomeEmail" />
          </Form.Item>

          <Form.Item name="subject" label="Subject" rules={[{ required: true }]}>
            <Input placeholder="e.g., Welcome to {{appName}}" />
          </Form.Item>

          <Form.Item name="htmlBody" label="HTML Body" rules={[{ required: true }]}>
            <TextArea rows={8} placeholder="HTML email content with {{variables}}" />
          </Form.Item>

          <Form.Item name="plainTextBody" label="Plain Text Body (Optional)">
            <TextArea rows={4} placeholder="Plain text fallback" />
          </Form.Item>

          <Form.Item name="isActive" label="Active" valuePropName="checked">
            <Switch />
          </Form.Item>

          <div style={{ background: '#f5f5f7', padding: 12, borderRadius: 8 }}>
            <Text type="secondary" style={{ fontSize: 12 }}>
              <strong>Available Variables:</strong> {'{'}{'{'} userName {'}'}{'}'}, {'{'}{'{'} email {'}'}{'}'}, {'{'}{'{'} link {'}'}{'}'}, {'{'}{'{'} date {'}'}{'}'}, {'{'}{'{'} appName {'}'}{'}'}, {'{'}{'{'} taskTitle {'}'}{'}'}, {'{'}{'{'} assignedBy {'}'}{'}'}, {'{'}{'{'} dueDate {'}'}{'}'}, {'{'}{'{'} submittedBy {'}'}{'}'}, {'{'}{'{'} requestTitle {'}'}{'}'}, {'{'}{'{'} rejectLink {'}'}{'}'}.
            </Text>
          </div>
        </Form>
      </Modal>
    </div>
  );
}
