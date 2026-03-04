import { Drawer, Typography, Spin, Empty } from 'antd';
import { useQuery } from '@tanstack/react-query';
import Markdown from 'react-markdown';
import { helpApi } from '../api/helpApi';

const { Text } = Typography;

interface HelpDrawerProps {
  open: boolean;
  onClose: () => void;
  moduleKey: string;
}

export default function HelpDrawer({ open, onClose, moduleKey }: HelpDrawerProps) {
  const slug = moduleKey.toLowerCase().replace(/[^a-z0-9]+/g, '-');

  const { data: article, isLoading } = useQuery({
    queryKey: ['helpArticle', slug],
    queryFn: () => helpApi.getBySlug(slug),
    enabled: open,
  });

  return (
    <Drawer
      title={article?.title || 'Help'}
      open={open}
      onClose={onClose}
      size="large"
      styles={{ body: { padding: '16px 24px' } }}
    >
      {isLoading ? (
        <div style={{ textAlign: 'center', padding: 40 }}>
          <Spin />
        </div>
      ) : article ? (
        <div className="help-content" style={{ fontSize: 14, lineHeight: 1.7, color: '#1d1d1f' }}>
          {article.category && (
            <Text style={{ fontSize: 12, color: '#86868b', display: 'block', marginBottom: 16 }}>
              {article.category}
            </Text>
          )}
          <Markdown>{article.content}</Markdown>
        </div>
      ) : (
        <Empty
          description="No help article available for this module yet."
          style={{ marginTop: 60 }}
        />
      )}
    </Drawer>
  );
}
