import { Avatar } from 'antd';
import { UserOutlined } from '@ant-design/icons';
import { useTenantTheme } from '../contexts/ThemeContext';

interface UserAvatarProps {
  firstName?: string;
  lastName?: string;
  profilePhotoUrl?: string | null;
  size?: number;
  style?: React.CSSProperties;
}

export default function UserAvatar({
  firstName,
  lastName,
  profilePhotoUrl,
  size = 32,
  style = {},
}: UserAvatarProps) {
  const { theme } = useTenantTheme();
  const primaryColor = theme?.primaryColor || '#0071e3';

  // If we have a profile photo, show it
  if (profilePhotoUrl) {
    return (
      <Avatar
        size={size}
        src={profilePhotoUrl}
        style={{
          border: '2px solid #f5f5f7',
          ...style,
        }}
      />
    );
  }

  // If we have first and last name, show initials
  if (firstName && lastName) {
    return (
      <Avatar
        size={size}
        style={{
          background: primaryColor,
          fontSize: size * 0.4,
          fontWeight: 600,
          ...style,
        }}
      >
        {firstName[0]}{lastName[0]}
      </Avatar>
    );
  }

  // Fallback to user icon
  return (
    <Avatar
      size={size}
      icon={<UserOutlined />}
      style={{
        background: '#86868b',
        ...style,
      }}
    />
  );
}
