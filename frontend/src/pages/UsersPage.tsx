import { useState, useCallback } from 'react';
import {
  Typography,
  Button,
  Table,
  Tag,
  Space,
  Input,
  Modal,
  message,
  Flex,
  Avatar,
} from 'antd';
import {
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  ExclamationCircleOutlined,
  SearchOutlined,
} from '@ant-design/icons';
import type { ColumnsType, TablePaginationConfig } from 'antd/es/table';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import dayjs from 'dayjs';
import { usersApi } from '../api/usersApi';
import { useTenantTheme } from '../contexts/ThemeContext';
import type { User } from '../types';
import PermissionGate from '../components/PermissionGate';

const { Text } = Typography;

export default function UsersPage() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const { theme } = useTenantTheme();
  const primaryColor = theme?.primaryColor || '#0071e3';

  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [searchTerm, setSearchTerm] = useState('');
  const [searchDebounce, setSearchDebounce] = useState('');

  const { data, isLoading } = useQuery({
    queryKey: ['users', page, pageSize, searchDebounce],
    queryFn: () =>
      usersApi.getUsers({
        pageNumber: page,
        pageSize,
        searchTerm: searchDebounce || undefined,
      }),
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => usersApi.deleteUser(id),
    onSuccess: () => {
      message.success('User deleted');
      queryClient.invalidateQueries({ queryKey: ['users'] });
    },
    onError: () => {
      message.error('Failed to delete user');
    },
  });

  const handleSearch = useCallback((value: string) => {
    setSearchDebounce(value);
    setPage(1);
  }, []);

  const handleDelete = (user: User) => {
    Modal.confirm({
      title: 'Delete user',
      icon: <ExclamationCircleOutlined />,
      content: `This will permanently delete "${user.firstName} ${user.lastName}". This action cannot be undone.`,
      okText: 'Delete',
      okType: 'danger',
      onOk: () => deleteMutation.mutateAsync(user.id),
    });
  };

  const handleTableChange = (pagination: TablePaginationConfig) => {
    setPage(pagination.current ?? 1);
    setPageSize(pagination.pageSize ?? 10);
  };

  const statusConfig: Record<string, { color: string; bg: string }> = {
    Active: { color: '#34c759', bg: '#f0fdf4' },
    Locked: { color: '#ff3b30', bg: '#fef2f2' },
    Inactive: { color: '#86868b', bg: '#f5f5f7' },
  };

  const columns: ColumnsType<User> = [
    {
      title: 'User',
      key: 'name',
      render: (_, record) => (
        <Flex align="center" gap={12}>
          <Avatar
            size={34}
            style={{
              background: primaryColor,
              fontSize: 13,
              fontWeight: 600,
            }}
          >
            {record.firstName[0]}{record.lastName[0]}
          </Avatar>
          <div>
            <div style={{ fontWeight: 500, color: '#1d1d1f', fontSize: 13 }}>
              {record.firstName} {record.lastName}
            </div>
            <div style={{ fontSize: 12, color: '#86868b' }}>{record.email}</div>
          </div>
        </Flex>
      ),
      sorter: (a, b) => a.firstName.localeCompare(b.firstName),
    },
    {
      title: 'Status',
      dataIndex: 'status',
      key: 'status',
      width: 100,
      render: (status: string) => {
        const cfg = statusConfig[status] ?? statusConfig.Inactive;
        return (
          <span
            style={{
              display: 'inline-flex',
              alignItems: 'center',
              gap: 6,
              fontSize: 12,
              fontWeight: 500,
              color: cfg.color,
            }}
          >
            <span
              style={{
                width: 6,
                height: 6,
                borderRadius: '50%',
                background: cfg.color,
              }}
            />
            {status}
          </span>
        );
      },
    },
    {
      title: 'Department',
      dataIndex: 'departmentName',
      key: 'departmentName',
      render: (value: string | null) => (
        <Text style={{ fontSize: 13, color: value ? '#1d1d1f' : '#c7c7cc' }}>
          {value ?? '--'}
        </Text>
      ),
    },
    {
      title: 'Roles',
      dataIndex: 'roles',
      key: 'roles',
      render: (roles: string[]) =>
        roles.map((role) => (
          <Tag
            key={role}
            style={{
              background: '#f0f5ff',
              color: primaryColor,
              border: 'none',
              fontWeight: 500,
            }}
          >
            {role}
          </Tag>
        )),
    },
    {
      title: 'Created',
      dataIndex: 'createdAt',
      key: 'createdAt',
      width: 120,
      render: (value: string) => (
        <Text style={{ fontSize: 13, color: '#86868b' }}>
          {dayjs(value).format('MMM D, YYYY')}
        </Text>
      ),
      sorter: (a, b) => dayjs(a.createdAt).unix() - dayjs(b.createdAt).unix(),
    },
    {
      title: '',
      key: 'actions',
      width: 100,
      align: 'right',
      render: (_, record) => (
        <Space size={0}>
          <PermissionGate permission="User.Update">
            <Button
              type="text"
              size="small"
              icon={<EditOutlined />}
              onClick={() => navigate(`/users/${record.id}/edit`)}
              style={{ color: '#86868b' }}
            />
          </PermissionGate>
          <PermissionGate permission="User.Delete">
            <Button
              type="text"
              size="small"
              danger
              icon={<DeleteOutlined />}
              onClick={() => handleDelete(record)}
            />
          </PermissionGate>
        </Space>
      ),
    },
  ];

  return (
    <div>
      {/* Page header */}
      <Flex align="center" justify="space-between" style={{ marginBottom: 24 }}>
        <div>
          <h2 style={{ fontSize: 22, fontWeight: 700, color: '#1d1d1f', margin: 0 }}>
            Users
          </h2>
          <Text style={{ fontSize: 13, color: '#86868b' }}>
            Manage user accounts and role assignments.
          </Text>
        </div>
        <PermissionGate permission="User.Create">
          <Button
            type="primary"
            icon={<PlusOutlined />}
            onClick={() => navigate('/users/new')}
          >
            Add user
          </Button>
        </PermissionGate>
      </Flex>

      {/* Filters */}
      <div
        style={{
          background: '#ffffff',
          borderRadius: 12,
          border: '1px solid #e5e5ea',
          padding: '16px 20px',
          marginBottom: 16,
        }}
      >
        <Input
          placeholder="Search by name or email..."
          prefix={<SearchOutlined style={{ color: '#c7c7cc' }} />}
          allowClear
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          onPressEnter={() => handleSearch(searchTerm)}
          onClear={() => handleSearch('')}
          style={{ maxWidth: 360, border: 'none', boxShadow: 'none' }}
        />
      </div>

      {/* Table */}
      <div
        style={{
          background: '#ffffff',
          borderRadius: 12,
          border: '1px solid #e5e5ea',
          overflow: 'hidden',
        }}
      >
        <Table<User>
          rowKey="id"
          columns={columns}
          dataSource={data?.items ?? []}
          loading={isLoading}
          onChange={handleTableChange}
          pagination={{
            current: page,
            pageSize,
            total: data?.totalCount ?? 0,
            showSizeChanger: true,
            showTotal: (total) => (
              <Text style={{ fontSize: 13, color: '#86868b' }}>{total} users</Text>
            ),
            style: { padding: '0 16px' },
          }}
        />
      </div>
    </div>
  );
}
