import API from "../../api";

export type SubscribeNotificationsAPIPayload = {
  token: string;
};

export const subscribeToPushNotifications = ({
  token,
}: SubscribeNotificationsAPIPayload): Promise<void> => {
  return API.post(`notifications:subscribe`, { token }).then((res) => res.data);
};

export const unsubscribePushNotifications = (): Promise<void> => {
  return API.post(`notifications:unsubscribe`).then((res) => res.data);
};
