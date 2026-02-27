import { useState } from 'react';
import {
  Typography,
  Table,
  Tag,
  Select,
  Modal,
  Descriptions,
} from 'antd';
import type { ColumnsType } from 'antd/es/table';
import { useQuery } from '@tanstack/react-query';
import { emailQueueApi } from '../api/emailQueueApi';
import type { EmailQueue, EmailStatus } from '../types';
import { EyeOutlined } from '@ant-design/icons';

const { Title, Text } = Typography;

const statusOptions = [
  { label: 'All', value: undefined },
  { label: 'Pending', value: 0 },
  { label: 'Sent', value: 1 },
  { label: 'Failed', value: 2 },
];

export default function EmailQueuePage() {
  const [statusFilter, setStatusFilter] = useState<number | undefined>(undefined);
  const [previewEmail, setPreviewEmail] = useState<EmailQueue | null>(null);

  const { data, isLoading } = useQuery({
    queryKey: ['emailQueue', statusFilter],
    queryFn: () => emailQueueApi.getQueue(statusFilter),
  });

  const getStatusColor = (status: EmailStatus) => {
    switch (status) {
      case 0: // Pending
        return 'blue';
      case 1: // Sent
        return 'green';
      case 2: // Failed
        return 'red';
      default:
        return 'default';
    }
  };

  const getStatusText = (status: EmailStatus) => {
    switch (status) {
      case 0:
        return 'Pending';
      case 1:
        return 'Sent';
      case 2:
        return 'Failed';
      default:
        return 'Unknown';
    }
  };

  const columns: ColumnsType<EmailQueue> = [
    {
      title: 'TO',
      dataIndex: 'to',
      key: 'to',
      width: 250,
    },
    {
      title: 'SUBJECT',
      dataIndex: 'subject',
      key: 'subject',
    },
    {
      title: 'STATUS',
      key: 'status',
      width: 120,
      render: (_, record) => (
        <Tag color={getStatusColor(record.status)}>
          {getStatusText(record.status)}
        </Tag>
      ),
    },
    {
      title: 'RETRY COUNT',
      dataIndex: 'retryCount',
      key: 'retryCount',
      width: 130,
    },
    {
      title: 'SENT AT',
      dataIndex: 'sentAt',
      key: 'sentAt',
      width: 180,
      render: (date: string) => (date ? new Date(date).toLocaleString() : '-'),
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
      width: 100,
      render: (_, record) => (
        <EyeOutlined
          style={{ cursor: 'pointer' }}
          onClick={() => setPreviewEmail(record)}
        />
      ),
    },
  ];

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 24 }}>
        <div>
          <Title level={2} style={{ margin: 0 }}>Email Queue</Title>
          <Text type="secondary">Monitor outgoing emails and their delivery status</Text>
        </div>
        <Select
          style={{ width: 150 }}
          value={statusFilter}
          onChange={setStatusFilter}
          options={statusOptions}
          placeholder="Filter by status"
        />
      </div>

      <Table
        columns={columns}
        dataSource={data || []}
        loading={isLoading}
        rowKey="id"
        pagination={{ pageSize: 50 }}
      />

      <Modal
        title="Email Preview"
        open={!!previewEmail}
        onCancel={() => setPreviewEmail(null)}
        footer={null}
        width={800}
      >
        {previewEmail && (
          <Descriptions column={1} bordered>
            <Descriptions.Item label="To">{previewEmail.to}</Descriptions.Item>
            <Descriptions.Item label="Subject">{previewEmail.subject}</Descriptions.Item>
            <Descriptions.Item label="Status">
              <Tag color={getStatusColor(previewEmail.status)}>
                {getStatusText(previewEmail.status)}
              </Tag>
            </Descriptions.Item>
            <Descriptions.Item label="Retry Count">{previewEmail.retryCount}</Descriptions.Item>
            {previewEmail.failureReason && (
              <Descriptions.Item label="Failure Reason">
                <Text type="danger">{previewEmail.failureReason}</Text>
              </Descriptions.Item>
            )}
            <Descriptions.Item label="Sent At">
              {previewEmail.sentAt ? new Date(previewEmail.sentAt).toLocaleString() : '-'}
            </Descriptions.Item>
            <Descriptions.Item label="Created At">
              {new Date(previewEmail.createdAt).toLocaleString()}
            </Descriptions.Item>
          </Descriptions>
        )}
      </Modal>
    </div>
  );
}
