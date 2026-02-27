import { useState } from 'react';
import { Typography, Flex, Input, Select, Empty, Spin, Tag, Button } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined, EyeOutlined } from '@ant-design/icons';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { helpApi } from '../api/helpApi';
import { useAuth } from '../contexts/AuthContext';
import { useTenantTheme } from '../contexts/ThemeContext';
import type { HelpArticle } from '../types';

const { Text } = Typography;
const { Search } = Input;

export default function HelpPage() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const { hasPermission } = useAuth();
  const { theme } = useTenantTheme();
  const primaryColor = theme?.primaryColor || '#0071e3';
  const [searchText, setSearchText] = useState('');
  const [categoryFilter, setCategoryFilter] = useState<string | undefined>(undefined);

  const { data: articles, isLoading } = useQuery({
    queryKey: ['helpArticles'],
    queryFn: () => helpApi.getAll(),
  });

  const deleteMutation = useMutation({
    mutationFn: helpApi.delete,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['helpArticles'] }),
  });

  const categories = articles
    ? [...new Set(articles.map((a) => a.category).filter(Boolean))]
    : [];

  const filtered = (articles ?? []).filter((a) => {
    if (categoryFilter && a.category !== categoryFilter) return false;
    if (searchText) {
      const q = searchText.toLowerCase();
      return (
        a.title.toLowerCase().includes(q) ||
        a.content.toLowerCase().includes(q) ||
        (a.tags?.toLowerCase().includes(q) ?? false)
      );
    }
    return true;
  });

  return (
    <div>
      <Flex align="center" justify="space-between" style={{ marginBottom: 24 }}>
        <div>
          <h2 style={{ fontSize: 22, fontWeight: 700, color: '#1d1d1f', margin: 0 }}>Help Center</h2>
          <Text style={{ fontSize: 13, color: '#86868b' }}>
            Browse documentation and guides for all modules.
          </Text>
        </div>
        {hasPermission('Help.Create') && (
          <Button type="primary" icon={<PlusOutlined />} onClick={() => navigate('/help/new')}>
            New Article
          </Button>
        )}
      </Flex>

      <Flex gap={12} style={{ marginBottom: 20 }}>
        <Search
          placeholder="Search articles..."
          allowClear
          onChange={(e) => setSearchText(e.target.value)}
          style={{ maxWidth: 320 }}
        />
        <Select
          allowClear
          placeholder="Category"
          style={{ width: 180 }}
          value={categoryFilter}
          onChange={setCategoryFilter}
          options={categories.map((c) => ({ label: c, value: c }))}
        />
      </Flex>

      <Spin spinning={isLoading}>
        {filtered.length === 0 && !isLoading ? (
          <Empty description="No help articles found." />
        ) : (
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(340px, 1fr))', gap: 16 }}>
            {filtered.map((article: HelpArticle) => (
              <div
                key={article.id}
                style={{
                  background: '#ffffff',
                  borderRadius: 12,
                  border: '1px solid #e5e5ea',
                  padding: 20,
                  cursor: 'pointer',
                  transition: 'border-color 0.2s',
                }}
                onClick={() => navigate(`/help/${article.slug}`)}
                onMouseEnter={(e) => (e.currentTarget.style.borderColor = primaryColor)}
                onMouseLeave={(e) => (e.currentTarget.style.borderColor = '#e5e5ea')}
              >
                <Flex justify="space-between" align="flex-start">
                  <div style={{ flex: 1, minWidth: 0 }}>
                    <Text style={{ fontSize: 15, fontWeight: 600, color: '#1d1d1f', display: 'block', marginBottom: 4 }}>
                      {article.title}
                    </Text>
                    {article.category && (
                      <Tag style={{ fontSize: 11, marginBottom: 8, background: `${primaryColor}15`, color: primaryColor, border: 'none' }}>
                        {article.category}
                      </Tag>
                    )}
                    {!article.isPublished && (
                      <Tag color="orange" style={{ fontSize: 11, marginBottom: 8 }}>Draft</Tag>
                    )}
                    <Text
                      style={{ fontSize: 13, color: '#86868b', display: 'block' }}
                      ellipsis
                    >
                      {article.content.substring(0, 120).replace(/[#*_`]/g, '')}...
                    </Text>
                  </div>
                </Flex>
                {article.tags && (
                  <Flex gap={4} style={{ marginTop: 10 }} wrap>
                    {article.tags.split(',').map((tag) => (
                      <Tag key={tag.trim()} style={{ fontSize: 11, color: '#86868b', background: '#f5f5f7', border: 'none' }}>
                        {tag.trim()}
                      </Tag>
                    ))}
                  </Flex>
                )}
                {(hasPermission('Help.Update') || hasPermission('Help.Delete')) && (
                  <Flex gap={8} style={{ marginTop: 12, borderTop: '1px solid #f2f2f7', paddingTop: 10 }}>
                    <Button
                      type="text"
                      size="small"
                      icon={<EyeOutlined />}
                      onClick={(e) => { e.stopPropagation(); navigate(`/help/${article.slug}`); }}
                    >
                      View
                    </Button>
                    {hasPermission('Help.Update') && (
                      <Button
                        type="text"
                        size="small"
                        icon={<EditOutlined />}
                        onClick={(e) => { e.stopPropagation(); navigate(`/help/${article.id}/edit`); }}
                      >
                        Edit
                      </Button>
                    )}
                    {hasPermission('Help.Delete') && (
                      <Button
                        type="text"
                        size="small"
                        danger
                        icon={<DeleteOutlined />}
                        loading={deleteMutation.isPending}
                        onClick={(e) => { e.stopPropagation(); deleteMutation.mutate(article.id); }}
                      >
                        Delete
                      </Button>
                    )}
                  </Flex>
                )}
              </div>
            ))}
          </div>
        )}
      </Spin>
    </div>
  );
}
