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

export const transformElectionRoundsApiToVM = (
  data: ElectionRoundsAPIResponse
): ElectionRoundVM[] => {
  return (
    data.electionRounds?.map((round) => ({
      id: round.id,
      countryId: round.countryId,
      country: round.country,
      title: round.title,
      englishTitle: round.englishTitle,
      startDate: round.startDate,
      status: round.status,
      createdOn: round.createdOn,
      lastModifiedOn: round.lastModifiedOn,
    })) || []
  );
};
