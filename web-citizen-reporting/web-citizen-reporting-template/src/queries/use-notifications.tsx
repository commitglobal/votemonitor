import { getNotifications } from "@/api/get-notifications";
import type { NotificationsModel } from "@/common/types";
import { electionRoundId } from "@/lib/utils";
import { skipToken, useQuery } from "@tanstack/react-query";
const STALE_TIME = 1000 * 60 * 15; // 15 minutes

export const useNotifications = <TResult = NotificationsModel,>(
  select?: (elections: NotificationsModel) => TResult
) => {
  return useQuery({
    queryKey: ["notifications"],
    queryFn: electionRoundId ? () => getNotifications() : skipToken,
    select,
    staleTime: STALE_TIME,
  });
};
