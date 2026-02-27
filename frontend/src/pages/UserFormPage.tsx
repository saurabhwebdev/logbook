import { useEffect } from 'react';
import {
  Typography,
  Form,
  Input,
  Select,
  Button,
  Space,
  Spin,
  message,
} from 'antd';
import { useNavigate, useParams } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AxiosError } from 'axios';
import { usersApi } from '../api/usersApi';
import { rolesApi } from '../api/rolesApi';
import { departmentsApi } from '../api/departmentsApi';
import type { CreateUserRequest, UpdateUserRequest } from '../types';

const { Title } = Typography;

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

  // Load user data when editing
  const userQuery = useQuery({
    queryKey: ['users', id],
    queryFn: () => usersApi.getUserById(id!),
    enabled: isEditing,
  });

  // Load roles for the select
  const rolesQuery = useQuery({
    queryKey: ['roles'],
    queryFn: () => rolesApi.getRoles(),
  });

  // Load departments for the select
  const departmentsQuery = useQuery({
    queryKey: ['departments'],
    queryFn: () => departmentsApi.getDepartments(),
  });

  // Populate form when user data loads
  useEffect(() => {
    if (userQuery.data) {
      const user = userQuery.data;
      // We need to map role names to role IDs
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
      message.success('User created successfully');
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
      message.success('User updated successfully');
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
      const updateData: UpdateUserRequest = {
        firstName: values.firstName,
        lastName: values.lastName,
        phoneNumber: values.phoneNumber,
        departmentId: values.departmentId,
        status: values.status!,
        roleIds: values.roleIds,
      };
      updateMutation.mutate(updateData);
    } else {
      const createData: CreateUserRequest = {
        email: values.email,
        password: values.password!,
        firstName: values.firstName,
        lastName: values.lastName,
        phoneNumber: values.phoneNumber,
        departmentId: values.departmentId,
        roleIds: values.roleIds,
      };
      createMutation.mutate(createData);
    }
  };

  const isSubmitting = createMutation.isPending || updateMutation.isPending;

  if (isEditing && userQuery.isLoading) {
    return (
      <div style={{ textAlign: 'center', padding: 48 }}>
        <Spin size="large" />
      </div>
    );
  }

  return (
    <div>
      <Title level={3}>{isEditing ? 'Edit User' : 'New User'}</Title>

      <Form<UserFormValues>
        form={form}
        layout="vertical"
        onFinish={onFinish}
        style={{ maxWidth: 600 }}
        initialValues={{ roleIds: [] }}
      >
        <Form.Item
          name="email"
          label="Email"
          rules={[
            { required: true, message: 'Email is required' },
            { type: 'email', message: 'Please enter a valid email' },
          ]}
        >
          <Input disabled={isEditing} placeholder="user@example.com" />
        </Form.Item>

        {!isEditing && (
          <Form.Item
            name="password"
            label="Password"
            rules={[
              { required: true, message: 'Password is required' },
              { min: 8, message: 'Password must be at least 8 characters' },
            ]}
          >
            <Input.Password placeholder="Enter password" />
          </Form.Item>
        )}

        <Form.Item
          name="firstName"
          label="First Name"
          rules={[{ required: true, message: 'First name is required' }]}
        >
          <Input placeholder="First name" />
        </Form.Item>

        <Form.Item
          name="lastName"
          label="Last Name"
          rules={[{ required: true, message: 'Last name is required' }]}
        >
          <Input placeholder="Last name" />
        </Form.Item>

        <Form.Item name="phoneNumber" label="Phone Number">
          <Input placeholder="Phone number (optional)" />
        </Form.Item>

        <Form.Item name="departmentId" label="Department">
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
            label="Status"
            rules={[{ required: true, message: 'Status is required' }]}
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
          label="Roles"
          rules={[{ required: true, message: 'At least one role is required' }]}
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

        <Form.Item>
          <Space>
            <Button type="primary" htmlType="submit" loading={isSubmitting}>
              {isEditing ? 'Update' : 'Create'}
            </Button>
            <Button onClick={() => navigate('/users')}>Cancel</Button>
          </Space>
        </Form.Item>
      </Form>
    </div>
  );
}
