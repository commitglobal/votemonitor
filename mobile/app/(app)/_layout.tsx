import React from "react";
import { Redirect, SplashScreen, Stack } from "expo-router";
import { PortalProvider } from "tamagui";
import { useAuth } from "../../hooks/useAuth";
import UserContextProvider from "../../contexts/user/UserContext.provider";
import NotificationContextProvider from "../../contexts/notification/NotificationContextProvider";

const AppLayout = () => {
  const { isAuthenticated } = useAuth();

  // TODO: This will be logout
  if (!isAuthenticated) {
    // On web, static rendering will stop here as the user is not authenticated
    // in the headless Node process that the pages are rendered in.
    SplashScreen.hideAsync();
    return <Redirect href="/login" />;
  }

  return (
    <PortalProvider>
      <UserContextProvider>
        <NotificationContextProvider>
          <Stack>
            <Stack.Screen name="(drawer)" options={{ headerShown: false }} />
            <Stack.Screen name="polling-station-wizzard" options={{ headerShown: false }} />
            <Stack.Screen name="form-questionnaire" options={{ headerShown: false }} />
            <Stack.Screen name="polling-station-questionnaire" options={{ headerShown: false }} />
            <Stack.Screen name="report-issue" options={{ headerShown: false }} />
            <Stack.Screen name="change-password" options={{ headerShown: false }} />
          </Stack>
        </NotificationContextProvider>
      </UserContextProvider>
    </PortalProvider>
  );
};

export default AppLayout;
