import { createContext, useContext, useMemo, type ReactNode } from 'react';
import { useQuery, useQueryClient } from '@tanstack/react-query';
import { themeApi, type TenantTheme } from '../api/themeApi';

/** Convert any CSS color string to #rrggbb hex. Handles #hex, bare hex, rgb(). */
export function normalizeColor(raw: string | null | undefined): string | null {
  if (!raw) return null;
  const s = raw.trim();
  if (!s) return null;

  // Already proper #hex
  if (/^#[0-9a-fA-F]{6}$/.test(s)) return s.toLowerCase();
  if (/^#[0-9a-fA-F]{3}$/.test(s)) {
    // Expand shorthand #abc → #aabbcc
    const r = s[1], g = s[2], b = s[3];
    return `#${r}${r}${g}${g}${b}${b}`.toLowerCase();
  }

  // Bare hex without #  (e.g. "13c4a3")
  if (/^[0-9a-fA-F]{6}$/.test(s)) return `#${s.toLowerCase()}`;
  if (/^[0-9a-fA-F]{3}$/.test(s)) {
    const r = s[0], g = s[1], b = s[2];
    return `#${r}${r}${g}${g}${b}${b}`.toLowerCase();
  }

  // rgb(r, g, b) or rgb(r,g,b)
  const rgbMatch = s.match(/^rgba?\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)/);
  if (rgbMatch) {
    const toHex = (n: number) => Math.max(0, Math.min(255, n)).toString(16).padStart(2, '0');
    return `#${toHex(+rgbMatch[1])}${toHex(+rgbMatch[2])}${toHex(+rgbMatch[3])}`;
  }

  // Fallback: return as-is (could be a named color like "red")
  return s;
}

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
    staleTime: Infinity,
    retry: 1,
  });

  const refreshTheme = () => {
    queryClient.invalidateQueries({ queryKey: ['tenantTheme'] });
  };

  // Normalize colors so the rest of the app always gets #hex values
  const normalizedTheme = useMemo<TenantTheme | null>(() => {
    if (!data) return null;
    return {
      ...data,
      primaryColor: normalizeColor(data.primaryColor),
      sidebarColor: normalizeColor(data.sidebarColor),
    };
  }, [data]);

  const value = useMemo(
    () => ({ theme: normalizedTheme, isLoading, refreshTheme }),
    [normalizedTheme, isLoading]
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
