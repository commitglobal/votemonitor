import { createContext } from 'react';

interface NotificationContextProps {
  unsubscribe: () => void;
}

export const NotificationContext = createContext<NotificationContextProps>({
  unsubscribe: () => {},
});
