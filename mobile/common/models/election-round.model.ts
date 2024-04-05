import { ElectionRoundsAPIResponse } from "../../services/definitions.api";

export type ElectionRoundVM = {
  id: string;
  countryId: string;
  country: string;
  title: string;
  englishTitle: string;
  startDate: string;
  status: "Archived" | "NotStarted" | "Started";
  createdOn: string;
  lastModifiedOn: string | null;
};
