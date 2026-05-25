import Notification from "@/components/Notification";
import { useNotifications } from "@/queries/use-notifications";
import { typography } from "../config/site";

function NotificationsList() {
  const { data: notification } = useNotifications();

  return (
    <div className="flex flex-col gap-12">
      <h1 className={typography.h1}>
        Notifications from {notification?.ngoName}
      </h1>
      <div className="flex flex-col gap-12">
        {notification?.notifications.map((notification) => (
          <Notification
            isInsideList
            id={notification.id}
            sentAt={notification.sentAt}
            title={notification.title}
            body={notification.body}
          />
        ))}
      </div>
    </div>
  );
}

export default NotificationsList;
