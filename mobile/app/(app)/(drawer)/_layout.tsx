import { GestureHandlerRootView } from "react-native-gesture-handler";
import { Drawer } from "expo-router/drawer";
import { getHeaderTitle } from "@react-navigation/elements";
import { useTheme } from "tamagui";
import React from "react";
import Eye from "../../../assets/icons/Eye.svg";
import ObservationSVG from "../../../assets/icons/observation.svg";
import { DrawerActions } from "@react-navigation/native";
import Header from "../../../components/Header";

export default function MainLayout() {
  const theme = useTheme();
  return (
    <GestureHandlerRootView style={{ flex: 1 }}>
      <Drawer
        screenOptions={{
          drawerType: "front",
          headerStyle: {
            backgroundColor: theme?.purple5?.val,
          },
          header: ({ navigation, route, options }) => {
            const title = getHeaderTitle(options, route.name);
            return (
              <Header
                // navigation={navigation}
                title={title}
                titleColor="white"
                // style={options.headerStyle}
                leftIcon={<Eye fill="white" />}
                leftIconColor="white"
                onLeftPress={() =>
                  navigation.dispatch(DrawerActions.openDrawer)
                }
                rightIcon={<ObservationSVG fill="white" />}
              />
            );
          },
        }}
      >
        <Drawer.Screen name="(tabs)" />
      </Drawer>
    </GestureHandlerRootView>
  );
}
