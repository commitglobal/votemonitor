import { skipToken, useQuery } from "@tanstack/react-query";
import { getNotifications } from "../api/notifications/notifications-get.api";
import { getCitizenUpdates } from "../api/citizen/get-citizen-updates";
import { citizenQueryKeys } from "./citizen.query";

export const NotificationsKeys = {
  notifications: (electionRoundId: string | undefined) =>
    ["notifications", "electionRoundId", electionRoundId] as const,
  citizenUpdates: (electionRoundId: string | undefined) =>
    [...citizenQueryKeys.all, "updates", "electionRoundId", electionRoundId] as const,
};

export const useNotifications = (electionRoundId: string | undefined) => {
  return useQuery({
    queryKey: NotificationsKeys.notifications(electionRoundId),
    queryFn: electionRoundId ? () => getNotifications({ electionRoundId }) : skipToken,
    staleTime: 0,
    refetchOnReconnect: "always",
    refetchOnWindowFocus: "always",
  });
};

export const useCitizenUpdates = (electionRoundId?: string) => {
  return useQuery({
    queryKey: NotificationsKeys.citizenUpdates(electionRoundId),
    queryFn: electionRoundId ? () => getCitizenUpdates({ electionRoundId }) : skipToken,
    staleTime: 0,
    refetchOnReconnect: "always",
    refetchOnWindowFocus: "always",
  });
};
