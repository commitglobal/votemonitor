import React, { useCallback, useEffect, useRef, useState } from "react";
import { registerForPushNotificationsAsync } from "../../common/utils/notifications";
import * as Notifications from "expo-notifications";
// import { registerPushToken, unregisterPushToken } from '../../services/settings/settings.api';
import { NotificationContext } from "./NotificationContext";

import { useAuth } from "../../hooks/useAuth";
import { subscribeToPushNotifications } from "../../services/api/notifications/notifications-subscribe.api";

import * as Sentry from "@sentry/react-native";

const NotificationContextProvider = ({
  children,
  navigation,
}: {
  children: React.ReactNode;
  navigation?: any;
}) => {
  const [pushToken, setPushToken] = useState<string>();

  // notifications
  const notificationListener = useRef<Notifications.Subscription>();
  const responseListener = useRef<Notifications.Subscription>();

  const { isAuthenticated } = useAuth();

  const [isRegistered, setIsRegistered] = useState<boolean>(false);

  const initNotifications = async () => {
    if (pushToken) {
      // Already registered
      return;
    }

    const token = await registerForPushNotificationsAsync();
    setPushToken(token);

    if (!token) {
      throw new Error("No token aquired");
    }

    try {
      await subscribeToPushNotifications({ token });
    } catch (err) {
      console.log(err);
      Sentry.captureException(err);
      throw err;
    }

    setIsRegistered(true);

    responseListener.current = Notifications.addNotificationResponseReceivedListener(
      (response: any) => {
        const {
          notification: {
            request: {
              content: { data: payload },
            },
          },
        } = response;

        console.log("🚀🚀🚀🚀 NOTIFICATION payload", payload);
      },
    ) as any;

    return () => {
      Notifications.removeNotificationSubscription(
        notificationListener.current as Notifications.Subscription,
      );
      Notifications.removeNotificationSubscription(
        responseListener.current as Notifications.Subscription,
      );
    };
  };

  const init = useCallback(initNotifications, [initNotifications]);

  useEffect(() => {
    // const state = navigation.getRootState();
    // if (isAuthenticated && !isRegistered && state?.routeNames?.includes('home')) {
    if (isAuthenticated) {
      init();
    }
  }, [navigation, init, isAuthenticated, isRegistered]);

  useEffect(() => {
    if (!isAuthenticated) {
      setIsRegistered(false);
    }
  }, [isAuthenticated]);

  const unsubscribe = () => {
    try {
      if (pushToken) {
        // TODO: call api to unsubscribe
        // unregisterPushToken(pushToken);
      }
    } catch (error) {
      console.error("Error while unregistering push token");
    }
  };

  return (
    <NotificationContext.Provider
      value={{
        unsubscribe,
      }}
    >
      {children}
    </NotificationContext.Provider>
  );
};

export default NotificationContextProvider;
