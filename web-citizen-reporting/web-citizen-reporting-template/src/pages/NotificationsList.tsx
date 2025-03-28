import { useNotifications } from "@/queries/use-notifications";

function NotificationsList() {
  const { data: notification } = useNotifications();

  return <>{JSON.stringify(notification)}</>;
}

export default NotificationsList;
