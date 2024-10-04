export type IncidentReportNote = {
  id: string;
  electionRoundId: string;
  incidentReportId: string;
  formId: string;
  questionId: string;
  text: string;
  createdAt: string;
  updatedAt: string | null;

  // Offline flag
  isNotSynched?: boolean;
};
