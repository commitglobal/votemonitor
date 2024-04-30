import { skipToken, useQuery } from "@tanstack/react-query";
import { electionRoundsKeys } from "../queries.service";
import { ElectionRoundsAllFormsAPIResponse, getElectionRoundAllForms } from "../definitions.api";
import { arrayToKeyObject } from "../../helpers/misc";

const transformAllForms = (data: ElectionRoundsAllFormsAPIResponse) =>
  arrayToKeyObject(data.forms || [], "id");

export const useElectionRoundAllForms = (electionRoundId: string | undefined) => {
  return useQuery({
    queryKey: electionRoundsKeys.forms(),
    queryFn: electionRoundId ? () => getElectionRoundAllForms(electionRoundId) : skipToken,
    select: transformAllForms,
  });
};
