import type { ReactNode } from 'react';
import { Flex, Card, Typography } from 'antd';
import { SafetyCertificateOutlined } from '@ant-design/icons';

const { Title, Text } = Typography;

interface AuthLayoutProps {
  children: ReactNode;
}

export default function AuthLayout({ children }: AuthLayoutProps) {
  return (
    <Flex
      vertical
      align="center"
      justify="center"
      style={{
        minHeight: '100vh',
        background: 'linear-gradient(135deg, #001529 0%, #003a70 100%)',
      }}
    >
      <Flex
        vertical
        align="center"
        style={{ marginBottom: 24 }}
      >
        <SafetyCertificateOutlined
          style={{ fontSize: 48, color: '#ffffff', marginBottom: 8 }}
        />
        <Title level={2} style={{ color: '#ffffff', margin: 0 }}>
          CoreEngine
        </Title>
        <Text style={{ color: 'rgba(255, 255, 255, 0.65)', fontSize: 14 }}>
          Enterprise Administration
        </Text>
      </Flex>
      <Card
        style={{
          width: 400,
          maxWidth: '90vw',
          borderRadius: 8,
          boxShadow: '0 8px 24px rgba(0, 0, 0, 0.2)',
        }}
      >
        {children}
      </Card>
    </Flex>
  );
}
