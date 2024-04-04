import React from "react";
import { GestureHandlerRootView } from "react-native-gesture-handler";
import { Drawer } from "expo-router/drawer";
import { getHeaderTitle } from "@react-navigation/elements";
import { DrawerActions } from "@react-navigation/native";
import Header from "../../../components/Header";
import { Icon } from "../../../components/Icon";
import { ScrollViewProps, StyleProp, ViewStyle } from "react-native";
import { DrawerContentScrollView, DrawerItem } from "@react-navigation/drawer";
import { useTheme } from "tamagui";

type DrawerContentProps = ScrollViewProps & {
  children?: React.ReactNode;
  backgroundColor: string;
};

export const DrawerContent = (props: DrawerContentProps) => {
  const votingSessions = [
    { name: "session 1" },
    { name: "session2" },
    { name: "session 3" },
  ];
  const theme = useTheme();
  return (
    <DrawerContentScrollView {...props}>
      {votingSessions.map((votingSession, index) => (
        <DrawerItem
          key={index}
          label={votingSession.name}
          inactiveTintColor={theme.yellow6?.val}
          onPress={() => console.log("")}
        />
      ))}
    </DrawerContentScrollView>
  );
};

export default function MainLayout() {
  const theme = useTheme();
  return (
    <GestureHandlerRootView style={{ flex: 1 }}>
      <Drawer
        drawerContent={() => (
          <DrawerContent backgroundColor={theme.purple5?.val} />
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
