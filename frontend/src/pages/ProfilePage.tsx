import { Typography, Card, Descriptions, Tag, Avatar, Flex, Divider } from 'antd';
import { UserOutlined, MailOutlined, BankOutlined, TeamOutlined } from '@ant-design/icons';
import { useAuth } from '../contexts/AuthContext';
import { useTenantTheme } from '../contexts/ThemeContext';

const { Text } = Typography;

export default function ProfilePage() {
  const { user } = useAuth();
  const { theme } = useTenantTheme();
  const primaryColor = theme?.primaryColor || '#0071e3';

  if (!user) {
    return null;
  }

  return (
    <div>
      {/* Page header */}
      <Flex align="center" justify="space-between" style={{ marginBottom: 24 }}>
        <div>
          <h2 style={{ fontSize: 22, fontWeight: 700, color: '#1d1d1f', margin: 0 }}>
            My Profile
          </h2>
          <Text style={{ fontSize: 13, color: '#86868b' }}>
            View your account information and assigned roles.
          </Text>
        </div>
      </Flex>

      {/* Profile card */}
      <Card
        style={{
          background: '#ffffff',
          borderRadius: 12,
          border: '1px solid #e5e5ea',
          marginBottom: 16,
        }}
      >
        <Flex align="center" gap={20} style={{ marginBottom: 24 }}>
          <Avatar
            size={80}
            style={{
              background: primaryColor,
              fontSize: 28,
              fontWeight: 600,
            }}
          >
            {user.firstName[0]}{user.lastName[0]}
          </Avatar>
          <div>
            <h3 style={{ fontSize: 20, fontWeight: 600, color: '#1d1d1f', margin: 0, marginBottom: 4 }}>
              {user.firstName} {user.lastName}
            </h3>
            <Flex align="center" gap={6} style={{ color: '#86868b', fontSize: 13 }}>
              <MailOutlined />
              <Text style={{ color: '#86868b', fontSize: 13 }}>{user.email}</Text>
            </Flex>
          </div>
        </Flex>

        <Divider style={{ margin: '20px 0' }} />

        <Descriptions column={1} labelStyle={{ fontWeight: 600, color: '#1d1d1f', width: 160 }}>
          <Descriptions.Item
            label={
              <Flex align="center" gap={8}>
                <UserOutlined />
                <span>User ID</span>
              </Flex>
            }
          >
            <Text style={{ fontFamily: 'monospace', fontSize: 12, color: '#86868b' }}>
              {user.id}
            </Text>
          </Descriptions.Item>

          <Descriptions.Item
            label={
              <Flex align="center" gap={8}>
                <BankOutlined />
                <span>Tenant</span>
              </Flex>
            }
          >
            <Text style={{ fontSize: 13, color: '#1d1d1f' }}>
              {user.tenantName}
            </Text>
          </Descriptions.Item>

          <Descriptions.Item
            label={
              <Flex align="center" gap={8}>
                <TeamOutlined />
                <span>Roles</span>
              </Flex>
            }
          >
            <Flex gap={6} wrap>
              {user.roles.map((role) => (
                <Tag
                  key={role}
                  style={{
                    background: `${primaryColor}15`,
                    color: primaryColor,
                    border: 'none',
                    fontWeight: 500,
                    padding: '4px 12px',
                    borderRadius: 6,
                  }}
                >
                  {role}
                </Tag>
              ))}
            </Flex>
          </Descriptions.Item>
        </Descriptions>
      </Card>

      {/* Permissions card */}
      <Card
        title={
          <Text style={{ fontSize: 15, fontWeight: 600, color: '#1d1d1f' }}>
            Assigned Permissions
          </Text>
        }
        style={{
          background: '#ffffff',
          borderRadius: 12,
          border: '1px solid #e5e5ea',
        }}
      >
        <Flex gap={8} wrap>
          {user.permissions.length > 0 ? (
            user.permissions.map((permission) => (
              <Tag
                key={permission}
                style={{
                  fontSize: 12,
                  color: '#6e6e73',
                  background: '#f5f5f7',
                  border: 'none',
                  padding: '4px 10px',
                  borderRadius: 6,
                }}
              >
                {permission}
              </Tag>
            ))
          ) : (
            <Text style={{ color: '#86868b', fontSize: 13 }}>No permissions assigned</Text>
          )}
        </Flex>
      </Card>
    </div>
  );
}
