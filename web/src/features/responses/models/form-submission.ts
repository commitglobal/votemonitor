import type {
  BaseQuestion,
  DateAnswer,
  FormSubmissionFollowUpStatus,
  FormType,
  MultiSelectAnswer,
  NumberAnswer,
  RatingAnswer,
  SingleSelectAnswer,
  TextAnswer,
} from '@/common/types';
import { Attachment, Note } from './common';

export interface FormSubmissionByEntry {
  email: string;
  observerName: string;
  formCode: string;
  formType: FormType;
  defaultLanguage: string;
  monitoringObserverId: string;
  numberOfFlaggedAnswers: number;
  numberOfQuestionsAnswered: number;
  pollingStationId: string;
  level1: string;
  level2: string;
  level3: string;
  level4: string;
  level5: string;
  mediaFilesCount: number;
  notesCount: number;
  number: string;
  phoneNumber: string;
  submissionId: string;
  tags: string[];
  timeSubmitted: string;
  followUpStatus: FormSubmissionFollowUpStatus;
}

export interface FormSubmissionByObserver {
  email: string;
  observerName: string;
  monitoringObserverId: string;
  numberOfFlaggedAnswers: number;
  numberOfFormsSubmitted: number;
  numberOfLocations: number;
  phoneNumber: string;
  tags: string[];
  followUpStatus?: FormSubmissionFollowUpStatus;
}

export interface FormSubmissionByForm {
  formId: string;
  formCode: string;
  formType: FormType;
  numberOfSubmissions: number;
  numberOfFlaggedAnswers: number;
  numberOfNotes: number;
  numberOfMediaFiles: number;
}

export interface FormSubmission
  extends Omit<
    FormSubmissionByEntry,
    'numberOfFlaggedAnswers' | 'numberOfQuestionAnswered' | 'mediaFilesCount' | 'notesCount'
  > {
  answers: (NumberAnswer | TextAnswer | DateAnswer | RatingAnswer | SingleSelectAnswer | MultiSelectAnswer)[];
  needsFollowup?: boolean;
  notes: Note[];
  attachments: Attachment[];
  questions: BaseQuestion[];
}

export interface TimestampsFilterOptions {
  firstSubmissionTimestamp: string;
  lastSubmissionTimestamp: string;
}

export interface FormFilterOption {
  formId: string;
  formCode: string;
  formName: string;
}

export interface FormSubmissionsFilters  {
  timestampsFilterOptions: TimestampsFilterOptions;
  formFilterOptions: FormFilterOption[];
}