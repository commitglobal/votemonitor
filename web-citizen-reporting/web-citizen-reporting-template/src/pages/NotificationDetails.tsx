import { useNotifications } from "@/queries/use-notifications";
import { Route } from "@/routes/notifications/$notificationId";

function NotificationDetails() {
  const { notificationId } = Route.useParams();
  const { data: notification } = useNotifications((notification) =>
    notification.notifications.find((n) => n.id == notificationId)
  );

  return <>{JSON.stringify(notification)}</>;
}

export default NotificationDetails;
