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
