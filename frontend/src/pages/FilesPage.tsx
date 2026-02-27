import { useState } from 'react';
import { Typography, Table, Flex, Button, Upload, Modal, Input, message, Popconfirm } from 'antd';
import { UploadOutlined, DeleteOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import type { UploadFile } from 'antd/es/upload';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import dayjs from 'dayjs';
import { filesApi } from '../api/filesApi';
import type { FileMetadata } from '../types';
import EmptyState from '../components/EmptyState';

const { Text } = Typography;
const { Search } = Input;

function formatFileSize(bytes: number): string {
  if (bytes < 1024) return `${bytes} B`;
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
}

export default function FilesPage() {
  const [search, setSearch] = useState('');
  const [uploadOpen, setUploadOpen] = useState(false);
  const [fileList, setFileList] = useState<UploadFile[]>([]);
  const [description, setDescription] = useState('');
  const [category, setCategory] = useState('');
  const queryClient = useQueryClient();

  const { data, isLoading } = useQuery({
    queryKey: ['files'],
    queryFn: () => filesApi.getAll(),
  });

  const uploadMutation = useMutation({
    mutationFn: (file: File) => filesApi.upload(file, description || undefined, category || undefined),
    onSuccess: () => {
      message.success('File uploaded');
      queryClient.invalidateQueries({ queryKey: ['files'] });
      setUploadOpen(false);
      setFileList([]);
      setDescription('');
      setCategory('');
    },
    onError: () => message.error('Upload failed'),
  });

  const deleteMutation = useMutation({
    mutationFn: filesApi.delete,
    onSuccess: () => {
      message.success('File deleted');
      queryClient.invalidateQueries({ queryKey: ['files'] });
    },
  });

  const handleUpload = () => {
    if (fileList.length === 0) return;
    const file = fileList[0] as unknown as { originFileObj: File };
    uploadMutation.mutate(file.originFileObj);
  };

  const filtered = (data ?? []).filter(
    (f) =>
      f.originalFileName.toLowerCase().includes(search.toLowerCase()) ||
      (f.category || '').toLowerCase().includes(search.toLowerCase())
  );

  const columns: ColumnsType<FileMetadata> = [
    {
      title: 'File',
      key: 'name',
      render: (_, record) => (
        <div>
          <div style={{ fontWeight: 500, color: '#1d1d1f', fontSize: 13 }}>{record.originalFileName}</div>
          {record.description && <div style={{ fontSize: 12, color: '#86868b' }}>{record.description}</div>}
        </div>
      ),
      sorter: (a, b) => a.originalFileName.localeCompare(b.originalFileName),
    },
    {
      title: 'Size',
      dataIndex: 'fileSize',
      key: 'fileSize',
      width: 100,
      render: (value: number) => <Text style={{ fontSize: 13, color: '#6e6e73' }}>{formatFileSize(value)}</Text>,
      sorter: (a, b) => a.fileSize - b.fileSize,
    },
    {
      title: 'Type',
      dataIndex: 'contentType',
      key: 'contentType',
      width: 140,
      render: (value: string) => (
        <code style={{ fontSize: 11, background: '#f5f5f7', padding: '2px 6px', borderRadius: 4 }}>{value}</code>
      ),
    },
    {
      title: 'Category',
      dataIndex: 'category',
      key: 'category',
      width: 120,
      render: (value: string | null) =>
        value ? (
          <span style={{ fontSize: 12, fontWeight: 500, color: '#6e6e73', background: '#f5f5f7', padding: '2px 10px', borderRadius: 10 }}>
            {value}
          </span>
        ) : <Text style={{ fontSize: 12, color: '#86868b' }}>-</Text>,
    },
    {
      title: 'Uploaded by',
      dataIndex: 'uploadedByName',
      key: 'uploadedByName',
      width: 130,
      render: (value: string | null) => <Text style={{ fontSize: 13, color: '#6e6e73' }}>{value || '-'}</Text>,
    },
    {
      title: 'Date',
      dataIndex: 'createdAt',
      key: 'createdAt',
      width: 110,
      render: (value: string) => <Text style={{ fontSize: 13, color: '#86868b' }}>{dayjs(value).format('MMM D, YYYY')}</Text>,
      sorter: (a, b) => dayjs(a.createdAt).unix() - dayjs(b.createdAt).unix(),
    },
    {
      title: '',
      key: 'actions',
      width: 60,
      render: (_, record) => (
        <Popconfirm title="Delete this file?" onConfirm={() => deleteMutation.mutate(record.id)} okText="Delete" okButtonProps={{ danger: true }}>
          <Button type="text" size="small" icon={<DeleteOutlined />} danger />
        </Popconfirm>
      ),
    },
  ];

  return (
    <div>
      <Flex align="center" justify="space-between" style={{ marginBottom: 24 }}>
        <div>
          <h2 style={{ fontSize: 22, fontWeight: 700, color: '#1d1d1f', margin: 0 }}>Files</h2>
          <Text style={{ fontSize: 13, color: '#86868b' }}>Upload and manage files.</Text>
        </div>
        <Button type="primary" icon={<UploadOutlined />} onClick={() => setUploadOpen(true)}>
          Upload
        </Button>
      </Flex>

      <div style={{ background: '#ffffff', borderRadius: 12, border: '1px solid #e5e5ea', padding: '16px 16px 0', marginBottom: 16 }}>
        <Search placeholder="Search files..." value={search} onChange={(e) => setSearch(e.target.value)} style={{ width: 320 }} allowClear />
      </div>

      <div style={{ background: '#ffffff', borderRadius: 12, border: '1px solid #e5e5ea', overflow: 'hidden' }}>
        <Table<FileMetadata>
          rowKey="id"
          columns={columns}
          dataSource={filtered}
          loading={isLoading}
          locale={{
            emptyText: (
              <EmptyState
                title={search ? "No files found" : "No files yet"}
                description={
                  search
                    ? "No files match your search criteria. Try adjusting your search terms."
                    : "Upload your first file to get started with file management."
                }
                size={180}
                action={
                  !search
                    ? {
                        label: 'Upload First File',
                        onClick: () => setUploadOpen(true),
                        icon: <UploadOutlined />,
                      }
                    : undefined
                }
              />
            ),
          }}
          pagination={{ showSizeChanger: true, style: { padding: '0 16px' } }}
        />
      </div>

      <Modal title="Upload File" open={uploadOpen} onCancel={() => setUploadOpen(false)} onOk={handleUpload} confirmLoading={uploadMutation.isPending} okText="Upload">
        <div style={{ marginTop: 16, display: 'flex', flexDirection: 'column', gap: 12 }}>
          <Upload fileList={fileList} onChange={({ fileList: fl }) => setFileList(fl.slice(-1))} beforeUpload={() => false} maxCount={1}>
            <Button icon={<UploadOutlined />}>Select File</Button>
          </Upload>
          <Input placeholder="Description (optional)" value={description} onChange={(e) => setDescription(e.target.value)} />
          <Input placeholder="Category (optional)" value={category} onChange={(e) => setCategory(e.target.value)} />
        </div>
      </Modal>
    </div>
  );
}
