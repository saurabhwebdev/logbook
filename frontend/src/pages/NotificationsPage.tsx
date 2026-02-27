import { Typography, List, Button, Flex, Empty, Spin, message } from 'antd';
import {
  CheckOutlined,
  CheckCircleOutlined,
  InfoCircleOutlined,
  WarningOutlined,
  CloseCircleOutlined,
} from '@ant-design/icons';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import dayjs from 'dayjs';
import relativeTime from 'dayjs/plugin/relativeTime';
import { notificationsApi } from '../api/notificationsApi';
import type { Notification as AppNotification } from '../types';

const { Text } = Typography;

dayjs.extend(relativeTime);

const typeConfig: Record<string, { icon: React.ReactNode; color: string }> = {
  Info: { icon: <InfoCircleOutlined />, color: '#0071e3' },
  Success: { icon: <CheckCircleOutlined />, color: '#34c759' },
  Warning: { icon: <WarningOutlined />, color: '#ff9500' },
  Error: { icon: <CloseCircleOutlined />, color: '#ff3b30' },
};

export default function NotificationsPage() {
  const queryClient = useQueryClient();

  const { data, isLoading } = useQuery({
    queryKey: ['notifications'],
    queryFn: () => notificationsApi.getMy(),
  });

  const markReadMutation = useMutation({
    mutationFn: notificationsApi.markRead,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['notifications'] }),
  });

  const markAllReadMutation = useMutation({
    mutationFn: notificationsApi.markAllRead,
    onSuccess: (count) => {
      message.success(`${count} notifications marked as read`);
      queryClient.invalidateQueries({ queryKey: ['notifications'] });
    },
  });

  const unreadCount = (data ?? []).filter((n) => !n.isRead).length;

  const renderItem = (item: AppNotification) => {
    const config = typeConfig[item.type] || typeConfig.Info;
    return (
      <div
        style={{
          padding: '16px 20px',
          borderBottom: '1px solid #f2f2f7',
          background: item.isRead ? 'transparent' : '#f0f5ff08',
          display: 'flex',
          alignItems: 'flex-start',
          gap: 14,
          cursor: item.isRead ? 'default' : 'pointer',
        }}
        onClick={() => {
          if (!item.isRead) markReadMutation.mutate(item.id);
        }}
      >
        <div
          style={{
            width: 32,
            height: 32,
            borderRadius: 8,
            background: `${config.color}15`,
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            color: config.color,
            fontSize: 14,
            flexShrink: 0,
            marginTop: 2,
          }}
        >
          {config.icon}
        </div>
        <div style={{ flex: 1, minWidth: 0 }}>
          <Flex align="center" gap={8}>
            <span style={{ fontWeight: item.isRead ? 400 : 600, fontSize: 13, color: '#1d1d1f' }}>
              {item.title}
            </span>
            {!item.isRead && (
              <span
                style={{
                  width: 6,
                  height: 6,
                  borderRadius: '50%',
                  background: '#0071e3',
                  flexShrink: 0,
                }}
              />
            )}
          </Flex>
          <div style={{ fontSize: 13, color: '#6e6e73', marginTop: 2, lineHeight: 1.4 }}>
            {item.message}
          </div>
          <div style={{ fontSize: 12, color: '#86868b', marginTop: 4 }}>
            {dayjs(item.createdAt).fromNow()}
          </div>
        </div>
      </div>
    );
  };

  return (
    <div>
      <Flex align="center" justify="space-between" style={{ marginBottom: 24 }}>
        <div>
          <h2 style={{ fontSize: 22, fontWeight: 700, color: '#1d1d1f', margin: 0 }}>
            Notifications
          </h2>
          <Text style={{ fontSize: 13, color: '#86868b' }}>
            {unreadCount > 0 ? `${unreadCount} unread` : 'All caught up'}
          </Text>
        </div>
        {unreadCount > 0 && (
          <Button
            icon={<CheckOutlined />}
            onClick={() => markAllReadMutation.mutate()}
            loading={markAllReadMutation.isPending}
          >
            Mark all read
          </Button>
        )}
      </Flex>

      <div
        style={{
          background: '#ffffff',
          borderRadius: 12,
          border: '1px solid #e5e5ea',
          overflow: 'hidden',
        }}
      >
        {isLoading ? (
          <Flex align="center" justify="center" style={{ padding: 60 }}>
            <Spin />
          </Flex>
        ) : (data ?? []).length === 0 ? (
          <Empty
            description="No notifications"
            style={{ padding: 60 }}
          />
        ) : (
          <List
            dataSource={data}
            renderItem={renderItem}
            split={false}
          />
        )}
      </div>
    </div>
  );
}
