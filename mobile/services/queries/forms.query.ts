import { skipToken, useQuery } from "@tanstack/react-query";
import { electionRoundsKeys } from "../queries.service";
import { ElectionRoundsAllFormsAPIResponse, getElectionRoundAllForms } from "../definitions.api";
import { arrayToKeyObject } from "../../helpers/misc";
import { useCallback } from "react";

const transformAllForms = (data: ElectionRoundsAllFormsAPIResponse) =>
  arrayToKeyObject(data.forms || [], "id");

export const useElectionRoundAllForms = <TResult = ElectionRoundsAllFormsAPIResponse>(
  electionRoundId: string | undefined,
  select: (data: ElectionRoundsAllFormsAPIResponse) => TResult,
) => {
  return useQuery({
    queryKey: electionRoundsKeys.forms(),
    queryFn: electionRoundId ? () => getElectionRoundAllForms(electionRoundId) : skipToken,
    select,
  });
};

export const useFormQuestions = (electionRoundId: string | undefined, formId: string | undefined) =>
  useElectionRoundAllForms(
    electionRoundId,
    useCallback(
      (data: ElectionRoundsAllFormsAPIResponse) => {
        const form = data.forms?.find((form) => form.id === formId);
        return arrayToKeyObject(form?.questions || [], "id");
      },
      [electionRoundId, formId],
    ),
  );

export const useFormTitle = (
  electionRoundId: string | undefined,
  formId: string | undefined,
  language: string,
) =>
  useElectionRoundAllForms(
    electionRoundId,
    useCallback(
      (data: ElectionRoundsAllFormsAPIResponse) => {
        console.log("Creating form title");
        const form = data.forms?.find((form) => form.id === formId);
        return `${form?.code} - ${form?.name[(language as string) || form.defaultLanguage]} (${(language as string) || form?.defaultLanguage})`;
      },
      [electionRoundId, formId, language],
    ),
  );
