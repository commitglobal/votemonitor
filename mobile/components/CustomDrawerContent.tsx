import React from "react";
import { DrawerContentScrollView, DrawerItem } from "@react-navigation/drawer";
import { useTheme } from "tamagui";

export const CustomDrawerContent = (props: any) => {
  const theme = useTheme();
  return (
    <DrawerContentScrollView {...props}>
      {/* <DrawerItemList {...props} /> */}
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

const votingSessions = [
  { name: "session 1" },
  { name: "session2" },
  { name: "session 3" },
];
