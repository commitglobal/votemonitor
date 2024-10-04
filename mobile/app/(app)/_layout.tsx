import React from "react";
import { Redirect, Stack } from "expo-router";
import { useAuth } from "../../hooks/useAuth";
import UserContextProvider from "../../contexts/user/UserContext.provider";
import NotificationContextProvider from "../../contexts/notification/NotificationContextProvider";

const AppLayout = () => {
  const { isAuthenticated } = useAuth();

  if (!isAuthenticated) {
    return <Redirect href="/login" />;
  }

  return (
    <UserContextProvider>
      <NotificationContextProvider>
        <Stack>
          <Stack.Screen name="(drawer)" options={{ headerShown: false }} />
          <Stack.Screen name="polling-station-wizzard" options={{ headerShown: false }} />
          <Stack.Screen name="manage-polling-stations" options={{ headerShown: false }} />
          <Stack.Screen name="form-questionnaire" options={{ headerShown: false }} />
          <Stack.Screen name="polling-station-questionnaire" options={{ headerShown: false }} />
          <Stack.Screen name="incident-report-forms" options={{ headerShown: false }} />
          <Stack.Screen name="add-quick-report" options={{ headerShown: false }} />
          <Stack.Screen name="new-report" options={{ headerShown: false }} />
          <Stack.Screen name="change-password" options={{ headerShown: false }} />
          <Stack.Screen name="about-votemonitor" options={{ headerShown: false }} />
        </Stack>
      </NotificationContextProvider>
    </UserContextProvider>
  );
};

export default AppLayout;
