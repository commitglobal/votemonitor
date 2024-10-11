import { skipToken, useQuery } from "@tanstack/react-query";
import { GuidesKeys } from "./attachments.query";
import { getGuides, Guide } from "../api/get-guides.api";
import { getCitizenGuides } from "../api/citizen/get-citizen-guides";
import { useCallback } from "react";

export const useGuides = <TResult = Guide[]>(
  electionRoundId: string | undefined,
  select?: (data: Guide[]) => TResult,
) => {
  return useQuery({
    queryKey: GuidesKeys.guides(electionRoundId),
    queryFn: electionRoundId ? () => getGuides({ electionRoundId }) : skipToken,
    select,
  });
};

export const useGuide = (guideId?: string, electionRoundId?: string) => {
  return useGuides(
    electionRoundId,
    useCallback(
      (data: Guide[]) => {
        if (!data || !guideId) return null;
        return data.find((guide) => guide.id === guideId) || null;
      },
      [electionRoundId, guideId],
    ),
  );
};

export const useCitizenGuides = <TResult = Guide[]>(
  electionRoundId?: string | null,
  select?: (data: Guide[]) => TResult,
) => {
  return useQuery({
    queryKey: GuidesKeys.citizenGuides(electionRoundId || undefined),
    queryFn: electionRoundId ? () => getCitizenGuides({ electionRoundId }) : skipToken,
    select,
  });
};

export const useResource = (guideId?: string, electionRoundId?: string | null) => {
  return useCitizenGuides(
    electionRoundId,
    useCallback(
      (data: Guide[]) => {
        if (!data || !guideId) return null;
        return data.find((guide) => guide.id === guideId) || null;
      },
      [electionRoundId, guideId],
    ),
  );
};
