import { useState } from 'react';
import { Typography, Table, Flex, Input, Modal, Form, Select, message } from 'antd';
import type { ColumnsType } from 'antd/es/table';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { configurationApi } from '../api/configurationApi';
import type { SystemConfiguration } from '../types';

const { Text } = Typography;
const { Search } = Input;

export default function SettingsPage() {
  const [search, setSearch] = useState('');
  const [editingConfig, setEditingConfig] = useState<SystemConfiguration | null>(null);
  const [form] = Form.useForm();
  const queryClient = useQueryClient();

  const { data, isLoading } = useQuery({
    queryKey: ['configuration'],
    queryFn: () => configurationApi.getAll(),
  });

  const upsertMutation = useMutation({
    mutationFn: configurationApi.upsert,
    onSuccess: () => {
      message.success('Configuration saved');
      queryClient.invalidateQueries({ queryKey: ['configuration'] });
      setEditingConfig(null);
    },
    onError: () => message.error('Failed to save configuration'),
  });

  const filtered = (data ?? []).filter(
    (c) =>
      c.key.toLowerCase().includes(search.toLowerCase()) ||
      c.category.toLowerCase().includes(search.toLowerCase())
  );

  const categories = [...new Set((data ?? []).map((c) => c.category))];

  const handleEdit = (config: SystemConfiguration) => {
    setEditingConfig(config);
    form.setFieldsValue(config);
  };

  const handleSave = () => {
    form.validateFields().then((values) => {
      upsertMutation.mutate(values);
    });
  };

  const columns: ColumnsType<SystemConfiguration> = [
    {
      title: 'Key',
      key: 'key',
      render: (_, record) => (
        <div>
          <div style={{ fontWeight: 500, color: '#1d1d1f', fontSize: 13 }}>{record.key}</div>
          {record.description && (
            <div style={{ fontSize: 12, color: '#86868b' }}>{record.description}</div>
          )}
        </div>
      ),
      sorter: (a, b) => a.key.localeCompare(b.key),
    },
    {
      title: 'Value',
      dataIndex: 'value',
      key: 'value',
      width: 200,
      render: (value: string) => (
        <code
          style={{
            fontSize: 12,
            background: '#f5f5f7',
            padding: '2px 8px',
            borderRadius: 4,
            color: '#1d1d1f',
          }}
        >
          {value}
        </code>
      ),
    },
    {
      title: 'Category',
      dataIndex: 'category',
      key: 'category',
      width: 120,
      render: (value: string) => (
        <span
          style={{
            fontSize: 12,
            fontWeight: 500,
            color: '#6e6e73',
            background: '#f5f5f7',
            padding: '2px 10px',
            borderRadius: 10,
          }}
        >
          {value}
        </span>
      ),
      filters: categories.map((c) => ({ text: c, value: c })),
      onFilter: (value, record) => record.category === value,
    },
    {
      title: 'Type',
      dataIndex: 'dataType',
      key: 'dataType',
      width: 80,
      render: (value: string) => (
        <Text style={{ fontSize: 12, color: '#86868b' }}>{value}</Text>
      ),
    },
    {
      title: '',
      key: 'actions',
      width: 60,
      render: (_, record) => (
        <a
          onClick={() => handleEdit(record)}
          style={{ fontSize: 13, color: '#0071e3', fontWeight: 500 }}
        >
          Edit
        </a>
      ),
    },
  ];

  return (
    <div>
      <Flex align="center" justify="space-between" style={{ marginBottom: 24 }}>
        <div>
          <h2 style={{ fontSize: 22, fontWeight: 700, color: '#1d1d1f', margin: 0 }}>Settings</h2>
          <Text style={{ fontSize: 13, color: '#86868b' }}>
            System configuration key-value pairs.
          </Text>
        </div>
      </Flex>

      <div
        style={{
          background: '#ffffff',
          borderRadius: 12,
          border: '1px solid #e5e5ea',
          padding: '16px 16px 0',
          marginBottom: 16,
        }}
      >
        <Search
          placeholder="Search by key or category..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          style={{ width: 320 }}
          allowClear
        />
      </div>

      <div
        style={{
          background: '#ffffff',
          borderRadius: 12,
          border: '1px solid #e5e5ea',
          overflow: 'hidden',
        }}
      >
        <Table<SystemConfiguration>
          rowKey="id"
          columns={columns}
          dataSource={filtered}
          loading={isLoading}
          pagination={false}
        />
      </div>

      <Modal
        title="Edit Configuration"
        open={!!editingConfig}
        onCancel={() => setEditingConfig(null)}
        onOk={handleSave}
        confirmLoading={upsertMutation.isPending}
        okText="Save"
      >
        <Form form={form} layout="vertical" style={{ marginTop: 16 }}>
          <Form.Item name="key" label="Key" rules={[{ required: true }]}>
            <Input disabled />
          </Form.Item>
          <Form.Item name="value" label="Value" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item name="category" label="Category" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item name="description" label="Description">
            <Input />
          </Form.Item>
          <Form.Item name="dataType" label="Data Type" rules={[{ required: true }]}>
            <Select
              options={[
                { label: 'String', value: 'String' },
                { label: 'Int', value: 'Int' },
                { label: 'Bool', value: 'Bool' },
                { label: 'Json', value: 'Json' },
              ]}
            />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}
