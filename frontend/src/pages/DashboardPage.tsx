import { Row, Col, Typography, Spin, Alert, Flex } from 'antd';
import {
  UserOutlined,
  TeamOutlined,
  ApartmentOutlined,
  FileSearchOutlined,
  FolderOutlined,
  BarChartOutlined,
  ExperimentOutlined,
  FlagOutlined,
  ApiOutlined,
} from '@ant-design/icons';
import { useQuery } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import dayjs from 'dayjs';
import { useAuth } from '../contexts/AuthContext';
import { dashboardApi } from '../api/dashboardApi';
import type { RecentActivity } from '../api/dashboardApi';

const { Text } = Typography;

interface StatCardProps {
  label: string;
  value: number;
  icon: React.ReactNode;
  color: string;
  onClick?: () => void;
}

function StatCard({ label, value, icon, color, onClick }: StatCardProps) {
  return (
    <div
      onClick={onClick}
      style={{
        background: '#ffffff',
        borderRadius: 12,
        border: '1px solid #e5e5ea',
        padding: '24px',
        cursor: onClick ? 'pointer' : 'default',
        transition: 'border-color 0.2s, transform 0.15s',
      }}
      onMouseEnter={(e) => {
        e.currentTarget.style.borderColor = '#d1d1d6';
        if (onClick) e.currentTarget.style.transform = 'translateY(-1px)';
      }}
      onMouseLeave={(e) => {
        e.currentTarget.style.borderColor = '#e5e5ea';
        e.currentTarget.style.transform = 'translateY(0)';
      }}
    >
      <Flex align="center" justify="space-between" style={{ marginBottom: 16 }}>
        <Text style={{ fontSize: 13, fontWeight: 500, color: '#86868b' }}>
          {label}
        </Text>
        <div
          style={{
            width: 36,
            height: 36,
            borderRadius: 10,
            background: color,
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            fontSize: 16,
            color: '#fff',
          }}
        >
          {icon}
        </div>
      </Flex>
      <div style={{ fontSize: 32, fontWeight: 700, color: '#1d1d1f', lineHeight: 1 }}>
        {value}
      </div>
    </div>
  );
}

const actionLabels: Record<string, string> = {
  Created: 'created',
  Updated: 'updated',
  Deleted: 'deleted',
};

export default function DashboardPage() {
  const { user } = useAuth();
  const navigate = useNavigate();

  const { data, isLoading, isError } = useQuery({
    queryKey: ['dashboardStats'],
    queryFn: dashboardApi.getStats,
  });

  return (
    <div>
      <div style={{ marginBottom: 28 }}>
        <h2 style={{ fontSize: 22, fontWeight: 700, color: '#1d1d1f', margin: 0 }}>
          Dashboard
        </h2>
        <Text style={{ fontSize: 14, color: '#86868b' }}>
          Welcome back, {user?.firstName}. Here's an overview of your system.
        </Text>
      </div>

      {isError && (
        <Alert
          message="Some data could not be loaded."
          type="warning"
          showIcon
          style={{ marginBottom: 20, borderRadius: 8 }}
        />
      )}

      <Spin spinning={isLoading}>
        <Row gutter={[20, 20]}>
          <Col xs={24} sm={12} lg={6}>
            <StatCard label="Users" value={data?.userCount ?? 0} icon={<UserOutlined />} color="#0071e3" onClick={() => navigate('/users')} />
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <StatCard label="Roles" value={data?.roleCount ?? 0} icon={<TeamOutlined />} color="#34c759" onClick={() => navigate('/roles')} />
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <StatCard label="Departments" value={data?.departmentCount ?? 0} icon={<ApartmentOutlined />} color="#ff9500" onClick={() => navigate('/departments')} />
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <StatCard label="Audit Events" value={data?.auditLogCount ?? 0} icon={<FileSearchOutlined />} color="#af52de" onClick={() => navigate('/audit-logs')} />
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <StatCard label="Files" value={data?.fileCount ?? 0} icon={<FolderOutlined />} color="#0071e3" onClick={() => navigate('/files')} />
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <StatCard label="Reports" value={data?.reportCount ?? 0} icon={<BarChartOutlined />} color="#ff3b30" onClick={() => navigate('/reports')} />
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <StatCard label="Active Tasks" value={data?.activeTaskCount ?? 0} icon={<ExperimentOutlined />} color="#ff9500" onClick={() => navigate('/demo-tasks')} />
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <StatCard label="Feature Flags On" value={data?.enabledFeatureFlagCount ?? 0} icon={<FlagOutlined />} color="#34c759" onClick={() => navigate('/feature-flags')} />
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <StatCard label="Active API Keys" value={data?.activeApiKeyCount ?? 0} icon={<ApiOutlined />} color="#af52de" onClick={() => navigate('/api-integration')} />
          </Col>
        </Row>

        {/* Recent Activity */}
        {(data?.recentActivity ?? []).length > 0 && (
          <div style={{ marginTop: 28 }}>
            <h3 style={{ fontSize: 16, fontWeight: 600, color: '#1d1d1f', marginBottom: 16 }}>Recent Activity</h3>
            <div style={{ background: '#ffffff', borderRadius: 12, border: '1px solid #e5e5ea', overflow: 'hidden' }}>
              {(data?.recentActivity ?? []).map((activity: RecentActivity, index: number) => (
                <div
                  key={index}
                  style={{
                    padding: '14px 20px',
                    borderBottom: index < (data?.recentActivity ?? []).length - 1 ? '1px solid #f2f2f7' : 'none',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'space-between',
                  }}
                >
                  <div>
                    <span style={{ fontSize: 13, color: '#1d1d1f', fontWeight: 500 }}>
                      {activity.entityName}
                    </span>
                    <span style={{ fontSize: 13, color: '#86868b', marginLeft: 6 }}>
                      {actionLabels[activity.action] || activity.action}
                    </span>
                    <code style={{ fontSize: 11, background: '#f5f5f7', padding: '1px 6px', borderRadius: 4, marginLeft: 8, color: '#6e6e73' }}>
                      {activity.entityId.length > 12 ? activity.entityId.substring(0, 8) + '...' : activity.entityId}
                    </code>
                  </div>
                  <Text style={{ fontSize: 12, color: '#86868b' }}>
                    {dayjs(activity.timestamp).format('MMM D, h:mm A')}
                  </Text>
                </div>
              ))}
            </div>
          </div>
        )}
      </Spin>
    </div>
  );
}
