import React from "react";
import { GestureHandlerRootView } from "react-native-gesture-handler";
import { Drawer } from "expo-router/drawer";
import { ScrollViewProps, Text } from "react-native";
import { DrawerContentScrollView } from "@react-navigation/drawer";
import { useTheme } from "tamagui";

type DrawerContentProps = ScrollViewProps & {
  children?: React.ReactNode;
  backgroundColor: string;
};

export const DrawerContent = (props: DrawerContentProps) => {
  return (
    <DrawerContentScrollView {...props}>
      <Text>Drawer Content</Text>
    </DrawerContentScrollView>
  );
};

export default function DrawerLayout() {
  const theme = useTheme();

  console.log("DrawerLayout");

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
