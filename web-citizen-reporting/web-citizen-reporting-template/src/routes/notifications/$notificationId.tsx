import NotFound from "@/pages/NotFound";
import NotificationDetails from "@/pages/NotificationDetails";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/notifications/$notificationId")({
  component: NotificationDetails,
  notFoundComponent: NotFound,
});
