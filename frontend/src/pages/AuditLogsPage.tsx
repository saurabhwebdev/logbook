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
  Flex,
} from 'antd';
import type { ColumnsType, TablePaginationConfig } from 'antd/es/table';
import { useQuery } from '@tanstack/react-query';
import dayjs from 'dayjs';
import type { Dayjs } from 'dayjs';
import { auditLogsApi } from '../api/auditLogsApi';
import { useTenantTheme } from '../contexts/ThemeContext';
import EmptyState from '../components/EmptyState';
import type { AuditLog } from '../types';

const { Text } = Typography;
const { RangePicker } = DatePicker;

export default function AuditLogsPage() {
  const { theme } = useTenantTheme();
  const primaryColor = theme?.primaryColor || '#0071e3';

  const actionConfig: Record<string, { color: string; bg: string }> = {
    Create: { color: '#34c759', bg: '#f0fdf4' },
    Update: { color: primaryColor, bg: '#f0f5ff' },
    Delete: { color: '#ff3b30', bg: '#fef2f2' },
  };
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
    if (!value) return '--';
    try {
      return JSON.stringify(JSON.parse(value), null, 2);
    } catch {
      return value;
    }
  };

  const columns: ColumnsType<AuditLog> = [
    {
      title: 'Time',
      dataIndex: 'timestamp',
      key: 'timestamp',
      width: 160,
      render: (value: string) => (
        <Text style={{ fontSize: 13, color: '#6e6e73', fontVariantNumeric: 'tabular-nums' }}>
          {dayjs(value).format('MMM D, YYYY HH:mm')}
        </Text>
      ),
    },
    {
      title: 'Action',
      dataIndex: 'action',
      key: 'action',
      width: 90,
      render: (value: string) => {
        const cfg = actionConfig[value] ?? { color: '#86868b', bg: '#f5f5f7' };
        return (
          <Tag style={{ background: cfg.bg, color: cfg.color, border: 'none', fontWeight: 500 }}>
            {value}
          </Tag>
        );
      },
    },
    {
      title: 'Entity',
      dataIndex: 'entityName',
      key: 'entityName',
      width: 130,
      render: (value: string) => (
        <Text style={{ fontSize: 13, fontWeight: 500, color: '#1d1d1f' }}>{value}</Text>
      ),
    },
    {
      title: 'Entity ID',
      dataIndex: 'entityId',
      key: 'entityId',
      ellipsis: true,
      width: 240,
      render: (value: string) => (
        <Text style={{ fontSize: 12, color: '#86868b', fontFamily: 'monospace' }}>{value}</Text>
      ),
    },
    {
      title: 'User',
      dataIndex: 'userId',
      key: 'userId',
      ellipsis: true,
      width: 240,
      render: (value: string) => (
        <Text style={{ fontSize: 12, color: '#86868b', fontFamily: 'monospace' }}>{value}</Text>
      ),
    },
    {
      title: 'IP',
      dataIndex: 'ipAddress',
      key: 'ipAddress',
      width: 120,
      render: (value: string | null) => (
        <Text style={{ fontSize: 12, color: '#86868b', fontFamily: 'monospace' }}>
          {value ?? '--'}
        </Text>
      ),
    },
  ];

  const expandedRowRender = (record: AuditLog) => (
    <Row gutter={16}>
      <Col span={12}>
        <Text style={{ fontWeight: 600, fontSize: 12, color: '#6e6e73', textTransform: 'uppercase', letterSpacing: '0.05em' }}>
          Old values
        </Text>
        <pre
          style={{
            background: '#f8f9fa',
            padding: 16,
            borderRadius: 8,
            maxHeight: 280,
            overflow: 'auto',
            fontSize: 12,
            fontFamily: "'SF Mono', 'Fira Code', monospace",
            marginTop: 8,
            border: '1px solid #e5e5ea',
            color: '#1d1d1f',
          }}
        >
          {formatJson(record.oldValues)}
        </pre>
      </Col>
      <Col span={12}>
        <Text style={{ fontWeight: 600, fontSize: 12, color: '#6e6e73', textTransform: 'uppercase', letterSpacing: '0.05em' }}>
          New values
        </Text>
        <pre
          style={{
            background: '#f8f9fa',
            padding: 16,
            borderRadius: 8,
            maxHeight: 280,
            overflow: 'auto',
            fontSize: 12,
            fontFamily: "'SF Mono', 'Fira Code', monospace",
            marginTop: 8,
            border: '1px solid #e5e5ea',
            color: '#1d1d1f',
          }}
        >
          {formatJson(record.newValues)}
        </pre>
      </Col>
    </Row>
  );

  return (
    <div>
      {/* Page header */}
      <Flex align="center" justify="space-between" style={{ marginBottom: 24 }}>
        <h2 style={{ fontSize: 22, fontWeight: 700, color: '#1d1d1f', margin: 0 }}>Audit Logs</h2>
      </Flex>

      {/* Filters */}
      <div
        style={{
          background: '#ffffff',
          borderRadius: 12,
          border: '1px solid #e5e5ea',
          padding: '16px 20px',
          marginBottom: 16,
        }}
      >
        <Flex gap={12} wrap="wrap">
          <RangePicker
            style={{ minWidth: 240 }}
            onChange={(dates) => setDateRange(dates as [Dayjs | null, Dayjs | null] | null)}
            placeholder={['Start date', 'End date']}
          />
          <Input
            placeholder="Entity name"
            value={entityName}
            onChange={(e) => { setEntityName(e.target.value); setPage(1); }}
            allowClear
            style={{ width: 160 }}
          />
          <Select
            placeholder="Action"
            value={action}
            onChange={(value) => { setAction(value); setPage(1); }}
            allowClear
            style={{ width: 120 }}
            options={[
              { label: 'Create', value: 'Create' },
              { label: 'Update', value: 'Update' },
              { label: 'Delete', value: 'Delete' },
            ]}
          />
          <Input
            placeholder="User ID"
            value={userId}
            onChange={(e) => { setUserId(e.target.value); setPage(1); }}
            allowClear
            style={{ width: 160 }}
          />
        </Flex>
      </div>

      {/* Table */}
      <div style={{ background: '#ffffff', borderRadius: 12, border: '1px solid #e5e5ea', overflow: 'hidden' }}>
        <Table<AuditLog>
          rowKey="id"
          columns={columns}
          dataSource={data?.items ?? []}
          loading={isLoading}
          onChange={handleTableChange}
          locale={{
            emptyText: (
              <EmptyState
                title={dateRange || entityName || action || userId ? "No audit logs found" : "No audit logs yet"}
                description={
                  dateRange || entityName || action || userId
                    ? "No audit logs match your current filters. Try adjusting your search criteria."
                    : "Audit logs will appear here as users make changes across the system. All create, update, and delete operations are automatically tracked."
                }
                size={180}
              />
            ),
          }}
          expandable={{
            expandedRowRender,
            rowExpandable: (record) => record.oldValues !== null || record.newValues !== null,
          }}
          pagination={{
            current: page,
            pageSize,
            total: data?.totalCount ?? 0,
            showSizeChanger: true,
            showTotal: (total) => <Text style={{ fontSize: 13, color: '#86868b' }}>{total} events</Text>,
            style: { padding: '0 16px' },
          }}
          scroll={{ x: 1000 }}
        />
      </div>
    </div>
  );
}
