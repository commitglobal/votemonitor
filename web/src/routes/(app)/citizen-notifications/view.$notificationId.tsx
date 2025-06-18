import { authApi } from "@/common/auth-api";
import CitizenNotificationDetails from "@/features/CitizenNotifications/CitizenNotificationDetails/CitizenNotificationDetails";
import { citizenNotificationsKeys } from "@/features/CitizenNotifications/hooks/citizen-notifications-queries";
import { CitizenNotificationModel } from "@/features/CitizenNotifications/models/citizen-notification";
import { redirectIfNotAuth } from "@/lib/utils";
import { queryOptions } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";

export const citizenNotificationQueryOptions = (electionRoundId: string, pushMessageId: string) => {
  return queryOptions({
    queryKey: citizenNotificationsKeys.detail(electionRoundId, pushMessageId),
    queryFn: async () => {

      const response = await authApi.get<CitizenNotificationModel>(
        `/election-rounds/${electionRoundId}/citizen-notifications/${pushMessageId}`
      );

      if (response.status !== 200) {
        throw new Error('Failed to fetch citizen notification details');
      }

      return response.data;
    },
    enabled: !!electionRoundId
  });
}


export const Route = createFileRoute('/(app)/citizen-notifications/view/$notificationId')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: CitizenNotificationDetails,
  loader: ({ context: { queryClient, currentElectionRoundContext }, params: { notificationId } }) => {
    const electionRoundId = currentElectionRoundContext.getState().currentElectionRoundId;

    return queryClient.ensureQueryData(citizenNotificationQueryOptions(electionRoundId, notificationId));
  }
});
