// import API from "../../api";
import {
  GetNotificationsApiPayload,
  NotificationsApiResponse,
} from "../notifications/notifications-get.api";

export const getCitizenUpdates = ({
  electionRoundId,
}: GetNotificationsApiPayload): Promise<Omit<NotificationsApiResponse, "ngoName">> => {
  //   return API.get(`election-rounds/${electionRoundId}/notifications:listReceived`).then(
  //     (res) => res.data,
  //   );

  console.log("😒 electionRoundId", electionRoundId);
  return Promise.resolve({
    notifications: [
      {
        id: "7e333bc1-b2dd-490a-ad1f-5dddc6954714",
        title: "Notificare noua test",
        body: "Notificare noua",
        sender: "Admin Alfa",
        sentAt: new Date("2024-10-08T11:07:39.943554Z"),
        isRead: true,
      },
      {
        id: "2920ea32-e2e6-4566-a2ec-e342281a537b",
        title: "Notificare noua test",
        body: "Notificare noua",
        sender: "Admin Alfa",
        sentAt: new Date("2024-10-08T11:03:06.878117Z"),
        isRead: true,
      },
      {
        id: "735da3be-8f2e-4769-9d90-6ede965d4f37",
        title: "Notificare noua test",
        body: "Notificare noua",
        sender: "Admin Alfa",
        sentAt: new Date("2024-10-08T10:18:54.256776Z"),
        isRead: true,
      },
      {
        id: "0741d5bf-bec9-43ac-b0f0-879a5b3dad73",
        title: "string",
        body: "<h1>Hello, this is me , html message!<br></h1><p class=",
        sender: "Admin Alfa",
        sentAt: new Date("2024-05-15T13:27:53.872885Z"),
        isRead: true,
      },
    ],
  });
};
