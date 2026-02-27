import { useState } from 'react';
import { Typography, Card, Descriptions, Tag, Avatar, Flex, Divider, Button, Form, Input, message, Modal } from 'antd';
import { UserOutlined, MailOutlined, BankOutlined, TeamOutlined, LockOutlined, KeyOutlined } from '@ant-design/icons';
import { useMutation } from '@tanstack/react-query';
import { useAuth } from '../contexts/AuthContext';
import { useTenantTheme } from '../contexts/ThemeContext';
import { authApi } from '../api/authApi';

const { Text } = Typography;

export default function ProfilePage() {
  const { user } = useAuth();
  const { theme } = useTenantTheme();
  const primaryColor = theme?.primaryColor || '#0071e3';
  const [isPasswordModalOpen, setIsPasswordModalOpen] = useState(false);
  const [form] = Form.useForm();

  const changePasswordMutation = useMutation({
    mutationFn: ({ currentPassword, newPassword }: { currentPassword: string; newPassword: string }) =>
      authApi.changePassword(currentPassword, newPassword),
    onSuccess: () => {
      message.success('Password changed successfully');
      setIsPasswordModalOpen(false);
      form.resetFields();
    },
    onError: (error: any) => {
      message.error(error.response?.data?.detail || 'Failed to change password');
    },
  });

  const handlePasswordChange = (values: any) => {
    changePasswordMutation.mutate({
      currentPassword: values.currentPassword,
      newPassword: values.newPassword,
    });
  };

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
        <Button icon={<LockOutlined />} onClick={() => setIsPasswordModalOpen(true)}>
          Change Password
        </Button>
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

      {/* Change Password Modal */}
      <Modal
        title={
          <Flex align="center" gap={8}>
            <KeyOutlined style={{ color: primaryColor }} />
            <span>Change Password</span>
          </Flex>
        }
        open={isPasswordModalOpen}
        onCancel={() => {
          setIsPasswordModalOpen(false);
          form.resetFields();
        }}
        footer={null}
        width={480}
      >
        <Form form={form} layout="vertical" onFinish={handlePasswordChange} style={{ marginTop: 20 }}>
          <Form.Item
            name="currentPassword"
            label="Current Password"
            rules={[{ required: true, message: 'Please enter your current password' }]}
          >
            <Input.Password size="large" prefix={<LockOutlined />} placeholder="Enter current password" />
          </Form.Item>

          <Form.Item
            name="newPassword"
            label="New Password"
            rules={[
              { required: true, message: 'Please enter a new password' },
              { min: 8, message: 'Password must be at least 8 characters' },
              {
                pattern: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$/,
                message: 'Password must contain uppercase, lowercase, number, and special character',
              },
            ]}
          >
            <Input.Password size="large" prefix={<KeyOutlined />} placeholder="Enter new password" />
          </Form.Item>

          <Form.Item
            name="confirmPassword"
            label="Confirm New Password"
            dependencies={['newPassword']}
            rules={[
              { required: true, message: 'Please confirm your new password' },
              ({ getFieldValue }) => ({
                validator(_, value) {
                  if (!value || getFieldValue('newPassword') === value) {
                    return Promise.resolve();
                  }
                  return Promise.reject(new Error('Passwords do not match'));
                },
              }),
            ]}
          >
            <Input.Password size="large" prefix={<KeyOutlined />} placeholder="Confirm new password" />
          </Form.Item>

          <Form.Item style={{ marginBottom: 0, marginTop: 24 }}>
            <Flex gap={8} justify="flex-end">
              <Button onClick={() => { setIsPasswordModalOpen(false); form.resetFields(); }}>
                Cancel
              </Button>
              <Button type="primary" htmlType="submit" loading={changePasswordMutation.isPending}>
                Change Password
              </Button>
            </Flex>
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}
