import React from "react";
import { Redirect, Stack } from "expo-router";
import { PortalProvider } from "tamagui";
import { useAuth } from "../../hooks/useAuth";
import UserContextProvider from "../../contexts/user/UserContext.provider";

const AppLayout = () => {
  const { isAuthenticated } = useAuth();

  // TODO: This will be logout
  if (!isAuthenticated) {
    // On web, static rendering will stop here as the user is not authenticated
    // in the headless Node process that the pages are rendered in.
    return <Redirect href="/login" />;
  }

  return (
    <PortalProvider>
      <UserContextProvider>
        <Stack>
          <Stack.Screen name="(drawer)" options={{ headerShown: false }} />
          <Stack.Screen name="polling-station-wizzard" options={{ headerShown: false }} />
          <Stack.Screen name="form-questionnaire" options={{ headerShown: false }} />
          <Stack.Screen name="polling-station-questionnaire" options={{ headerShown: false }} />
          <Stack.Screen name="report-issue" options={{ headerShown: false }} />
          <Stack.Screen name="change-language" options={{ headerShown: false }} />
          <Stack.Screen name="change-password" options={{ headerShown: false }} />
        </Stack>
      </UserContextProvider>
    </PortalProvider>
  );
};

export default AppLayout;
