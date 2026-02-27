import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Card,
  Table,
  Button,
  Tag,
  Modal,
  Form,
  Input,
  Switch,
  Select,
  message,
  Empty,
} from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import { workflowDefinitionsApi } from '../api/workflowDefinitionsApi';
import type { WorkflowDefinition } from '../types';
import { useTenantTheme } from '../contexts/ThemeContext';
import dayjs from 'dayjs';

const { TextArea } = Input;

export default function WorkflowDefinitionsPage() {
  const { theme } = useTenantTheme();
  const primaryColor = theme?.primaryColor || '#0071e3';
  const queryClient = useQueryClient();
  const [form] = Form.useForm();

  const [modalVisible, setModalVisible] = useState(false);

  const { data, isLoading } = useQuery({
    queryKey: ['workflow-definitions'],
    queryFn: () => workflowDefinitionsApi.getAll(undefined, undefined, undefined, 1, 50),
  });

  const createMutation = useMutation({
    mutationFn: workflowDefinitionsApi.create,
    onSuccess: () => {
      message.success('Workflow definition created successfully');
      queryClient.invalidateQueries({ queryKey: ['workflow-definitions'] });
      setModalVisible(false);
      form.resetFields();
    },
    onError: () => {
      message.error('Failed to create workflow definition');
    },
  });

  const handleCreate = () => {
    form.validateFields().then((values) => {
      createMutation.mutate(values);
    });
  };

  const columns = [
    {
      title: 'NAME',
      dataIndex: 'name',
      key: 'name',
      render: (text: string, record: WorkflowDefinition) => (
        <div>
          <div style={{ fontWeight: 500, color: '#1d1d1f' }}>{text}</div>
          <div style={{ fontSize: 12, color: '#86868b' }}>{record.description}</div>
        </div>
      ),
    },
    {
      title: 'CATEGORY',
      dataIndex: 'category',
      key: 'category',
      width: 150,
      render: (text: string) => <Tag>{text}</Tag>,
    },
    {
      title: 'VERSION',
      dataIndex: 'version',
      key: 'version',
      width: 100,
      render: (version: number) => <span style={{ color: '#6e6e73' }}>v{version}</span>,
    },
    {
      title: 'STATUS',
      dataIndex: 'isActive',
      key: 'isActive',
      width: 100,
      render: (isActive: boolean) => (
        <Tag color={isActive ? 'green' : 'default'}>{isActive ? 'Active' : 'Inactive'}</Tag>
      ),
    },
    {
      title: 'CREATED',
      dataIndex: 'createdAt',
      key: 'createdAt',
      width: 150,
      render: (date: string) => (
        <span style={{ color: '#86868b', fontSize: 13 }}>
          {dayjs(date).format('MMM D, YYYY')}
        </span>
      ),
    },
  ];

  return (
    <div style={{ padding: 24 }}>
      <div style={{ marginBottom: 24, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <div>
          <h1 style={{ margin: 0, fontSize: 28, fontWeight: 600, color: '#1d1d1f' }}>
            Workflow Definitions
          </h1>
          <p style={{ margin: '4px 0 0', color: '#86868b', fontSize: 14 }}>
            Define and manage workflow templates
          </p>
        </div>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => setModalVisible(true)}
          style={{ backgroundColor: primaryColor }}
        >
          New Definition
        </Button>
      </div>

      <Card
        style={{
          borderRadius: 12,
          border: '1px solid #e5e5ea',
          boxShadow: 'none',
        }}
      >
        <Table
          dataSource={data?.items || []}
          columns={columns}
          rowKey="id"
          loading={isLoading}
          pagination={false}
          locale={{
            emptyText: (
              <Empty
                image={Empty.PRESENTED_IMAGE_SIMPLE}
                description={<span style={{ color: '#86868b' }}>No workflow definitions found</span>}
              />
            ),
          }}
        />
      </Card>

      <Modal
        title="Create Workflow Definition"
        open={modalVisible}
        onOk={handleCreate}
        onCancel={() => {
          setModalVisible(false);
          form.resetFields();
        }}
        confirmLoading={createMutation.isPending}
        width={700}
      >
        <Form form={form} layout="vertical" style={{ marginTop: 24 }}>
          <Form.Item
            name="name"
            label="Name"
            rules={[{ required: true, message: 'Please enter a name' }]}
          >
            <Input placeholder="e.g., Purchase Order Approval" />
          </Form.Item>

          <Form.Item
            name="description"
            label="Description"
            rules={[{ required: true, message: 'Please enter a description' }]}
          >
            <TextArea rows={3} placeholder="Describe the workflow purpose..." />
          </Form.Item>

          <Form.Item
            name="category"
            label="Category"
            rules={[{ required: true, message: 'Please select a category' }]}
          >
            <Select
              options={[
                { label: 'Approval', value: 'Approval' },
                { label: 'Notification', value: 'Notification' },
                { label: 'Data Processing', value: 'DataProcessing' },
              ]}
            />
          </Form.Item>

          <Form.Item
            name="configurationJson"
            label="Configuration (JSON)"
            rules={[{ required: true, message: 'Please enter configuration JSON' }]}
            extra="Define workflow steps in JSON format"
          >
            <TextArea
              rows={10}
              placeholder={`{
  "Steps": [
    { "Name": "Submit", "Type": "Start" },
    { "Name": "ManagerApproval", "Type": "Approval" },
    { "Name": "Complete", "Type": "End" }
  ]
}`}
              style={{ fontFamily: 'monospace', fontSize: 12 }}
            />
          </Form.Item>

          <Form.Item name="isActive" label="Active" valuePropName="checked" initialValue={true}>
            <Switch />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}
