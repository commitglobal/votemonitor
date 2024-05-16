import { useContext } from "react";
import { NotificationContext } from "../contexts/notification/NotificationContext";

export function useNotification() {
  return useContext(NotificationContext);
}
