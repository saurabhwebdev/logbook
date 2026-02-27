import { useState } from 'react';
import { Typography, Card, Descriptions, Tag, Avatar, Flex, Divider, Button, Form, Input, message, Modal, Upload, Space } from 'antd';
import { UserOutlined, MailOutlined, BankOutlined, TeamOutlined, LockOutlined, KeyOutlined, CameraOutlined, DeleteOutlined, LoadingOutlined, ExclamationCircleOutlined } from '@ant-design/icons';
import { useMutation } from '@tanstack/react-query';
import { useAuth } from '../contexts/AuthContext';
import { useTenantTheme } from '../contexts/ThemeContext';
import { authApi } from '../api/authApi';
import { usersApi } from '../api/usersApi';

const { Text } = Typography;

export default function ProfilePage() {
  const { user, refreshUser } = useAuth();
  const { theme } = useTenantTheme();
  const primaryColor = theme?.primaryColor || '#0071e3';
  const [isPasswordModalOpen, setIsPasswordModalOpen] = useState(false);
  const [uploading, setUploading] = useState(false);
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

  const uploadPhotoMutation = useMutation({
    mutationFn: (file: File) => usersApi.uploadProfilePhoto(file),
    onSuccess: (photoUrl) => {
      message.success('Profile photo updated successfully');
      setUploading(false);
      // Update local storage user data with new photo URL
      const storedUser = localStorage.getItem('user');
      if (storedUser && user) {
        const updatedUser = { ...user, profilePhotoUrl: photoUrl };
        localStorage.setItem('user', JSON.stringify(updatedUser));
        refreshUser();
      }
    },
    onError: (error: any) => {
      message.error(error.response?.data?.detail || 'Failed to upload photo');
      setUploading(false);
    },
  });

  const deletePhotoMutation = useMutation({
    mutationFn: () => usersApi.deleteProfilePhoto(),
    onSuccess: () => {
      message.success('Profile photo removed successfully');
      // Update local storage user data to remove photo URL
      const storedUser = localStorage.getItem('user');
      if (storedUser && user) {
        const updatedUser = { ...user, profilePhotoUrl: null };
        localStorage.setItem('user', JSON.stringify(updatedUser));
        refreshUser();
      }
    },
    onError: (error: any) => {
      message.error(error.response?.data?.detail || 'Failed to remove photo');
    },
  });

  const handlePasswordChange = (values: any) => {
    changePasswordMutation.mutate({
      currentPassword: values.currentPassword,
      newPassword: values.newPassword,
    });
  };

  const beforeUpload = (file: File) => {
    // Validate file type
    const isImage = file.type.startsWith('image/');
    if (!isImage) {
      message.error('You can only upload image files!');
      return false;
    }

    // Validate file size (5MB max)
    const isLt5M = file.size / 1024 / 1024 < 5;
    if (!isLt5M) {
      message.error('Image must be smaller than 5MB!');
      return false;
    }

    return true;
  };

  const handleUpload = async (options: any) => {
    const { file } = options;
    setUploading(true);
    uploadPhotoMutation.mutate(file);
  };

  const handleDeletePhoto = () => {
    Modal.confirm({
      title: 'Remove profile photo',
      icon: <ExclamationCircleOutlined />,
      content: 'Are you sure you want to remove your profile photo?',
      okText: 'Remove',
      okType: 'danger',
      onOk: () => deletePhotoMutation.mutateAsync(),
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
        <Flex align="flex-start" gap={24} style={{ marginBottom: 24 }}>
          <div style={{ position: 'relative' }}>
            {user.profilePhotoUrl ? (
              <Avatar
                size={100}
                src={user.profilePhotoUrl}
                style={{
                  border: '3px solid #f5f5f7',
                }}
              />
            ) : (
              <Avatar
                size={100}
                style={{
                  background: primaryColor,
                  fontSize: 36,
                  fontWeight: 600,
                }}
              >
                {user.firstName[0]}{user.lastName[0]}
              </Avatar>
            )}
            {uploading && (
              <div
                style={{
                  position: 'absolute',
                  top: 0,
                  left: 0,
                  width: 100,
                  height: 100,
                  borderRadius: '50%',
                  background: 'rgba(0, 0, 0, 0.5)',
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                }}
              >
                <LoadingOutlined style={{ fontSize: 24, color: '#fff' }} />
              </div>
            )}
          </div>
          <div style={{ flex: 1 }}>
            <h3 style={{ fontSize: 20, fontWeight: 600, color: '#1d1d1f', margin: 0, marginBottom: 4 }}>
              {user.firstName} {user.lastName}
            </h3>
            <Flex align="center" gap={6} style={{ color: '#86868b', fontSize: 13, marginBottom: 16 }}>
              <MailOutlined />
              <Text style={{ color: '#86868b', fontSize: 13 }}>{user.email}</Text>
            </Flex>
            <Space size={8}>
              <Upload
                accept="image/*"
                showUploadList={false}
                beforeUpload={beforeUpload}
                customRequest={handleUpload}
                disabled={uploading || uploadPhotoMutation.isPending}
              >
                <Button
                  icon={<CameraOutlined />}
                  size="small"
                  loading={uploading || uploadPhotoMutation.isPending}
                >
                  {user.profilePhotoUrl ? 'Change Photo' : 'Upload Photo'}
                </Button>
              </Upload>
              {user.profilePhotoUrl && (
                <Button
                  icon={<DeleteOutlined />}
                  size="small"
                  danger
                  onClick={handleDeletePhoto}
                  loading={deletePhotoMutation.isPending}
                >
                  Remove Photo
                </Button>
              )}
            </Space>
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
