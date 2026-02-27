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

const { Title } = Typography;

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
    mutationFn: (data: CreateDepartmentRequest) =>
      departmentsApi.createDepartment(data),
    onSuccess: () => {
      message.success('Department created successfully');
      queryClient.invalidateQueries({ queryKey: ['departments'] });
      closeModal();
    },
    onError: (error: AxiosError<{ message?: string; title?: string }>) => {
      const msg =
        error.response?.data?.message ||
        error.response?.data?.title ||
        'Failed to create department';
      message.error(msg);
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({
      id,
      data,
    }: {
      id: string;
      data: UpdateDepartmentRequest;
    }) => departmentsApi.updateDepartment(id, data),
    onSuccess: () => {
      message.success('Department updated successfully');
      queryClient.invalidateQueries({ queryKey: ['departments'] });
      closeModal();
    },
    onError: (error: AxiosError<{ message?: string; title?: string }>) => {
      const msg =
        error.response?.data?.message ||
        error.response?.data?.title ||
        'Failed to update department';
      message.error(msg);
    },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => departmentsApi.deleteDepartment(id),
    onSuccess: () => {
      message.success('Department deleted successfully');
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
          data: {
            name: values.name,
            description: values.description,
            code: values.code,
            parentDepartmentId: values.parentDepartmentId,
          },
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
      // form validation failed
    }
  };

  const handleDelete = (dept: Department) => {
    if (dept.userCount > 0) {
      message.warning(
        'Cannot delete a department that still has users assigned to it.'
      );
      return;
    }
    Modal.confirm({
      title: 'Delete Department',
      icon: <ExclamationCircleOutlined />,
      content: `Are you sure you want to delete department "${dept.name}"?`,
      okText: 'Delete',
      okType: 'danger',
      onOk: () => deleteMutation.mutateAsync(dept.id),
    });
  };

  const columns: ColumnsType<Department> = [
    {
      title: 'Name',
      dataIndex: 'name',
      key: 'name',
      sorter: (a, b) => a.name.localeCompare(b.name),
    },
    {
      title: 'Code',
      dataIndex: 'code',
      key: 'code',
      render: (value: string | null) => value ?? '-',
    },
    {
      title: 'Parent Department',
      dataIndex: 'parentDepartmentName',
      key: 'parentDepartmentName',
      render: (value: string | null) => value ?? '-',
    },
    {
      title: 'User Count',
      dataIndex: 'userCount',
      key: 'userCount',
      sorter: (a, b) => a.userCount - b.userCount,
    },
    {
      title: 'Created At',
      dataIndex: 'createdAt',
      key: 'createdAt',
      render: (value: string) => dayjs(value).format('YYYY-MM-DD HH:mm'),
      sorter: (a, b) => dayjs(a.createdAt).unix() - dayjs(b.createdAt).unix(),
    },
    {
      title: 'Actions',
      key: 'actions',
      render: (_, record) => (
        <Space>
          <PermissionGate permission="Department.Update">
            <Button
              type="link"
              icon={<EditOutlined />}
              onClick={() => openEditModal(record)}
            >
              Edit
            </Button>
          </PermissionGate>
          <PermissionGate permission="Department.Delete">
            <Button
              type="link"
              danger
              icon={<DeleteOutlined />}
              disabled={record.userCount > 0}
              onClick={() => handleDelete(record)}
            >
              Delete
            </Button>
          </PermissionGate>
        </Space>
      ),
    },
  ];

  // Filter out the currently-editing department from parent choices to prevent self-reference
  const parentOptions = (departmentsQuery.data ?? [])
    .filter((d) => d.id !== editingDept?.id)
    .map((d) => ({ label: d.name, value: d.id }));

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
          Departments
        </Title>
        <PermissionGate permission="Department.Create">
          <Button
            type="primary"
            icon={<PlusOutlined />}
            onClick={openCreateModal}
          >
            New Department
          </Button>
        </PermissionGate>
      </div>

      <Table<Department>
        rowKey="id"
        columns={columns}
        dataSource={departmentsQuery.data ?? []}
        loading={departmentsQuery.isLoading}
        pagination={{ showSizeChanger: true }}
      />

      <Modal
        title={editingDept ? 'Edit Department' : 'New Department'}
        open={modalOpen}
        onOk={handleModalOk}
        onCancel={closeModal}
        confirmLoading={isMutating}
        destroyOnClose
      >
        <Form<DepartmentFormValues>
          form={form}
          layout="vertical"
        >
          <Form.Item
            name="name"
            label="Name"
            rules={[{ required: true, message: 'Department name is required' }]}
          >
            <Input placeholder="Department name" />
          </Form.Item>

          <Form.Item
            name="description"
            label="Description"
            rules={[{ required: true, message: 'Description is required' }]}
          >
            <Input.TextArea rows={2} placeholder="Department description" />
          </Form.Item>

          <Form.Item name="code" label="Code">
            <Input placeholder="Department code (optional)" />
          </Form.Item>

          <Form.Item name="parentDepartmentId" label="Parent Department">
            <Select
              allowClear
              placeholder="Select parent department (optional)"
              options={parentOptions}
            />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}
