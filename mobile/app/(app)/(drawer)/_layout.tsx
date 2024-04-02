import { GestureHandlerRootView } from "react-native-gesture-handler";
import { Drawer } from "expo-router/drawer";
import { getHeaderTitle } from "@react-navigation/elements";
import { useTheme } from "tamagui";
import React from "react";
import Eye from "../../../assets/icons/Eye.svg";
import ObservationSVG from "../../../assets/icons/observation.svg";
import { DrawerActions } from "@react-navigation/native";
import Header from "../../../components/Header";
import { Icon } from "../../../components/Icon";

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
                title="2024 EU Parliamentary RO"
                titleColor="white"
                backgroundColor={theme.purple5?.val}
                // style={options.headerStyle}
                leftIcon={<Icon icon="menuAlt2" color="white" />}
                leftIconColor="white"
                onLeftPress={() =>
                  navigation.dispatch(DrawerActions.openDrawer)
                }
                rightIcon={<Icon icon="dotsVertical" color="white" />}
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
