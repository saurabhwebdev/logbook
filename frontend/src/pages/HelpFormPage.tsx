import { useEffect } from 'react';
import { Typography, Form, Input, Select, InputNumber, Switch, Button, Spin, Flex, message } from 'antd';
import { ArrowLeftOutlined } from '@ant-design/icons';
import { useNavigate, useParams } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import Markdown from 'react-markdown';
import { helpApi } from '../api/helpApi';

const { Text } = Typography;
const { TextArea } = Input;

const MODULE_KEYS = [
  'Dashboard', 'Users', 'Roles', 'Departments', 'AuditLogs', 'Tenants',
  'Settings', 'FeatureFlags', 'Notifications', 'StateMachine',
  'Files', 'Reports', 'ApiIntegration', 'DemoTasks', 'Theming', 'Help',
];

const CATEGORIES = ['Getting Started', 'Admin Guide', 'User Guide', 'Developer Guide', 'FAQ'];

export default function HelpFormPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [form] = Form.useForm();
  const isEdit = !!id;

  const contentValue = Form.useWatch('content', form);

  const { data: articles, isLoading: loadingArticles } = useQuery({
    queryKey: ['helpArticles'],
    queryFn: () => helpApi.getAll(),
    enabled: isEdit,
  });

  const existingArticle = isEdit ? articles?.find((a) => a.id === id) : undefined;

  useEffect(() => {
    if (existingArticle) {
      form.setFieldsValue({
        title: existingArticle.title,
        slug: existingArticle.slug,
        moduleKey: existingArticle.moduleKey,
        content: existingArticle.content,
        category: existingArticle.category,
        sortOrder: existingArticle.sortOrder,
        isPublished: existingArticle.isPublished,
        tags: existingArticle.tags,
      });
    }
  }, [existingArticle, form]);

  const createMutation = useMutation({
    mutationFn: helpApi.create,
    onSuccess: () => {
      message.success('Article created.');
      queryClient.invalidateQueries({ queryKey: ['helpArticles'] });
      navigate('/help');
    },
    onError: () => message.error('Failed to create article.'),
  });

  const updateMutation = useMutation({
    mutationFn: (data: Parameters<typeof helpApi.update>[1]) => helpApi.update(id!, data),
    onSuccess: () => {
      message.success('Article updated.');
      queryClient.invalidateQueries({ queryKey: ['helpArticles'] });
      navigate('/help');
    },
    onError: () => message.error('Failed to update article.'),
  });

  const handleFinish = (values: Record<string, unknown>) => {
    const payload = {
      title: values.title as string,
      slug: values.slug as string,
      moduleKey: (values.moduleKey as string) || null,
      content: values.content as string,
      category: (values.category as string) || null,
      sortOrder: (values.sortOrder as number) || 0,
      isPublished: (values.isPublished as boolean) ?? true,
      tags: (values.tags as string) || null,
    };

    if (isEdit) {
      updateMutation.mutate({ id: id!, ...payload });
    } else {
      createMutation.mutate(payload);
    }
  };

  const autoSlug = (title: string) => {
    return title
      .toLowerCase()
      .replace(/[^a-z0-9]+/g, '-')
      .replace(/^-|-$/g, '');
  };

  const isPending = createMutation.isPending || updateMutation.isPending;

  if (isEdit && loadingArticles) {
    return <Spin style={{ display: 'block', margin: '60px auto' }} />;
  }

  return (
    <div>
      <Flex align="center" gap={12} style={{ marginBottom: 24 }}>
        <Button type="text" icon={<ArrowLeftOutlined />} onClick={() => navigate('/help')}>
          Back
        </Button>
        <h2 style={{ fontSize: 22, fontWeight: 700, color: '#1d1d1f', margin: 0 }}>
          {isEdit ? 'Edit Article' : 'New Article'}
        </h2>
      </Flex>

      <Flex gap={24} align="flex-start">
        {/* Form */}
        <div style={{ flex: 1, background: '#ffffff', borderRadius: 12, border: '1px solid #e5e5ea', padding: 24 }}>
          <Form
            form={form}
            layout="vertical"
            onFinish={handleFinish}
            initialValues={{ sortOrder: 0, isPublished: true }}
          >
            <Form.Item name="title" label="Title" rules={[{ required: true, max: 200 }]}>
              <Input
                placeholder="e.g., Getting Started with Users"
                onChange={(e) => {
                  if (!isEdit) {
                    form.setFieldValue('slug', autoSlug(e.target.value));
                  }
                }}
              />
            </Form.Item>

            <Form.Item
              name="slug"
              label="Slug"
              rules={[
                { required: true, max: 200 },
                { pattern: /^[a-z0-9]+(-[a-z0-9]+)*$/, message: 'Lowercase letters, numbers, hyphens only' },
              ]}
            >
              <Input placeholder="e.g., getting-started-users" style={{ fontFamily: 'monospace' }} />
            </Form.Item>

            <Flex gap={16}>
              <Form.Item name="moduleKey" label="Module" style={{ flex: 1 }}>
                <Select allowClear placeholder="Link to a module" options={MODULE_KEYS.map((k) => ({ label: k, value: k }))} />
              </Form.Item>
              <Form.Item name="category" label="Category" style={{ flex: 1 }}>
                <Select allowClear placeholder="Select category" options={CATEGORIES.map((c) => ({ label: c, value: c }))} />
              </Form.Item>
            </Flex>

            <Form.Item name="content" label="Content (Markdown)" rules={[{ required: true }]}>
              <TextArea rows={14} placeholder="Write help content using Markdown..." style={{ fontFamily: 'monospace', fontSize: 13 }} />
            </Form.Item>

            <Flex gap={16}>
              <Form.Item name="sortOrder" label="Sort Order" style={{ width: 120 }}>
                <InputNumber min={0} />
              </Form.Item>
              <Form.Item name="isPublished" label="Published" valuePropName="checked">
                <Switch />
              </Form.Item>
            </Flex>

            <Form.Item name="tags" label="Tags (comma-separated)">
              <Input placeholder="e.g., users, authentication, getting-started" />
            </Form.Item>

            <Flex justify="flex-end" gap={12}>
              <Button onClick={() => navigate('/help')}>Cancel</Button>
              <Button type="primary" htmlType="submit" loading={isPending}>
                {isEdit ? 'Update' : 'Create'}
              </Button>
            </Flex>
          </Form>
        </div>

        {/* Live Preview */}
        <div style={{ flex: 1, background: '#ffffff', borderRadius: 12, border: '1px solid #e5e5ea', padding: 24, minHeight: 400 }}>
          <Text style={{ fontSize: 12, fontWeight: 600, color: '#86868b', textTransform: 'uppercase', letterSpacing: 0.5, display: 'block', marginBottom: 16 }}>
            Preview
          </Text>
          <div style={{ fontSize: 14, lineHeight: 1.8, color: '#1d1d1f' }}>
            {contentValue ? (
              <Markdown>{contentValue}</Markdown>
            ) : (
              <Text style={{ color: '#86868b', fontStyle: 'italic' }}>Start typing to see preview...</Text>
            )}
          </div>
        </div>
      </Flex>
    </div>
  );
}
