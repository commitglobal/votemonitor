export type Note = {
  id: string;
  electionRoundId: string;
  submissionId: string;
  questionId: string;
  text: string;
  lastUpdatedAt: string;

  // Offline flag
  isNotSynched?: boolean;
};
