import { Redirect, Stack } from "expo-router";
import { useAuth } from "../../hooks/useAuth";
import { GestureHandlerRootView } from "react-native-gesture-handler";
import { Drawer } from "expo-router/drawer";

const AppLayout = () => {
  const { isAuthenticated } = useAuth();

  if (!isAuthenticated) {
    // On web, static rendering will stop here as the user is not authenticated
    // in the headless Node process that the pages are rendered in.
    return <Redirect href="/login" />;
  }

  return (
    <GestureHandlerRootView style={{ flex: 1 }}>
      <Drawer screenOptions={{ drawerType: "front" }}>
        <Drawer.Screen name="(tabs)" options={{ headerShown: true }} />
      </Drawer>
    </GestureHandlerRootView>
  );
};

export default AppLayout;
