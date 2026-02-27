import { Typography, Table, Tag, Flex } from 'antd';
import type { ColumnsType } from 'antd/es/table';
import { useQuery } from '@tanstack/react-query';
import dayjs from 'dayjs';
import { tenantsApi } from '../api/tenantsApi';
import type { Tenant } from '../types';

const { Text } = Typography;

export default function TenantsPage() {
  const { data, isLoading } = useQuery({
    queryKey: ['tenants'],
    queryFn: () => tenantsApi.getTenants(),
  });

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
  ];

  return (
    <div>
      <Flex align="center" justify="space-between" style={{ marginBottom: 24 }}>
        <div>
          <h2 style={{ fontSize: 22, fontWeight: 700, color: '#1d1d1f', margin: 0 }}>Tenants</h2>
          <Text style={{ fontSize: 13, color: '#86868b' }}>Manage multi-tenant organizations.</Text>
        </div>
      </Flex>

      <div style={{ background: '#ffffff', borderRadius: 12, border: '1px solid #e5e5ea', overflow: 'hidden' }}>
        <Table<Tenant>
          rowKey="id"
          columns={columns}
          dataSource={data ?? []}
          loading={isLoading}
          pagination={{ showSizeChanger: true, style: { padding: '0 16px' } }}
        />
      </div>
    </div>
  );
}
