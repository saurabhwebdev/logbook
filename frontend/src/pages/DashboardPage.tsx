import { Row, Col, Typography, Spin, Alert, Flex } from 'antd';
import {
  UserOutlined,
  TeamOutlined,
  ApartmentOutlined,
  FileSearchOutlined,
} from '@ant-design/icons';
import { useQuery } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { usersApi } from '../api/usersApi';
import { rolesApi } from '../api/rolesApi';
import { departmentsApi } from '../api/departmentsApi';
import { auditLogsApi } from '../api/auditLogsApi';

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

export default function DashboardPage() {
  const { user } = useAuth();
  const navigate = useNavigate();

  const usersQuery = useQuery({
    queryKey: ['users', 'count'],
    queryFn: () => usersApi.getUsers({ pageNumber: 1, pageSize: 1 }),
  });

  const rolesQuery = useQuery({
    queryKey: ['roles'],
    queryFn: () => rolesApi.getRoles(),
  });

  const departmentsQuery = useQuery({
    queryKey: ['departments'],
    queryFn: () => departmentsApi.getDepartments(),
  });

  const auditLogsQuery = useQuery({
    queryKey: ['auditLogs', 'count'],
    queryFn: () => auditLogsApi.getAuditLogs({ pageNumber: 1, pageSize: 1 }),
  });

  const isLoading =
    usersQuery.isLoading ||
    rolesQuery.isLoading ||
    departmentsQuery.isLoading ||
    auditLogsQuery.isLoading;

  const hasError =
    usersQuery.isError ||
    rolesQuery.isError ||
    departmentsQuery.isError ||
    auditLogsQuery.isError;

  return (
    <div>
      {/* Page header */}
      <div style={{ marginBottom: 28 }}>
        <h2 style={{ fontSize: 22, fontWeight: 700, color: '#1d1d1f', margin: 0 }}>
          Dashboard
        </h2>
        <Text style={{ fontSize: 14, color: '#86868b' }}>
          Welcome back, {user?.firstName}. Here's an overview of your system.
        </Text>
      </div>

      {hasError && (
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
            <StatCard
              label="Total Users"
              value={usersQuery.data?.totalCount ?? 0}
              icon={<UserOutlined />}
              color="#0071e3"
              onClick={() => navigate('/users')}
            />
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <StatCard
              label="Roles"
              value={rolesQuery.data?.length ?? 0}
              icon={<TeamOutlined />}
              color="#34c759"
              onClick={() => navigate('/roles')}
            />
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <StatCard
              label="Departments"
              value={departmentsQuery.data?.length ?? 0}
              icon={<ApartmentOutlined />}
              color="#ff9500"
              onClick={() => navigate('/departments')}
            />
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <StatCard
              label="Audit Events"
              value={auditLogsQuery.data?.totalCount ?? 0}
              icon={<FileSearchOutlined />}
              color="#af52de"
              onClick={() => navigate('/audit-logs')}
            />
          </Col>
        </Row>
      </Spin>
    </div>
  );
}
