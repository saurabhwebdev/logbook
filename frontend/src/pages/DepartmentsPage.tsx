import { useState } from 'react';
import {
  Typography,
  Button,
  Table,
  Space,
  Modal,
  Form,
  Input,
  Select,
  message,
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
import dayjs from 'dayjs';
import { departmentsApi } from '../api/departmentsApi';
import type {
  CreateDepartmentRequest,
  UpdateDepartmentRequest,
} from '../api/departmentsApi';
import type { Department } from '../types';
import PermissionGate from '../components/PermissionGate';

const { Text } = Typography;

interface DepartmentFormValues {
  name: string;
  description: string;
  code?: string;
  parentDepartmentId?: string;
}

export default function DepartmentsPage() {
  const queryClient = useQueryClient();
  const [modalOpen, setModalOpen] = useState(false);
  const [editingDept, setEditingDept] = useState<Department | null>(null);
  const [form] = Form.useForm<DepartmentFormValues>();

  const departmentsQuery = useQuery({
    queryKey: ['departments'],
    queryFn: () => departmentsApi.getDepartments(),
  });

  const createMutation = useMutation({
    mutationFn: (data: CreateDepartmentRequest) => departmentsApi.createDepartment(data),
    onSuccess: () => {
      message.success('Department created');
      queryClient.invalidateQueries({ queryKey: ['departments'] });
      closeModal();
    },
    onError: (error: AxiosError<{ message?: string; title?: string }>) => {
      message.error(error.response?.data?.message || error.response?.data?.title || 'Failed to create department');
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateDepartmentRequest }) =>
      departmentsApi.updateDepartment(id, data),
    onSuccess: () => {
      message.success('Department updated');
      queryClient.invalidateQueries({ queryKey: ['departments'] });
      closeModal();
    },
    onError: (error: AxiosError<{ message?: string; title?: string }>) => {
      message.error(error.response?.data?.message || error.response?.data?.title || 'Failed to update department');
    },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => departmentsApi.deleteDepartment(id),
    onSuccess: () => {
      message.success('Department deleted');
      queryClient.invalidateQueries({ queryKey: ['departments'] });
    },
    onError: () => {
      message.error('Failed to delete department');
    },
  });

  const openCreateModal = () => {
    setEditingDept(null);
    form.resetFields();
    setModalOpen(true);
  };

  const openEditModal = (dept: Department) => {
    setEditingDept(dept);
    form.setFieldsValue({
      name: dept.name,
      description: '',
      code: dept.code ?? undefined,
      parentDepartmentId: dept.parentDepartmentId ?? undefined,
    });
    setModalOpen(true);
  };

  const closeModal = () => {
    setModalOpen(false);
    setEditingDept(null);
    form.resetFields();
  };

  const handleModalOk = async () => {
    try {
      const values = await form.validateFields();
      if (editingDept) {
        updateMutation.mutate({
          id: editingDept.id,
          data: { name: values.name, description: values.description, code: values.code, parentDepartmentId: values.parentDepartmentId },
        });
      } else {
        createMutation.mutate({
          name: values.name,
          description: values.description,
          code: values.code,
          parentDepartmentId: values.parentDepartmentId,
        });
      }
    } catch {
      // validation
    }
  };

  const handleDelete = (dept: Department) => {
    if (dept.userCount > 0) {
      message.warning('Cannot delete a department with assigned users.');
      return;
    }
    Modal.confirm({
      title: 'Delete department',
      icon: <ExclamationCircleOutlined />,
      content: `This will permanently delete "${dept.name}". This action cannot be undone.`,
      okText: 'Delete',
      okType: 'danger',
      onOk: () => deleteMutation.mutateAsync(dept.id),
    });
  };

  const columns: ColumnsType<Department> = [
    {
      title: 'Department',
      key: 'name',
      render: (_, record) => (
        <div>
          <div style={{ fontWeight: 500, color: '#1d1d1f', fontSize: 13 }}>
            {record.name}
          </div>
          {record.code && (
            <div style={{ fontSize: 12, color: '#86868b' }}>{record.code}</div>
          )}
        </div>
      ),
      sorter: (a, b) => a.name.localeCompare(b.name),
    },
    {
      title: 'Parent',
      dataIndex: 'parentDepartmentName',
      key: 'parentDepartmentName',
      render: (value: string | null) => (
        <Text style={{ fontSize: 13, color: value ? '#1d1d1f' : '#c7c7cc' }}>
          {value ?? '--'}
        </Text>
      ),
    },
    {
      title: 'Members',
      dataIndex: 'userCount',
      key: 'userCount',
      width: 100,
      render: (count: number) => (
        <Text style={{ fontSize: 13, color: '#6e6e73' }}>{count}</Text>
      ),
      sorter: (a, b) => a.userCount - b.userCount,
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
          <PermissionGate permission="Department.Update">
            <Button type="text" size="small" icon={<EditOutlined />} onClick={() => openEditModal(record)} style={{ color: '#86868b' }} />
          </PermissionGate>
          <PermissionGate permission="Department.Delete">
            <Button type="text" size="small" danger icon={<DeleteOutlined />} disabled={record.userCount > 0} onClick={() => handleDelete(record)} />
          </PermissionGate>
        </Space>
      ),
    },
  ];

  const parentOptions = (departmentsQuery.data ?? [])
    .filter((d) => d.id !== editingDept?.id)
    .map((d) => ({ label: d.name, value: d.id }));

  const isMutating = createMutation.isPending || updateMutation.isPending;

  return (
    <div>
      <Flex align="center" justify="space-between" style={{ marginBottom: 24 }}>
        <h2 style={{ fontSize: 22, fontWeight: 700, color: '#1d1d1f', margin: 0 }}>Departments</h2>
        <PermissionGate permission="Department.Create">
          <Button type="primary" icon={<PlusOutlined />} onClick={openCreateModal}>
            Add department
          </Button>
        </PermissionGate>
      </Flex>

      <div style={{ background: '#ffffff', borderRadius: 12, border: '1px solid #e5e5ea', overflow: 'hidden' }}>
        <Table<Department>
          rowKey="id"
          columns={columns}
          dataSource={departmentsQuery.data ?? []}
          loading={departmentsQuery.isLoading}
          pagination={{ showSizeChanger: true, style: { padding: '0 16px' } }}
        />
      </div>

      <Modal
        title={editingDept ? 'Edit department' : 'New department'}
        open={modalOpen}
        onOk={handleModalOk}
        onCancel={closeModal}
        confirmLoading={isMutating}
        destroyOnClose
        okText={editingDept ? 'Save changes' : 'Create'}
      >
        <Form<DepartmentFormValues> form={form} layout="vertical" requiredMark={false}>
          <Form.Item name="name" label={<Text style={{ fontWeight: 500, fontSize: 13 }}>Name</Text>} rules={[{ required: true, message: 'Required' }]}>
            <Input placeholder="e.g. Engineering, Marketing" />
          </Form.Item>
          <Form.Item name="description" label={<Text style={{ fontWeight: 500, fontSize: 13 }}>Description</Text>} rules={[{ required: true, message: 'Required' }]}>
            <Input.TextArea rows={2} placeholder="What does this department do?" />
          </Form.Item>
          <Form.Item name="code" label={<Text style={{ fontWeight: 500, fontSize: 13 }}>Code</Text>}>
            <Input placeholder="e.g. ENG, MKT (optional)" />
          </Form.Item>
          <Form.Item name="parentDepartmentId" label={<Text style={{ fontWeight: 500, fontSize: 13 }}>Parent department</Text>}>
            <Select allowClear placeholder="None" options={parentOptions} />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}
