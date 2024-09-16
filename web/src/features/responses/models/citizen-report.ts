import type {
  BaseQuestion,
  DateAnswer,
  FollowUpStatus,
  FormType,
  MultiSelectAnswer,
  NumberAnswer,
  RatingAnswer,
  SingleSelectAnswer,
  TextAnswer,
  TranslatedString,
} from '@/common/types';
import { Note, Attachment } from './common';

export interface CitizenReportByEntry {
  citizenReportId: string;
  formCode: string;
  formDefaultLanguage: string;
  formName: TranslatedString;
  timeSubmitted: Date;
  numberOfQuestionsAnswered: number;
  numberOfFlaggedAnswers: number;
  notesCount: number;
  mediaFilesCount: number;
  followUpStatus: FollowUpStatus;
}

export interface CitizenReportsAggregatedByForm {
  formId: string;
  formCode: string;
  formName: TranslatedString;
  formDefaultLanguage: string;
  defaultLanguage: string;
  numberOfSubmissions: number;
  numberOfFlaggedAnswers: number;
  numberOfNotes: number;
  numberOfMediaFiles: number;
}

export interface CitizenReport
  extends Omit<
    CitizenReportByEntry,
    'numberOfFlaggedAnswers' | 'numberOfQuestionAnswered' | 'mediaFilesCount' | 'notesCount'
  > {
  answers: (NumberAnswer | TextAnswer | DateAnswer | RatingAnswer | SingleSelectAnswer | MultiSelectAnswer)[];
  attachments: Attachment[];
  needsFollowup?: boolean;
  notes: Note[];
  questions: BaseQuestion[];
}
