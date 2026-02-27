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
  Flex,
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

const { Text } = Typography;

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
      message.success('Role created');
      queryClient.invalidateQueries({ queryKey: ['roles'] });
      closeModal();
    },
    onError: (error: AxiosError<{ message?: string; title?: string }>) => {
      message.error(error.response?.data?.message || error.response?.data?.title || 'Failed to create role');
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateRoleRequest }) =>
      rolesApi.updateRole(id, data),
    onSuccess: () => {
      message.success('Role updated');
      queryClient.invalidateQueries({ queryKey: ['roles'] });
      closeModal();
    },
    onError: (error: AxiosError<{ message?: string; title?: string }>) => {
      message.error(error.response?.data?.message || error.response?.data?.title || 'Failed to update role');
    },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => rolesApi.deleteRole(id),
    onSuccess: () => {
      message.success('Role deleted');
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
          data: { name: values.name, description: values.description, permissionIds: values.permissionIds ?? [] },
        });
      } else {
        createMutation.mutate({
          name: values.name,
          description: values.description,
          permissionIds: values.permissionIds ?? [],
        });
      }
    } catch {
      // validation
    }
  };

  const handleDelete = (role: Role) => {
    if (role.isSystemRole) {
      message.warning('System roles cannot be deleted');
      return;
    }
    Modal.confirm({
      title: 'Delete role',
      icon: <ExclamationCircleOutlined />,
      content: `This will permanently delete "${role.name}". This action cannot be undone.`,
      okText: 'Delete',
      okType: 'danger',
      onOk: () => deleteMutation.mutateAsync(role.id),
    });
  };

  const columns: ColumnsType<Role> = [
    {
      title: 'Role',
      key: 'name',
      render: (_, record) => (
        <div>
          <div style={{ fontWeight: 500, color: '#1d1d1f', fontSize: 13 }}>
            {record.name}
          </div>
          <div style={{ fontSize: 12, color: '#86868b' }}>{record.description}</div>
        </div>
      ),
      sorter: (a, b) => a.name.localeCompare(b.name),
    },
    {
      title: 'Type',
      dataIndex: 'isSystemRole',
      key: 'isSystemRole',
      width: 100,
      render: (value: boolean) =>
        value ? (
          <Tag style={{ background: '#fff7ed', color: '#f59e0b', border: 'none', fontWeight: 500 }}>System</Tag>
        ) : (
          <Tag style={{ background: '#f5f5f7', color: '#86868b', border: 'none', fontWeight: 500 }}>Custom</Tag>
        ),
    },
    {
      title: 'Permissions',
      dataIndex: 'permissionCount',
      key: 'permissionCount',
      width: 120,
      render: (count: number) => (
        <Text style={{ fontSize: 13, color: '#6e6e73' }}>{count}</Text>
      ),
      sorter: (a, b) => a.permissionCount - b.permissionCount,
    },
    {
      title: '',
      key: 'actions',
      width: 100,
      align: 'right',
      render: (_, record) => (
        <Space size={0}>
          <PermissionGate permission="Role.Update">
            <Button
              type="text"
              size="small"
              icon={<EditOutlined />}
              onClick={() => openEditModal(record)}
              style={{ color: '#86868b' }}
            />
          </PermissionGate>
          <PermissionGate permission="Role.Delete">
            <Button
              type="text"
              size="small"
              danger
              icon={<DeleteOutlined />}
              disabled={record.isSystemRole}
              onClick={() => handleDelete(record)}
            />
          </PermissionGate>
        </Space>
      ),
    },
  ];

  const isMutating = createMutation.isPending || updateMutation.isPending;

  return (
    <div>
      <Flex align="center" justify="space-between" style={{ marginBottom: 24 }}>
        <div>
          <h2 style={{ fontSize: 22, fontWeight: 700, color: '#1d1d1f', margin: 0 }}>Roles</h2>
          <Text style={{ fontSize: 13, color: '#86868b' }}>Define roles and assign permissions.</Text>
        </div>
        <PermissionGate permission="Role.Create">
          <Button type="primary" icon={<PlusOutlined />} onClick={openCreateModal}>
            Add role
          </Button>
        </PermissionGate>
      </Flex>

      <div
        style={{
          background: '#ffffff',
          borderRadius: 12,
          border: '1px solid #e5e5ea',
          overflow: 'hidden',
        }}
      >
        <Table<Role>
          rowKey="id"
          columns={columns}
          dataSource={rolesQuery.data ?? []}
          loading={rolesQuery.isLoading}
          pagination={{ showSizeChanger: true, style: { padding: '0 16px' } }}
        />
      </div>

      <Modal
        title={editingRole ? 'Edit role' : 'New role'}
        open={modalOpen}
        onOk={handleModalOk}
        onCancel={closeModal}
        confirmLoading={isMutating}
        width={640}
        destroyOnClose
        okText={editingRole ? 'Save changes' : 'Create'}
      >
        <Form<RoleFormValues>
          form={form}
          layout="vertical"
          initialValues={{ permissionIds: [] }}
          requiredMark={false}
        >
          <Form.Item
            name="name"
            label={<Text style={{ fontWeight: 500, fontSize: 13 }}>Name</Text>}
            rules={[{ required: true, message: 'Required' }]}
          >
            <Input placeholder="e.g. Editor, Viewer" />
          </Form.Item>

          <Form.Item
            name="description"
            label={<Text style={{ fontWeight: 500, fontSize: 13 }}>Description</Text>}
            rules={[{ required: true, message: 'Required' }]}
          >
            <Input.TextArea rows={2} placeholder="What can this role do?" />
          </Form.Item>

          <Form.Item
            name="permissionIds"
            label={<Text style={{ fontWeight: 500, fontSize: 13 }}>Permissions</Text>}
          >
            {permissionsQuery.isLoading ? (
              <Spin />
            ) : (
              <Checkbox.Group style={{ width: '100%' }}>
                {Object.entries(permissionGroups).map(([module, perms]) => (
                  <div
                    key={module}
                    style={{
                      marginBottom: 16,
                      padding: '12px 16px',
                      background: '#f8f9fa',
                      borderRadius: 8,
                    }}
                  >
                    <Text style={{ fontWeight: 600, fontSize: 12, color: '#6e6e73', textTransform: 'uppercase', letterSpacing: '0.05em' }}>
                      {module}
                    </Text>
                    <div style={{ display: 'flex', flexWrap: 'wrap', gap: 12, marginTop: 8 }}>
                      {perms.map((perm) => (
                        <Checkbox key={perm} value={perm} style={{ fontSize: 13 }}>
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
