import type {
  BaseQuestion,
  DateAnswer,
  IssueReportFollowUpStatus,
  MultiSelectAnswer,
  NumberAnswer,
  RatingAnswer,
  SingleSelectAnswer,
  TextAnswer,
  TranslatedString,
} from '@/common/types';
import { Attachment, Note } from './common';

export enum IssueReportLocationType {
  PollingStation = 'PollingStation',
  OtherLocation = 'OtherLocation',
}

export interface IssueReportByEntry {
  issueReportId: string;
  email: string;
  observerName: string;
  formCode: string;
  formName: TranslatedString;
  formDefaultLanguage: string;
  monitoringObserverId: string;
  numberOfFlaggedAnswers: number;
  numberOfQuestionsAnswered: number;
  locationType: IssueReportLocationType;
  locationDescription?: string;
  pollingStationId?: string;
  level1?: string;
  level2?: string;
  level3?: string;
  level4?: string;
  level5?: string;
  pollingStationNumber?: string;
  mediaFilesCount: number;
  notesCount: number;
  phoneNumber: string;
  tags: string[];
  timeSubmitted: string;
  followUpStatus: IssueReportFollowUpStatus;
}

export interface IssueReportByObserver {
  email: string;
  observerName: string;
  monitoringObserverId: string;
  numberOfFlaggedAnswers: number;
  numberOfIssuesSubmitted: number;
  phoneNumber: string;
  tags: string[];
  followUpStatus?: IssueReportFollowUpStatus;
}

export interface IssueReportByForm {
  formId: string;
  formCode: string;
  formName: TranslatedString;
  formDefaultLanguage: string;
  numberOfSubmissions: number;
  numberOfFlaggedAnswers: number;
  numberOfNotes: number;
  numberOfMediaFiles: number;
}

export interface IssueReport
  extends Omit<
    IssueReportByEntry,
    'numberOfFlaggedAnswers' | 'numberOfQuestionAnswered' | 'mediaFilesCount' | 'notesCount'
  > {
  answers: (NumberAnswer | TextAnswer | DateAnswer | RatingAnswer | SingleSelectAnswer | MultiSelectAnswer)[];
  needsFollowup?: boolean;
  notes: Note[];
  attachments: Attachment[];
  questions: BaseQuestion[];
}
