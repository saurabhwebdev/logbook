import { Typography, Card, Row, Col, Button, Space, message, Statistic } from 'antd';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { backgroundJobsApi } from '../api/backgroundJobsApi';
import {
  CheckCircleOutlined,
  CloseCircleOutlined,
  SyncOutlined,
  ClockCircleOutlined,
  PlayCircleOutlined,
  DeleteOutlined,
} from '@ant-design/icons';
import PermissionGate from '../components/PermissionGate';

const { Title, Text } = Typography;

export default function BackgroundJobsPage() {
  const queryClient = useQueryClient();

  const { data: stats, isLoading } = useQuery({
    queryKey: ['backgroundJobStats'],
    queryFn: () => backgroundJobsApi.getStats(),
    refetchInterval: 10000, // Refresh every 10 seconds
  });

  const triggerEmailQueueMutation = useMutation({
    mutationFn: () => backgroundJobsApi.triggerProcessEmailQueue(),
    onSuccess: () => {
      message.success('Email queue processing job triggered');
      queryClient.invalidateQueries({ queryKey: ['backgroundJobStats'] });
    },
    onError: () => message.error('Failed to trigger job'),
  });

  const triggerCleanupMutation = useMutation({
    mutationFn: () => backgroundJobsApi.triggerCleanupAuditLogs(),
    onSuccess: () => {
      message.success('Audit logs cleanup job triggered');
      queryClient.invalidateQueries({ queryKey: ['backgroundJobStats'] });
    },
    onError: () => message.error('Failed to trigger job'),
  });

  return (
    <div>
      <div style={{ marginBottom: 24 }}>
        <Title level={2} style={{ margin: 0 }}>Background Jobs</Title>
        <Text type="secondary">Monitor and manage background job execution</Text>
      </div>

      <Row gutter={[16, 16]} style={{ marginBottom: 24 }}>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="Succeeded Jobs"
              value={stats?.succeededJobs || 0}
              prefix={<CheckCircleOutlined style={{ color: '#34c759' }} />}
              loading={isLoading}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="Failed Jobs"
              value={stats?.failedJobs || 0}
              prefix={<CloseCircleOutlined style={{ color: '#ff3b30' }} />}
              loading={isLoading}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="Processing Jobs"
              value={stats?.processingJobs || 0}
              prefix={<SyncOutlined spin style={{ color: '#0071e3' }} />}
              loading={isLoading}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="Scheduled Jobs"
              value={stats?.scheduledJobs || 0}
              prefix={<ClockCircleOutlined style={{ color: '#ff9500' }} />}
              loading={isLoading}
            />
          </Card>
        </Col>
      </Row>

      <Card title="Manual Job Triggers" style={{ marginBottom: 24 }}>
        <PermissionGate permissions={['BackgroundJob.Manage']}>
          <Space direction="vertical" size="middle" style={{ width: '100%' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '12px 0', borderBottom: '1px solid #e5e5ea' }}>
              <div>
                <div style={{ fontWeight: 500 }}>Process Email Queue</div>
                <Text type="secondary" style={{ fontSize: 12 }}>Send pending emails from the queue</Text>
              </div>
              <Button
                icon={<PlayCircleOutlined />}
                onClick={() => triggerEmailQueueMutation.mutate()}
                loading={triggerEmailQueueMutation.isPending}
              >
                Trigger Now
              </Button>
            </div>

            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '12px 0' }}>
              <div>
                <div style={{ fontWeight: 500 }}>Cleanup Audit Logs</div>
                <Text type="secondary" style={{ fontSize: 12 }}>Delete audit logs older than 90 days</Text>
              </div>
              <Button
                icon={<DeleteOutlined />}
                onClick={() => triggerCleanupMutation.mutate()}
                loading={triggerCleanupMutation.isPending}
              >
                Trigger Now
              </Button>
            </div>
          </Space>
        </PermissionGate>
      </Card>

      <Card title="Hangfire Dashboard">
        <Text type="secondary" style={{ display: 'block', marginBottom: 16 }}>
          View detailed job execution history, queues, and statistics in the Hangfire dashboard.
        </Text>
        <PermissionGate permissions={['BackgroundJob.Read']}>
          <div style={{ background: '#f5f5f7', padding: 16, borderRadius: 8, marginBottom: 16 }}>
            <Text type="secondary" style={{ fontSize: 13 }}>
              Access the Hangfire Dashboard directly at:{' '}
              <a href="http://localhost:5034/hangfire" target="_blank" rel="noopener noreferrer" style={{ color: '#0071e3' }}>
                http://localhost:5034/hangfire
              </a>
            </Text>
          </div>
          <iframe
            src="http://localhost:5034/hangfire"
            style={{
              width: '100%',
              height: '600px',
              border: '1px solid #e5e5ea',
              borderRadius: '8px',
            }}
            title="Hangfire Dashboard"
          />
        </PermissionGate>
      </Card>
    </div>
  );
}
