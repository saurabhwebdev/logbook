import { Button, Flex } from 'antd';
import Lottie from 'lottie-react';
import emptyStateAnimation from '../assets/animations/empty-state.json';

interface EmptyStateProps {
  title: string;
  description: string;
  size?: number;
  action?: {
    label: string;
    onClick: () => void;
    icon?: React.ReactNode;
  };
}

export default function EmptyState({
  title,
  description,
  size = 200,
  action,
}: EmptyStateProps) {
  return (
    <Flex
      vertical
      align="center"
      justify="center"
      style={{
        padding: '60px 24px',
      }}
    >
      <div
        style={{
          width: size,
          height: size,
          marginBottom: 16,
        }}
      >
        <Lottie
          animationData={emptyStateAnimation}
          loop
          style={{ width: '100%', height: '100%' }}
        />
      </div>

      <div
        style={{
          fontSize: 16,
          fontWeight: 600,
          color: '#1d1d1f',
          marginBottom: 8,
          textAlign: 'center',
          fontFamily: 'Inter, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif',
        }}
      >
        {title}
      </div>

      <div
        style={{
          fontSize: 14,
          color: '#86868b',
          textAlign: 'center',
          maxWidth: 400,
          lineHeight: 1.5,
          fontFamily: 'Inter, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif',
        }}
      >
        {description}
      </div>

      {action && (
        <Button
          type="primary"
          icon={action.icon}
          onClick={action.onClick}
          style={{ marginTop: 24 }}
        >
          {action.label}
        </Button>
      )}
    </Flex>
  );
}
