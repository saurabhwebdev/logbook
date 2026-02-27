import { useState } from 'react';
import { Typography, Table, Flex, Button, Modal, Form, Input, message, Popconfirm, Tabs } from 'antd';
import { PlusOutlined, DeleteOutlined, StopOutlined, CopyOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import dayjs from 'dayjs';
import { apiIntegrationApi } from '../api/apiIntegrationApi';
import type { ApiKeyInfo, WebhookInfo } from '../types';

const { Text } = Typography;

export default function ApiIntegrationPage() {
  const [keyModalOpen, setKeyModalOpen] = useState(false);
  const [webhookModalOpen, setWebhookModalOpen] = useState(false);
  const [newKey, setNewKey] = useState<string | null>(null);
  const [newSecret, setNewSecret] = useState<string | null>(null);
  const [keyForm] = Form.useForm();
  const [webhookForm] = Form.useForm();
  const queryClient = useQueryClient();

  const keysQuery = useQuery({ queryKey: ['apiKeys'], queryFn: apiIntegrationApi.getApiKeys });
  const webhooksQuery = useQuery({ queryKey: ['webhooks'], queryFn: apiIntegrationApi.getWebhooks });

  const createKeyMutation = useMutation({
    mutationFn: apiIntegrationApi.createApiKey,
    onSuccess: (data) => {
      setNewKey(data.rawKey);
      queryClient.invalidateQueries({ queryKey: ['apiKeys'] });
      keyForm.resetFields();
    },
    onError: () => message.error('Failed to create API key'),
  });

  const revokeKeyMutation = useMutation({
    mutationFn: apiIntegrationApi.revokeApiKey,
    onSuccess: () => {
      message.success('API key revoked');
      queryClient.invalidateQueries({ queryKey: ['apiKeys'] });
    },
  });

  const createWebhookMutation = useMutation({
    mutationFn: apiIntegrationApi.createWebhook,
    onSuccess: (data) => {
      setNewSecret(data.secret);
      queryClient.invalidateQueries({ queryKey: ['webhooks'] });
      webhookForm.resetFields();
    },
    onError: () => message.error('Failed to create webhook'),
  });

  const deleteWebhookMutation = useMutation({
    mutationFn: apiIntegrationApi.deleteWebhook,
    onSuccess: () => {
      message.success('Webhook deleted');
      queryClient.invalidateQueries({ queryKey: ['webhooks'] });
    },
  });

  const copyToClipboard = (text: string) => {
    navigator.clipboard.writeText(text);
    message.success('Copied to clipboard');
  };

  const keyColumns: ColumnsType<ApiKeyInfo> = [
    {
      title: 'Name',
      key: 'name',
      render: (_, r) => (
        <div>
          <div style={{ fontWeight: 500, color: '#1d1d1f', fontSize: 13 }}>{r.name}</div>
          <code style={{ fontSize: 11, color: '#86868b' }}>{r.keyPrefix}...</code>
        </div>
      ),
    },
    {
      title: 'Status',
      key: 'isActive',
      width: 100,
      render: (_, r) => (
        <span style={{ display: 'inline-flex', alignItems: 'center', gap: 6, fontSize: 12, fontWeight: 500, color: r.isActive ? '#34c759' : '#86868b' }}>
          <span style={{ width: 6, height: 6, borderRadius: '50%', background: r.isActive ? '#34c759' : '#86868b' }} />
          {r.isActive ? 'Active' : 'Revoked'}
        </span>
      ),
    },
    {
      title: 'Expires',
      key: 'expiresAt',
      width: 110,
      render: (_, r) => <Text style={{ fontSize: 13, color: '#86868b' }}>{r.expiresAt ? dayjs(r.expiresAt).format('MMM D, YYYY') : 'Never'}</Text>,
    },
    {
      title: 'Last used',
      key: 'lastUsedAt',
      width: 110,
      render: (_, r) => <Text style={{ fontSize: 13, color: '#86868b' }}>{r.lastUsedAt ? dayjs(r.lastUsedAt).format('MMM D, YYYY') : 'Never'}</Text>,
    },
    {
      title: '',
      key: 'actions',
      width: 60,
      render: (_, r) => r.isActive ? (
        <Popconfirm title="Revoke this key?" onConfirm={() => revokeKeyMutation.mutate(r.id)} okText="Revoke" okButtonProps={{ danger: true }}>
          <Button type="text" size="small" icon={<StopOutlined />} danger />
        </Popconfirm>
      ) : null,
    },
  ];

  const webhookColumns: ColumnsType<WebhookInfo> = [
    {
      title: 'Webhook',
      key: 'name',
      render: (_, r) => (
        <div>
          <div style={{ fontWeight: 500, color: '#1d1d1f', fontSize: 13 }}>{r.name}</div>
          <code style={{ fontSize: 11, color: '#86868b' }}>{r.endpointUrl}</code>
        </div>
      ),
    },
    {
      title: 'Events',
      dataIndex: 'eventTypes',
      key: 'eventTypes',
      width: 200,
      render: (value: string) => (
        <Flex gap={4} wrap="wrap">
          {value.split(',').map((e) => (
            <span key={e} style={{ fontSize: 11, background: '#f5f5f7', padding: '1px 8px', borderRadius: 8, color: '#6e6e73' }}>{e.trim()}</span>
          ))}
        </Flex>
      ),
    },
    {
      title: 'Status',
      key: 'isActive',
      width: 100,
      render: (_, r) => (
        <span style={{ display: 'inline-flex', alignItems: 'center', gap: 6, fontSize: 12, fontWeight: 500, color: r.isActive ? '#34c759' : '#86868b' }}>
          <span style={{ width: 6, height: 6, borderRadius: '50%', background: r.isActive ? '#34c759' : '#86868b' }} />
          {r.isActive ? 'Active' : 'Inactive'}
        </span>
      ),
    },
    {
      title: 'Failures',
      dataIndex: 'failureCount',
      key: 'failureCount',
      width: 80,
      render: (value: number) => <Text style={{ fontSize: 13, color: value > 0 ? '#ff3b30' : '#86868b' }}>{value}</Text>,
    },
    {
      title: '',
      key: 'actions',
      width: 60,
      render: (_, r) => (
        <Popconfirm title="Delete this webhook?" onConfirm={() => deleteWebhookMutation.mutate(r.id)} okText="Delete" okButtonProps={{ danger: true }}>
          <Button type="text" size="small" icon={<DeleteOutlined />} danger />
        </Popconfirm>
      ),
    },
  ];

  return (
    <div>
      <Flex align="center" justify="space-between" style={{ marginBottom: 24 }}>
        <div>
          <h2 style={{ fontSize: 22, fontWeight: 700, color: '#1d1d1f', margin: 0 }}>API Integration</h2>
          <Text style={{ fontSize: 13, color: '#86868b' }}>Manage API keys and webhook subscriptions.</Text>
        </div>
      </Flex>

      <Tabs
        items={[
          {
            key: 'keys',
            label: 'API Keys',
            children: (
              <>
                <Flex justify="flex-end" style={{ marginBottom: 16 }}>
                  <Button type="primary" icon={<PlusOutlined />} onClick={() => { setKeyModalOpen(true); setNewKey(null); }}>New API Key</Button>
                </Flex>
                <div style={{ background: '#ffffff', borderRadius: 12, border: '1px solid #e5e5ea', overflow: 'hidden' }}>
                  <Table<ApiKeyInfo> rowKey="id" columns={keyColumns} dataSource={keysQuery.data ?? []} loading={keysQuery.isLoading} pagination={false} />
                </div>
              </>
            ),
          },
          {
            key: 'webhooks',
            label: 'Webhooks',
            children: (
              <>
                <Flex justify="flex-end" style={{ marginBottom: 16 }}>
                  <Button type="primary" icon={<PlusOutlined />} onClick={() => { setWebhookModalOpen(true); setNewSecret(null); }}>New Webhook</Button>
                </Flex>
                <div style={{ background: '#ffffff', borderRadius: 12, border: '1px solid #e5e5ea', overflow: 'hidden' }}>
                  <Table<WebhookInfo> rowKey="id" columns={webhookColumns} dataSource={webhooksQuery.data ?? []} loading={webhooksQuery.isLoading} pagination={false} />
                </div>
              </>
            ),
          },
        ]}
      />

      {/* Create API Key Modal */}
      <Modal
        title={newKey ? 'API Key Created' : 'New API Key'}
        open={keyModalOpen}
        onCancel={() => { setKeyModalOpen(false); setNewKey(null); }}
        footer={newKey ? [<Button key="close" type="primary" onClick={() => { setKeyModalOpen(false); setNewKey(null); }}>Done</Button>] : undefined}
        onOk={() => keyForm.validateFields().then((v) => createKeyMutation.mutate(v))}
        confirmLoading={createKeyMutation.isPending}
        okText="Create"
      >
        {newKey ? (
          <div style={{ marginTop: 16 }}>
            <Text style={{ fontSize: 13, color: '#ff9500', display: 'block', marginBottom: 12 }}>Copy this key now. It won't be shown again.</Text>
            <Flex gap={8}>
              <Input value={newKey} readOnly style={{ fontFamily: 'monospace', fontSize: 12 }} />
              <Button icon={<CopyOutlined />} onClick={() => copyToClipboard(newKey)} />
            </Flex>
          </div>
        ) : (
          <Form form={keyForm} layout="vertical" style={{ marginTop: 16 }}>
            <Form.Item name="name" label="Name" rules={[{ required: true }]}><Input placeholder="e.g. Production API" /></Form.Item>
            <Form.Item name="scopes" label="Scopes (optional)"><Input placeholder="e.g. User.Read,Department.Read" /></Form.Item>
          </Form>
        )}
      </Modal>

      {/* Create Webhook Modal */}
      <Modal
        title={newSecret ? 'Webhook Created' : 'New Webhook'}
        open={webhookModalOpen}
        onCancel={() => { setWebhookModalOpen(false); setNewSecret(null); }}
        footer={newSecret ? [<Button key="close" type="primary" onClick={() => { setWebhookModalOpen(false); setNewSecret(null); }}>Done</Button>] : undefined}
        onOk={() => webhookForm.validateFields().then((v) => createWebhookMutation.mutate(v))}
        confirmLoading={createWebhookMutation.isPending}
        okText="Create"
      >
        {newSecret ? (
          <div style={{ marginTop: 16 }}>
            <Text style={{ fontSize: 13, color: '#ff9500', display: 'block', marginBottom: 12 }}>Copy this signing secret now. It won't be shown again.</Text>
            <Flex gap={8}>
              <Input value={newSecret} readOnly style={{ fontFamily: 'monospace', fontSize: 12 }} />
              <Button icon={<CopyOutlined />} onClick={() => copyToClipboard(newSecret)} />
            </Flex>
          </div>
        ) : (
          <Form form={webhookForm} layout="vertical" style={{ marginTop: 16 }}>
            <Form.Item name="name" label="Name" rules={[{ required: true }]}><Input placeholder="e.g. Order notifications" /></Form.Item>
            <Form.Item name="endpointUrl" label="Endpoint URL" rules={[{ required: true, type: 'url' }]}><Input placeholder="https://example.com/webhook" /></Form.Item>
            <Form.Item name="eventTypes" label="Event Types" rules={[{ required: true }]}><Input placeholder="e.g. user.created,task.completed" /></Form.Item>
          </Form>
        )}
      </Modal>
    </div>
  );
}
