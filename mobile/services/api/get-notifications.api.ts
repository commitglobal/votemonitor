import API from "../api";

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
  ngoName: string;
  notifications: Notification[];
};

export const getNotifications = ({
  electionRoundId,
}: GetNotificationsApiPayload): Promise<NotificationsApiResponse> => {
  return API.get(`election-rounds/${electionRoundId}/notifications:listReceived`).then(
    (res) => res.data,
  );
};
