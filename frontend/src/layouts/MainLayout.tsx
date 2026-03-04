import { useState, useMemo, useCallback } from 'react';
import {
  Layout,
  Menu,
  Dropdown,
  Typography,
  Flex,
  ConfigProvider,
  Breadcrumb,
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
  SettingOutlined,
  FlagOutlined,
  BellOutlined,
  NodeIndexOutlined,
  FolderOutlined,
  BarChartOutlined,
  ApiOutlined,
  ExperimentOutlined,
  FormatPainterOutlined,
  QuestionCircleOutlined,
  ClockCircleOutlined,
  MailOutlined,
  SendOutlined,
  ScheduleOutlined,
  CheckSquareOutlined,
  ControlOutlined,
  GoldOutlined,
  SwapOutlined,
  BookOutlined,
} from '@ant-design/icons';
import { Outlet, useNavigate, useLocation } from 'react-router-dom';
import type { MenuProps } from 'antd';
import { useAuth } from '../contexts/AuthContext';
import { useTenantTheme } from '../contexts/ThemeContext';
import HelpDrawer from '../components/HelpDrawer';
import UserAvatar from '../components/UserAvatar';
import GlobalSearch from '../components/GlobalSearch';

const { Sider, Content } = Layout;
const { Text } = Typography;

const SIDEBAR_WIDTH = 240;
const SIDEBAR_COLLAPSED_WIDTH = 72;

// Map routes to module keys for contextual help
const ROUTE_MODULE_MAP: Record<string, string> = {
  '/': 'Dashboard',
  '/users': 'Users',
  '/roles': 'Roles',
  '/departments': 'Departments',
  '/audit-logs': 'AuditLogs',
  '/activity': 'Activity',
  '/tenants': 'Tenants',
  '/settings': 'Settings',
  '/feature-flags': 'FeatureFlags',
  '/notifications': 'Notifications',
  '/state-machine': 'StateMachine',
  '/files': 'Files',
  '/reports': 'Reports',
  '/api-integration': 'ApiIntegration',
  '/demo-tasks': 'DemoTasks',
  '/theming': 'Theming',
  '/help': 'Help',
  '/email-templates': 'EmailTemplates',
  '/email-queue': 'EmailQueue',
  '/background-jobs': 'BackgroundJobs',
  '/my-tasks': 'MyTasks',
  '/workflows': 'Workflows',
  '/workflow-definitions': 'WorkflowDefinitions',
  // Logbook Mining Modules
  '/mine-sites': 'MineSites',
  '/shift-management': 'ShiftManagement',
  '/statutory-registers': 'StatutoryRegisters',
};

export default function MainLayout() {
  const [collapsed, setCollapsed] = useState(false);
  const [helpOpen, setHelpOpen] = useState(false);
  const { user, logout, hasPermission } = useAuth();

  const { theme: tenantTheme } = useTenantTheme();
  const sidebarBg = tenantTheme?.sidebarColor || '#ffffff';
  const sidebarTextColor = tenantTheme?.sidebarTextColor || '#6e6e73';
  const primaryColor = tenantTheme?.primaryColor || '#0071e3';
  const tenantLogo = tenantTheme?.logoUrl;
  const navigate = useNavigate();
  const location = useLocation();

  const currentModuleKey = useMemo(() => {
    const path = location.pathname;
    if (path === '/') return 'Dashboard';
    const match = path.match(/^\/[^/]+/);
    return match ? (ROUTE_MODULE_MAP[match[0]] || 'Dashboard') : 'Dashboard';
  }, [location.pathname]);

  const openHelp = useCallback(() => setHelpOpen(true), []);

  const menuItems = useMemo(() => {
    const items: MenuProps['items'] = [
      {
        key: '/',
        icon: <DashboardOutlined />,
        label: 'Dashboard',
      },
    ];

    // ===== Logbook Mining Modules (primary business modules) =====
    if (hasPermission('MineSite.Read')) {
      items.push({
        key: '/mine-sites',
        icon: <GoldOutlined />,
        label: 'Mine Sites',
      });
    }

    if (hasPermission('ShiftDefinition.Read')) {
      items.push({
        key: '/shift-management',
        icon: <SwapOutlined />,
        label: 'Shift Management',
      });
    }

    if (hasPermission('StatutoryRegister.Read')) {
      items.push({
        key: '/statutory-registers',
        icon: <BookOutlined />,
        label: 'Statutory Registers',
      });
    }

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
      items.push({
        key: '/activity',
        icon: <ClockCircleOutlined />,
        label: 'My Activity',
      });
    }

    if (hasPermission('Tenant.Read')) {
      items.push({
        key: '/tenants',
        icon: <BankOutlined />,
        label: 'Tenants',
      });
    }

    // Phase 2 navigation
    if (hasPermission('Configuration.Read')) {
      items.push({
        key: '/settings',
        icon: <SettingOutlined />,
        label: 'Settings',
      });
    }

    if (hasPermission('FeatureFlag.Read')) {
      items.push({
        key: '/feature-flags',
        icon: <FlagOutlined />,
        label: 'Feature Flags',
      });
    }

    items.push({
      key: '/notifications',
      icon: <BellOutlined />,
      label: 'Notifications',
    });

    // Workflow Engine
    if (hasPermission('WorkflowTask.View')) {
      items.push({
        key: '/my-tasks',
        icon: <CheckSquareOutlined />,
        label: 'My Tasks',
      });
    }

    if (hasPermission('Workflow.View')) {
      items.push({
        key: '/workflows',
        icon: <ApartmentOutlined />,
        label: 'Workflows',
      });
    }

    if (hasPermission('WorkflowDefinition.Read')) {
      items.push({
        key: '/workflow-definitions',
        icon: <ControlOutlined />,
        label: 'Workflow Definitions',
      });
    }

    if (hasPermission('StateMachine.Read')) {
      items.push({
        key: '/state-machine',
        icon: <NodeIndexOutlined />,
        label: 'State Machine',
      });
    }

    // Phase 3 navigation
    if (hasPermission('File.Read')) {
      items.push({
        key: '/files',
        icon: <FolderOutlined />,
        label: 'Files',
      });
    }

    if (hasPermission('Report.Read')) {
      items.push({
        key: '/reports',
        icon: <BarChartOutlined />,
        label: 'Reports',
      });
    }

    if (hasPermission('ApiIntegration.Read')) {
      items.push({
        key: '/api-integration',
        icon: <ApiOutlined />,
        label: 'API Integration',
      });
    }

    if (hasPermission('DemoTask.Read')) {
      items.push({
        key: '/demo-tasks',
        icon: <ExperimentOutlined />,
        label: 'Tasks (Demo)',
      });
    }

    // Phase 4 navigation
    if (hasPermission('Tenant.Update')) {
      items.push({
        key: '/theming',
        icon: <FormatPainterOutlined />,
        label: 'Theming',
      });
    }

    // Email Module
    if (hasPermission('EmailTemplate.Read')) {
      items.push({
        key: '/email-templates',
        icon: <MailOutlined />,
        label: 'Email Templates',
      });
    }

    if (hasPermission('Email.Read')) {
      items.push({
        key: '/email-queue',
        icon: <SendOutlined />,
        label: 'Email Queue',
      });
    }

    if (hasPermission('BackgroundJob.Read')) {
      items.push({
        key: '/background-jobs',
        icon: <ScheduleOutlined />,
        label: 'Background Jobs',
      });
    }

    // Help Module
    if (hasPermission('Help.Read')) {
      items.push({
        key: '/help',
        icon: <QuestionCircleOutlined />,
        label: 'Help Center',
      });
    }

    return items;
  }, [hasPermission]);

  const selectedKey = useMemo(() => {
    const path = location.pathname;
    if (path === '/') return '/';
    const match = path.match(/^\/[^/]+/);
    return match ? match[0] : '/';
  }, [location.pathname]);

  const breadcrumbItems = useMemo(() => {
    const path = location.pathname;
    if (path === '/') return [{ title: 'Dashboard' }];

    const segments = path.split('/').filter(Boolean);
    const items: Array<{ title: string; href?: string }> = [
      { title: 'Home', href: '/' }
    ];

    // Map route segments to readable names and paths
    const routeNames: Record<string, string> = {
      users: 'Users',
      roles: 'Roles',
      departments: 'Departments',
      'audit-logs': 'Audit Logs',
      activity: 'My Activity',
      tenants: 'Tenants',
      settings: 'Settings',
      'feature-flags': 'Feature Flags',
      notifications: 'Notifications',
      'state-machine': 'State Machine',
      files: 'Files',
      reports: 'Reports',
      'api-integration': 'API Integration',
      'demo-tasks': 'Tasks',
      theming: 'Theming',
      help: 'Help Center',
      new: 'New',
      edit: 'Edit',
      // Logbook Mining Modules
      'mine-sites': 'Mine Sites',
      'shift-management': 'Shift Management',
      'statutory-registers': 'Statutory Registers',
    };

    let currentPath = '';
    segments.forEach((segment, index) => {
      // Skip IDs (UUIDs or numeric IDs)
      if (/^[0-9a-f-]{36}$/.test(segment) || /^\d+$/.test(segment)) return;

      currentPath += `/${segment}`;
      const name = routeNames[segment] || segment;

      // Only make it clickable if it's not the last item
      const isLast = index === segments.length - 1 ||
                     (index < segments.length - 1 && /^[0-9a-f-]{36}$/.test(segments[index + 1]));

      items.push({
        title: name,
        href: isLast ? undefined : currentPath
      });
    });

    return items;
  }, [location.pathname]);

  const userMenuItems: MenuProps['items'] = [
    {
      key: 'info',
      label: (
        <div style={{ padding: '4px 0' }}>
          <div style={{ fontWeight: 600, fontSize: 13, color: '#1d1d1f' }}>
            {user?.firstName} {user?.lastName}
          </div>
          <div style={{ fontSize: 12, color: '#86868b' }}>{user?.email}</div>
        </div>
      ),
      disabled: true,
    },
    { type: 'divider' },
    {
      key: 'profile',
      icon: <UserOutlined />,
      label: 'My Profile',
    },
    {
      key: 'logout',
      icon: <LogoutOutlined />,
      label: 'Sign out',
      danger: true,
    },
  ];

  const handleMenuClick: MenuProps['onClick'] = ({ key }) => {
    navigate(key);
  };

  const handleUserMenuClick: MenuProps['onClick'] = ({ key }) => {
    if (key === 'logout') {
      logout();
    } else if (key === 'profile') {
      navigate('/profile');
    }
  };

  const siderWidth = collapsed ? SIDEBAR_COLLAPSED_WIDTH : SIDEBAR_WIDTH;

  return (
    <Layout style={{ minHeight: '100vh', background: '#f8f9fa' }}>
      {/* Sidebar */}
      <style>
        {`
          .sidebar-scroll::-webkit-scrollbar {
            width: 6px;
          }
          .sidebar-scroll::-webkit-scrollbar-track {
            background: transparent;
          }
          .sidebar-scroll::-webkit-scrollbar-thumb {
            background: #e5e5ea;
            border-radius: 3px;
          }
          .sidebar-scroll::-webkit-scrollbar-thumb:hover {
            background: #d1d1d6;
          }
        `}
      </style>
      <Sider
        collapsible
        collapsed={collapsed}
        onCollapse={setCollapsed}
        width={SIDEBAR_WIDTH}
        collapsedWidth={SIDEBAR_COLLAPSED_WIDTH}
        trigger={null}
        className="sidebar-scroll"
        style={{
          overflow: 'auto',
          height: '100vh',
          position: 'fixed',
          left: 0,
          top: 0,
          bottom: 0,
          background: sidebarBg,
          boxShadow: '2px 0 6px rgba(0, 0, 0, 0.04)',
          zIndex: 20,
        }}
      >
        {/* Logo */}
        <Flex
          align="center"
          gap={10}
          style={{
            height: 56,
            padding: collapsed ? '0 20px' : '0 20px',
            borderBottom: '1px solid #f2f2f7',
            cursor: 'pointer',
            overflow: 'hidden',
            whiteSpace: 'nowrap',
          }}
          onClick={() => navigate('/')}
        >
          {tenantLogo ? (
            <img src={tenantLogo} alt="Logo" style={{ width: 32, height: 32, minWidth: 32, borderRadius: 8, objectFit: 'contain' }} />
          ) : (
            <div
              style={{
                width: 32,
                height: 32,
                minWidth: 32,
                borderRadius: 8,
                background: primaryColor,
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                color: '#fff',
                fontWeight: 700,
                fontSize: 14,
                letterSpacing: -0.5,
              }}
            >
              CE
            </div>
          )}
          {!collapsed && (
            <Text style={{ fontWeight: 700, fontSize: 16, color: sidebarTextColor, letterSpacing: -0.3 }}>
              {tenantTheme?.tenantName || 'CoreEngine'}
            </Text>
          )}
        </Flex>

        {/* Navigation */}
        <div style={{ padding: '12px 0', flex: 1 }}>
          <ConfigProvider theme={{ components: { Menu: { itemColor: sidebarTextColor, itemBg: 'transparent', subMenuItemBg: 'transparent' } } }}>
            <Menu
              mode="inline"
              selectedKeys={[selectedKey]}
              items={menuItems}
              onClick={handleMenuClick}
              style={{ border: 'none', background: 'transparent' }}
            />
          </ConfigProvider>
        </div>

        {/* Sidebar footer — collapse toggle */}
        <Flex
          align="center"
          justify={collapsed ? 'center' : 'flex-start'}
          gap={10}
          style={{
            height: 48,
            padding: collapsed ? '0' : '0 20px',
            borderTop: '1px solid #f2f2f7',
            cursor: 'pointer',
            color: sidebarTextColor,
            transition: 'all 0.2s',
          }}
          onClick={() => setCollapsed(!collapsed)}
        >
          {collapsed ? (
            <MenuUnfoldOutlined style={{ fontSize: 16 }} />
          ) : (
            <>
              <MenuFoldOutlined style={{ fontSize: 16 }} />
              <Text style={{ fontSize: 13, color: sidebarTextColor, fontWeight: 500 }}>
                Collapse
              </Text>
            </>
          )}
        </Flex>
      </Sider>

      {/* Main content area */}
      <Layout
        style={{
          marginLeft: siderWidth,
          transition: 'margin-left 0.2s',
          background: '#f8f9fa',
          minHeight: '100vh',
        }}
      >
        {/* Top bar */}
        <Flex
          align="center"
          justify="space-between"
          style={{
            height: 56,
            padding: '0 28px',
            background: '#ffffff',
            boxShadow: '0 1px 3px rgba(0, 0, 0, 0.03)',
            position: 'sticky',
            top: 0,
            zIndex: 10,
          }}
        >
          {/* Left side — breadcrumb */}
          <ConfigProvider
            theme={{
              components: {
                Breadcrumb: {
                  linkColor: primaryColor,
                  linkHoverColor: primaryColor,
                  itemColor: '#1d1d1f',
                  separatorColor: '#c7c7cc',
                },
              },
            }}
          >
            <Breadcrumb
              items={breadcrumbItems.map(item => ({
                ...item,
                onClick: item.href ? () => navigate(item.href!) : undefined,
              }))}
              style={{ fontSize: 13, cursor: 'pointer' }}
            />
          </ConfigProvider>

          {/* Right side — search + help + user dropdown */}
          <Flex align="center" gap={16}>
          <GlobalSearch />
          <QuestionCircleOutlined
            onClick={openHelp}
            style={{ fontSize: 18, color: '#86868b', cursor: 'pointer', transition: 'color 0.2s' }}
            onMouseEnter={(e) => (e.currentTarget.style.color = primaryColor)}
            onMouseLeave={(e) => (e.currentTarget.style.color = '#86868b')}
          />
          <Dropdown
            menu={{ items: userMenuItems, onClick: handleUserMenuClick }}
            placement="bottomRight"
            trigger={['click']}
          >
            <Flex align="center" gap={10} style={{ cursor: 'pointer' }}>
              <Text style={{ fontSize: 13, fontWeight: 500, color: '#1d1d1f' }}>
                {user?.firstName} {user?.lastName}
              </Text>
              <UserAvatar
                firstName={user?.firstName}
                lastName={user?.lastName}
                profilePhotoUrl={user?.profilePhotoUrl}
                size={32}
              />
            </Flex>
          </Dropdown>
          </Flex>
        </Flex>

        {/* Page content */}
        <Content
          style={{
            padding: 28,
            minHeight: 'calc(100vh - 56px)',
            position: 'relative',
          }}
        >
          {/* Animated background glow */}
          <style>
            {`
              @keyframes float1 {
                0%, 100% { transform: translate(-50%, -50%) scale(1); }
                50% { transform: translate(-45%, -55%) scale(1.1); }
              }
              @keyframes float2 {
                0%, 100% { transform: translate(0, 0) scale(1); }
                50% { transform: translate(-5%, 5%) scale(1.15); }
              }
              .glow-orb-1 {
                animation: float1 15s ease-in-out infinite;
              }
              .glow-orb-2 {
                animation: float2 18s ease-in-out infinite;
              }
            `}
          </style>
          <div
            className="glow-orb-1"
            style={{
              position: 'fixed',
              top: '20%',
              left: '50%',
              width: '900px',
              height: '900px',
              transform: 'translate(-50%, -50%)',
              background: `radial-gradient(circle, ${primaryColor}30 0%, ${primaryColor}18 30%, transparent 70%)`,
              filter: 'blur(70px)',
              pointerEvents: 'none',
              zIndex: 0,
            }}
          />
          <div
            className="glow-orb-2"
            style={{
              position: 'fixed',
              bottom: '10%',
              right: '10%',
              width: '700px',
              height: '700px',
              background: `radial-gradient(circle, ${primaryColor}25 0%, ${primaryColor}12 40%, transparent 70%)`,
              filter: 'blur(80px)',
              pointerEvents: 'none',
              zIndex: 0,
            }}
          />
          <div style={{ position: 'relative', zIndex: 1 }}>
            <Outlet />
          </div>
        </Content>
      </Layout>

      <HelpDrawer open={helpOpen} onClose={() => setHelpOpen(false)} moduleKey={currentModuleKey} />
    </Layout>
  );
}
