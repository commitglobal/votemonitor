import Notification from "@/components/notifications";
import { useNotifications } from "@/queries/use-notifications";
import { typographyClasses } from "@/routes/typography";

function NotificationsList() {
  const { data: notification } = useNotifications();

  return (
    <div>
      <h1 className={`${typographyClasses.h1} mb-16`}>
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
