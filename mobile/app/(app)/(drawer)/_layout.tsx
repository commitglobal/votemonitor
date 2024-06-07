import React from "react";
import { GestureHandlerRootView } from "react-native-gesture-handler";
import { Drawer } from "expo-router/drawer";
import { ScrollViewProps } from "react-native";
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
      {electionRounds?.map((round, index) => (
        <DrawerItem
          key={index}
          label={`${round.status} - ${round.title}`}
          inactiveTintColor={theme.yellow6?.val}
          onPress={() => console.log("")}
          allowFontScaling={false}
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
          headerShown: false,
        }}
      >
        <Drawer.Screen name="(tabs)" />
      </Drawer>
    </GestureHandlerRootView>
  );
}
