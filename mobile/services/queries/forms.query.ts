import { useQuery, skipToken } from "@tanstack/react-query";
import { ElectionRoundsAllFormsAPIResponse, getElectionRoundAllForms } from "../definitions.api";
import { electionRoundsKeys } from "../queries.service";
import { useCallback } from "react";

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

export const useFormById = (electionRoundId: string | undefined, formId: string) =>
  useElectionRoundAllForms(
    electionRoundId,
    useCallback(
      (data: ElectionRoundsAllFormsAPIResponse) => {
        return data.forms.find((form) => form.id === formId);
      },
      [electionRoundId],
    ),
  );
