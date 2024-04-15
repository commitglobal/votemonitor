import { Redirect, Stack } from "expo-router";
import { useAuth } from "../../hooks/useAuth";
import { PortalProvider } from "tamagui";

const AppLayout = () => {
  const { isAuthenticated } = useAuth();

  if (!isAuthenticated) {
    // On web, static rendering will stop here as the user is not authenticated
    // in the headless Node process that the pages are rendered in.
    return <Redirect href="/login" />;
  }

  return (
    <PortalProvider>
      <Stack>
        <Stack.Screen name="(drawer)" options={{ headerShown: false }} />
        <Stack.Screen name="polling-station-wizzard" options={{ headerShown: false }} />
        <Stack.Screen name="form-questionnaire" />
        <Stack.Screen name="polling-station-questionnaire" options={{ headerShown: false }} />
      </Stack>
    </PortalProvider>
  );
};

export default AppLayout;
