import { skipToken, useQuery } from "@tanstack/react-query";
import { GuidesKeys } from "./attachments.query";
import { getGuides } from "../api/get-guides.api";
import { getCitizenGuides } from "../api/citizen/get-citizen-guides";

export const useGuides = (electionRoundId: string | undefined) => {
  return useQuery({
    queryKey: GuidesKeys.guides(electionRoundId),
    queryFn: electionRoundId ? () => getGuides({ electionRoundId }) : skipToken,
  });
};

export const useCitizenGuides = (electionRoundId?: string | null) => {
  return useQuery({
    queryKey: GuidesKeys.citizenGuides(electionRoundId || undefined),
    queryFn: electionRoundId ? () => getCitizenGuides({ electionRoundId }) : skipToken,
  });
};
