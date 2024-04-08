import { Redirect, Stack } from "expo-router";
import { PortalProvider } from "tamagui";
import { useAuth } from "../../hooks/useAuth";
import { useUserData } from "../../contexts/user/UserContext.provider";
import { Typography } from "../../components/Typography";
import { Screen } from "../../components/Screen";

const AppLayout = () => {
  const { isAuthenticated } = useAuth();
  const { isLoading } = useUserData();

  // TODO: This will be logout
  if (!isAuthenticated) {
    // On web, static rendering will stop here as the user is not authenticated
    // in the headless Node process that the pages are rendered in.
    return <Redirect href="/login" />;
  }

  if (isLoading) {
    return (
      <Screen safeAreaEdges={["top"]} backgroundColor="red">
        <Typography preset="heading">Loading...</Typography>
      </Screen>
    );
  }

  return (
    <PortalProvider>
      <Stack>
        <Stack.Screen name="(drawer)" options={{ headerShown: false }} />
        <Stack.Screen name="polling-station-wizzard" options={{ headerShown: false }} />
        <Stack.Screen name="form-questionnaire" />
        <Stack.Screen name="polling-station-questionnaire" />
      </Stack>
    </PortalProvider>
  );
};

export default AppLayout;
