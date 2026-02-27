import { useState, useEffect } from 'react';
import { Typography, Flex, Button, Input, message, Spin, ColorPicker } from 'antd';
import { SaveOutlined } from '@ant-design/icons';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { themeApi } from '../api/themeApi';

const { Text } = Typography;

export default function ThemingPage() {
  const queryClient = useQueryClient();
  const [logoUrl, setLogoUrl] = useState('');
  const [primaryColor, setPrimaryColor] = useState('#0071e3');
  const [sidebarColor, setSidebarColor] = useState('#ffffff');

  const { data, isLoading } = useQuery({
    queryKey: ['tenantTheme'],
    queryFn: themeApi.getTheme,
  });

  useEffect(() => {
    if (data) {
      setLogoUrl(data.logoUrl || '');
      setPrimaryColor(data.primaryColor || '#0071e3');
      setSidebarColor(data.sidebarColor || '#ffffff');
    }
  }, [data]);

  const saveMutation = useMutation({
    mutationFn: themeApi.updateTheme,
    onSuccess: () => {
      message.success('Theme saved successfully.');
      queryClient.invalidateQueries({ queryKey: ['tenantTheme'] });
    },
    onError: () => message.error('Failed to save theme'),
  });

  const handleSave = () => {
    saveMutation.mutate({
      logoUrl: logoUrl || null,
      primaryColor: primaryColor || null,
      sidebarColor: sidebarColor || null,
    });
  };

  return (
    <div>
      <Flex align="center" justify="space-between" style={{ marginBottom: 24 }}>
        <div>
          <h2 style={{ fontSize: 22, fontWeight: 700, color: '#1d1d1f', margin: 0 }}>Theming</h2>
          <Text style={{ fontSize: 13, color: '#86868b' }}>Customize your tenant branding and colors.</Text>
        </div>
        <Button type="primary" icon={<SaveOutlined />} onClick={handleSave} loading={saveMutation.isPending}>
          Save Changes
        </Button>
      </Flex>

      <Spin spinning={isLoading}>
        <div style={{ background: '#ffffff', borderRadius: 12, border: '1px solid #e5e5ea', padding: 24, maxWidth: 600 }}>
          <div style={{ marginBottom: 24 }}>
            <Text style={{ fontSize: 13, fontWeight: 500, color: '#1d1d1f', display: 'block', marginBottom: 8 }}>Logo URL</Text>
            <Input
              value={logoUrl}
              onChange={(e) => setLogoUrl(e.target.value)}
              placeholder="https://example.com/logo.png"
            />
            {logoUrl && (
              <div style={{ marginTop: 12, padding: 16, background: '#f5f5f7', borderRadius: 8, textAlign: 'center' }}>
                <img src={logoUrl} alt="Logo preview" style={{ maxHeight: 48, maxWidth: 200 }} onError={(e) => { (e.target as HTMLImageElement).style.display = 'none'; }} />
              </div>
            )}
          </div>

          <div style={{ marginBottom: 24 }}>
            <Text style={{ fontSize: 13, fontWeight: 500, color: '#1d1d1f', display: 'block', marginBottom: 8 }}>Primary Color</Text>
            <Flex align="center" gap={12}>
              <ColorPicker value={primaryColor} onChange={(_, hex) => setPrimaryColor(hex)} />
              <Input value={primaryColor} onChange={(e) => setPrimaryColor(e.target.value)} style={{ width: 120, fontFamily: 'monospace' }} />
              <div style={{ width: 80, height: 36, borderRadius: 8, background: primaryColor, border: '1px solid #e5e5ea' }} />
            </Flex>
          </div>

          <div style={{ marginBottom: 24 }}>
            <Text style={{ fontSize: 13, fontWeight: 500, color: '#1d1d1f', display: 'block', marginBottom: 8 }}>Sidebar Color</Text>
            <Flex align="center" gap={12}>
              <ColorPicker value={sidebarColor} onChange={(_, hex) => setSidebarColor(hex)} />
              <Input value={sidebarColor} onChange={(e) => setSidebarColor(e.target.value)} style={{ width: 120, fontFamily: 'monospace' }} />
              <div style={{ width: 80, height: 36, borderRadius: 8, background: sidebarColor, border: '1px solid #e5e5ea' }} />
            </Flex>
          </div>

          <div style={{ padding: 16, background: '#f5f5f7', borderRadius: 8 }}>
            <Text style={{ fontSize: 12, color: '#86868b' }}>
              Preview: Changes will take effect immediately after saving. The primary color affects buttons and links. The sidebar color changes the navigation background.
            </Text>
          </div>
        </div>
      </Spin>
    </div>
  );
}
