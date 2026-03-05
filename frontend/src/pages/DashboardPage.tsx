import { useState } from 'react';
import { Row, Col, Typography, Spin, Alert, Flex, Tag, Tabs, Progress } from 'antd';
import {
  UserOutlined,
  TeamOutlined,
  ApartmentOutlined,
  FileSearchOutlined,
  FolderOutlined,
  BarChartOutlined,
  ExperimentOutlined,
  FlagOutlined,
  ApiOutlined,
  GoldOutlined,
  AlertOutlined,
  AuditOutlined,
  ToolOutlined,
  IdcardOutlined,
  ThunderboltOutlined,
  DatabaseOutlined,
  SafetyCertificateOutlined,
  CloudOutlined,
  CompressOutlined,
  GlobalOutlined,
  WarningOutlined,
  CheckCircleOutlined,
  CloseCircleOutlined,
  ClockCircleOutlined,
} from '@ant-design/icons';
import { useQuery } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import dayjs from 'dayjs';
import { useAuth } from '../contexts/AuthContext';
import { useTenantTheme } from '../contexts/ThemeContext';
import { dashboardApi } from '../api/dashboardApi';
import type { RecentActivity, RecentMiningActivity } from '../api/dashboardApi';

const { Text } = Typography;

interface StatCardProps {
  label: string;
  value: number | string;
  icon: React.ReactNode;
  color: string;
  onClick?: () => void;
  subtitle?: string;
}

function StatCard({ label, value, icon, color, onClick, subtitle }: StatCardProps) {
  return (
    <div
      onClick={onClick}
      style={{
        background: '#ffffff',
        borderRadius: 12,
        border: '1px solid #e5e5ea',
        padding: '20px',
        cursor: onClick ? 'pointer' : 'default',
        transition: 'border-color 0.2s, transform 0.15s',
        height: '100%',
      }}
      onMouseEnter={(e) => {
        e.currentTarget.style.borderColor = '#d1d1d6';
        if (onClick) e.currentTarget.style.transform = 'translateY(-1px)';
      }}
      onMouseLeave={(e) => {
        e.currentTarget.style.borderColor = '#e5e5ea';
        e.currentTarget.style.transform = 'translateY(0)';
      }}
    >
      <Flex align="center" justify="space-between" style={{ marginBottom: 12 }}>
        <Text style={{ fontSize: 12, fontWeight: 500, color: '#86868b', textTransform: 'uppercase', letterSpacing: 0.5 }}>
          {label}
        </Text>
        <div
          style={{
            width: 34,
            height: 34,
            borderRadius: 9,
            background: color,
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            fontSize: 15,
            color: '#fff',
          }}
        >
          {icon}
        </div>
      </Flex>
      <div style={{ fontSize: 28, fontWeight: 700, color: '#1d1d1f', lineHeight: 1 }}>
        {value}
      </div>
      {subtitle && (
        <Text style={{ fontSize: 12, color: '#86868b', marginTop: 4, display: 'block' }}>
          {subtitle}
        </Text>
      )}
    </div>
  );
}

interface SectionHeaderProps {
  title: string;
  icon: React.ReactNode;
  color: string;
}

function SectionHeader({ title, icon, color }: SectionHeaderProps) {
  return (
    <Flex align="center" gap={8} style={{ marginBottom: 16, marginTop: 8 }}>
      <div style={{
        width: 28, height: 28, borderRadius: 7, background: color,
        display: 'flex', alignItems: 'center', justifyContent: 'center',
        fontSize: 13, color: '#fff',
      }}>
        {icon}
      </div>
      <Text style={{ fontSize: 15, fontWeight: 600, color: '#1d1d1f' }}>{title}</Text>
    </Flex>
  );
}

const statusColors: Record<string, string> = {
  Open: 'orange',
  Active: 'green',
  Pending: 'blue',
  Closed: 'default',
  Resolved: 'default',
  Draft: 'default',
  Approved: 'green',
  Expired: 'red',
  Completed: 'green',
  Scheduled: 'blue',
  InProgress: 'processing',
  Critical: 'red',
  High: 'red',
  Medium: 'orange',
  Low: 'green',
  Compliant: 'green',
  NonCompliant: 'red',
  PartiallyCompliant: 'orange',
};

function RecentActivityList({ items, emptyText }: { items: RecentMiningActivity[]; emptyText: string }) {
  if (!items || items.length === 0) {
    return <Text style={{ fontSize: 13, color: '#86868b', padding: '12px 0', display: 'block' }}>{emptyText}</Text>;
  }

  return (
    <div style={{ background: '#ffffff', borderRadius: 10, border: '1px solid #e5e5ea', overflow: 'hidden' }}>
      {items.map((item, index) => (
        <div
          key={item.id}
          style={{
            padding: '12px 16px',
            borderBottom: index < items.length - 1 ? '1px solid #f2f2f7' : 'none',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
          }}
        >
          <div style={{ flex: 1, minWidth: 0 }}>
            <Text style={{ fontSize: 13, fontWeight: 500, color: '#1d1d1f', display: 'block' }} ellipsis>
              {item.title}
            </Text>
            <Flex gap={6} style={{ marginTop: 4 }}>
              <Tag color={statusColors[item.status] || 'default'} style={{ fontSize: 11, margin: 0 }}>{item.status}</Tag>
              {item.severity && (
                <Tag color={statusColors[item.severity] || 'default'} style={{ fontSize: 11, margin: 0 }}>{item.severity}</Tag>
              )}
            </Flex>
          </div>
          <Text style={{ fontSize: 11, color: '#86868b', whiteSpace: 'nowrap', marginLeft: 12 }}>
            {dayjs(item.date).format('MMM D')}
          </Text>
        </div>
      ))}
    </div>
  );
}

const actionLabels: Record<string, string> = {
  Created: 'created',
  Updated: 'updated',
  Deleted: 'deleted',
};

export default function DashboardPage() {
  const { user } = useAuth();
  const { theme } = useTenantTheme();
  const primaryColor = theme?.primaryColor || '#0071e3';
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState('mining');

  const { data: platformData, isLoading: platformLoading, isError: platformError } = useQuery({
    queryKey: ['dashboardStats'],
    queryFn: dashboardApi.getStats,
  });

  const { data: miningData, isLoading: miningLoading, isError: miningError } = useQuery({
    queryKey: ['miningDashboardStats'],
    queryFn: dashboardApi.getMiningStats,
  });

  const compliancePercent = miningData && miningData.totalRequirements > 0
    ? Math.round((miningData.compliantCount / miningData.totalRequirements) * 100)
    : 0;

  const equipmentAvailability = miningData && miningData.totalEquipment > 0
    ? Math.round((miningData.operationalEquipment / miningData.totalEquipment) * 100)
    : 0;

  return (
    <div>
      <div style={{ marginBottom: 24 }}>
        <h2 style={{ fontSize: 22, fontWeight: 700, color: '#1d1d1f', margin: 0 }}>
          Dashboard
        </h2>
        <Text style={{ fontSize: 14, color: '#86868b' }}>
          Welcome back, {user?.firstName}. Here's your mining operations overview.
        </Text>
      </div>

      {(platformError || miningError) && (
        <Alert
          message="Some data could not be loaded."
          type="warning"
          showIcon
          style={{ marginBottom: 20, borderRadius: 8 }}
        />
      )}

      <Tabs
        activeKey={activeTab}
        onChange={setActiveTab}
        items={[
          {
            key: 'mining',
            label: 'Mining Operations',
            children: (
              <Spin spinning={miningLoading}>
                {/* Key Metrics Row */}
                <Row gutter={[16, 16]}>
                  <Col xs={24} sm={12} lg={6}>
                    <StatCard
                      label="Mine Sites"
                      value={miningData?.activeMineSites ?? 0}
                      subtitle={`of ${miningData?.totalMineSites ?? 0} total`}
                      icon={<GoldOutlined />}
                      color={primaryColor}
                      onClick={() => navigate('/mine-sites')}
                    />
                  </Col>
                  <Col xs={24} sm={12} lg={6}>
                    <StatCard
                      label="Open Incidents"
                      value={miningData?.openIncidents ?? 0}
                      subtitle={`${miningData?.incidentsThisMonth ?? 0} this month`}
                      icon={<AlertOutlined />}
                      color="#ff3b30"
                      onClick={() => navigate('/safety-incidents')}
                    />
                  </Col>
                  <Col xs={24} sm={12} lg={6}>
                    <StatCard
                      label="Active Permits"
                      value={miningData?.activePermits ?? 0}
                      subtitle={`${miningData?.pendingPermits ?? 0} pending approval`}
                      icon={<SafetyCertificateOutlined />}
                      color="#34c759"
                      onClick={() => navigate('/work-permits')}
                    />
                  </Col>
                  <Col xs={24} sm={12} lg={6}>
                    <StatCard
                      label="Production (Month)"
                      value={`${((miningData?.productionThisMonth ?? 0) / 1000).toFixed(1)}k`}
                      subtitle={`${((miningData?.totalProductionTonnes ?? 0) / 1000).toFixed(1)}k tonnes total`}
                      icon={<DatabaseOutlined />}
                      color="#ff9500"
                      onClick={() => navigate('/production')}
                    />
                  </Col>
                </Row>

                {/* Safety & Inspections */}
                <div style={{ marginTop: 24 }}>
                  <SectionHeader title="Safety & Compliance" icon={<AlertOutlined />} color="#ff3b30" />
                  <Row gutter={[16, 16]}>
                    <Col xs={24} sm={12} lg={4}>
                      <StatCard
                        label="Total Incidents"
                        value={miningData?.totalIncidents ?? 0}
                        icon={<AlertOutlined />}
                        color="#ff3b30"
                        onClick={() => navigate('/safety-incidents')}
                      />
                    </Col>
                    <Col xs={24} sm={12} lg={4}>
                      <StatCard
                        label="Lost Time Days"
                        value={miningData?.lostTimeDays ?? 0}
                        icon={<ClockCircleOutlined />}
                        color="#ff9500"
                      />
                    </Col>
                    <Col xs={24} sm={12} lg={4}>
                      <StatCard
                        label="Open Findings"
                        value={miningData?.openFindings ?? 0}
                        icon={<AuditOutlined />}
                        color="#af52de"
                        onClick={() => navigate('/inspections')}
                      />
                    </Col>
                    <Col xs={24} sm={12} lg={4}>
                      <StatCard
                        label="Overdue Inspections"
                        value={miningData?.overdueInspections ?? 0}
                        icon={<WarningOutlined />}
                        color={miningData?.overdueInspections ? '#ff3b30' : '#34c759'}
                        onClick={() => navigate('/inspections')}
                      />
                    </Col>
                    <Col xs={24} sm={12} lg={4}>
                      <StatCard
                        label="Env. Exceedances"
                        value={miningData?.environmentalExceedances ?? 0}
                        icon={<CloudOutlined />}
                        color="#ff9500"
                        onClick={() => navigate('/environmental')}
                      />
                    </Col>
                    <Col xs={24} sm={12} lg={4}>
                      <StatCard
                        label="Gas Exceedances"
                        value={miningData?.gasExceedances ?? 0}
                        icon={<CompressOutlined />}
                        color={miningData?.gasExceedances ? '#ff3b30' : '#34c759'}
                        onClick={() => navigate('/ventilation')}
                      />
                    </Col>
                  </Row>
                </div>

                {/* Equipment & Personnel */}
                <div style={{ marginTop: 24 }}>
                  <SectionHeader title="Equipment & Personnel" icon={<ToolOutlined />} color="#0071e3" />
                  <Row gutter={[16, 16]}>
                    <Col xs={24} sm={12} lg={6}>
                      <div style={{
                        background: '#ffffff', borderRadius: 12, border: '1px solid #e5e5ea', padding: '20px',
                        cursor: 'pointer',
                      }} onClick={() => navigate('/equipment')}>
                        <Text style={{ fontSize: 12, fontWeight: 500, color: '#86868b', textTransform: 'uppercase', letterSpacing: 0.5, display: 'block', marginBottom: 8 }}>
                          Equipment Availability
                        </Text>
                        <Progress
                          percent={equipmentAvailability}
                          strokeColor={equipmentAvailability >= 80 ? '#34c759' : equipmentAvailability >= 60 ? '#ff9500' : '#ff3b30'}
                          format={(p) => `${p}%`}
                        />
                        <Flex justify="space-between" style={{ marginTop: 8 }}>
                          <Text style={{ fontSize: 11, color: '#86868b' }}>{miningData?.operationalEquipment ?? 0} operational</Text>
                          <Text style={{ fontSize: 11, color: '#86868b' }}>{miningData?.underMaintenanceEquipment ?? 0} in maintenance</Text>
                        </Flex>
                      </div>
                    </Col>
                    <Col xs={24} sm={12} lg={6}>
                      <div style={{
                        background: '#ffffff', borderRadius: 12, border: '1px solid #e5e5ea', padding: '20px',
                        cursor: 'pointer',
                      }} onClick={() => navigate('/compliance')}>
                        <Text style={{ fontSize: 12, fontWeight: 500, color: '#86868b', textTransform: 'uppercase', letterSpacing: 0.5, display: 'block', marginBottom: 8 }}>
                          Compliance Rate
                        </Text>
                        <Progress
                          percent={compliancePercent}
                          strokeColor={compliancePercent >= 80 ? '#34c759' : compliancePercent >= 60 ? '#ff9500' : '#ff3b30'}
                          format={(p) => `${p}%`}
                        />
                        <Flex justify="space-between" style={{ marginTop: 8 }}>
                          <Text style={{ fontSize: 11, color: '#34c759' }}><CheckCircleOutlined /> {miningData?.compliantCount ?? 0}</Text>
                          <Text style={{ fontSize: 11, color: '#ff3b30' }}><CloseCircleOutlined /> {miningData?.nonCompliantCount ?? 0}</Text>
                          <Text style={{ fontSize: 11, color: '#86868b' }}>{miningData?.overdueAudits ?? 0} overdue</Text>
                        </Flex>
                      </div>
                    </Col>
                    <Col xs={24} sm={12} lg={4}>
                      <StatCard
                        label="Active Personnel"
                        value={miningData?.activePersonnel ?? 0}
                        subtitle={`of ${miningData?.totalPersonnel ?? 0} total`}
                        icon={<IdcardOutlined />}
                        color="#0071e3"
                        onClick={() => navigate('/personnel')}
                      />
                    </Col>
                    <Col xs={24} sm={12} lg={4}>
                      <StatCard
                        label="Expiring Certs"
                        value={miningData?.expiringCertifications ?? 0}
                        subtitle="within 30 days"
                        icon={<WarningOutlined />}
                        color={miningData?.expiringCertifications ? '#ff9500' : '#34c759'}
                        onClick={() => navigate('/personnel')}
                      />
                    </Col>
                    <Col xs={24} sm={12} lg={4}>
                      <StatCard
                        label="Overdue Maint."
                        value={miningData?.overdueMaintenanceCount ?? 0}
                        icon={<ToolOutlined />}
                        color={miningData?.overdueMaintenanceCount ? '#ff3b30' : '#34c759'}
                        onClick={() => navigate('/equipment')}
                      />
                    </Col>
                  </Row>
                </div>

                {/* Operations */}
                <div style={{ marginTop: 24 }}>
                  <SectionHeader title="Operations" icon={<DatabaseOutlined />} color="#ff9500" />
                  <Row gutter={[16, 16]}>
                    <Col xs={24} sm={12} lg={4}>
                      <StatCard
                        label="Blasts (Month)"
                        value={miningData?.blastsThisMonth ?? 0}
                        subtitle={`${miningData?.totalBlasts ?? 0} total`}
                        icon={<ThunderboltOutlined />}
                        color="#ff9500"
                        onClick={() => navigate('/blasting')}
                      />
                    </Col>
                    <Col xs={24} sm={12} lg={4}>
                      <StatCard
                        label="Explosives Used"
                        value={`${((miningData?.totalExplosivesUsedKg ?? 0) / 1000).toFixed(1)}t`}
                        icon={<ThunderboltOutlined />}
                        color="#ff3b30"
                      />
                    </Col>
                    <Col xs={24} sm={12} lg={4}>
                      <StatCard
                        label="Dispatches (Month)"
                        value={miningData?.dispatchThisMonth ?? 0}
                        subtitle={`${miningData?.dispatchCount ?? 0} total`}
                        icon={<DatabaseOutlined />}
                        color="#0071e3"
                        onClick={() => navigate('/production')}
                      />
                    </Col>
                    <Col xs={24} sm={12} lg={4}>
                      <StatCard
                        label="Expired Permits"
                        value={miningData?.expiredPermits ?? 0}
                        icon={<SafetyCertificateOutlined />}
                        color={miningData?.expiredPermits ? '#ff3b30' : '#34c759'}
                        onClick={() => navigate('/work-permits')}
                      />
                    </Col>
                    <Col xs={24} sm={12} lg={4}>
                      <StatCard
                        label="Unstable Areas"
                        value={miningData?.unstableAssessments ?? 0}
                        icon={<GlobalOutlined />}
                        color={miningData?.unstableAssessments ? '#ff3b30' : '#34c759'}
                        onClick={() => navigate('/geotechnical')}
                      />
                    </Col>
                    <Col xs={24} sm={12} lg={4}>
                      <StatCard
                        label="Pending Surveys"
                        value={miningData?.pendingSurveys ?? 0}
                        icon={<GlobalOutlined />}
                        color="#af52de"
                        onClick={() => navigate('/geotechnical')}
                      />
                    </Col>
                  </Row>
                </div>

                {/* Recent Activity Lists */}
                <div style={{ marginTop: 24 }}>
                  <Row gutter={[16, 16]}>
                    <Col xs={24} lg={8}>
                      <Text style={{ fontSize: 14, fontWeight: 600, color: '#1d1d1f', display: 'block', marginBottom: 12 }}>
                        Recent Safety Incidents
                      </Text>
                      <RecentActivityList
                        items={miningData?.recentSafetyIncidents ?? []}
                        emptyText="No recent safety incidents"
                      />
                    </Col>
                    <Col xs={24} lg={8}>
                      <Text style={{ fontSize: 14, fontWeight: 600, color: '#1d1d1f', display: 'block', marginBottom: 12 }}>
                        Recent Inspections
                      </Text>
                      <RecentActivityList
                        items={miningData?.recentInspections ?? []}
                        emptyText="No recent inspections"
                      />
                    </Col>
                    <Col xs={24} lg={8}>
                      <Text style={{ fontSize: 14, fontWeight: 600, color: '#1d1d1f', display: 'block', marginBottom: 12 }}>
                        Recent Work Permits
                      </Text>
                      <RecentActivityList
                        items={miningData?.recentPermits ?? []}
                        emptyText="No recent work permits"
                      />
                    </Col>
                  </Row>
                </div>
              </Spin>
            ),
          },
          {
            key: 'platform',
            label: 'Platform Overview',
            children: (
              <Spin spinning={platformLoading}>
                <Row gutter={[16, 16]}>
                  <Col xs={24} sm={12} lg={6}>
                    <StatCard label="Users" value={platformData?.userCount ?? 0} icon={<UserOutlined />} color={primaryColor} onClick={() => navigate('/users')} />
                  </Col>
                  <Col xs={24} sm={12} lg={6}>
                    <StatCard label="Roles" value={platformData?.roleCount ?? 0} icon={<TeamOutlined />} color="#34c759" onClick={() => navigate('/roles')} />
                  </Col>
                  <Col xs={24} sm={12} lg={6}>
                    <StatCard label="Departments" value={platformData?.departmentCount ?? 0} icon={<ApartmentOutlined />} color="#ff9500" onClick={() => navigate('/departments')} />
                  </Col>
                  <Col xs={24} sm={12} lg={6}>
                    <StatCard label="Audit Events" value={platformData?.auditLogCount ?? 0} icon={<FileSearchOutlined />} color="#af52de" onClick={() => navigate('/audit-logs')} />
                  </Col>
                  <Col xs={24} sm={12} lg={6}>
                    <StatCard label="Files" value={platformData?.fileCount ?? 0} icon={<FolderOutlined />} color={primaryColor} onClick={() => navigate('/files')} />
                  </Col>
                  <Col xs={24} sm={12} lg={6}>
                    <StatCard label="Reports" value={platformData?.reportCount ?? 0} icon={<BarChartOutlined />} color="#ff3b30" onClick={() => navigate('/reports')} />
                  </Col>
                  <Col xs={24} sm={12} lg={6}>
                    <StatCard label="Active Tasks" value={platformData?.activeTaskCount ?? 0} icon={<ExperimentOutlined />} color="#ff9500" onClick={() => navigate('/demo-tasks')} />
                  </Col>
                  <Col xs={24} sm={12} lg={6}>
                    <StatCard label="Feature Flags On" value={platformData?.enabledFeatureFlagCount ?? 0} icon={<FlagOutlined />} color="#34c759" onClick={() => navigate('/feature-flags')} />
                  </Col>
                  <Col xs={24} sm={12} lg={6}>
                    <StatCard label="Active API Keys" value={platformData?.activeApiKeyCount ?? 0} icon={<ApiOutlined />} color="#af52de" onClick={() => navigate('/api-integration')} />
                  </Col>
                </Row>

                {/* Recent Activity */}
                {(platformData?.recentActivity ?? []).length > 0 && (
                  <div style={{ marginTop: 28 }}>
                    <h3 style={{ fontSize: 16, fontWeight: 600, color: '#1d1d1f', marginBottom: 16 }}>Recent Activity</h3>
                    <div style={{ background: '#ffffff', borderRadius: 12, border: '1px solid #e5e5ea', overflow: 'hidden' }}>
                      {(platformData?.recentActivity ?? []).map((activity: RecentActivity, index: number) => (
                        <div
                          key={index}
                          style={{
                            padding: '14px 20px',
                            borderBottom: index < (platformData?.recentActivity ?? []).length - 1 ? '1px solid #f2f2f7' : 'none',
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'space-between',
                          }}
                        >
                          <div>
                            <span style={{ fontSize: 13, color: '#1d1d1f', fontWeight: 500 }}>
                              {activity.entityName}
                            </span>
                            <span style={{ fontSize: 13, color: '#86868b', marginLeft: 6 }}>
                              {actionLabels[activity.action] || activity.action}
                            </span>
                            <code style={{ fontSize: 11, background: '#f5f5f7', padding: '1px 6px', borderRadius: 4, marginLeft: 8, color: '#6e6e73' }}>
                              {activity.entityId.length > 12 ? activity.entityId.substring(0, 8) + '...' : activity.entityId}
                            </code>
                          </div>
                          <Text style={{ fontSize: 12, color: '#86868b' }}>
                            {dayjs(activity.timestamp).format('MMM D, h:mm A')}
                          </Text>
                        </div>
                      ))}
                    </div>
                  </div>
                )}
              </Spin>
            ),
          },
        ]}
      />
    </div>
  );
}
