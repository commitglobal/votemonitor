export type Note = {
  id: string;
  electionRoundId: string;
  pollingStationId: string;
  formId: string;
  questionId: string;
  text: string;
  lastUpdatedAt: string;

  // Offline flag
  isNotSynched?: boolean;
};
