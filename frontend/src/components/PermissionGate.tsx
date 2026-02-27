import type { ReactNode } from 'react';
import { useAuth } from '../contexts/AuthContext';

interface PermissionGateProps {
  /** Single permission to check */
  permission?: string;
  /** Multiple permissions to check */
  permissions?: string[];
  /** If true, the user must have ALL listed permissions; otherwise ANY permission suffices */
  requireAll?: boolean;
  /** Content to render if the user lacks the required permission(s) */
  fallback?: ReactNode;
  children: ReactNode;
}

export default function PermissionGate({
  permission,
  permissions,
  requireAll = false,
  fallback = null,
  children,
}: PermissionGateProps) {
  const { user, hasPermission, hasAnyPermission } = useAuth();

  if (!user) {
    return <>{fallback}</>;
  }

  // Single permission check
  if (permission) {
    if (!hasPermission(permission)) {
      return <>{fallback}</>;
    }
  }

  // Multiple permissions check
  if (permissions && permissions.length > 0) {
    if (requireAll) {
      // User must have ALL specified permissions
      const hasAll = permissions.every((p) => hasPermission(p));
      if (!hasAll) {
        return <>{fallback}</>;
      }
    } else {
      // User must have ANY of the specified permissions
      if (!hasAnyPermission(permissions)) {
        return <>{fallback}</>;
      }
    }
  }

  return <>{children}</>;
}
