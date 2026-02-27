import { Navigate, Outlet } from 'react-router-dom';
import { Spin, Result, Button, Flex } from 'antd';
import { useAuth } from '../contexts/AuthContext';

interface ProtectedRouteProps {
  permission?: string;
}

export default function ProtectedRoute({ permission }: ProtectedRouteProps) {
  const { isAuthenticated, loading, hasPermission } = useAuth();

  if (loading) {
    return (
      <Flex
        align="center"
        justify="center"
        style={{ height: '100vh', background: '#f8f9fa' }}
      >
        <Spin size="large" />
      </Flex>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (permission && !hasPermission(permission)) {
    return (
      <Flex align="center" justify="center" style={{ height: '100vh', background: '#f8f9fa' }}>
        <Result
          status="403"
          title="Access denied"
          subTitle="You don't have permission to view this page."
          extra={
            <Button type="primary" onClick={() => window.history.back()}>
              Go back
            </Button>
          }
        />
      </Flex>
    );
  }

  return <Outlet />;
}
