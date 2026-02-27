import { useState, useMemo } from 'react';
import {
  Typography,
  Button,
  Table,
  Tag,
  Space,
  Modal,
  Form,
  Input,
  Checkbox,
  message,
  Spin,
} from 'antd';
import {
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  ExclamationCircleOutlined,
} from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AxiosError } from 'axios';
import { rolesApi } from '../api/rolesApi';
import type { CreateRoleRequest, UpdateRoleRequest } from '../api/rolesApi';
import { permissionsApi } from '../api/permissionsApi';
import type { Role } from '../types';
import PermissionGate from '../components/PermissionGate';

const { Title } = Typography;

interface RoleFormValues {
  name: string;
  description: string;
  permissionIds: string[];
}

export default function RolesPage() {
  const queryClient = useQueryClient();
  const [modalOpen, setModalOpen] = useState(false);
  const [editingRole, setEditingRole] = useState<Role | null>(null);
  const [form] = Form.useForm<RoleFormValues>();

  const rolesQuery = useQuery({
    queryKey: ['roles'],
    queryFn: () => rolesApi.getRoles(),
  });

  const permissionsQuery = useQuery({
    queryKey: ['permissions'],
    queryFn: () => permissionsApi.getPermissions(),
  });

  // Group permissions by module
  const permissionGroups = useMemo(() => {
    if (!permissionsQuery.data) return {};
    const groups: Record<string, string[]> = {};
    for (const perm of permissionsQuery.data) {
      const dotIndex = perm.indexOf('.');
      const module = dotIndex > -1 ? perm.substring(0, dotIndex) : perm;
      if (!groups[module]) groups[module] = [];
      groups[module].push(perm);
    }
    return groups;
  }, [permissionsQuery.data]);

  const createMutation = useMutation({
    mutationFn: (data: CreateRoleRequest) => rolesApi.createRole(data),
    onSuccess: () => {
      message.success('Role created successfully');
      queryClient.invalidateQueries({ queryKey: ['roles'] });
      closeModal();
    },
    onError: (error: AxiosError<{ message?: string; title?: string }>) => {
      const msg =
        error.response?.data?.message ||
        error.response?.data?.title ||
        'Failed to create role';
      message.error(msg);
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateRoleRequest }) =>
      rolesApi.updateRole(id, data),
    onSuccess: () => {
      message.success('Role updated successfully');
      queryClient.invalidateQueries({ queryKey: ['roles'] });
      closeModal();
    },
    onError: (error: AxiosError<{ message?: string; title?: string }>) => {
      const msg =
        error.response?.data?.message ||
        error.response?.data?.title ||
        'Failed to update role';
      message.error(msg);
    },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => rolesApi.deleteRole(id),
    onSuccess: () => {
      message.success('Role deleted successfully');
      queryClient.invalidateQueries({ queryKey: ['roles'] });
    },
    onError: () => {
      message.error('Failed to delete role');
    },
  });

  const openCreateModal = () => {
    setEditingRole(null);
    form.resetFields();
    setModalOpen(true);
  };

  const openEditModal = (role: Role) => {
    setEditingRole(role);
    form.setFieldsValue({
      name: role.name,
      description: role.description,
      permissionIds: role.permissions,
    });
    setModalOpen(true);
  };

  const closeModal = () => {
    setModalOpen(false);
    setEditingRole(null);
    form.resetFields();
  };

  const handleModalOk = async () => {
    try {
      const values = await form.validateFields();
      if (editingRole) {
        updateMutation.mutate({
          id: editingRole.id,
          data: {
            name: values.name,
            description: values.description,
            permissionIds: values.permissionIds ?? [],
          },
        });
      } else {
        createMutation.mutate({
          name: values.name,
          description: values.description,
          permissionIds: values.permissionIds ?? [],
        });
      }
    } catch {
      // form validation failed
    }
  };

  const handleDelete = (role: Role) => {
    if (role.isSystemRole) {
      message.warning('System roles cannot be deleted');
      return;
    }
    Modal.confirm({
      title: 'Delete Role',
      icon: <ExclamationCircleOutlined />,
      content: `Are you sure you want to delete role "${role.name}"?`,
      okText: 'Delete',
      okType: 'danger',
      onOk: () => deleteMutation.mutateAsync(role.id),
    });
  };

  const columns: ColumnsType<Role> = [
    {
      title: 'Name',
      dataIndex: 'name',
      key: 'name',
      sorter: (a, b) => a.name.localeCompare(b.name),
    },
    {
      title: 'Description',
      dataIndex: 'description',
      key: 'description',
      ellipsis: true,
    },
    {
      title: 'System Role',
      dataIndex: 'isSystemRole',
      key: 'isSystemRole',
      render: (value: boolean) =>
        value ? <Tag color="gold">System</Tag> : <Tag>Custom</Tag>,
    },
    {
      title: 'Permission Count',
      dataIndex: 'permissionCount',
      key: 'permissionCount',
      sorter: (a, b) => a.permissionCount - b.permissionCount,
    },
    {
      title: 'Actions',
      key: 'actions',
      render: (_, record) => (
        <Space>
          <PermissionGate permission="Role.Update">
            <Button
              type="link"
              icon={<EditOutlined />}
              onClick={() => openEditModal(record)}
            >
              Edit
            </Button>
          </PermissionGate>
          <PermissionGate permission="Role.Delete">
            <Button
              type="link"
              danger
              icon={<DeleteOutlined />}
              disabled={record.isSystemRole}
              onClick={() => handleDelete(record)}
            >
              Delete
            </Button>
          </PermissionGate>
        </Space>
      ),
    },
  ];

  const isMutating = createMutation.isPending || updateMutation.isPending;

  return (
    <div>
      <div
        style={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          marginBottom: 16,
        }}
      >
        <Title level={3} style={{ margin: 0 }}>
          Roles
        </Title>
        <PermissionGate permission="Role.Create">
          <Button
            type="primary"
            icon={<PlusOutlined />}
            onClick={openCreateModal}
          >
            New Role
          </Button>
        </PermissionGate>
      </div>

      <Table<Role>
        rowKey="id"
        columns={columns}
        dataSource={rolesQuery.data ?? []}
        loading={rolesQuery.isLoading}
        pagination={{ showSizeChanger: true }}
      />

      <Modal
        title={editingRole ? 'Edit Role' : 'New Role'}
        open={modalOpen}
        onOk={handleModalOk}
        onCancel={closeModal}
        confirmLoading={isMutating}
        width={700}
        destroyOnClose
      >
        <Form<RoleFormValues>
          form={form}
          layout="vertical"
          initialValues={{ permissionIds: [] }}
        >
          <Form.Item
            name="name"
            label="Name"
            rules={[{ required: true, message: 'Role name is required' }]}
          >
            <Input placeholder="Role name" />
          </Form.Item>

          <Form.Item
            name="description"
            label="Description"
            rules={[{ required: true, message: 'Description is required' }]}
          >
            <Input.TextArea rows={2} placeholder="Role description" />
          </Form.Item>

          <Form.Item name="permissionIds" label="Permissions">
            {permissionsQuery.isLoading ? (
              <Spin />
            ) : (
              <Checkbox.Group style={{ width: '100%' }}>
                {Object.entries(permissionGroups).map(([module, perms]) => (
                  <div key={module} style={{ marginBottom: 12 }}>
                    <Typography.Text strong style={{ display: 'block', marginBottom: 4 }}>
                      {module}
                    </Typography.Text>
                    <div style={{ display: 'flex', flexWrap: 'wrap', gap: 8 }}>
                      {perms.map((perm) => (
                        <Checkbox key={perm} value={perm}>
                          {perm.substring(perm.indexOf('.') + 1)}
                        </Checkbox>
                      ))}
                    </div>
                  </div>
                ))}
              </Checkbox.Group>
            )}
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}
