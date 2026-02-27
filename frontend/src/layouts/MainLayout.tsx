import { useState, useMemo } from 'react';
import {
  Layout,
  Menu,
  Dropdown,
  Avatar,
  Typography,
  Tag,
  Flex,
  theme,
} from 'antd';
import {
  DashboardOutlined,
  UserOutlined,
  TeamOutlined,
  ApartmentOutlined,
  FileSearchOutlined,
  BankOutlined,
  LogoutOutlined,
  MenuFoldOutlined,
  MenuUnfoldOutlined,
  ProfileOutlined,
} from '@ant-design/icons';
import { Outlet, useNavigate, useLocation } from 'react-router-dom';
import type { MenuProps } from 'antd';
import { useAuth } from '../contexts/AuthContext';

const { Header, Sider, Content } = Layout;
const { Text } = Typography;

export default function MainLayout() {
  const [collapsed, setCollapsed] = useState(false);
  const { user, logout, hasPermission } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const {
    token: { colorBgContainer, borderRadiusLG },
  } = theme.useToken();

  const menuItems = useMemo(() => {
    const items: MenuProps['items'] = [
      {
        key: '/',
        icon: <DashboardOutlined />,
        label: 'Dashboard',
      },
    ];

    if (hasPermission('User.Read')) {
      items.push({
        key: '/users',
        icon: <UserOutlined />,
        label: 'Users',
      });
    }

    if (hasPermission('Role.Read')) {
      items.push({
        key: '/roles',
        icon: <TeamOutlined />,
        label: 'Roles',
      });
    }

    if (hasPermission('Department.Read')) {
      items.push({
        key: '/departments',
        icon: <ApartmentOutlined />,
        label: 'Departments',
      });
    }

    if (hasPermission('AuditLog.Read')) {
      items.push({
        key: '/audit-logs',
        icon: <FileSearchOutlined />,
        label: 'Audit Logs',
      });
    }

    if (hasPermission('Tenant.Read')) {
      items.push({
        key: '/tenants',
        icon: <BankOutlined />,
        label: 'Tenants',
      });
    }

    return items;
  }, [hasPermission]);

  // Determine the active menu key from the current path
  const selectedKey = useMemo(() => {
    const path = location.pathname;
    if (path === '/') return '/';
    // Match the first segment, e.g. /users/new -> /users
    const match = path.match(/^\/[^/]+/);
    return match ? match[0] : '/';
  }, [location.pathname]);

  const userMenuItems: MenuProps['items'] = [
    {
      key: 'profile',
      icon: <ProfileOutlined />,
      label: 'Profile',
      disabled: true,
    },
    { type: 'divider' },
    {
      key: 'logout',
      icon: <LogoutOutlined />,
      label: 'Logout',
      danger: true,
    },
  ];

  const handleMenuClick: MenuProps['onClick'] = ({ key }) => {
    navigate(key);
  };

  const handleUserMenuClick: MenuProps['onClick'] = ({ key }) => {
    if (key === 'logout') {
      logout();
    }
  };

  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Sider
        collapsible
        collapsed={collapsed}
        onCollapse={setCollapsed}
        theme="dark"
        style={{
          overflow: 'auto',
          height: '100vh',
          position: 'fixed',
          left: 0,
          top: 0,
          bottom: 0,
        }}
      >
        <Flex
          align="center"
          justify="center"
          style={{
            height: 64,
            color: '#ffffff',
            fontSize: collapsed ? 14 : 18,
            fontWeight: 700,
            letterSpacing: 1,
            whiteSpace: 'nowrap',
            overflow: 'hidden',
          }}
        >
          {collapsed ? 'CE' : 'CoreEngine'}
        </Flex>
        <Menu
          theme="dark"
          mode="inline"
          selectedKeys={[selectedKey]}
          items={menuItems}
          onClick={handleMenuClick}
        />
      </Sider>

      <Layout style={{ marginLeft: collapsed ? 80 : 200, transition: 'margin-left 0.2s' }}>
        <Header
          style={{
            padding: '0 24px',
            background: colorBgContainer,
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
            boxShadow: '0 1px 4px rgba(0, 0, 0, 0.08)',
            position: 'sticky',
            top: 0,
            zIndex: 10,
          }}
        >
          <Flex align="center" gap={16}>
            <span
              onClick={() => setCollapsed(!collapsed)}
              style={{ fontSize: 18, cursor: 'pointer', lineHeight: 1 }}
            >
              {collapsed ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />}
            </span>
          </Flex>

          <Flex align="center" gap={16}>
            {user?.tenantName && (
              <Tag color="blue">{user.tenantName}</Tag>
            )}
            <Dropdown
              menu={{ items: userMenuItems, onClick: handleUserMenuClick }}
              placement="bottomRight"
              trigger={['click']}
            >
              <Flex
                align="center"
                gap={8}
                style={{ cursor: 'pointer' }}
              >
                <Avatar size="small" icon={<UserOutlined />} />
                <Text style={{ maxWidth: 150, overflow: 'hidden', textOverflow: 'ellipsis' }}>
                  {user?.firstName} {user?.lastName}
                </Text>
              </Flex>
            </Dropdown>
          </Flex>
        </Header>

        <Content
          style={{
            margin: 24,
            padding: 24,
            background: colorBgContainer,
            borderRadius: borderRadiusLG,
            overflow: 'auto',
            minHeight: 'calc(100vh - 112px)',
          }}
        >
          <Outlet />
        </Content>
      </Layout>
    </Layout>
  );
}
