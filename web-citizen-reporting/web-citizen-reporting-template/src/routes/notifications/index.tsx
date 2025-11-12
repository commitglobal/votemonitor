import NotificationsList from "@/pages/NotificationsList";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/notifications/")({
  component: NotificationsList,
});
