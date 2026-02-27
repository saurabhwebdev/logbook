import { Typography, Table, Switch, Flex, message } from 'antd';
import type { ColumnsType } from 'antd/es/table';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { featureFlagsApi } from '../api/featureFlagsApi';
import type { FeatureFlag } from '../types';

const { Text } = Typography;

export default function FeatureFlagsPage() {
  const queryClient = useQueryClient();

  const { data, isLoading } = useQuery({
    queryKey: ['featureFlags'],
    queryFn: () => featureFlagsApi.getAll(),
  });

  const toggleMutation = useMutation({
    mutationFn: ({ id, isEnabled }: { id: string; isEnabled: boolean }) =>
      featureFlagsApi.toggle(id, isEnabled),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['featureFlags'] });
    },
    onError: () => message.error('Failed to toggle feature flag'),
  });

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
  ];

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
          pagination={false}
        />
      </div>
    </div>
  );
}
