import { electionRoundId } from "@/lib/utils";
import { API } from "./api";
import type { NotificationsModel } from "@/common/types";

export const getNotifications = (): Promise<NotificationsModel> => {
  return API.get(
    `/api/election-rounds/${electionRoundId}/citizen-notifications:listReceived`
  ).then((res) => res.data);
};
