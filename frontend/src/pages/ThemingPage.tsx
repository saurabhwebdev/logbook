import { useState, useEffect } from 'react';
import { Typography, Flex, Button, Input, message, Spin, ColorPicker } from 'antd';
import { SaveOutlined } from '@ant-design/icons';
import { useMutation } from '@tanstack/react-query';
import { themeApi } from '../api/themeApi';
import { useTenantTheme, normalizeColor } from '../contexts/ThemeContext';

const { Text } = Typography;

export default function ThemingPage() {
  const { theme, isLoading, refreshTheme } = useTenantTheme();
  const [logoUrl, setLogoUrl] = useState('');
  const [primaryColor, setPrimaryColor] = useState('#0071e3');
  const [sidebarColor, setSidebarColor] = useState('#ffffff');
  const [sidebarTextColor, setSidebarTextColor] = useState('#6e6e73');

  useEffect(() => {
    if (theme) {
      setLogoUrl(theme.logoUrl || '');
      setPrimaryColor(theme.primaryColor || '#0071e3');
      setSidebarColor(theme.sidebarColor || '#ffffff');
      setSidebarTextColor(theme.sidebarTextColor || '#6e6e73');
    }
  }, [theme]);

  const saveMutation = useMutation({
    mutationFn: themeApi.updateTheme,
    onSuccess: () => {
      message.success('Theme saved successfully.');
      refreshTheme();
    },
    onError: () => message.error('Failed to save theme'),
  });

  const handleSave = () => {
    saveMutation.mutate({
      logoUrl: logoUrl || null,
      primaryColor: normalizeColor(primaryColor),
      sidebarColor: normalizeColor(sidebarColor),
      sidebarTextColor: normalizeColor(sidebarTextColor),
    });
  };

  return (
    <div>
      <Flex align="center" justify="space-between" style={{ marginBottom: 24 }}>
        <h2 style={{ fontSize: 22, fontWeight: 700, color: '#1d1d1f', margin: 0 }}>Theming</h2>
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

          <div style={{ marginBottom: 24 }}>
            <Text style={{ fontSize: 13, fontWeight: 500, color: '#1d1d1f', display: 'block', marginBottom: 8 }}>Sidebar Text Color</Text>
            <Flex align="center" gap={12}>
              <ColorPicker value={sidebarTextColor} onChange={(_, hex) => setSidebarTextColor(hex)} />
              <Input value={sidebarTextColor} onChange={(e) => setSidebarTextColor(e.target.value)} style={{ width: 120, fontFamily: 'monospace' }} />
              <div style={{ width: 80, height: 36, borderRadius: 8, background: sidebarColor, border: '1px solid #e5e5ea', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                <span style={{ color: sidebarTextColor, fontSize: 12, fontWeight: 500 }}>Aa</span>
              </div>
            </Flex>
          </div>

          <div style={{ padding: 16, background: '#f5f5f7', borderRadius: 8 }}>
            <Text style={{ fontSize: 12, color: '#86868b' }}>
              Preview: Changes will take effect immediately after saving. The primary color affects buttons and links. The sidebar color changes the navigation background. The sidebar text color controls menu labels and the tenant name.
            </Text>
          </div>
        </div>
      </Spin>
    </div>
  );
}
