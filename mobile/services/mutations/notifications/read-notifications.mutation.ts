import { useMutation, useQueryClient } from "@tanstack/react-query";
import { readNotifications } from "../../api/notifications/notifications-read.api";
import { NotificationsKeys } from "../../queries/notifications.query";
import {
  NotificationsApiResponse,
  Notification,
} from "../../api/notifications/notifications-get.api";

export const useReadNotifications = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      electionRoundId,
      notificationIds,
    }: {
      electionRoundId: string;
      notificationIds: string[];
    }) => readNotifications(electionRoundId, { notificationIds }),
    onMutate: async ({ electionRoundId, notificationIds }) => {
      // Cancel any outgoing refetches
      // (so they don't overwrite our optimistic update)
      await queryClient.cancelQueries({
        queryKey: NotificationsKeys.notifications(electionRoundId),
      });

      // Snapshot the previous value
      const previousNotifications = queryClient.getQueryData(
        NotificationsKeys.notifications(electionRoundId),
      );

      // Optimistically update to the new value, where if a notification id can be found in the array of notification ids to be read, we set the isRead to true
      queryClient.setQueryData(
        NotificationsKeys.notifications(electionRoundId),
        (prevNotifications: NotificationsApiResponse) => {
          if (!prevNotifications || !prevNotifications.notifications) return prevNotifications;

          return {
            ...prevNotifications,
            notifications: prevNotifications.notifications.map((notification: Notification) => {
              if (notificationIds.includes(notification.id)) {
                return { ...notification, isRead: true };
              }
              return notification;
            }),
          };
        },
      );

      // Return a context object with the snapshotted value
      return { previousNotifications };
    },
    onError: (error, variables, context) => {
      console.log("ERROR WHILE READING NOTIFICATIONS ⛔️", error);
      // reset the query data to the previous notifications
      if (context?.previousNotifications) {
        queryClient.setQueryData(
          NotificationsKeys.notifications(variables.electionRoundId),
          context.previousNotifications,
        );
      }
    },
  });
};
