import { Typography, Timeline, Spin, Flex, Card } from 'antd';
import {
  PlusCircleOutlined,
  EditOutlined,
  DeleteOutlined,
  FileOutlined,
} from '@ant-design/icons';
import { useQuery } from '@tanstack/react-query';
import dayjs from 'dayjs';
import relativeTime from 'dayjs/plugin/relativeTime';
import { auditLogsApi } from '../api/auditLogsApi';
import { useTenantTheme } from '../contexts/ThemeContext';
import EmptyState from '../components/EmptyState';
import type { UserActivity } from '../types';

const { Text } = Typography;

dayjs.extend(relativeTime);

// Map actions to icons and colors
const getActionConfig = (action: string, primaryColor: string) => {
  const lowerAction = action.toLowerCase();
  if (lowerAction.includes('create') || lowerAction.includes('insert')) {
    return { icon: <PlusCircleOutlined />, color: '#34c759' };
  }
  if (lowerAction.includes('update') || lowerAction.includes('modify')) {
    return { icon: <EditOutlined />, color: primaryColor };
  }
  if (lowerAction.includes('delete') || lowerAction.includes('remove')) {
    return { icon: <DeleteOutlined />, color: '#ff3b30' };
  }
  return { icon: <FileOutlined />, color: '#86868b' };
};

// Format entity name for display (PascalCase to readable)
const formatEntityName = (entityName: string): string => {
  return entityName.replace(/([A-Z])/g, ' $1').trim();
};

// Generate activity description
const getActivityDescription = (activity: UserActivity): string => {
  const entity = formatEntityName(activity.entityName);
  const action = activity.action.toLowerCase();

  if (action.includes('create') || action.includes('insert')) {
    return `Created a new ${entity}`;
  }
  if (action.includes('update') || action.includes('modify')) {
    // Try to extract what was changed from values if available
    if (activity.newValues) {
      try {
        const changes = JSON.parse(activity.newValues);
        const changedFields = Object.keys(changes);
        if (changedFields.length > 0) {
          const fieldStr = changedFields.slice(0, 2).join(', ');
          return `Updated ${entity} (${fieldStr}${changedFields.length > 2 ? '...' : ''})`;
        }
      } catch {
        // If parsing fails, use generic message
      }
    }
    return `Updated a ${entity}`;
  }
  if (action.includes('delete') || action.includes('remove')) {
    return `Deleted a ${entity}`;
  }
  return `${activity.action} on ${entity}`;
};

export default function ActivityFeedPage() {
  const { theme } = useTenantTheme();
  const primaryColor = theme?.primaryColor || '#0071e3';

  const { data: activities, isLoading } = useQuery({
    queryKey: ['userActivity'],
    queryFn: () => auditLogsApi.getUserActivity(50),
  });

  const renderActivity = (activity: UserActivity) => {
    const config = getActionConfig(activity.action, primaryColor);
    const description = getActivityDescription(activity);

    return {
      dot: (
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
          }}
        >
          {config.icon}
        </div>
      ),
      children: (
        <div style={{ paddingBottom: 16 }}>
          <div
            style={{
              fontSize: 14,
              fontWeight: 500,
              color: '#1d1d1f',
              marginBottom: 4,
            }}
          >
            {description}
          </div>
          <Flex align="center" gap={8}>
            <Text
              style={{
                fontSize: 12,
                color: '#86868b',
              }}
            >
              {dayjs(activity.timestamp).fromNow()}
            </Text>
            <span style={{ color: '#d1d1d6' }}>•</span>
            <Text
              style={{
                fontSize: 12,
                color: '#86868b',
              }}
            >
              ID: {activity.entityId.substring(0, 8)}...
            </Text>
          </Flex>
        </div>
      ),
    };
  };

  return (
    <div>
      <div style={{ marginBottom: 24 }}>
        <h2 style={{ fontSize: 22, fontWeight: 700, color: '#1d1d1f', margin: 0 }}>
          My Activity
        </h2>
        <Text style={{ fontSize: 13, color: '#86868b' }}>
          Your recent actions across the system
        </Text>
      </div>

      <Card
        style={{
          borderRadius: 12,
          border: '1px solid #e5e5ea',
        }}
        styles={{
          body: {
            padding: isLoading || !activities || activities.length === 0 ? 24 : 0,
          },
        }}
      >
        {isLoading ? (
          <Flex align="center" justify="center" style={{ padding: 60 }}>
            <Spin />
          </Flex>
        ) : !activities || activities.length === 0 ? (
          <EmptyState
            title="No activity yet"
            description="Your recent actions and changes will appear here once you start interacting with the system."
            size={180}
          />
        ) : (
          <div style={{ padding: '32px 24px' }}>
            <Timeline
              items={activities.map(renderActivity)}
              mode="left"
            />
          </div>
        )}
      </Card>
    </div>
  );
}
