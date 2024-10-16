import API from "../../api";

export const readNotifications = (
  electionRoundId: string,
  body: {
    notificationIds: string[];
  },
): Promise<void> => {
  return API.put(`/election-rounds/${electionRoundId}/notifications:read`, body).then(
    (res) => res.data,
  );
};
