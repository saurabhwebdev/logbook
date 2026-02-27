import { useState } from 'react';
import {
  Typography,
  Table,
  Tag,
  Input,
  Select,
  DatePicker,
  Row,
  Col,
} from 'antd';
import type { ColumnsType, TablePaginationConfig } from 'antd/es/table';
import { useQuery } from '@tanstack/react-query';
import dayjs from 'dayjs';
import type { Dayjs } from 'dayjs';
import { auditLogsApi } from '../api/auditLogsApi';
import type { AuditLog } from '../types';

const { Title } = Typography;
const { RangePicker } = DatePicker;

const actionColorMap: Record<string, string> = {
  Create: 'green',
  Update: 'blue',
  Delete: 'red',
};

export default function AuditLogsPage() {
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [dateRange, setDateRange] = useState<[Dayjs | null, Dayjs | null] | null>(null);
  const [entityName, setEntityName] = useState('');
  const [action, setAction] = useState<string | undefined>(undefined);
  const [userId, setUserId] = useState('');

  const { data, isLoading } = useQuery({
    queryKey: ['auditLogs', page, pageSize, dateRange, entityName, action, userId],
    queryFn: () =>
      auditLogsApi.getAuditLogs({
        pageNumber: page,
        pageSize,
        startDate: dateRange?.[0]?.startOf('day').toISOString(),
        endDate: dateRange?.[1]?.endOf('day').toISOString(),
        entityName: entityName || undefined,
        action: action || undefined,
        userId: userId || undefined,
      }),
  });

  const handleTableChange = (pagination: TablePaginationConfig) => {
    setPage(pagination.current ?? 1);
    setPageSize(pagination.pageSize ?? 10);
  };

  const formatJson = (value: string | null): string => {
    if (!value) return 'N/A';
    try {
      return JSON.stringify(JSON.parse(value), null, 2);
    } catch {
      return value;
    }
  };

  const columns: ColumnsType<AuditLog> = [
    {
      title: 'Timestamp',
      dataIndex: 'timestamp',
      key: 'timestamp',
      render: (value: string) => dayjs(value).format('YYYY-MM-DD HH:mm:ss'),
      width: 180,
    },
    {
      title: 'Action',
      dataIndex: 'action',
      key: 'action',
      render: (value: string) => (
        <Tag color={actionColorMap[value] ?? 'default'}>{value}</Tag>
      ),
      width: 100,
    },
    {
      title: 'Entity',
      dataIndex: 'entityName',
      key: 'entityName',
      width: 140,
    },
    {
      title: 'Entity ID',
      dataIndex: 'entityId',
      key: 'entityId',
      ellipsis: true,
      width: 280,
    },
    {
      title: 'User',
      dataIndex: 'userId',
      key: 'userId',
      ellipsis: true,
      width: 280,
    },
    {
      title: 'IP Address',
      dataIndex: 'ipAddress',
      key: 'ipAddress',
      render: (value: string | null) => value ?? '-',
      width: 140,
    },
  ];

  const expandedRowRender = (record: AuditLog) => (
    <Row gutter={16}>
      <Col span={12}>
        <Typography.Text strong>Old Values:</Typography.Text>
        <pre
          style={{
            background: '#f5f5f5',
            padding: 12,
            borderRadius: 4,
            maxHeight: 300,
            overflow: 'auto',
            fontSize: 12,
          }}
        >
          {formatJson(record.oldValues)}
        </pre>
      </Col>
      <Col span={12}>
        <Typography.Text strong>New Values:</Typography.Text>
        <pre
          style={{
            background: '#f5f5f5',
            padding: 12,
            borderRadius: 4,
            maxHeight: 300,
            overflow: 'auto',
            fontSize: 12,
          }}
        >
          {formatJson(record.newValues)}
        </pre>
      </Col>
    </Row>
  );

  return (
    <div>
      <Title level={3}>Audit Logs</Title>

      <Row gutter={16} style={{ marginBottom: 16 }}>
        <Col span={6}>
          <RangePicker
            style={{ width: '100%' }}
            onChange={(dates) =>
              setDateRange(dates as [Dayjs | null, Dayjs | null] | null)
            }
            placeholder={['Start Date', 'End Date']}
          />
        </Col>
        <Col span={5}>
          <Input
            placeholder="Entity Name"
            value={entityName}
            onChange={(e) => {
              setEntityName(e.target.value);
              setPage(1);
            }}
            allowClear
          />
        </Col>
        <Col span={4}>
          <Select
            placeholder="Action"
            value={action}
            onChange={(value) => {
              setAction(value);
              setPage(1);
            }}
            allowClear
            style={{ width: '100%' }}
            options={[
              { label: 'Create', value: 'Create' },
              { label: 'Update', value: 'Update' },
              { label: 'Delete', value: 'Delete' },
            ]}
          />
        </Col>
        <Col span={5}>
          <Input
            placeholder="User ID"
            value={userId}
            onChange={(e) => {
              setUserId(e.target.value);
              setPage(1);
            }}
            allowClear
          />
        </Col>
      </Row>

      <Table<AuditLog>
        rowKey="id"
        columns={columns}
        dataSource={data?.items ?? []}
        loading={isLoading}
        onChange={handleTableChange}
        expandable={{
          expandedRowRender,
          rowExpandable: (record) =>
            record.oldValues !== null || record.newValues !== null,
        }}
        pagination={{
          current: page,
          pageSize,
          total: data?.totalCount ?? 0,
          showSizeChanger: true,
          showTotal: (total) => `Total ${total} records`,
        }}
        scroll={{ x: 1100 }}
      />
    </div>
  );
}
