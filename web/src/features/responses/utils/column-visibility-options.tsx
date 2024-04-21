import type { VisibilityState } from '@tanstack/react-table';

export type FilterBy = 'byEntry' | 'byObserver' | 'byForm';

export const formSubmissionsByEntryDefaultColumns: VisibilityState = {
  id: false,
  submittedAt: true,
  formType: true,
  pollingStationNumber: true,
  pollingStationLevel1: false,
  pollingStationLevel2: false,
  pollingStationLevel3: false,
  observerName: false,
  numberOfQuestionAnswered: true,
  numberOfFlaggedAnswers: true,
};

export const formSubmissionsByObserverDefaultColumns: VisibilityState = {
  observerName: true,
  tags: true,
  numberOfFormsSubmitted: true,
  numberOfFlaggedAnswers: true,
};

export const formSubmissionsByFormDefaultColumns: VisibilityState = {
  name: true,
  formType: true,
  defaultLanguage: true,
  totalNumberOfQuestionsAnswered: true,
  totalNumberOfFlaggedAnswers: true,
};

export const formSubmissionsDefaultColumns: Record<FilterBy, VisibilityState> = {
  byEntry: formSubmissionsByEntryDefaultColumns,
  byObserver: formSubmissionsByObserverDefaultColumns,
  byForm: formSubmissionsByFormDefaultColumns,
};

type ColumnOption = { id: string; label: string; enableHiding: boolean };

const byEntryColumnVisibilityOptions: ColumnOption[] = [
  { id: 'id', label: 'Entry ID', enableHiding: true },
  { id: 'submittedAt', label: 'Time submitted', enableHiding: true },
  { id: 'formType', label: 'Form type', enableHiding: true },
  { id: 'pollingStationNumber', label: 'Station number', enableHiding: true },
  { id: 'pollingStationLevel1', label: 'Location - L1', enableHiding: true },
  { id: 'pollingStationLevel2', label: 'Location - L2', enableHiding: true },
  { id: 'pollingStationLevel3', label: 'Location - L3', enableHiding: true },
  { id: 'observerName', label: 'Observer', enableHiding: true },
  { id: 'numberOfQuestionAnswered', label: 'Responses', enableHiding: true },
  { id: 'numberOfFlaggedAnswers', label: 'Flagged answers', enableHiding: true },
];

const byObserverColumnVisibilityOptions: ColumnOption[] = [
  { id: 'observerName', label: 'Observer name', enableHiding: false },
  { id: 'tags', label: 'Observer tags', enableHiding: true },
  { id: 'numberOfFormsSubmitted', label: 'Forms', enableHiding: false },
  { id: 'numberOfFlaggedAnswers', label: 'Flagged answers', enableHiding: true },
];

const byFormColumnVisibilityOptions: ColumnOption[] = [
  { id: 'name', label: 'Form name', enableHiding: false },
  { id: 'formType', label: 'Form type', enableHiding: false },
  { id: 'defaultLanguage', label: 'Language', enableHiding: true },
  { id: 'totalNumberOfQuestionsAnswered', label: 'Responses', enableHiding: true },
  { id: 'totalNumberOfFlaggedAnswers', label: 'Flagged answers', enableHiding: true },
];

export const columnVisibilityOptions: Record<FilterBy, ColumnOption[]> = {
  byEntry: byEntryColumnVisibilityOptions,
  byObserver: byObserverColumnVisibilityOptions,
  byForm: byFormColumnVisibilityOptions,
};
