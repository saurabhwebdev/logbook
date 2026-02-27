import { createContext, useContext, useMemo, type ReactNode } from 'react';
import { useQuery, useQueryClient } from '@tanstack/react-query';
import { themeApi, type TenantTheme } from '../api/themeApi';

interface ThemeContextType {
  theme: TenantTheme | null;
  isLoading: boolean;
  refreshTheme: () => void;
}

const ThemeContext = createContext<ThemeContextType | undefined>(undefined);

export function ThemeProvider({ children }: { children: ReactNode }) {
  const queryClient = useQueryClient();

  const { data, isLoading } = useQuery({
    queryKey: ['tenantTheme'],
    queryFn: themeApi.getTheme,
    staleTime: 0,
    retry: 1,
  });

  const refreshTheme = () => {
    queryClient.invalidateQueries({ queryKey: ['tenantTheme'] });
  };

  const value = useMemo(
    () => ({ theme: data ?? null, isLoading, refreshTheme }),
    [data, isLoading]
  );

  return <ThemeContext.Provider value={value}>{children}</ThemeContext.Provider>;
}

export function useTenantTheme(): ThemeContextType {
  const context = useContext(ThemeContext);
  if (context === undefined) {
    throw new Error('useTenantTheme must be used within a ThemeProvider');
  }
  return context;
}
