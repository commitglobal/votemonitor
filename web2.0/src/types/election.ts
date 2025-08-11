import { z } from "zod";
import { SortOrder } from "./common";

export enum ElectionRoundStatus {
  Archived = "Archived",
  Started = "Started",
  NotStarted = "NotStarted",
}

export interface ElectionModel {
  id: string;
  countryId: string;
  countryIso2: string;
  countryIso3: string;
  countryNumericCode: string;
  countryName: string;
  countryFullName: string;
  title: string;
  englishTitle: string;
  startDate: string;
  createdOn: string;
  lastModifiedOn: string;
  status: ElectionRoundStatus;
}

export const electionsSearchSchema = z.object({
  countryId: z.string().optional(),
  searchText: z.string().optional(),
  electionRoundStatus: z.enum(ElectionRoundStatus).optional(),
  sortColumnName: z.string().optional(),
  sortOrder: z.enum(SortOrder).optional(),
  pageNumber: z.number().default(1),
  pageSize: z.number().default(25),
});

export type ElectionsSearch = z.infer<typeof electionsSearchSchema>;
