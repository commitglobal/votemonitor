import type {
  BaseQuestion,
  DateAnswer,
  IncidentReportFollowUpStatus,
  MultiSelectAnswer,
  NumberAnswer,
  RatingAnswer,
  SingleSelectAnswer,
  TextAnswer,
  TranslatedString,
} from '@/common/types';
import { Attachment, Note } from './common';

export enum IncidentReportLocationType {
  PollingStation = 'PollingStation',
  OtherLocation = 'OtherLocation',
}

export interface IncidentReportByEntry {
  incidentReportId: string;
  email: string;
  observerName: string;
  formCode: string;
  formName: TranslatedString;
  formDefaultLanguage: string;
  monitoringObserverId: string;
  numberOfFlaggedAnswers: number;
  numberOfQuestionsAnswered: number;
  locationType: IncidentReportLocationType;
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
  followUpStatus: IncidentReportFollowUpStatus;
}

export interface IncidentReportByObserver {
  email: string;
  observerName: string;
  monitoringObserverId: string;
  numberOfFlaggedAnswers: number;
  numberOfIncidentsSubmitted: number;
  phoneNumber: string;
  tags: string[];
  followUpStatus?: IncidentReportFollowUpStatus;
}

export interface IncidentReportByForm {
  formId: string;
  formCode: string;
  formName: TranslatedString;
  formDefaultLanguage: string;
  numberOfSubmissions: number;
  numberOfFlaggedAnswers: number;
  numberOfNotes: number;
  numberOfMediaFiles: number;
}

export interface IncidentReport
  extends Omit<
    IncidentReportByEntry,
    'numberOfFlaggedAnswers' | 'numberOfQuestionAnswered' | 'mediaFilesCount' | 'notesCount'
  > {
  answers: (NumberAnswer | TextAnswer | DateAnswer | RatingAnswer | SingleSelectAnswer | MultiSelectAnswer)[];
  needsFollowup?: boolean;
  notes: Note[];
  attachments: Attachment[];
  questions: BaseQuestion[];
}
