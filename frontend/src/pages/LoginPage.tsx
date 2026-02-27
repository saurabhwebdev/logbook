import { useState } from 'react';
import { Form, Input, Button, Checkbox, Typography, message, Flex } from 'antd';
import { MailOutlined, LockOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { AxiosError } from 'axios';
import AuthLayout from '../layouts/AuthLayout';
import { useAuth } from '../contexts/AuthContext';

const { Text } = Typography;

interface LoginFormValues {
  email: string;
  password: string;
  remember: boolean;
}

export default function LoginPage() {
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  const onFinish = async (values: LoginFormValues) => {
    setLoading(true);
    try {
      await login(values.email, values.password);
      message.success('Welcome back!');
      navigate('/');
    } catch (error) {
      if (error instanceof AxiosError && error.response?.data) {
        const data = error.response.data;
        const errorMessage =
          typeof data === 'string'
            ? data
            : data.message || data.title || 'Login failed';
        message.error(errorMessage);
      } else {
        message.error('An unexpected error occurred. Please try again.');
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <AuthLayout>
      <div style={{ marginBottom: 32 }}>
        <h2 style={{ fontSize: 24, fontWeight: 700, color: '#1d1d1f', marginBottom: 8 }}>
          Welcome back
        </h2>
        <Text style={{ fontSize: 14, color: '#86868b' }}>
          Enter your credentials to access the admin panel.
        </Text>
      </div>

      <Form<LoginFormValues>
        name="login"
        initialValues={{ remember: true }}
        onFinish={onFinish}
        layout="vertical"
        size="large"
        requiredMark={false}
      >
        <Form.Item
          name="email"
          label={<Text style={{ fontWeight: 500, fontSize: 13, color: '#1d1d1f' }}>Email</Text>}
          rules={[
            { required: true, message: 'Please enter your email' },
            { type: 'email', message: 'Please enter a valid email' },
          ]}
        >
          <Input
            prefix={<MailOutlined style={{ color: '#86868b' }} />}
            placeholder="admin@coreengine.local"
            autoComplete="email"
          />
        </Form.Item>

        <Form.Item
          name="password"
          label={<Text style={{ fontWeight: 500, fontSize: 13, color: '#1d1d1f' }}>Password</Text>}
          rules={[{ required: true, message: 'Please enter your password' }]}
        >
          <Input.Password
            prefix={<LockOutlined style={{ color: '#86868b' }} />}
            placeholder="Enter your password"
            autoComplete="current-password"
          />
        </Form.Item>

        <Flex justify="space-between" align="center" style={{ marginBottom: 24 }}>
          <Form.Item name="remember" valuePropName="checked" noStyle>
            <Checkbox>
              <Text style={{ fontSize: 13, color: '#6e6e73' }}>Remember me</Text>
            </Checkbox>
          </Form.Item>
        </Flex>

        <Form.Item style={{ marginBottom: 0 }}>
          <Button
            type="primary"
            htmlType="submit"
            loading={loading}
            block
            style={{ height: 42, fontWeight: 600, fontSize: 14 }}
          >
            Sign in
          </Button>
        </Form.Item>
      </Form>
    </AuthLayout>
  );
}
