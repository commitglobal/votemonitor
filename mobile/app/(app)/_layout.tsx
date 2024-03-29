import { Redirect } from "expo-router";
import { useAuth } from "../../hooks/useAuth";
import { GestureHandlerRootView } from "react-native-gesture-handler";
import { Drawer } from "expo-router/drawer";
import { useTheme } from "tamagui";
import {
  DrawerContentScrollView,
  DrawerItem,
  DrawerItemList,
} from "@react-navigation/drawer";

const AppLayout = () => {
  const { isAuthenticated } = useAuth();
  const theme = useTheme();

  if (!isAuthenticated) {
    // On web, static rendering will stop here as the user is not authenticated
    // in the headless Node process that the pages are rendered in.
    return <Redirect href="/login" />;
  }

  return (
    <GestureHandlerRootView style={{ flex: 1 }}>
      <Drawer
        screenOptions={{
          drawerType: "front",
          drawerStyle: { backgroundColor: theme.purple5.val },
          drawerActiveTintColor: theme.yellow10.val,
          drawerActiveBackgroundColor: "transparent",
        }}
        drawerContent={CustomDrawerContent}
      >
        <Drawer.Screen name="(tabs)" options={{ headerShown: true }} />
      </Drawer>
    </GestureHandlerRootView>
  );
};

function CustomDrawerContent(props) {
  const theme = useTheme();
  return (
    <DrawerContentScrollView {...props}>
      {/* <DrawerItemList {...props} /> */}
      {votingSessions.map((votingSession) => (
        <DrawerItem
          label={votingSession.name}
          inactiveTintColor={theme.yellow6.val}
          onPress={() => console.log("")}
        />
      ))}
    </DrawerContentScrollView>
  );
}

export default AppLayout;

const votingSessions = [
  { name: "session 1" },
  { name: "session2" },
  { name: "session 3" },
];
