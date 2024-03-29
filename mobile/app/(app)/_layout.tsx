import { Redirect, useNavigation } from "expo-router";
import { DrawerActions } from "@react-navigation/native";
import { useAuth } from "../../hooks/useAuth";
import { GestureHandlerRootView } from "react-native-gesture-handler";
import { Drawer } from "expo-router/drawer";
import { useTheme } from "tamagui";
import { TouchableOpacity } from "react-native";
import DotsVerticalSVG from "../../assets/icons/dots-vertical.svg";
import MenuAlt2SVG from "../../assets/icons/menu-alt-2.svg";
import { Typography } from "../../components/Typography";

const AppLayout = () => {
  const { isAuthenticated } = useAuth();
  const theme = useTheme();
  const navigation = useNavigation();

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
        }}
      >
        <Drawer.Screen
          name="(tabs)"
          options={{
            headerShown: true,
            headerStyle: { backgroundColor: theme.purple5.val },
            headerTintColor: "white",
            headerTitleAlign: "center",
            headerTitleStyle: {
              fontSize: 16,
            },
            headerTitle: (props) => (
              <Typography
                preset="body1"
                style={{
                  fontWeight: "500",
                  color: props.tintColor,
                  lineHeight: 24,
                }}
              >
                2024 EU Parliamentary RO
              </Typography>
            ),
            // drawer icon left
            headerLeft: () => (
              <TouchableOpacity
                onPress={() => navigation.dispatch(DrawerActions.openDrawer())}
                style={{ marginLeft: 14 }}
              >
                <MenuAlt2SVG fill="white" />
              </TouchableOpacity>
            ),

            //TODO: decide what this right should do, onPress=...
            headerRight: () => (
              <TouchableOpacity
                style={{ marginRight: 14 }}
                onPress={() => console.log("To be decided what to do here")}
              >
                <DotsVerticalSVG fill="white" />
              </TouchableOpacity>
            ),
          }}
        />
      </Drawer>
    </GestureHandlerRootView>
  );
};

export default AppLayout;
