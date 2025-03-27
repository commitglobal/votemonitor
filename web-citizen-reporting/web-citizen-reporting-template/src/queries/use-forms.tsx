import { getForms } from "@/api/get-forms";
import type { FormModel } from "@/common/types";
import { electionRoundId } from "@/lib/utils";
import { skipToken, useQuery } from "@tanstack/react-query";
const STALE_TIME = 1000 * 60 * 15; // 15 minutes

export const useForms = <TResult = Array<FormModel>,>(
  select?: (elections: Array<FormModel>) => TResult
) => {
  return useQuery({
    queryKey: ["forms"],
    placeholderData: [],
    queryFn: electionRoundId ? () => getForms() : skipToken,
    select,
    staleTime: STALE_TIME,
  });
};
