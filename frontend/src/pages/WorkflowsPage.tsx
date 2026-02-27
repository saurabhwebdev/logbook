import { useQuery } from '@tanstack/react-query';
import { Card, Row, Col, Statistic, Table, Tag, Empty } from 'antd';
import {
  ApartmentOutlined,
  PlayCircleOutlined,
  CheckCircleOutlined,
  ClockCircleOutlined,
} from '@ant-design/icons';
import { workflowInstancesApi } from '../api/workflowInstancesApi';
import type { WorkflowInstance } from '../types';
import { useTenantTheme } from '../contexts/ThemeContext';
import dayjs from 'dayjs';

export default function WorkflowsPage() {
  const { theme } = useTenantTheme();
  const primaryColor = theme?.primaryColor || '#0071e3';

  const { data: stats } = useQuery({
    queryKey: ['workflow-statistics'],
    queryFn: () => workflowInstancesApi.getStatistics(),
  });

  const { data, isLoading } = useQuery({
    queryKey: ['workflow-instances'],
    queryFn: () => workflowInstancesApi.getAll(undefined, undefined, undefined, 1, 50),
  });

  const getStatusTag = (status: string) => {
    const colors: Record<string, string> = {
      Running: 'blue',
      Completed: 'green',
      Cancelled: 'red',
    };
    return <Tag color={colors[status] || 'default'}>{status}</Tag>;
  };

  const columns = [
    {
      title: 'WORKFLOW',
      dataIndex: 'workflowDefinitionName',
      key: 'workflowDefinitionName',
      render: (text: string, record: WorkflowInstance) => (
        <div>
          <div style={{ fontWeight: 500, color: '#1d1d1f' }}>{text}</div>
          <div style={{ fontSize: 12, color: '#86868b' }}>
            {record.entityType}: {record.entityId}
          </div>
        </div>
      ),
    },
    {
      title: 'CURRENT STEP',
      dataIndex: 'currentStepName',
      key: 'currentStepName',
      width: 200,
      render: (text: string) => <span style={{ color: '#6e6e73' }}>{text}</span>,
    },
    {
      title: 'STATUS',
      dataIndex: 'status',
      key: 'status',
      width: 120,
      render: (status: string) => getStatusTag(status),
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
      <div style={{ marginBottom: 24 }}>
        <h1 style={{ margin: 0, fontSize: 28, fontWeight: 600, color: '#1d1d1f' }}>
          Workflows
        </h1>
        <p style={{ margin: '4px 0 0', color: '#86868b', fontSize: 14 }}>
          Workflow instances and statistics
        </p>
      </div>

      <Row gutter={[16, 16]} style={{ marginBottom: 24 }}>
        <Col xs={24} sm={12} lg={6}>
          <Card
            style={{
              borderRadius: 12,
              border: '1px solid #e5e5ea',
              boxShadow: 'none',
            }}
          >
            <Statistic
              title={<span style={{ color: '#86868b', fontSize: 13 }}>Total Definitions</span>}
              value={stats?.totalDefinitions || 0}
              prefix={<ApartmentOutlined style={{ color: primaryColor }} />}
              valueStyle={{ color: '#1d1d1f', fontSize: 28, fontWeight: 600 }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card
            style={{
              borderRadius: 12,
              border: '1px solid #e5e5ea',
              boxShadow: 'none',
            }}
          >
            <Statistic
              title={<span style={{ color: '#86868b', fontSize: 13 }}>Active Instances</span>}
              value={stats?.activeInstances || 0}
              prefix={<PlayCircleOutlined style={{ color: '#0066cc' }} />}
              valueStyle={{ color: '#1d1d1f', fontSize: 28, fontWeight: 600 }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card
            style={{
              borderRadius: 12,
              border: '1px solid #e5e5ea',
              boxShadow: 'none',
            }}
          >
            <Statistic
              title={<span style={{ color: '#86868b', fontSize: 13 }}>Completed Today</span>}
              value={stats?.completedToday || 0}
              prefix={<CheckCircleOutlined style={{ color: '#34c759' }} />}
              valueStyle={{ color: '#1d1d1f', fontSize: 28, fontWeight: 600 }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card
            style={{
              borderRadius: 12,
              border: '1px solid #e5e5ea',
              boxShadow: 'none',
            }}
          >
            <Statistic
              title={<span style={{ color: '#86868b', fontSize: 13 }}>Pending Tasks</span>}
              value={stats?.pendingTasks || 0}
              prefix={<ClockCircleOutlined style={{ color: '#ff9500' }} />}
              valueStyle={{ color: '#1d1d1f', fontSize: 28, fontWeight: 600 }}
            />
          </Card>
        </Col>
      </Row>

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
                description={<span style={{ color: '#86868b' }}>No workflow instances found</span>}
              />
            ),
          }}
        />
      </Card>
    </div>
  );
}
