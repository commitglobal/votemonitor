import API from "../api";
import * as Crypto from "expo-crypto";

type GetNotificationsApiPayload = {
  electionRoundId: string;
};

export type Notification = {
  id: string;
  title: string;
  body: string;
  sender: string;
  sentAt: Date;
};

export type NotificationsApiResponse = {
  notifications: Notification[];
};

export const getNotifications = ({
  electionRoundId,
}: GetNotificationsApiPayload): Promise<NotificationsApiResponse> => {
  return API.get(`election-rounds/${electionRoundId}/notifications:listReceived`).then((res) => {
    res.data.notifications = res.data.notifications.map((not: any) => ({
      id: Crypto.randomUUID(),
      ...not,
    }));

    return res.data;
  });
};
