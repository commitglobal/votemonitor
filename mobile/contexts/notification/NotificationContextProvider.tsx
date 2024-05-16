import React, { useCallback, useEffect, useRef, useState } from "react";
import { registerForPushNotificationsAsync } from "../../common/utils/notifications";
import * as Notifications from "expo-notifications";
// import { registerPushToken, unregisterPushToken } from '../../services/settings/settings.api';
import { NotificationContext } from "./NotificationContext";

import { useAuth } from "../../hooks/useAuth";
import {
  subscribeToPushNotifications,
  unsubscribePushNotifications,
} from "../../services/api/notifications/notifications-subscribe.api";

import * as Sentry from "@sentry/react-native";

import { router } from "expo-router";

Notifications.setNotificationHandler({
  handleNotification: async () => ({
    shouldShowAlert: true,
    shouldPlaySound: true,
    shouldSetBadge: false,
  }),
});

const NotificationContextProvider = ({ children }: { children: React.ReactNode }) => {
  const [pushToken, setPushToken] = useState<string | undefined>();

  const notificationListener = useRef<Notifications.Subscription>();
  const responseListener = useRef<Notifications.Subscription>();

  const { isAuthenticated } = useAuth();

  const initNotifications = async () => {
    if (pushToken) {
      // Already registered
      // TODO: save token in storage?
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
  };

  const init = useCallback(initNotifications, [initNotifications]);

  useEffect(() => {
    if (isAuthenticated) {
      init();
    }
  }, [isAuthenticated]);

  useEffect(() => {
    // notificationListener.current = Notifications.addNotificationReceivedListener(
    //   async (notification) => {
    //     console.log(notification);
    //   },
    // );

    responseListener.current = Notifications.addNotificationResponseReceivedListener(
      (response: any) => {
        console.log("🚀🚀🚀🚀 NOTIFICATION payload", response);
        router.push("/inbox");
        // TODO @radulescuandrew invalidate inbox query to refetchd
      },
    ) as any;

    return () => {
      notificationListener.current &&
        Notifications.removeNotificationSubscription(
          notificationListener.current as Notifications.Subscription,
        );
      responseListener.current &&
        Notifications.removeNotificationSubscription(
          responseListener.current as Notifications.Subscription,
        );
    };
  }, []);

  const unsubscribe = async () => {
    console.log("Unsubscribe", pushToken);
    try {
      if (pushToken) {
        await unsubscribePushNotifications();
        setPushToken(undefined);
      }
    } catch (error) {
      console.error("Error while unregistering push token");
      Sentry.captureException(error);
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
