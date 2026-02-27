import type { ReactNode } from 'react';
import { Flex, Typography } from 'antd';

const { Text } = Typography;

interface AuthLayoutProps {
  children: ReactNode;
}

export default function AuthLayout({ children }: AuthLayoutProps) {
  return (
    <Flex
      style={{
        minHeight: '100vh',
        background: '#f8f9fa',
      }}
    >
      {/* Left panel — branding */}
      <Flex
        vertical
        justify="center"
        style={{
          width: '45%',
          minHeight: '100vh',
          background: 'linear-gradient(160deg, #0071e3, #00a1ff)',
          padding: '60px 48px',
          position: 'relative',
          overflow: 'hidden',
        }}
      >
        {/* Decorative circles */}
        <div
          style={{
            position: 'absolute',
            top: -120,
            right: -80,
            width: 400,
            height: 400,
            borderRadius: '50%',
            background: 'rgba(255,255,255,0.06)',
          }}
        />
        <div
          style={{
            position: 'absolute',
            bottom: -100,
            left: -60,
            width: 300,
            height: 300,
            borderRadius: '50%',
            background: 'rgba(255,255,255,0.04)',
          }}
        />

        <div style={{ position: 'relative', zIndex: 1 }}>
          {/* Logo */}
          <Flex align="center" gap={12} style={{ marginBottom: 48 }}>
            <div
              style={{
                width: 40,
                height: 40,
                borderRadius: 10,
                background: 'rgba(255,255,255,0.2)',
                backdropFilter: 'blur(10px)',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                color: '#fff',
                fontWeight: 700,
                fontSize: 16,
              }}
            >
              CE
            </div>
            <Text style={{ color: '#fff', fontWeight: 700, fontSize: 20, letterSpacing: -0.3 }}>
              CoreEngine
            </Text>
          </Flex>

          <div style={{ color: '#fff' }}>
            <h1
              style={{
                fontSize: 36,
                fontWeight: 700,
                lineHeight: 1.2,
                marginBottom: 16,
                letterSpacing: -0.5,
              }}
            >
              Enterprise platform,{' '}
              <span style={{ opacity: 0.8 }}>built for scale.</span>
            </h1>
            <p style={{ fontSize: 16, lineHeight: 1.6, opacity: 0.8, maxWidth: 400 }}>
              Multi-tenant architecture with role-based access, audit logging, and workflow automation — all in one framework.
            </p>
          </div>
        </div>
      </Flex>

      {/* Right panel — form */}
      <Flex
        vertical
        align="center"
        justify="center"
        style={{
          flex: 1,
          padding: '48px',
        }}
      >
        <div style={{ width: '100%', maxWidth: 400 }}>
          {children}
        </div>
      </Flex>
    </Flex>
  );
}
