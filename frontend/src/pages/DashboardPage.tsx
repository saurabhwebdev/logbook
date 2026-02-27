import { Row, Col, Card, Statistic, Typography, Spin, Alert } from 'antd';
import {
  UserOutlined,
  TeamOutlined,
  ApartmentOutlined,
  FileSearchOutlined,
} from '@ant-design/icons';
import { useQuery } from '@tanstack/react-query';
import { useAuth } from '../contexts/AuthContext';
import { usersApi } from '../api/usersApi';
import { rolesApi } from '../api/rolesApi';
import { departmentsApi } from '../api/departmentsApi';
import { auditLogsApi } from '../api/auditLogsApi';

const { Title, Text } = Typography;

export default function DashboardPage() {
  const { user } = useAuth();

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
    queryFn: () =>
      auditLogsApi.getAuditLogs({ pageNumber: 1, pageSize: 1 }),
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
      <Title level={3}>Dashboard</Title>
      <Text type="secondary" style={{ fontSize: 16, display: 'block', marginBottom: 24 }}>
        Welcome back, {user?.firstName}!
      </Text>

      {hasError && (
        <Alert
          message="Some dashboard data could not be loaded."
          type="warning"
          showIcon
          style={{ marginBottom: 16 }}
        />
      )}

      <Spin spinning={isLoading}>
        <Row gutter={16}>
          <Col span={6}>
            <Card>
              <Statistic
                title="Total Users"
                value={usersQuery.data?.totalCount ?? 0}
                prefix={<UserOutlined />}
              />
            </Card>
          </Col>
          <Col span={6}>
            <Card>
              <Statistic
                title="Total Roles"
                value={rolesQuery.data?.length ?? 0}
                prefix={<TeamOutlined />}
              />
            </Card>
          </Col>
          <Col span={6}>
            <Card>
              <Statistic
                title="Total Departments"
                value={departmentsQuery.data?.length ?? 0}
                prefix={<ApartmentOutlined />}
              />
            </Card>
          </Col>
          <Col span={6}>
            <Card>
              <Statistic
                title="Recent Audit Actions"
                value={auditLogsQuery.data?.totalCount ?? 0}
                prefix={<FileSearchOutlined />}
              />
            </Card>
          </Col>
        </Row>
      </Spin>
    </div>
  );
}
