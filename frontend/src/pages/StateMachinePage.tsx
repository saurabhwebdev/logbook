import { Typography, Table, Flex, Tag, Empty, Spin } from 'antd';
import type { ColumnsType } from 'antd/es/table';
import { useQuery } from '@tanstack/react-query';
import { ArrowRightOutlined } from '@ant-design/icons';
import { stateMachineApi } from '../api/stateMachineApi';
import { useTenantTheme } from '../contexts/ThemeContext';
import type { StateDefinition, StateTransitionDefinition } from '../types';

const { Text } = Typography;

export default function StateMachinePage() {
  const { theme } = useTenantTheme();
  const primaryColor = theme?.primaryColor || '#0071e3';
  const { data, isLoading } = useQuery({
    queryKey: ['statemachine', 'Task'],
    queryFn: () => stateMachineApi.getDefinitions('Task'),
  });

  const stateColumns: ColumnsType<StateDefinition> = [
    {
      title: 'State',
      key: 'stateName',
      render: (_, record) => (
        <Flex align="center" gap={8}>
          <span
            style={{
              width: 10,
              height: 10,
              borderRadius: '50%',
              background: record.color || '#86868b',
              flexShrink: 0,
            }}
          />
          <span style={{ fontWeight: 500, fontSize: 13, color: '#1d1d1f' }}>
            {record.stateName}
          </span>
          {record.isInitial && (
            <Tag
              color="blue"
              style={{ fontSize: 11, borderRadius: 10, lineHeight: '18px', margin: 0 }}
            >
              Initial
            </Tag>
          )}
          {record.isFinal && (
            <Tag
              color="default"
              style={{ fontSize: 11, borderRadius: 10, lineHeight: '18px', margin: 0 }}
            >
              Final
            </Tag>
          )}
        </Flex>
      ),
    },
    {
      title: 'Order',
      dataIndex: 'sortOrder',
      key: 'sortOrder',
      width: 80,
      render: (value: number) => (
        <Text style={{ fontSize: 13, color: '#86868b' }}>{value}</Text>
      ),
    },
  ];

  const transitionColumns: ColumnsType<StateTransitionDefinition> = [
    {
      title: 'Transition',
      key: 'transition',
      render: (_, record) => (
        <Flex align="center" gap={8}>
          <code
            style={{
              fontSize: 12,
              background: '#f5f5f7',
              padding: '2px 8px',
              borderRadius: 4,
            }}
          >
            {record.fromState}
          </code>
          <ArrowRightOutlined style={{ fontSize: 11, color: '#86868b' }} />
          <code
            style={{
              fontSize: 12,
              background: '#f5f5f7',
              padding: '2px 8px',
              borderRadius: 4,
            }}
          >
            {record.toState}
          </code>
        </Flex>
      ),
    },
    {
      title: 'Trigger',
      dataIndex: 'triggerName',
      key: 'triggerName',
      width: 140,
      render: (value: string) => (
        <span style={{ fontWeight: 500, fontSize: 13, color: primaryColor }}>{value}</span>
      ),
    },
    {
      title: 'Description',
      dataIndex: 'description',
      key: 'description',
      render: (value: string | null) => (
        <Text style={{ fontSize: 13, color: '#6e6e73' }}>{value || '-'}</Text>
      ),
    },
    {
      title: 'Permission',
      dataIndex: 'requiredPermission',
      key: 'requiredPermission',
      width: 160,
      render: (value: string | null) =>
        value ? (
          <code style={{ fontSize: 11, background: '#f5f5f7', padding: '2px 6px', borderRadius: 4 }}>
            {value}
          </code>
        ) : (
          <Text style={{ fontSize: 12, color: '#86868b' }}>None</Text>
        ),
    },
  ];

  return (
    <div>
      <Flex align="center" justify="space-between" style={{ marginBottom: 24 }}>
        <div>
          <h2 style={{ fontSize: 22, fontWeight: 700, color: '#1d1d1f', margin: 0 }}>
            State Machine
          </h2>
          <Text style={{ fontSize: 13, color: '#86868b' }}>
            Entity lifecycle states and transitions. Showing: Task
          </Text>
        </div>
      </Flex>

      {isLoading ? (
        <Flex align="center" justify="center" style={{ padding: 60 }}>
          <Spin />
        </Flex>
      ) : !data || (data.states.length === 0 && data.transitions.length === 0) ? (
        <div
          style={{
            background: '#ffffff',
            borderRadius: 12,
            border: '1px solid #e5e5ea',
            overflow: 'hidden',
          }}
        >
          <Empty description="No state definitions found" style={{ padding: 60 }} />
        </div>
      ) : (
        <>
          {/* States */}
          <div style={{ marginBottom: 24 }}>
            <Text
              style={{
                fontSize: 14,
                fontWeight: 600,
                color: '#1d1d1f',
                display: 'block',
                marginBottom: 12,
              }}
            >
              States
            </Text>
            <div
              style={{
                background: '#ffffff',
                borderRadius: 12,
                border: '1px solid #e5e5ea',
                overflow: 'hidden',
              }}
            >
              <Table<StateDefinition>
                rowKey="id"
                columns={stateColumns}
                dataSource={data.states}
                pagination={false}
                size="small"
              />
            </div>
          </div>

          {/* Transitions */}
          <div>
            <Text
              style={{
                fontSize: 14,
                fontWeight: 600,
                color: '#1d1d1f',
                display: 'block',
                marginBottom: 12,
              }}
            >
              Transitions
            </Text>
            <div
              style={{
                background: '#ffffff',
                borderRadius: 12,
                border: '1px solid #e5e5ea',
                overflow: 'hidden',
              }}
            >
              <Table<StateTransitionDefinition>
                rowKey="id"
                columns={transitionColumns}
                dataSource={data.transitions}
                pagination={false}
                size="small"
              />
            </div>
          </div>
        </>
      )}
    </div>
  );
}
