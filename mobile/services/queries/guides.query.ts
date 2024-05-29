import { skipToken, useQuery } from "@tanstack/react-query";
import { GuidesKeys } from "./attachments.query";
import { getGuides } from "../api/get-guides.api";

export const useGuides = (electionRoundId: string | undefined) => {
  return useQuery({
    queryKey: GuidesKeys.guides(electionRoundId),
    queryFn: electionRoundId ? () => getGuides({ electionRoundId }) : skipToken,
  });
};
