import { useQuery, skipToken } from "@tanstack/react-query";
import { ElectionRoundsAllFormsAPIResponse, getElectionRoundAllForms } from "../definitions.api";
import { electionRoundsKeys } from "../queries.service";

export const useElectionRoundAllForms = <TResult = ElectionRoundsAllFormsAPIResponse>(
  electionRoundId: string | undefined,
  select?: (data: ElectionRoundsAllFormsAPIResponse) => TResult,
) => {
  return useQuery({
    queryKey: electionRoundsKeys.forms(),
    queryFn: electionRoundId ? () => getElectionRoundAllForms(electionRoundId) : skipToken,
    select,
  });
};
