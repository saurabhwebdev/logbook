import { Flex, Button, Spin, Empty, Tag } from 'antd';
import { ArrowLeftOutlined, EditOutlined } from '@ant-design/icons';
import { useQuery } from '@tanstack/react-query';
import { useParams, useNavigate } from 'react-router-dom';
import Markdown from 'react-markdown';
import { helpApi } from '../api/helpApi';
import { useAuth } from '../contexts/AuthContext';
import { useTenantTheme } from '../contexts/ThemeContext';

export default function HelpArticleViewPage() {
  const { slug } = useParams<{ slug: string }>();
  const navigate = useNavigate();
  const { hasPermission } = useAuth();
  const { theme } = useTenantTheme();
  const primaryColor = theme?.primaryColor || '#0071e3';

  const { data: article, isLoading } = useQuery({
    queryKey: ['helpArticle', slug],
    queryFn: () => helpApi.getBySlug(slug!),
    enabled: !!slug,
  });

  return (
    <div>
      <Flex align="center" justify="space-between" style={{ marginBottom: 24 }}>
        <Flex align="center" gap={12}>
          <Button type="text" icon={<ArrowLeftOutlined />} onClick={() => navigate('/help')}>
            Back
          </Button>
        </Flex>
        {hasPermission('Help.Update') && article && (
          <Button icon={<EditOutlined />} onClick={() => navigate(`/help/${article.id}/edit`)}>
            Edit Article
          </Button>
        )}
      </Flex>

      <Spin spinning={isLoading}>
        {!article && !isLoading ? (
          <Empty description="Article not found." />
        ) : article ? (
          <div style={{ background: '#ffffff', borderRadius: 12, border: '1px solid #e5e5ea', padding: 32, maxWidth: 800 }}>
            <h1 style={{ fontSize: 24, fontWeight: 700, color: '#1d1d1f', margin: 0, marginBottom: 8 }}>
              {article.title}
            </h1>
            <Flex gap={8} style={{ marginBottom: 20 }}>
              {article.category && (
                <Tag style={{ background: `${primaryColor}15`, color: primaryColor, border: 'none' }}>
                  {article.category}
                </Tag>
              )}
              {article.moduleKey && <Tag>{article.moduleKey}</Tag>}
              {!article.isPublished && <Tag color="orange">Draft</Tag>}
            </Flex>
            <div style={{ fontSize: 14, lineHeight: 1.8, color: '#1d1d1f' }}>
              <Markdown>{article.content}</Markdown>
            </div>
            {article.tags && (
              <Flex gap={4} style={{ marginTop: 24, paddingTop: 16, borderTop: '1px solid #f2f2f7' }} wrap>
                {article.tags.split(',').map((tag) => (
                  <Tag key={tag.trim()} style={{ fontSize: 11, color: '#86868b', background: '#f5f5f7', border: 'none' }}>
                    {tag.trim()}
                  </Tag>
                ))}
              </Flex>
            )}
          </div>
        ) : null}
      </Spin>
    </div>
  );
}
