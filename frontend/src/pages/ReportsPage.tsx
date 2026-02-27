import { useState } from 'react';
import { Typography, Table, Flex, Button, Modal, Form, Input, Select, message, Popconfirm } from 'antd';
import { PlusOutlined, DeleteOutlined, DownloadOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import dayjs from 'dayjs';
import { reportsApi } from '../api/reportsApi';
import { useTenantTheme } from '../contexts/ThemeContext';
import type { ReportDefinition } from '../types';

const { Text } = Typography;

export default function ReportsPage() {
  const { theme } = useTenantTheme();
  const primaryColor = theme?.primaryColor || '#0071e3';
  const [createOpen, setCreateOpen] = useState(false);
  const [form] = Form.useForm();
  const queryClient = useQueryClient();

  const { data, isLoading } = useQuery({
    queryKey: ['reports'],
    queryFn: () => reportsApi.getAll(),
  });

  const createMutation = useMutation({
    mutationFn: reportsApi.create,
    onSuccess: () => {
      message.success('Report created');
      queryClient.invalidateQueries({ queryKey: ['reports'] });
      setCreateOpen(false);
      form.resetFields();
    },
    onError: () => message.error('Failed to create report'),
  });

  const deleteMutation = useMutation({
    mutationFn: reportsApi.delete,
    onSuccess: () => {
      message.success('Report deleted');
      queryClient.invalidateQueries({ queryKey: ['reports'] });
    },
  });

  const exportMutation = useMutation({
    mutationFn: reportsApi.export,
    onSuccess: () => message.success('Report exported'),
    onError: () => message.error('Export failed'),
  });

  const handleCreate = () => {
    form.validateFields().then((values) => {
      createMutation.mutate({ ...values, columnsJson: values.columnsJson || '[]' });
    });
  };

  const columns: ColumnsType<ReportDefinition> = [
    {
      title: 'Report',
      key: 'name',
      render: (_, record) => (
        <div>
          <div style={{ fontWeight: 500, color: '#1d1d1f', fontSize: 13 }}>{record.name}</div>
          {record.description && <div style={{ fontSize: 12, color: '#86868b' }}>{record.description}</div>}
        </div>
      ),
      sorter: (a, b) => a.name.localeCompare(b.name),
    },
    {
      title: 'Entity',
      dataIndex: 'entityType',
      key: 'entityType',
      width: 120,
      render: (value: string) => (
        <span style={{ fontSize: 12, fontWeight: 500, color: '#6e6e73', background: '#f5f5f7', padding: '2px 10px', borderRadius: 10 }}>{value}</span>
      ),
    },
    {
      title: 'Format',
      dataIndex: 'exportFormat',
      key: 'exportFormat',
      width: 100,
      render: (value: string) => <Text style={{ fontSize: 13, color: '#6e6e73' }}>{value}</Text>,
    },
    {
      title: 'Status',
      dataIndex: 'isActive',
      key: 'isActive',
      width: 100,
      render: (value: boolean) => (
        <span style={{ display: 'inline-flex', alignItems: 'center', gap: 6, fontSize: 12, fontWeight: 500, color: value ? '#34c759' : '#86868b' }}>
          <span style={{ width: 6, height: 6, borderRadius: '50%', background: value ? '#34c759' : '#86868b' }} />
          {value ? 'Active' : 'Inactive'}
        </span>
      ),
    },
    {
      title: 'Created',
      dataIndex: 'createdAt',
      key: 'createdAt',
      width: 110,
      render: (value: string) => <Text style={{ fontSize: 13, color: '#86868b' }}>{dayjs(value).format('MMM D, YYYY')}</Text>,
    },
    {
      title: '',
      key: 'actions',
      width: 100,
      render: (_, record) => (
        <Flex gap={4}>
          <Button type="text" size="small" icon={<DownloadOutlined />} style={{ color: primaryColor }} onClick={() => exportMutation.mutate(record.id)} loading={exportMutation.isPending} />
          <Popconfirm title="Delete this report?" onConfirm={() => deleteMutation.mutate(record.id)} okText="Delete" okButtonProps={{ danger: true }}>
            <Button type="text" size="small" icon={<DeleteOutlined />} danger />
          </Popconfirm>
        </Flex>
      ),
    },
  ];

  return (
    <div>
      <Flex align="center" justify="space-between" style={{ marginBottom: 24 }}>
        <div>
          <h2 style={{ fontSize: 22, fontWeight: 700, color: '#1d1d1f', margin: 0 }}>Reports</h2>
          <Text style={{ fontSize: 13, color: '#86868b' }}>Define and manage export reports.</Text>
        </div>
        <Button type="primary" icon={<PlusOutlined />} onClick={() => setCreateOpen(true)}>
          New Report
        </Button>
      </Flex>

      <div style={{ background: '#ffffff', borderRadius: 12, border: '1px solid #e5e5ea', overflow: 'hidden' }}>
        <Table<ReportDefinition> rowKey="id" columns={columns} dataSource={data ?? []} loading={isLoading} pagination={false} />
      </div>

      <Modal title="New Report" open={createOpen} onCancel={() => setCreateOpen(false)} onOk={handleCreate} confirmLoading={createMutation.isPending} okText="Create">
        <Form form={form} layout="vertical" style={{ marginTop: 16 }}>
          <Form.Item name="name" label="Name" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item name="description" label="Description">
            <Input />
          </Form.Item>
          <Form.Item name="entityType" label="Entity Type" rules={[{ required: true }]}>
            <Select options={[
              { label: 'Users', value: 'User' },
              { label: 'Departments', value: 'Department' },
              { label: 'Roles', value: 'Role' },
              { label: 'Audit Logs', value: 'AuditLog' },
              { label: 'Demo Tasks', value: 'DemoTask' },
            ]} />
          </Form.Item>
          <Form.Item name="columnsJson" label="Columns (JSON)" initialValue="[]">
            <Input.TextArea rows={3} />
          </Form.Item>
          <Form.Item name="exportFormat" label="Export Format" rules={[{ required: true }]} initialValue="Excel">
            <Select options={[
              { label: 'Excel', value: 'Excel' },
              { label: 'CSV', value: 'Csv' },
              { label: 'PDF', value: 'PDF' },
            ]} />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}
