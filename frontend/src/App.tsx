import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ConfigProvider } from 'antd';
import { AuthProvider } from './contexts/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';
import MainLayout from './layouts/MainLayout';
import LoginPage from './pages/LoginPage';
import DashboardPage from './pages/DashboardPage';
import UsersPage from './pages/UsersPage';
import UserFormPage from './pages/UserFormPage';
import RolesPage from './pages/RolesPage';
import DepartmentsPage from './pages/DepartmentsPage';
import AuditLogsPage from './pages/AuditLogsPage';
import TenantsPage from './pages/TenantsPage';
import SettingsPage from './pages/SettingsPage';
import FeatureFlagsPage from './pages/FeatureFlagsPage';
import NotificationsPage from './pages/NotificationsPage';
import StateMachinePage from './pages/StateMachinePage';
import FilesPage from './pages/FilesPage';
import ReportsPage from './pages/ReportsPage';
import ApiIntegrationPage from './pages/ApiIntegrationPage';
import DemoTasksPage from './pages/DemoTasksPage';
import ThemingPage from './pages/ThemingPage';
import HelpPage from './pages/HelpPage';
import HelpArticleViewPage from './pages/HelpArticleViewPage';
import HelpFormPage from './pages/HelpFormPage';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      retry: 1,
      staleTime: 30_000,
    },
  },
});

export default function App() {
  return (
    <ConfigProvider
      theme={{
        token: {
          fontFamily: "'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif",
          colorPrimary: '#0071e3',
          colorBgContainer: '#ffffff',
          colorBgLayout: '#f8f9fa',
          borderRadius: 8,
          colorBorder: '#e5e5ea',
          colorBorderSecondary: '#f2f2f7',
          colorText: '#1d1d1f',
          colorTextSecondary: '#6e6e73',
          colorTextTertiary: '#86868b',
          fontSize: 14,
          controlHeight: 38,
        },
        components: {
          Menu: {
            itemBg: 'transparent',
            itemSelectedBg: '#f0f5ff',
            itemSelectedColor: '#0071e3',
            itemHoverBg: '#f5f5f7',
            itemColor: '#6e6e73',
            iconSize: 16,
            itemBorderRadius: 8,
            itemMarginInline: 8,
            subMenuItemBg: 'transparent',
          },
          Button: {
            primaryShadow: 'none',
            defaultShadow: 'none',
          },
          Table: {
            headerBg: 'transparent',
            rowHoverBg: '#f8f9fa',
            borderColor: '#f2f2f7',
          },
          Card: {
            paddingLG: 24,
          },
        },
      }}
    >
      <QueryClientProvider client={queryClient}>
        <BrowserRouter>
          <AuthProvider>
            <Routes>
              <Route path="/login" element={<LoginPage />} />
              <Route element={<ProtectedRoute />}>
                <Route element={<MainLayout />}>
                  <Route index element={<DashboardPage />} />
                  <Route path="users" element={<UsersPage />} />
                  <Route path="users/new" element={<UserFormPage />} />
                  <Route path="users/:id/edit" element={<UserFormPage />} />
                  <Route path="roles" element={<RolesPage />} />
                  <Route path="departments" element={<DepartmentsPage />} />
                  <Route path="audit-logs" element={<AuditLogsPage />} />
                  <Route path="tenants" element={<TenantsPage />} />
                  <Route path="settings" element={<SettingsPage />} />
                  <Route path="feature-flags" element={<FeatureFlagsPage />} />
                  <Route path="notifications" element={<NotificationsPage />} />
                  <Route path="state-machine" element={<StateMachinePage />} />
                  <Route path="files" element={<FilesPage />} />
                  <Route path="reports" element={<ReportsPage />} />
                  <Route path="api-integration" element={<ApiIntegrationPage />} />
                  <Route path="demo-tasks" element={<DemoTasksPage />} />
                  <Route path="theming" element={<ThemingPage />} />
                  <Route path="help" element={<HelpPage />} />
                  <Route path="help/new" element={<HelpFormPage />} />
                  <Route path="help/:slug" element={<HelpArticleViewPage />} />
                  <Route path="help/:id/edit" element={<HelpFormPage />} />
                </Route>
              </Route>
            </Routes>
          </AuthProvider>
        </BrowserRouter>
      </QueryClientProvider>
    </ConfigProvider>
  );
}
