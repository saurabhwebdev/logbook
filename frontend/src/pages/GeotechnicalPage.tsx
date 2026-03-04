import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Table, Button, Modal, Form, Input, Select, DatePicker, Tag, Drawer, Tabs,
  Typography, Flex, Switch, InputNumber, message, Descriptions, Popconfirm,
} from 'antd';
import { PlusOutlined, EyeOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import { geotechnicalApi } from '../api/geotechnicalApi';
import { mineSitesApi } from '../api/mineSitesApi';
import type { GeotechnicalAssessment, SurveyRecord } from '../types';
import type {
  CreateGeotechnicalAssessmentRequest, UpdateGeotechnicalAssessmentRequest,
  CreateSurveyRecordRequest, UpdateSurveyRecordRequest,
} from '../api/geotechnicalApi';

const { Title, Text } = Typography;
const { TextArea } = Input;

const assessmentTypes = ['SlopeStability', 'RockMassClassification', 'GroundCondition', 'SubsidenceMonitoring', 'WaterTable', 'FoundationAssessment', 'Other'];
const groundConditions = ['Good', 'Fair', 'Poor', 'VeryPoor', 'Critical'];
const stabilityStatuses = ['Stable', 'Marginal', 'Unstable', 'Critical'];
const recordStatuses = ['Draft', 'Reviewed', 'Approved'];
const surveyTypes = ['Boundary', 'Topographic', 'Underground', 'Stockpile', 'Pit', 'AsBuilt', 'Monitoring', 'Other'];

const stabilityColors: Record<string, string> = {
  Stable: '#34c759', Marginal: '#ff9500', Unstable: '#ff3b30', Critical: '#af52de',
};
const statusColors: Record<string, string> = {
  Draft: '#86868b', Reviewed: '#0071e3', Approved: '#34c759',
};
const assessmentTypeColors: Record<string, string> = {
  SlopeStability: '#ff9500', RockMassClassification: '#0071e3', GroundCondition: '#8b6914',
  SubsidenceMonitoring: '#af52de', WaterTable: '#007aff', FoundationAssessment: '#34c759', Other: '#86868b',
};
const groundConditionColors: Record<string, string> = {
  Good: '#34c759', Fair: '#ff9500', Poor: '#ff3b30', VeryPoor: '#af52de', Critical: '#ff453a',
};
const surveyTypeColors: Record<string, string> = {
  Boundary: '#0071e3', Topographic: '#34c759', Underground: '#8b6914', Stockpile: '#ff9500',
  Pit: '#ff3b30', AsBuilt: '#af52de', Monitoring: '#007aff', Other: '#86868b',
};

export default function GeotechnicalPage() {
  return (
    <div>
      <Flex justify="space-between" align="center" style={{ marginBottom: 24 }}>
        <div>
          <Title level={3} style={{ margin: 0, fontWeight: 700, letterSpacing: -0.5 }}>Geotechnical & Survey</Title>
          <Text style={{ color: '#86868b', fontSize: 13 }}>Manage geotechnical assessments and survey records</Text>
        </div>
      </Flex>

      <Tabs items={[
        { key: 'assessments', label: 'Assessments', children: <AssessmentsTab /> },
        { key: 'surveys', label: 'Surveys', children: <SurveysTab /> },
      ]} />
    </div>
  );
}

function AssessmentsTab() {
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [selected, setSelected] = useState<GeotechnicalAssessment | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<GeotechnicalAssessment | null>(null);
  const [filterStatus, setFilterStatus] = useState<string | undefined>();
  const [form] = Form.useForm();
  const queryClient = useQueryClient();

  const { data: assessments = [], isLoading } = useQuery({
    queryKey: ['geotechnicalAssessments', filterStatus],
    queryFn: () => geotechnicalApi.getAssessments(undefined, filterStatus),
  });

  const { data: mineSites = [] } = useQuery({
    queryKey: ['mineSites'],
    queryFn: () => mineSitesApi.getMineSites(),
  });

  const createMutation = useMutation({
    mutationFn: (data: CreateGeotechnicalAssessmentRequest) => geotechnicalApi.createAssessment(data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['geotechnicalAssessments'] }); setModalOpen(false); form.resetFields(); message.success('Assessment created'); },
  });

  const updateMutation = useMutation({
    mutationFn: (data: UpdateGeotechnicalAssessmentRequest) => geotechnicalApi.updateAssessment(data.id, data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['geotechnicalAssessments'] }); setModalOpen(false); setEditing(null); form.resetFields(); message.success('Assessment updated'); },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => geotechnicalApi.deleteAssessment(id),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['geotechnicalAssessments'] }); message.success('Assessment deleted'); },
  });

  const handleSubmit = async () => {
    const values = await form.validateFields();
    const payload = {
      ...values,
      date: values.date?.toISOString(),
      nextAssessmentDate: values.nextAssessmentDate?.toISOString(),
    };
    if (editing) {
      updateMutation.mutate({ ...payload, id: editing.id });
    } else {
      createMutation.mutate(payload);
    }
  };

  const openEdit = (record: GeotechnicalAssessment) => {
    setEditing(record);
    form.setFieldsValue({
      ...record,
      date: record.date ? dayjs(record.date) : undefined,
      nextAssessmentDate: record.nextAssessmentDate ? dayjs(record.nextAssessmentDate) : undefined,
    });
    setModalOpen(true);
  };

  const columns = [
    { title: 'Number', dataIndex: 'assessmentNumber', key: 'assessmentNumber', width: 120 },
    { title: 'Title', dataIndex: 'title', key: 'title', width: 200, ellipsis: true },
    { title: 'Type', dataIndex: 'assessmentType', key: 'assessmentType', width: 170, render: (t: string) => <Tag color={assessmentTypeColors[t]}>{t}</Tag> },
    { title: 'Mine Site', dataIndex: 'mineSiteName', key: 'mineSiteName', width: 140 },
    { title: 'Date', dataIndex: 'date', key: 'date', width: 110, render: (d: string) => dayjs(d).format('DD/MM/YYYY') },
    { title: 'Assessor', dataIndex: 'assessorName', key: 'assessorName', width: 140 },
    { title: 'Stability', dataIndex: 'stabilityStatus', key: 'stabilityStatus', width: 110, render: (s: string) => <Tag color={stabilityColors[s]}>{s}</Tag> },
    { title: 'Status', dataIndex: 'status', key: 'status', width: 100, render: (s: string) => <Tag color={statusColors[s]}>{s}</Tag> },
    {
      title: 'Actions', key: 'actions', width: 130,
      render: (_: unknown, record: GeotechnicalAssessment) => (
        <Flex gap={4}>
          <Button type="text" size="small" icon={<EyeOutlined />} onClick={() => { setSelected(record); setDrawerOpen(true); }} />
          <Button type="text" size="small" icon={<EditOutlined />} onClick={() => openEdit(record)} />
          <Popconfirm title="Delete this assessment?" onConfirm={() => deleteMutation.mutate(record.id)} okText="Yes" cancelText="No">
            <Button type="text" size="small" danger icon={<DeleteOutlined />} />
          </Popconfirm>
        </Flex>
      ),
    },
  ];

  return (
    <>
      <Flex justify="flex-end" gap={8} style={{ marginBottom: 16 }}>
        <Select placeholder="Filter by status" allowClear style={{ width: 180 }} onChange={setFilterStatus} options={recordStatuses.map(s => ({ label: s, value: s }))} />
        <Button type="primary" icon={<PlusOutlined />} onClick={() => { setEditing(null); form.resetFields(); setModalOpen(true); }}>New Assessment</Button>
      </Flex>

      <Table dataSource={assessments} columns={columns} rowKey="id" loading={isLoading} size="middle"
        style={{ background: '#fff', borderRadius: 12, overflow: 'hidden' }}
        pagination={{ pageSize: 15, showSizeChanger: false }} />

      <Modal title={editing ? 'Edit Geotechnical Assessment' : 'New Geotechnical Assessment'} open={modalOpen} onCancel={() => { setModalOpen(false); setEditing(null); }}
        onOk={handleSubmit} width={720} okText={editing ? 'Update' : 'Create'}
        confirmLoading={createMutation.isPending || updateMutation.isPending}>
        <Form form={form} layout="vertical" style={{ maxHeight: 500, overflow: 'auto', paddingRight: 8 }}>
          <Flex gap={16}>
            <Form.Item name="mineSiteId" label="Mine Site" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select placeholder="Select mine site" disabled={!!editing} options={mineSites.map(m => ({ label: m.name, value: m.id }))} />
            </Form.Item>
            <Form.Item name="mineAreaId" label="Mine Area" style={{ flex: 1 }}>
              <Select placeholder="Select mine area (optional)" allowClear disabled={!!editing} options={[]} />
            </Form.Item>
          </Flex>
          <Form.Item name="title" label="Title" rules={[{ required: true }]}>
            <Input placeholder="e.g., Slope Stability Assessment - North Wall" />
          </Form.Item>
          <Flex gap={16}>
            <Form.Item name="assessmentType" label="Assessment Type" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select options={assessmentTypes.map(t => ({ label: t, value: t }))} />
            </Form.Item>
            <Form.Item name="date" label="Assessment Date" rules={[{ required: true }]} style={{ flex: 1 }}>
              <DatePicker style={{ width: '100%' }} />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="assessorName" label="Assessor Name" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Input />
            </Form.Item>
            <Form.Item name="location" label="Location" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Input placeholder="e.g., North Pit Wall, Level 3" />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="rockMassRating" label="Rock Mass Rating (RMR)" style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} />
            </Form.Item>
            <Form.Item name="slopeAngle" label="Slope Angle" style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} addonAfter="deg" />
            </Form.Item>
            <Form.Item name="waterTableDepth" label="Water Table Depth" style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} addonAfter="m" />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="groundCondition" label="Ground Condition" style={{ flex: 1 }}>
              <Select allowClear options={groundConditions.map(g => ({ label: g, value: g }))} />
            </Form.Item>
            <Form.Item name="stabilityStatus" label="Stability Status" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select options={stabilityStatuses.map(s => ({ label: s, value: s }))} />
            </Form.Item>
          </Flex>
          <Form.Item name="recommendedActions" label="Recommended Actions">
            <TextArea rows={2} />
          </Form.Item>
          <Flex gap={16}>
            <Form.Item name="monitoringRequired" label="Monitoring Required" valuePropName="checked" style={{ flex: 1 }}>
              <Switch />
            </Form.Item>
            <Form.Item name="nextAssessmentDate" label="Next Assessment Date" style={{ flex: 1 }}>
              <DatePicker style={{ width: '100%' }} />
            </Form.Item>
          </Flex>
          <Form.Item name="notes" label="Notes">
            <TextArea rows={2} />
          </Form.Item>
          {editing && (
            <Form.Item name="status" label="Status">
              <Select options={recordStatuses.map(s => ({ label: s, value: s }))} />
            </Form.Item>
          )}
        </Form>
      </Modal>

      <Drawer title={selected?.title} open={drawerOpen} onClose={() => { setDrawerOpen(false); setSelected(null); }} width={700}>
        {selected && <AssessmentDetails assessment={selected} />}
      </Drawer>
    </>
  );
}

function AssessmentDetails({ assessment }: { assessment: GeotechnicalAssessment }) {
  return (
    <Descriptions column={2} size="small" bordered>
      <Descriptions.Item label="Number">{assessment.assessmentNumber}</Descriptions.Item>
      <Descriptions.Item label="Status"><Tag color={statusColors[assessment.status]}>{assessment.status}</Tag></Descriptions.Item>
      <Descriptions.Item label="Title" span={2}>{assessment.title}</Descriptions.Item>
      <Descriptions.Item label="Type"><Tag color={assessmentTypeColors[assessment.assessmentType]}>{assessment.assessmentType}</Tag></Descriptions.Item>
      <Descriptions.Item label="Stability"><Tag color={stabilityColors[assessment.stabilityStatus]}>{assessment.stabilityStatus}</Tag></Descriptions.Item>
      <Descriptions.Item label="Mine Site">{assessment.mineSiteName}</Descriptions.Item>
      {assessment.mineAreaName && <Descriptions.Item label="Mine Area">{assessment.mineAreaName}</Descriptions.Item>}
      <Descriptions.Item label="Date">{dayjs(assessment.date).format('DD/MM/YYYY')}</Descriptions.Item>
      <Descriptions.Item label="Assessor">{assessment.assessorName}</Descriptions.Item>
      <Descriptions.Item label="Location" span={2}>{assessment.location}</Descriptions.Item>
      {assessment.rockMassRating != null && <Descriptions.Item label="Rock Mass Rating">{assessment.rockMassRating}</Descriptions.Item>}
      {assessment.slopeAngle != null && <Descriptions.Item label="Slope Angle">{assessment.slopeAngle} deg</Descriptions.Item>}
      {assessment.waterTableDepth != null && <Descriptions.Item label="Water Table Depth">{assessment.waterTableDepth} m</Descriptions.Item>}
      {assessment.groundCondition && <Descriptions.Item label="Ground Condition"><Tag color={groundConditionColors[assessment.groundCondition]}>{assessment.groundCondition}</Tag></Descriptions.Item>}
      <Descriptions.Item label="Monitoring Required">{assessment.monitoringRequired ? 'Yes' : 'No'}</Descriptions.Item>
      {assessment.nextAssessmentDate && <Descriptions.Item label="Next Assessment">{dayjs(assessment.nextAssessmentDate).format('DD/MM/YYYY')}</Descriptions.Item>}
      {assessment.recommendedActions && <Descriptions.Item label="Recommended Actions" span={2}>{assessment.recommendedActions}</Descriptions.Item>}
      {assessment.notes && <Descriptions.Item label="Notes" span={2}>{assessment.notes}</Descriptions.Item>}
      <Descriptions.Item label="Created">{dayjs(assessment.createdAt).format('DD/MM/YYYY HH:mm')}</Descriptions.Item>
    </Descriptions>
  );
}

function SurveysTab() {
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [selected, setSelected] = useState<SurveyRecord | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<SurveyRecord | null>(null);
  const [filterType, setFilterType] = useState<string | undefined>();
  const [form] = Form.useForm();
  const queryClient = useQueryClient();

  const { data: surveys = [], isLoading } = useQuery({
    queryKey: ['surveyRecords', filterType],
    queryFn: () => geotechnicalApi.getSurveys(undefined, filterType),
  });

  const { data: mineSites = [] } = useQuery({
    queryKey: ['mineSites'],
    queryFn: () => mineSitesApi.getMineSites(),
  });

  const createMutation = useMutation({
    mutationFn: (data: CreateSurveyRecordRequest) => geotechnicalApi.createSurvey(data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['surveyRecords'] }); setModalOpen(false); form.resetFields(); message.success('Survey record created'); },
  });

  const updateMutation = useMutation({
    mutationFn: (data: UpdateSurveyRecordRequest) => geotechnicalApi.updateSurvey(data.id, data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['surveyRecords'] }); setModalOpen(false); setEditing(null); form.resetFields(); message.success('Survey record updated'); },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => geotechnicalApi.deleteSurvey(id),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['surveyRecords'] }); message.success('Survey record deleted'); },
  });

  const handleSubmit = async () => {
    const values = await form.validateFields();
    const payload = {
      ...values,
      date: values.date?.toISOString(),
    };
    if (editing) {
      updateMutation.mutate({ ...payload, id: editing.id });
    } else {
      createMutation.mutate(payload);
    }
  };

  const openEdit = (record: SurveyRecord) => {
    setEditing(record);
    form.setFieldsValue({
      ...record,
      date: record.date ? dayjs(record.date) : undefined,
    });
    setModalOpen(true);
  };

  const columns = [
    { title: 'Number', dataIndex: 'surveyNumber', key: 'surveyNumber', width: 120 },
    { title: 'Title', dataIndex: 'title', key: 'title', width: 200, ellipsis: true },
    { title: 'Type', dataIndex: 'surveyType', key: 'surveyType', width: 130, render: (t: string) => <Tag color={surveyTypeColors[t]}>{t}</Tag> },
    { title: 'Mine Site', dataIndex: 'mineSiteName', key: 'mineSiteName', width: 140 },
    { title: 'Date', dataIndex: 'date', key: 'date', width: 110, render: (d: string) => dayjs(d).format('DD/MM/YYYY') },
    { title: 'Surveyor', dataIndex: 'surveyorName', key: 'surveyorName', width: 140 },
    { title: 'Location', dataIndex: 'location', key: 'location', width: 150, ellipsis: true },
    { title: 'Status', dataIndex: 'status', key: 'status', width: 100, render: (s: string) => <Tag color={statusColors[s]}>{s}</Tag> },
    {
      title: 'Actions', key: 'actions', width: 130,
      render: (_: unknown, record: SurveyRecord) => (
        <Flex gap={4}>
          <Button type="text" size="small" icon={<EyeOutlined />} onClick={() => { setSelected(record); setDrawerOpen(true); }} />
          <Button type="text" size="small" icon={<EditOutlined />} onClick={() => openEdit(record)} />
          <Popconfirm title="Delete this survey record?" onConfirm={() => deleteMutation.mutate(record.id)} okText="Yes" cancelText="No">
            <Button type="text" size="small" danger icon={<DeleteOutlined />} />
          </Popconfirm>
        </Flex>
      ),
    },
  ];

  return (
    <>
      <Flex justify="flex-end" gap={8} style={{ marginBottom: 16 }}>
        <Select placeholder="Filter by type" allowClear style={{ width: 180 }} onChange={setFilterType} options={surveyTypes.map(t => ({ label: t, value: t }))} />
        <Button type="primary" icon={<PlusOutlined />} onClick={() => { setEditing(null); form.resetFields(); setModalOpen(true); }}>New Survey</Button>
      </Flex>

      <Table dataSource={surveys} columns={columns} rowKey="id" loading={isLoading} size="middle"
        style={{ background: '#fff', borderRadius: 12, overflow: 'hidden' }}
        pagination={{ pageSize: 15, showSizeChanger: false }} />

      <Modal title={editing ? 'Edit Survey Record' : 'New Survey Record'} open={modalOpen} onCancel={() => { setModalOpen(false); setEditing(null); }}
        onOk={handleSubmit} width={720} okText={editing ? 'Update' : 'Create'}
        confirmLoading={createMutation.isPending || updateMutation.isPending}>
        <Form form={form} layout="vertical" style={{ maxHeight: 500, overflow: 'auto', paddingRight: 8 }}>
          <Flex gap={16}>
            <Form.Item name="mineSiteId" label="Mine Site" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select placeholder="Select mine site" disabled={!!editing} options={mineSites.map(m => ({ label: m.name, value: m.id }))} />
            </Form.Item>
            <Form.Item name="mineAreaId" label="Mine Area" style={{ flex: 1 }}>
              <Select placeholder="Select mine area (optional)" allowClear disabled={!!editing} options={[]} />
            </Form.Item>
          </Flex>
          <Form.Item name="title" label="Title" rules={[{ required: true }]}>
            <Input placeholder="e.g., Pit Boundary Survey - Q1 2025" />
          </Form.Item>
          <Flex gap={16}>
            <Form.Item name="surveyType" label="Survey Type" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Select options={surveyTypes.map(t => ({ label: t, value: t }))} />
            </Form.Item>
            <Form.Item name="date" label="Survey Date" rules={[{ required: true }]} style={{ flex: 1 }}>
              <DatePicker style={{ width: '100%' }} />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="surveyorName" label="Surveyor Name" rules={[{ required: true }]} style={{ flex: 1 }}>
              <Input />
            </Form.Item>
            <Form.Item name="surveyorLicense" label="Surveyor License" style={{ flex: 1 }}>
              <Input />
            </Form.Item>
          </Flex>
          <Form.Item name="location" label="Location" rules={[{ required: true }]}>
            <Input placeholder="e.g., North Pit Boundary, Main Stockpile" />
          </Form.Item>
          <Flex gap={16}>
            <Form.Item name="easting" label="Easting" style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} />
            </Form.Item>
            <Form.Item name="northing" label="Northing" style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} />
            </Form.Item>
            <Form.Item name="elevation" label="Elevation" style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="datum" label="Datum" style={{ flex: 1 }}>
              <Input placeholder="e.g., WGS84" />
            </Form.Item>
            <Form.Item name="coordinateSystem" label="Coordinate System" style={{ flex: 1 }}>
              <Input placeholder="e.g., UTM Zone 35S" />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="equipmentUsed" label="Equipment Used" style={{ flex: 1 }}>
              <Input placeholder="e.g., Total Station, GPS RTK" />
            </Form.Item>
            <Form.Item name="accuracy" label="Accuracy" style={{ flex: 1 }}>
              <Input placeholder="e.g., +/- 5mm" />
            </Form.Item>
          </Flex>
          <Flex gap={16}>
            <Form.Item name="volumeCalculated" label="Volume Calculated" style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} addonAfter="m3" />
            </Form.Item>
            <Form.Item name="areaCalculated" label="Area Calculated" style={{ flex: 1 }}>
              <InputNumber style={{ width: '100%' }} addonAfter="m2" />
            </Form.Item>
          </Flex>
          <Form.Item name="findings" label="Findings">
            <TextArea rows={2} />
          </Form.Item>
          <Form.Item name="notes" label="Notes">
            <TextArea rows={2} />
          </Form.Item>
          {editing && (
            <Form.Item name="status" label="Status">
              <Select options={recordStatuses.map(s => ({ label: s, value: s }))} />
            </Form.Item>
          )}
        </Form>
      </Modal>

      <Drawer title={selected?.title} open={drawerOpen} onClose={() => { setDrawerOpen(false); setSelected(null); }} width={700}>
        {selected && <SurveyDetails survey={selected} />}
      </Drawer>
    </>
  );
}

function SurveyDetails({ survey }: { survey: SurveyRecord }) {
  return (
    <Descriptions column={2} size="small" bordered>
      <Descriptions.Item label="Number">{survey.surveyNumber}</Descriptions.Item>
      <Descriptions.Item label="Status"><Tag color={statusColors[survey.status]}>{survey.status}</Tag></Descriptions.Item>
      <Descriptions.Item label="Title" span={2}>{survey.title}</Descriptions.Item>
      <Descriptions.Item label="Type"><Tag color={surveyTypeColors[survey.surveyType]}>{survey.surveyType}</Tag></Descriptions.Item>
      <Descriptions.Item label="Mine Site">{survey.mineSiteName}</Descriptions.Item>
      {survey.mineAreaName && <Descriptions.Item label="Mine Area">{survey.mineAreaName}</Descriptions.Item>}
      <Descriptions.Item label="Date">{dayjs(survey.date).format('DD/MM/YYYY')}</Descriptions.Item>
      <Descriptions.Item label="Surveyor">{survey.surveyorName}</Descriptions.Item>
      {survey.surveyorLicense && <Descriptions.Item label="License">{survey.surveyorLicense}</Descriptions.Item>}
      <Descriptions.Item label="Location" span={2}>{survey.location}</Descriptions.Item>
      {survey.easting != null && <Descriptions.Item label="Easting">{survey.easting}</Descriptions.Item>}
      {survey.northing != null && <Descriptions.Item label="Northing">{survey.northing}</Descriptions.Item>}
      {survey.elevation != null && <Descriptions.Item label="Elevation">{survey.elevation}</Descriptions.Item>}
      {survey.datum && <Descriptions.Item label="Datum">{survey.datum}</Descriptions.Item>}
      {survey.coordinateSystem && <Descriptions.Item label="Coordinate System">{survey.coordinateSystem}</Descriptions.Item>}
      {survey.equipmentUsed && <Descriptions.Item label="Equipment Used">{survey.equipmentUsed}</Descriptions.Item>}
      {survey.accuracy && <Descriptions.Item label="Accuracy">{survey.accuracy}</Descriptions.Item>}
      {survey.volumeCalculated != null && <Descriptions.Item label="Volume">{survey.volumeCalculated} m3</Descriptions.Item>}
      {survey.areaCalculated != null && <Descriptions.Item label="Area">{survey.areaCalculated} m2</Descriptions.Item>}
      {survey.findings && <Descriptions.Item label="Findings" span={2}>{survey.findings}</Descriptions.Item>}
      {survey.notes && <Descriptions.Item label="Notes" span={2}>{survey.notes}</Descriptions.Item>}
      <Descriptions.Item label="Created">{dayjs(survey.createdAt).format('DD/MM/YYYY HH:mm')}</Descriptions.Item>
    </Descriptions>
  );
}
