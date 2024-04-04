import React from "react";
import { GestureHandlerRootView } from "react-native-gesture-handler";
import { Drawer } from "expo-router/drawer";
import { getHeaderTitle } from "@react-navigation/elements";
import { useTheme } from "tamagui";
import { DrawerActions } from "@react-navigation/native";
import Header from "../../../components/Header";
import { Icon } from "../../../components/Icon";
import { StyleProp, ViewStyle } from "react-native";
import { CustomDrawerContent } from "../../../components/CustomDrawerContent";

export default function MainLayout() {
  const theme = useTheme();
  return (
    <GestureHandlerRootView style={{ flex: 1 }}>
      <Drawer
        drawerContent={() => (
          <CustomDrawerContent backgroundColor={theme.purple5?.val} />
        )}
        screenOptions={{
          drawerType: "front",
          header: ({ navigation, route, options }) => {
            const title = getHeaderTitle(options, route.name);
            return (
              <Header
                title={title}
                titleColor="white"
                backgroundColor={theme.purple5?.val}
                barStyle="light-content"
                style={options.headerStyle as StyleProp<ViewStyle>}
                leftIcon={<Icon icon="menuAlt2" color="white" />}
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
