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
  const [pushToken, setPushToken] = useState<string | undefined>();

  // notifications
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
  };

  const init = useCallback(initNotifications, [initNotifications]);

  useEffect(() => {
    if (isAuthenticated) {
      init();
    }

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
  }, [navigation, isAuthenticated]);

  const unsubscribe = () => {
    try {
      if (pushToken) {
        // TODO: call api to unsubscribe
        // unregisterPushToken(pushToken);
        setPushToken(undefined);
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
