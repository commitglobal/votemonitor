import { getGuides } from "@/api/get-guides";
import type { GuideModel } from "@/common/types";
import { electionRoundId } from "@/lib/utils";
import { skipToken, useQuery } from "@tanstack/react-query";

export const useGuides = <TResult = Array<GuideModel>,>(
  select?: (elections: Array<GuideModel>) => TResult
) => {
  return useQuery({
    queryKey: ["guides"],
    placeholderData: [],
    queryFn: electionRoundId ? () => getGuides() : skipToken,
    select,
    staleTime: Infinity,
  });
};
