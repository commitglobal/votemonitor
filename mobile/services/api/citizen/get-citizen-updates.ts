import API from "../../api";
import {
  GetNotificationsApiPayload,
  NotificationsApiResponse,
} from "../notifications/notifications-get.api";

export const getCitizenUpdates = ({
  electionRoundId,
}: GetNotificationsApiPayload): Promise<Omit<NotificationsApiResponse, "ngoName">> => {
  return API.get(`election-rounds/${electionRoundId}/citizen-notifications:listReceived`).then(
    (res) => res.data,
  );
};
