import { GestureHandlerRootView } from "react-native-gesture-handler";
import { Drawer } from "expo-router/drawer";
import { getHeaderTitle } from "@react-navigation/elements";
import { View, Text, XStack, useTheme } from "tamagui";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import React from "react";
import Eye from "../../../assets/icons/Eye.svg";
import {
  StyleProp,
  TouchableOpacity,
  TouchableOpacityProps,
  ViewStyle,
} from "react-native";
import ObservationSVG from "../../../assets/icons/observation.svg";
import { DrawerActions } from "@react-navigation/native";
import { Typography } from "../../../components/Typography";
// import { tokens } from "../../../theme/tokens";
import { DrawerNavigationProp } from "@react-navigation/drawer";
import Header from "../../../components/Header";

export default function MainLayout() {
  const theme = useTheme();
  return (
    <GestureHandlerRootView style={{ flex: 1 }}>
      <Drawer
        screenOptions={{
          drawerType: "front",
          header: ({ navigation, route, options }) => {
            const title = getHeaderTitle(options, route.name);
            return (
              <Header
                backgroundColor={theme?.purple5?.val}
                title="2024 EU Parliamentary RO"
                titleColor="white"
                style={options.headerStyle}
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
