export type Note = {
  id: string;
  electionRoundId: string;
  pollingStationId: string;
  formId: string;
  questionId: string;
  text: string;
  createdAt: string;
  updatedAt: string | null;
};
