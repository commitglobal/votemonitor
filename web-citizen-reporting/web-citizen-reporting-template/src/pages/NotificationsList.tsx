import Notification from "@/components/notifications";
import { useNotifications } from "@/queries/use-notifications";
import { typographyClasses } from "../config/site";

function NotificationsList() {
  const { data: notification } = useNotifications();

  return (
    <div className="flex flex-col gap-12">
      <h1 className={typographyClasses.h1}>
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
