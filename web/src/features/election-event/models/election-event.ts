export type ElectionEventStatus = "NotStarted" | "Started" | "Archived";

export interface ElectionEvent {
  id: string;
  title: string;
  englishTitle: string;
  startDate: string;
  status: ElectionEventStatus;
  countryId: string;
  countryIso2: string
  countryIso3: string
  countryNumericCode: string
  countryName: string
  countryFullName: string
  createdOn: string;
  lastModifiedOn: string;
}