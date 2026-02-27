import { useEffect } from 'react';
import {
  Typography,
  Form,
  Input,
  Select,
  Button,
  Spin,
  message,
  Flex,
} from 'antd';
import { ArrowLeftOutlined } from '@ant-design/icons';
import { useNavigate, useParams } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AxiosError } from 'axios';
import { usersApi } from '../api/usersApi';
import { rolesApi } from '../api/rolesApi';
import { departmentsApi } from '../api/departmentsApi';
import type { CreateUserRequest, UpdateUserRequest } from '../types';

const { Text } = Typography;

interface UserFormValues {
  email: string;
  password?: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  departmentId?: string;
  status?: string;
  roleIds: string[];
}

export default function UserFormPage() {
  const { id } = useParams<{ id: string }>();
  const isEditing = Boolean(id);
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [form] = Form.useForm<UserFormValues>();

  const userQuery = useQuery({
    queryKey: ['users', id],
    queryFn: () => usersApi.getUserById(id!),
    enabled: isEditing,
  });

  const rolesQuery = useQuery({
    queryKey: ['roles'],
    queryFn: () => rolesApi.getRoles(),
  });

  const departmentsQuery = useQuery({
    queryKey: ['departments'],
    queryFn: () => departmentsApi.getDepartments(),
  });

  useEffect(() => {
    if (userQuery.data) {
      const user = userQuery.data;
      const roleIds =
        rolesQuery.data
          ?.filter((r) => user.roles.includes(r.name))
          .map((r) => r.id) ?? [];

      form.setFieldsValue({
        email: user.email,
        firstName: user.firstName,
        lastName: user.lastName,
        phoneNumber: user.phoneNumber ?? undefined,
        departmentId: user.departmentId ?? undefined,
        status: user.status,
        roleIds,
      });
    }
  }, [userQuery.data, rolesQuery.data, form]);

  const createMutation = useMutation({
    mutationFn: (data: CreateUserRequest) => usersApi.createUser(data),
    onSuccess: () => {
      message.success('User created');
      queryClient.invalidateQueries({ queryKey: ['users'] });
      navigate('/users');
    },
    onError: (error: AxiosError<{ message?: string; title?: string }>) => {
      const msg =
        error.response?.data?.message ||
        error.response?.data?.title ||
        'Failed to create user';
      message.error(msg);
    },
  });

  const updateMutation = useMutation({
    mutationFn: (data: UpdateUserRequest) => usersApi.updateUser(id!, data),
    onSuccess: () => {
      message.success('User updated');
      queryClient.invalidateQueries({ queryKey: ['users'] });
      navigate('/users');
    },
    onError: (error: AxiosError<{ message?: string; title?: string }>) => {
      const msg =
        error.response?.data?.message ||
        error.response?.data?.title ||
        'Failed to update user';
      message.error(msg);
    },
  });

  const onFinish = (values: UserFormValues) => {
    if (isEditing) {
      updateMutation.mutate({
        firstName: values.firstName,
        lastName: values.lastName,
        phoneNumber: values.phoneNumber,
        departmentId: values.departmentId,
        status: values.status!,
        roleIds: values.roleIds,
      });
    } else {
      createMutation.mutate({
        email: values.email,
        password: values.password!,
        firstName: values.firstName,
        lastName: values.lastName,
        phoneNumber: values.phoneNumber,
        departmentId: values.departmentId,
        roleIds: values.roleIds,
      });
    }
  };

  const isSubmitting = createMutation.isPending || updateMutation.isPending;

  if (isEditing && userQuery.isLoading) {
    return (
      <Flex align="center" justify="center" style={{ padding: 80 }}>
        <Spin size="large" />
      </Flex>
    );
  }

  return (
    <div>
      {/* Page header */}
      <Flex align="center" gap={12} style={{ marginBottom: 28 }}>
        <Button
          type="text"
          icon={<ArrowLeftOutlined />}
          onClick={() => navigate('/users')}
          style={{ color: '#86868b' }}
        />
        <div>
          <h2 style={{ fontSize: 22, fontWeight: 700, color: '#1d1d1f', margin: 0 }}>
            {isEditing ? 'Edit user' : 'New user'}
          </h2>
          <Text style={{ fontSize: 13, color: '#86868b' }}>
            {isEditing ? 'Update user details and role assignments.' : 'Create a new user account.'}
          </Text>
        </div>
      </Flex>

      {/* Form card */}
      <div
        style={{
          background: '#ffffff',
          borderRadius: 12,
          border: '1px solid #e5e5ea',
          padding: '28px 32px',
          maxWidth: 560,
        }}
      >
        <Form<UserFormValues>
          form={form}
          layout="vertical"
          onFinish={onFinish}
          initialValues={{ roleIds: [] }}
          requiredMark={false}
        >
          <Form.Item
            name="email"
            label={<Text style={{ fontWeight: 500, fontSize: 13 }}>Email</Text>}
            rules={[
              { required: true, message: 'Email is required' },
              { type: 'email', message: 'Enter a valid email' },
            ]}
          >
            <Input disabled={isEditing} placeholder="user@example.com" />
          </Form.Item>

          {!isEditing && (
            <Form.Item
              name="password"
              label={<Text style={{ fontWeight: 500, fontSize: 13 }}>Password</Text>}
              rules={[
                { required: true, message: 'Password is required' },
                { min: 8, message: 'Minimum 8 characters' },
              ]}
            >
              <Input.Password placeholder="Minimum 8 characters" />
            </Form.Item>
          )}

          <Flex gap={16}>
            <Form.Item
              name="firstName"
              label={<Text style={{ fontWeight: 500, fontSize: 13 }}>First name</Text>}
              rules={[{ required: true, message: 'Required' }]}
              style={{ flex: 1 }}
            >
              <Input placeholder="First name" />
            </Form.Item>

            <Form.Item
              name="lastName"
              label={<Text style={{ fontWeight: 500, fontSize: 13 }}>Last name</Text>}
              rules={[{ required: true, message: 'Required' }]}
              style={{ flex: 1 }}
            >
              <Input placeholder="Last name" />
            </Form.Item>
          </Flex>

          <Form.Item
            name="phoneNumber"
            label={<Text style={{ fontWeight: 500, fontSize: 13 }}>Phone</Text>}
          >
            <Input placeholder="Optional" />
          </Form.Item>

          <Form.Item
            name="departmentId"
            label={<Text style={{ fontWeight: 500, fontSize: 13 }}>Department</Text>}
          >
            <Select
              allowClear
              placeholder="Select department"
              loading={departmentsQuery.isLoading}
              options={departmentsQuery.data?.map((dept) => ({
                label: dept.name,
                value: dept.id,
              }))}
            />
          </Form.Item>

          {isEditing && (
            <Form.Item
              name="status"
              label={<Text style={{ fontWeight: 500, fontSize: 13 }}>Status</Text>}
              rules={[{ required: true, message: 'Required' }]}
            >
              <Select
                placeholder="Select status"
                options={[
                  { label: 'Active', value: 'Active' },
                  { label: 'Inactive', value: 'Inactive' },
                  { label: 'Locked', value: 'Locked' },
                ]}
              />
            </Form.Item>
          )}

          <Form.Item
            name="roleIds"
            label={<Text style={{ fontWeight: 500, fontSize: 13 }}>Roles</Text>}
            rules={[{ required: true, message: 'Select at least one role' }]}
          >
            <Select
              mode="multiple"
              placeholder="Select roles"
              loading={rolesQuery.isLoading}
              options={rolesQuery.data?.map((role) => ({
                label: role.name,
                value: role.id,
              }))}
            />
          </Form.Item>

          <Flex gap={12} style={{ marginTop: 8 }}>
            <Button type="primary" htmlType="submit" loading={isSubmitting}>
              {isEditing ? 'Save changes' : 'Create user'}
            </Button>
            <Button onClick={() => navigate('/users')}>Cancel</Button>
          </Flex>
        </Form>
      </div>
    </div>
  );
}
