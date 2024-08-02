import { useQuery, skipToken } from "@tanstack/react-query";
import {
  ElectionRoundsAllFormsAPIResponse,
  getElectionRoundAllForms,
  getElectionRoundFormById,
} from "../definitions.api";
import { electionRoundsKeys } from "../queries.service";

export const useElectionRoundAllForms = <TResult = ElectionRoundsAllFormsAPIResponse>(
  electionRoundId: string | undefined,
  select?: (data: ElectionRoundsAllFormsAPIResponse) => TResult,
) => {
  return useQuery({
    queryKey: electionRoundsKeys.forms(electionRoundId),
    queryFn: electionRoundId ? () => getElectionRoundAllForms(electionRoundId) : skipToken,
    select,
  });
};

export const useFormById = (electionRoundId: string | undefined, formId: string) => {
  return useQuery({
    queryKey: electionRoundsKeys.form(electionRoundId, formId),
    queryFn:
      electionRoundId && formId
        ? () => getElectionRoundFormById(electionRoundId, formId)
        : skipToken,
  });
};
