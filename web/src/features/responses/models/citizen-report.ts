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
} from '@/common/types';
import { Note, Attachment } from './common';

export interface CitizenReportByEntry {
  email: string;
  formCode: string;
  formType: FormType;
  defaultLanguage: string;
  numberOfFlaggedAnswers: number;
  numberOfQuestionsAnswered: number;
  mediaFilesCount: number;
  notesCount: number;
  citizenReportId: string;
  tags: string[];
  timeSubmitted: string;
  followUpStatus: FollowUpStatus;
}

export interface CitizenReportFormAggregated {
  formId: string;
  formCode: string;
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
