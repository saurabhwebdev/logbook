import { useState, useCallback, useRef } from 'react';
import { AutoComplete, Input, Flex, Typography, Spin } from 'antd';
import {
  SearchOutlined,
  UserOutlined,
  TeamOutlined,
  ApartmentOutlined,
  FolderOutlined,
  BarChartOutlined,
  ExperimentOutlined,
  QuestionCircleOutlined,
} from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { searchApi, type SearchResult, type GlobalSearchResult } from '../api/searchApi';
import { useTenantTheme } from '../contexts/ThemeContext';

const { Text } = Typography;

interface SearchOption {
  value: string;
  label: React.ReactNode;
  url: string;
}

const entityIcons: Record<string, React.ReactNode> = {
  User: <UserOutlined />,
  Role: <TeamOutlined />,
  Department: <ApartmentOutlined />,
  File: <FolderOutlined />,
  Report: <BarChartOutlined />,
  DemoTask: <ExperimentOutlined />,
  HelpArticle: <QuestionCircleOutlined />,
};

const entityLabels: Record<string, string> = {
  User: 'Users',
  Role: 'Roles',
  Department: 'Departments',
  File: 'Files',
  Report: 'Reports',
  DemoTask: 'Tasks',
  HelpArticle: 'Help Articles',
};

export default function GlobalSearch() {
  const [searchValue, setSearchValue] = useState('');
  const [options, setOptions] = useState<SearchOption[]>([]);
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const { theme: tenantTheme } = useTenantTheme();
  const primaryColor = tenantTheme?.primaryColor || '#0071e3';
  const debounceTimerRef = useRef<number | null>(null);

  const buildOptions = (results: GlobalSearchResult): SearchOption[] => {
    const allOptions: SearchOption[] = [];

    const addGroupResults = (
      items: SearchResult[],
      entityType: string
    ) => {
      if (items.length === 0) return;

      // Add group header
      allOptions.push({
        value: `header-${entityType}`,
        label: (
          <div
            style={{
              fontSize: 11,
              fontWeight: 600,
              color: '#86868b',
              textTransform: 'uppercase',
              letterSpacing: 0.5,
              padding: '8px 0 4px',
              pointerEvents: 'none',
            }}
          >
            {entityLabels[entityType]}
          </div>
        ),
        url: '',
      });

      // Add items
      items.forEach((item) => {
        allOptions.push({
          value: `${entityType}-${item.entityId}`,
          label: (
            <Flex gap={12} align="center" style={{ padding: '4px 0' }}>
              <div
                style={{
                  fontSize: 16,
                  color: primaryColor,
                  display: 'flex',
                  alignItems: 'center',
                }}
              >
                {entityIcons[entityType]}
              </div>
              <div style={{ flex: 1, minWidth: 0 }}>
                <div
                  style={{
                    fontSize: 13,
                    fontWeight: 500,
                    color: '#1d1d1f',
                    overflow: 'hidden',
                    textOverflow: 'ellipsis',
                    whiteSpace: 'nowrap',
                  }}
                >
                  {item.title}
                </div>
                {item.description && (
                  <div
                    style={{
                      fontSize: 12,
                      color: '#86868b',
                      overflow: 'hidden',
                      textOverflow: 'ellipsis',
                      whiteSpace: 'nowrap',
                    }}
                  >
                    {item.description}
                  </div>
                )}
              </div>
            </Flex>
          ),
          url: item.url,
        });
      });
    };

    addGroupResults(results.users, 'User');
    addGroupResults(results.roles, 'Role');
    addGroupResults(results.departments, 'Department');
    addGroupResults(results.files, 'File');
    addGroupResults(results.reports, 'Report');
    addGroupResults(results.demoTasks, 'DemoTask');
    addGroupResults(results.helpArticles, 'HelpArticle');

    return allOptions;
  };

  const handleSearch = useCallback(
    async (value: string) => {
      if (debounceTimerRef.current) {
        clearTimeout(debounceTimerRef.current);
      }

      if (!value.trim()) {
        setOptions([]);
        return;
      }

      debounceTimerRef.current = window.setTimeout(async () => {
        setLoading(true);
        try {
          const results = await searchApi.globalSearch(value);
          const searchOptions = buildOptions(results);

          if (searchOptions.length === 0) {
            setOptions([
              {
                value: 'no-results',
                label: (
                  <Text
                    style={{
                      fontSize: 13,
                      color: '#86868b',
                      padding: '12px 0',
                      display: 'block',
                      textAlign: 'center',
                    }}
                  >
                    No results found
                  </Text>
                ),
                url: '',
              },
            ]);
          } else {
            setOptions(searchOptions);
          }
        } catch (error) {
          console.error('Search error:', error);
          setOptions([]);
        } finally {
          setLoading(false);
        }
      }, 500);
    },
    [primaryColor]
  );

  const handleSelect = (value: string, option: SearchOption) => {
    if (option.url && !value.startsWith('header-') && value !== 'no-results') {
      navigate(option.url);
      setSearchValue('');
      setOptions([]);
    }
  };

  return (
    <AutoComplete
      value={searchValue}
      options={options}
      onSearch={handleSearch}
      onSelect={handleSelect}
      onChange={setSearchValue}
      style={{ width: 280 }}
      classNames={{ popup: { root: 'global-search-dropdown' } }}
      notFoundContent={loading ? <Spin size="small" /> : null}
    >
      <Input
        prefix={
          loading ? (
            <Spin size="small" />
          ) : (
            <SearchOutlined style={{ color: '#86868b' }} />
          )
        }
        placeholder="Search..."
        style={{
          borderRadius: 8,
          fontSize: 13,
          height: 36,
          background: '#f5f5f7',
          border: 'none',
        }}
      />
    </AutoComplete>
  );
}
