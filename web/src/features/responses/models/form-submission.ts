import type {
  BaseQuestion,
  DateAnswer,
  MultiSelectAnswer,
  NumberAnswer,
  RatingAnswer,
  SingleSelectAnswer,
  TextAnswer,
} from '@/common/types';

export enum FormType {
  ClosingAndCounting = 'ClosingAndCounting',
  Opening = 'Opening',
  Voting = 'Voting',
}

export enum FormStatus {
  Drafted = 'Drafted',
  Obsolete = 'Obsolete',
  Published = 'Published',
}

export interface FormSubmissionByEntry {
  email: string;
  firstName: string;
  formCode: string;
  formType: FormType;
  lastName: string;
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
  status?: string;
}

export interface FormSubmissionByObserver {
  email: string;
  firstName: string;
  lastName: string;
  monitoringObserverId: string;
  numberOfFlaggedAnswers: number;
  numberOfFormsSubmitted: number;
  numberOfLocations: number;
  phoneNumber: string;
  tags: string[];
  status?: string;
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

interface BaseQuestionExtraData {
  monitoringObserverId: string;
  questionId: string;
  timeSubmitted: string;
}

export interface Note extends BaseQuestionExtraData {
  text: string;
}

export interface Attachment extends BaseQuestionExtraData {
  fileName: string;
  filePath: string;
  mimeType: string;
  presignedUrl: string;
  uploadedFileName: string;
  urlValidityInSeconds: string;
}

export interface FormSubmission
  extends Omit<
    FormSubmissionByEntry,
    'numberOfFlaggedAnswers' | 'numberOfQuestionAnswered' | 'mediaFilesCount' | 'notesCount'
  > {
  answers: (NumberAnswer | TextAnswer | DateAnswer | RatingAnswer | SingleSelectAnswer | MultiSelectAnswer)[];
  attachments: Attachment[];
  notes: Note[];
  questions: BaseQuestion[];
}
