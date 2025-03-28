import Notification from "@/components/notifications";
import { useNotifications } from "@/queries/use-notifications";
import { Route } from "@/routes/notifications/$notificationId";

function NotificationDetails() {
  const { notificationId } = Route.useParams();
  const { data: notification } = useNotifications((notification) =>
    notification.notifications.find((n) => n.id == notificationId)
  );

  if (!notification) return <></>;

  return (
    <Notification
      id={notification.id}
      sentAt={notification.sentAt}
      title={notification.title}
      body={notification.body}
    />
  );
}

export default NotificationDetails;
