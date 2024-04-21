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
  id: string;
  firstName: string;
  formCode: string;
  formType: FormType;
  lastName: string;
  monitoringObserverId: string;
  numberOfFlaggedAnswers: number;
  numberOfQuestionAnswered: number;
  pollingStationId: string;
  pollingStationLevel1: string;
  pollingStationLevel2: string;
  pollingStationLevel3: string;
  pollingStationLevel4: string;
  pollingStationLevel5: string;
  pollingStationNumber: string;
  submissionId: string;
  submittedAt: string;
}

export interface FormSubmissionByObserver {
  firstName: string;
  lastActivity: string;
  lastName: string;
  monitoringObserverId: string;
  numberOfFlaggedAnswers: number;
  numberOfFormsSubmitted: number;
  numberOfNotes: number;
  numberOfQuestionAnswered: number;
  numberOfUploads: number;
  tags: string[];
}

export interface FormSubmissionByForm {
  electionRoundId: string;
  monitoringNgoId: string;
  formId: string;
  formCode: string;
  formType: FormType;
  name: string;
  description: string;
  defaultLanguage: string;
  languages: string[];
  responders: {
    responderId: string;
    firstName: string;
    lastName: string;
    email: string;
    phoneNumber: string;
  }[];
  pollingStations: Record<string, string[]>;
  submissionCount: number;
  totalNumberOfQuestionsAnswered: number;
  totalNumberOfFlaggedAnswers: number;
  aggregates: Record<
    string,
    {
      questionId: string;
      answersAggregated: number;
      responders: string[];
    }
  >;
}
