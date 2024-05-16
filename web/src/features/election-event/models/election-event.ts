export type ElectionEventStatus = "NotStarted" | "Started" | "Archived";

export interface ElectionEvent {
  id: string;
  countryId: string;
  country: string;
  title: string;
  englishTitle: string;
  startDate: string;
  status: ElectionEventStatus;
  createdOn: string;
  lastModifiedOn: string;
}