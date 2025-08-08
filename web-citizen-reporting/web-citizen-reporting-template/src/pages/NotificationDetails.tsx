import Notification from "@/components/Notification";

import { Spinner } from "@/components/Spinner";
import { useNotifications } from "@/queries/use-notifications";
import { Route } from "@/routes/notifications/$notificationId";
import { notFound } from "@tanstack/react-router";

function NotificationDetails() {
  const { notificationId } = Route.useParams();
  const { data: notification, isLoading } = useNotifications((notification) =>
    notification.notifications.find((n) => n.id == notificationId)
  );
  if (isLoading) return <Spinner show={isLoading} />;

  if (!notification) throw notFound();

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
