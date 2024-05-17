import type { VisibilityState } from '@tanstack/react-table';

export type FilterBy = 'byEntry' | 'byObserver' | 'byForm';

export const formSubmissionsByEntryDefaultColumns: VisibilityState = {
  submissionId: false,
  timeSubmitted: true,
  formCode: true,
  formType: true,
  number: true,
  level1: false,
  level2: false,
  level3: false,
  level4: false,
  level5: false,
  observerName: false,
  tags: false,
  numberOfQuestionsAnswered: true,
  numberOfFlaggedAnswers: true,
  notesCount: false,
  mediaFilesCount: false,
  status: true,
};

export const formSubmissionsByObserverDefaultColumns: VisibilityState = {
  observerName: true,
  phoneNumber: true,
  tags: true,
  numberOfLocations: true,
  numberOfFormsSubmitted: true,
  numberOfFlaggedAnswers: true,
  status: true,
};

export const formSubmissionsByFormDefaultColumns: VisibilityState = {
  formCode: true,
  formType: true,
  numberOfAttachments: true,
  numberOfFlaggedAnswers: true,
  numberOfNotes: true,
  numberOfMediaFiles: true,
};

export const formSubmissionsDefaultColumns: Record<FilterBy, VisibilityState> = {
  byEntry: formSubmissionsByEntryDefaultColumns,
  byObserver: formSubmissionsByObserverDefaultColumns,
  byForm: formSubmissionsByFormDefaultColumns,
};

type ColumnOption = { id: string; label: string; enableHiding: boolean };

const byEntryColumnVisibilityOptions: ColumnOption[] = [
  { id: 'timeSubmitted', label: 'Time submitted', enableHiding: true },
  { id: 'formCode', label: 'Form code', enableHiding: true },
  { id: 'formType', label: 'Form type', enableHiding: true },
  { id: 'number', label: 'Station number', enableHiding: true },
  { id: 'level1', label: 'Location - L1', enableHiding: true },
  { id: 'level2', label: 'Location - L2', enableHiding: true },
  { id: 'level3', label: 'Location - L3', enableHiding: true },
  { id: 'level4', label: 'Location - L4', enableHiding: true },
  { id: 'level5', label: 'Location - L5', enableHiding: true },
  { id: 'observerName', label: 'Observer', enableHiding: true },
  { id: 'tags', label: 'Tags', enableHiding: true },
  { id: 'numberOfQuestionsAnswered', label: 'Responses', enableHiding: true },
  { id: 'numberOfFlaggedAnswers', label: 'Flagged answers', enableHiding: true },
  { id: 'notesCount', label: 'Question notes', enableHiding: true },
  { id: 'mediaFilesCount', label: 'Media files', enableHiding: true },
  { id: 'status', label: 'Status', enableHiding: true },
];

const byObserverColumnVisibilityOptions: ColumnOption[] = [
  { id: 'observerName', label: 'Observer name', enableHiding: false },
  { id: 'phoneNumber', label: 'Observer contact', enableHiding: true },
  { id: 'tags', label: 'Observer tags', enableHiding: true },
  { id: 'numberOfLocations', label: 'Locations', enableHiding: false },
  { id: 'numberOfFormsSubmitted', label: 'Forms', enableHiding: false },
  { id: 'numberOfFlaggedAnswers', label: 'Flagged answers', enableHiding: true },
  { id: 'status', label: 'Status', enableHiding: true },
];

const byFormColumnVisibilityOptions: ColumnOption[] = [
  { id: 'formCode', label: 'Form code', enableHiding: false },
  { id: 'formType', label: 'Form type', enableHiding: false },
  { id: 'numberOfSubmissions', label: 'Responses', enableHiding: true },
  { id: 'numberOfFlaggedAnswers', label: 'Flagged answers', enableHiding: true },
  { id: 'numberOfNotes', label: 'Question notes', enableHiding: true },
  { id: 'numberOfMediaFiles', label: 'Media files', enableHiding: true },
];

export const columnVisibilityOptions: Record<FilterBy, ColumnOption[]> = {
  byEntry: byEntryColumnVisibilityOptions,
  byObserver: byObserverColumnVisibilityOptions,
  byForm: byFormColumnVisibilityOptions,
};

export const quickReportsColumnVisibilityOptions: ColumnOption[] = [
  { id: 'id', label: 'Entry ID', enableHiding: true },
  { id: 'timestamp', label: 'Time submitted', enableHiding: true },
  { id: 'title', label: 'Issue title', enableHiding: true },
  { id: 'description', label: 'Description', enableHiding: true },
  { id: 'numberOfAttachments', label: 'Medial files', enableHiding: true },
  { id: 'number', label: 'Station number', enableHiding: true },
  { id: 'level1', label: 'Location - L1', enableHiding: true },
  { id: 'level2', label: 'Location - L2', enableHiding: true },
  { id: 'level3', label: 'Location - L3', enableHiding: true },
  { id: 'level4', label: 'Location - L4', enableHiding: true },
  { id: 'level5', label: 'Location - L5', enableHiding: true },
  { id: 'observerName', label: 'Observer', enableHiding: true },
];

export const quickReportsDefaultColumns: VisibilityState = {
  id: false,
  timestamp: true,
  title: true,
  description: true,
  numberOfAttachments: true,
  number: true,
  level1: false,
  level2: false,
  level3: false,
  level4: false,
  level5: false,
  observerName: true,
  email: false,
};
