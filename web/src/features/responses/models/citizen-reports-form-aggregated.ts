import type { FormType } from '@/common/types';
import {
  Attachment,
  DateQuestionAggregate,
  MultiSelectQuestionAggregate,
  Note,
  NumberQuestionAggregate,
  RatingQuestionAggregate,
  SingleSelectQuestionAggregate,
  TextQuestionAggregate,
} from './common';

export interface CitizenReportsFormAggregated {
  submissionsAggregate: {
    electionRoundId: string;
    monitoringNgoId: string;
    formId: string;
    formCode: string;
    formType: FormType;
    name: Record<string, string>;
    description: Record<string, string>;
    defaultLanguage: string;
    languages: string[];
    submissionCount: number;
    totalNumberOfQuestionsAnswered: number;
    totalNumberOfFlaggedAnswers: number;
    aggregates: Record<
      string,
      | NumberQuestionAggregate
      | TextQuestionAggregate
      | DateQuestionAggregate
      | RatingQuestionAggregate
      | SingleSelectQuestionAggregate
      | MultiSelectQuestionAggregate
    >;
  };
  attachments: Attachment[];
  notes: Note[];
}
