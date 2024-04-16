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
import { useUserData } from "../../../contexts/user/UserContext.provider";

type DrawerContentProps = ScrollViewProps & {
  children?: React.ReactNode;
  backgroundColor: string;
};

export const DrawerContent = (props: DrawerContentProps) => {
  const { electionRounds } = useUserData();

  const theme = useTheme();
  return (
    <DrawerContentScrollView {...props}>
      {electionRounds.map((round, index) => (
        <DrawerItem
          key={index}
          label={`${round.status} - ${round.title}`}
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
        drawerContent={() => <DrawerContent backgroundColor={theme.purple5?.val} />}
        screenOptions={{
          drawerType: "front",
          header: ({ navigation, route, options }) => {
            const title = getHeaderTitle(options, route.name);
            return (
              <Header
                title={title}
                titleColor="white"
                barStyle="light-content"
                style={options.headerStyle as StyleProp<ViewStyle>}
                leftIcon={<Icon icon="menuAlt2" color="white" />}
                onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
                // rightIcon={<Icon icon="dotsVertical" color="white" />}
                // onRightPress={() => {
                //   console.log("on right action press");
                // }}
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
