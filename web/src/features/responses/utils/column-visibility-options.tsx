import { VisibilityState } from '@tanstack/react-table';
import { CitizenReportByEntry } from '../models/citizen-report';
import { FormSubmissionByEntry, FormSubmissionByForm, FormSubmissionByObserver } from '../models/form-submission';
import { IncidentReportByEntry, IncidentReportByForm, IncidentReportByObserver } from '../models/incident-report';
import { QuickReport } from '../models/quick-report';

export type FormSubmissionsViewBy = 'byEntry' | 'byObserver' | 'byForm';
export type CitizenReportsViewBy = 'byEntry' | 'byForm';
export type IncidentReportsViewBy = 'byEntry' | 'byObserver' | 'byForm';

type TableColumnVisibilityState<T> = Record<keyof T, boolean>;

export const formSubmissionsByEntryDefaultColumns: TableColumnVisibilityState<FormSubmissionByEntry> = {
  submissionId: false,
  timeSubmitted: true,
  formCode: true,
  formName: true,
  formType: true,
  level1: false,
  level2: false,
  level3: false,
  level4: false,
  level5: false,
  number: false,
  observerName: false,
  ngoName: false,
  tags: false,
  numberOfQuestionsAnswered: true,
  numberOfFlaggedAnswers: true,
  notesCount: false,
  mediaFilesCount: false,
  followUpStatus: true,
  // delete ?
  defaultLanguage: false,
  email: false,
  monitoringObserverId: false,
  phoneNumber: false,
  pollingStationId: false,
};

export const formSubmissionsByObserverDefaultColumns: TableColumnVisibilityState<FormSubmissionByObserver> = {
  observerName: true,
  phoneNumber: true,
  tags: true,
  ngoName: true,
  numberOfLocations: true,
  numberOfFormsSubmitted: true,
  numberOfFlaggedAnswers: true,
  followUpStatus: true,
  // delete?
  email: false,
  monitoringObserverId: false,
};

export const formSubmissionsByFormDefaultColumns: TableColumnVisibilityState<FormSubmissionByForm> = {
  formName: true,
  formCode: true,
  formType: true,
  numberOfFlaggedAnswers: true,
  numberOfNotes: true,
  numberOfMediaFiles: true,
  // delete >
  formId: false,
  numberOfSubmissions: false,
  defaultLanguage: false,
};

export const observerFormSubmissionsDefaultColumns: TableColumnVisibilityState<FormSubmissionByEntry> = {
  submissionId: false,
  timeSubmitted: true,
  formCode: true,
  formType: true,
  formName: true,
  level1: false,
  level2: false,
  level3: false,
  level4: false,
  level5: false,
  number: true,
  numberOfQuestionsAnswered: true,
  numberOfFlaggedAnswers: true,
  notesCount: false,
  mediaFilesCount: false,
  followUpStatus: true,
  // delete
  defaultLanguage: false,
  email: false,
  monitoringObserverId: false,
  observerName: false,
  phoneNumber: false,
  pollingStationId: false,
  tags: false,
  ngoName: false,
};

export const formSubmissionsDefaultColumns: Record<FormSubmissionsViewBy, VisibilityState> = {
  byEntry: formSubmissionsByEntryDefaultColumns,
  byObserver: formSubmissionsByObserverDefaultColumns,
  byForm: formSubmissionsByFormDefaultColumns,
};

export type ColumnOption<T> = { id: keyof T; label: string; enableHiding: boolean };

const formSubmissionsByEntryColumnVisibilityOptions: ColumnOption<FormSubmissionByEntry>[] = [
  { id: 'submissionId', label: 'Entry ID', enableHiding: true },
  { id: 'timeSubmitted', label: 'Time submitted', enableHiding: true },
  { id: 'formCode', label: 'Form code', enableHiding: true },
  { id: 'formType', label: 'Form type', enableHiding: true },
  { id: 'formName', label: 'Form name', enableHiding: true },
  { id: 'level1', label: 'Location - L1', enableHiding: true },
  { id: 'level2', label: 'Location - L2', enableHiding: true },
  { id: 'level3', label: 'Location - L3', enableHiding: true },
  { id: 'level4', label: 'Location - L4', enableHiding: true },
  { id: 'level5', label: 'Location - L5', enableHiding: true },
  { id: 'number', label: 'Station number', enableHiding: true },
  { id: 'observerName', label: 'Observer', enableHiding: true },
  { id: 'ngoName', label: 'NGO', enableHiding: true },
  { id: 'tags', label: 'Observer tags', enableHiding: true },
  { id: 'numberOfQuestionsAnswered', label: 'Questions answered', enableHiding: true },
  { id: 'numberOfFlaggedAnswers', label: 'Flagged answers', enableHiding: true },
  { id: 'notesCount', label: 'Question notes', enableHiding: true },
  { id: 'mediaFilesCount', label: 'Media files', enableHiding: true },
  { id: 'followUpStatus', label: 'Follow-up status', enableHiding: true },
];

const formSubmissionsByObserverColumnVisibilityOptions: ColumnOption<FormSubmissionByObserver>[] = [
  { id: 'observerName', label: 'Observer name', enableHiding: false },
  { id: 'phoneNumber', label: 'Observer contact', enableHiding: true },
  { id: 'ngoName', label: 'NGO', enableHiding: true },
  { id: 'tags', label: 'Observer tags', enableHiding: true },
  { id: 'numberOfLocations', label: 'Locations', enableHiding: false },
  { id: 'numberOfFormsSubmitted', label: 'Forms', enableHiding: false },
  { id: 'numberOfFlaggedAnswers', label: 'Flagged answers', enableHiding: true },
  { id: 'followUpStatus', label: 'Follow-up status', enableHiding: true },
];

const formSubmissionsByFormColumnVisibilityOptions: ColumnOption<FormSubmissionByForm>[] = [
  { id: 'formCode', label: 'Form code', enableHiding: false },
  { id: 'formType', label: 'Form type', enableHiding: false },
  { id: 'formName', label: 'Form name', enableHiding: false },
  { id: 'numberOfSubmissions', label: 'Responses', enableHiding: true },
  { id: 'numberOfFlaggedAnswers', label: 'Flagged answers', enableHiding: true },
  { id: 'numberOfNotes', label: 'Question notes', enableHiding: true },
  { id: 'numberOfMediaFiles', label: 'Media files', enableHiding: true },
];

export const formSubmissionsColumnVisibilityOptions: Record<
  FormSubmissionsViewBy,
  | ColumnOption<FormSubmissionByEntry>[]
  | ColumnOption<FormSubmissionByObserver>[]
  | ColumnOption<FormSubmissionByForm>[]
> = {
  byEntry: formSubmissionsByEntryColumnVisibilityOptions,
  byObserver: formSubmissionsByObserverColumnVisibilityOptions,
  byForm: formSubmissionsByFormColumnVisibilityOptions,
};

export const quickReportsColumnVisibilityOptions: ColumnOption<QuickReport>[] = [
  { id: 'timestamp', label: 'Time submitted', enableHiding: true },
  { id: 'quickReportLocationType', label: 'Location type', enableHiding: true },
  { id: 'incidentCategory', label: 'Incident category', enableHiding: true },
  { id: 'observerName', label: 'Observer', enableHiding: true },
  { id: 'ngoName', label: 'NGO', enableHiding: true },
  { id: 'followUpStatus', label: 'Follow-up status', enableHiding: true },
  { id: 'title', label: 'Issue title', enableHiding: true },
  { id: 'description', label: 'Description', enableHiding: true },
  { id: 'numberOfAttachments', label: 'Media files', enableHiding: true },
  { id: 'level1', label: 'Location - L1', enableHiding: true },
  { id: 'level2', label: 'Location - L2', enableHiding: true },
  { id: 'level3', label: 'Location - L3', enableHiding: true },
  { id: 'level4', label: 'Location - L4', enableHiding: true },
  { id: 'level5', label: 'Location - L5', enableHiding: true },
  { id: 'number', label: 'Station number', enableHiding: true },
  { id: 'pollingStationDetails', label: 'Polling station details', enableHiding: true },

];

export const quickReportsDefaultColumns: TableColumnVisibilityState<QuickReport> = {
  timestamp: true,
  quickReportLocationType: true,
  incidentCategory: true,
  title: true,
  description: true,
  numberOfAttachments: true,
  observerName: true,
  email: false,
  ngoName: false,
  level1: false,
  level2: false,
  level3: false,
  level4: false,
  level5: false,
  number: false,
  pollingStationDetails: false,
  followUpStatus: true,
  address: false,

  attachments: false,
  id: false,
  monitoringObserverId: false,
  pollingStationId: false,
};

export const observerQuickReportsColumns: TableColumnVisibilityState<QuickReport> = {
  timestamp: true,
  quickReportLocationType: true,
  incidentCategory: true,
  title: true,
  description: true,
  numberOfAttachments: true,
  level1: false,
  level2: false,
  level3: false,
  level4: false,
  level5: false,
  number: false,
  pollingStationDetails: false,
  followUpStatus: true,
  address: false,

  // delete
  attachments: false,
  id: false,
  monitoringObserverId: false,
  pollingStationId: false,
  observerName: true,
  email: false,
  ngoName: false,
};

export const citizenReportsColumnVisibilityOptions: ColumnOption<CitizenReportByEntry>[] = [
  { id: 'timeSubmitted', label: 'Time submitted', enableHiding: true },
  { id: 'formCode', label: 'Form code', enableHiding: true },
  { id: 'formName', label: 'Form name', enableHiding: true },
  { id: 'numberOfQuestionsAnswered', label: 'Questions answered', enableHiding: true },
  { id: 'numberOfFlaggedAnswers', label: 'Flagged answers', enableHiding: true },
  { id: 'notesCount', label: 'Question notes', enableHiding: true },
  { id: 'mediaFilesCount', label: 'Media files', enableHiding: true },
  { id: 'followUpStatus', label: 'Follow-up status', enableHiding: true },
];

export const citizenReportsDefaultColumns: TableColumnVisibilityState<CitizenReportByEntry> = {
  citizenReportId: false,
  timeSubmitted: true,
  formCode: true,
  formName: true,
  numberOfQuestionsAnswered: true,
  numberOfFlaggedAnswers: true,
  notesCount: false,
  mediaFilesCount: false,
  followUpStatus: true,
  level1: true,
  level2: true,
  level3: true,
  level4: true,
  level5: true,
  // delete,
  formDefaultLanguage: false,
};

const incidentReportsByEntryColumnVisibilityOptions: ColumnOption<IncidentReportByEntry>[] = [
  { id: 'incidentReportId', label: 'Entry ID', enableHiding: true },
  { id: 'timeSubmitted', label: 'Time submitted', enableHiding: true },
  { id: 'formCode', label: 'Form code', enableHiding: true },
  { id: 'formName', label: 'Form Name', enableHiding: true },
  { id: 'pollingStationLevel1', label: 'Location - L1', enableHiding: true },
  { id: 'pollingStationLevel2', label: 'Location - L2', enableHiding: true },
  { id: 'pollingStationLevel3', label: 'Location - L3', enableHiding: true },
  { id: 'pollingStationLevel4', label: 'Location - L4', enableHiding: true },
  { id: 'pollingStationLevel5', label: 'Location - L5', enableHiding: true },
  { id: 'pollingStationNumber', label: 'Station number', enableHiding: true },
  { id: 'observerName', label: 'Observer', enableHiding: true },
  { id: 'tags', label: 'Observer tags', enableHiding: true },
  { id: 'ngoName', label: 'NGO', enableHiding: true },
  { id: 'numberOfQuestionsAnswered', label: 'Questions answered', enableHiding: true },
  { id: 'numberOfFlaggedAnswers', label: 'Flagged answers', enableHiding: true },
  { id: 'notesCount', label: 'Question notes', enableHiding: true },
  { id: 'mediaFilesCount', label: 'Media files', enableHiding: true },
  { id: 'followUpStatus', label: 'Follow-up status', enableHiding: true },
];

const incidentReportsByObserverColumnVisibilityOptions: ColumnOption<IncidentReportByObserver>[] = [
  { id: 'observerName', label: 'Observer name', enableHiding: false },
  { id: 'phoneNumber', label: 'Observer contact', enableHiding: true },
  { id: 'ngoName', label: 'NGO', enableHiding: true },
  { id: 'tags', label: 'Observer tags', enableHiding: true },
  { id: 'numberOfIncidentsSubmitted', label: 'Number of submissions', enableHiding: false },
  { id: 'numberOfFlaggedAnswers', label: 'Flagged answers', enableHiding: true },
  { id: 'followUpStatus', label: 'Follow-up status', enableHiding: true },
];

const incidentReportsByFormColumnVisibilityOptions: ColumnOption<IncidentReportByForm>[] = [
  { id: 'formCode', label: 'Form code', enableHiding: false },
  { id: 'formName', label: 'Form name', enableHiding: false },
  { id: 'numberOfSubmissions', label: 'Responses', enableHiding: true },
  { id: 'numberOfFlaggedAnswers', label: 'Flagged answers', enableHiding: true },
  { id: 'numberOfNotes', label: 'Question notes', enableHiding: true },
  { id: 'numberOfMediaFiles', label: 'Media files', enableHiding: true },
];

export const incidentReportsColumnVisibilityOptions: Record<
  IncidentReportsViewBy,
  | ColumnOption<IncidentReportByEntry>[]
  | ColumnOption<IncidentReportByObserver>[]
  | ColumnOption<IncidentReportByForm>[]
> = {
  byEntry: incidentReportsByEntryColumnVisibilityOptions,
  byObserver: incidentReportsByObserverColumnVisibilityOptions,
  byForm: incidentReportsByFormColumnVisibilityOptions,
};

export const observersFormSubmissionsColumnVisibilityOptions: ColumnOption<FormSubmissionByEntry>[] = [
  { id: 'timeSubmitted', label: 'Time submitted', enableHiding: true },
  { id: 'formCode', label: 'Form code', enableHiding: true },
  { id: 'formType', label: 'Form type', enableHiding: true },
  { id: 'formName', label: 'Form name', enableHiding: true },
  { id: 'level1', label: 'Location - L1', enableHiding: true },
  { id: 'level2', label: 'Location - L2', enableHiding: true },
  { id: 'level3', label: 'Location - L3', enableHiding: true },
  { id: 'level4', label: 'Location - L4', enableHiding: true },
  { id: 'level5', label: 'Location - L5', enableHiding: true },
  { id: 'number', label: 'Station number', enableHiding: true },
  { id: 'numberOfQuestionsAnswered', label: 'Questions answered', enableHiding: true },
  { id: 'numberOfFlaggedAnswers', label: 'Flagged answers', enableHiding: true },
  { id: 'notesCount', label: 'Question notes', enableHiding: true },
  { id: 'mediaFilesCount', label: 'Media files', enableHiding: true },
  { id: 'followUpStatus', label: 'Follow-up status', enableHiding: true },
];

export const observersIncidentReportsColumnVisibilityOptions: ColumnOption<IncidentReportByEntry>[] = [
  { id: 'timeSubmitted', label: 'Time submitted', enableHiding: true },
  { id: 'formCode', label: 'Form code', enableHiding: true },
  { id: 'formName', label: 'Form name', enableHiding: true },
  { id: 'locationType', label: 'Location type', enableHiding: true },
  { id: 'pollingStationLevel1', label: 'Location - L1', enableHiding: true },
  { id: 'pollingStationLevel2', label: 'Location - L2', enableHiding: true },
  { id: 'pollingStationLevel3', label: 'Location - L3', enableHiding: true },
  { id: 'pollingStationLevel4', label: 'Location - L4', enableHiding: true },
  { id: 'pollingStationLevel5', label: 'Location - L5', enableHiding: true },
  { id: 'pollingStationNumber', label: 'Station number', enableHiding: true },
  { id: 'locationDescription', label: 'Location description', enableHiding: true },
  { id: 'numberOfQuestionsAnswered', label: 'Questions answered', enableHiding: true },
  { id: 'numberOfFlaggedAnswers', label: 'Flagged answers', enableHiding: true },
  { id: 'notesCount', label: 'Question notes', enableHiding: true },
  { id: 'mediaFilesCount', label: 'Media files', enableHiding: true },
  { id: 'followUpStatus', label: 'Follow-up status', enableHiding: true },
];

export const incidentReportsByEntryDefaultColumns: TableColumnVisibilityState<IncidentReportByEntry> = {
  incidentReportId: false,
  timeSubmitted: true,
  formCode: true,
  formName: true,
  locationType: true,
  locationDescription: false,
  pollingStationNumber: false,
  pollingStationLevel1: false,
  pollingStationLevel2: false,
  pollingStationLevel3: false,
  pollingStationLevel4: false,
  pollingStationLevel5: false,
  observerName: false,
  tags: false,
  numberOfQuestionsAnswered: true,
  numberOfFlaggedAnswers: true,
  notesCount: false,
  mediaFilesCount: false,
  followUpStatus: true,
  // delete ?
  formDefaultLanguage: false,
  monitoringObserverId: false,
  phoneNumber: false,
  ngoName: false,
  pollingStationId: false,
};

export const incidentReportsByObserverDefaultColumns: TableColumnVisibilityState<IncidentReportByObserver> = {
  observerName: true,
  phoneNumber: true,
  tags: true,
  ngoName: true,
  numberOfFlaggedAnswers: true,
  followUpStatus: true,
  numberOfIncidentsSubmitted: true,
  // delete ?
  monitoringObserverId: false,
};

export const incidentReportsByFormDefaultColumns: TableColumnVisibilityState<IncidentReportByForm> = {
  formCode: true,
  formName: true,
  numberOfFlaggedAnswers: true,
  numberOfNotes: true,
  numberOfMediaFiles: true,
  numberOfSubmissions: true,
  // delete ?
  formId: false,
  formDefaultLanguage: false,
};

export const observerIncidentReportsColumns: TableColumnVisibilityState<IncidentReportByEntry> = {
  incidentReportId: false,
  timeSubmitted: true,
  formCode: true,
  formName: true,
  locationType: true,
  locationDescription: false,
  pollingStationLevel1: false,
  pollingStationLevel2: false,
  pollingStationLevel3: false,
  pollingStationLevel4: false,
  pollingStationLevel5: false,
  pollingStationNumber: false,
  numberOfQuestionsAnswered: true,
  numberOfFlaggedAnswers: true,
  notesCount: false,
  mediaFilesCount: false,
  followUpStatus: true,
  // delete
  formDefaultLanguage: false,
  monitoringObserverId: false,
  observerName: false,
  phoneNumber: false,
  pollingStationId: false,
  tags: false,
  ngoName: false,
};

export const incidentReportsDefaultColumns: Record<IncidentReportsViewBy, VisibilityState> = {
  byEntry: incidentReportsByEntryDefaultColumns,
  byObserver: incidentReportsByObserverDefaultColumns,
  byForm: incidentReportsByFormDefaultColumns,
};
