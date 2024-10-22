import React, { useMemo } from "react";
import { GestureHandlerRootView } from "react-native-gesture-handler";
import { Drawer } from "expo-router/drawer";
import { ScrollViewProps } from "react-native";
import { DrawerContentScrollView, DrawerItem } from "@react-navigation/drawer";
import { useTheme, XStack } from "tamagui";
import { useUserData } from "../../../../contexts/user/UserContext.provider";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { AppMode } from "../../../../contexts/app-mode/AppModeContext.provider";

import { AppModeSwitchButton } from "../../../../components/AppModeSwitchButton";
import { Icon } from "../../../../components/Icon";

type DrawerContentProps = ScrollViewProps & {
  children?: React.ReactNode;
  backgroundColor: string;
};

export const DrawerContent = (props: DrawerContentProps) => {
  const { electionRounds, activeElectionRound, setSelectedElectionRoundId } = useUserData();

  const startedElectionRounds = useMemo(
    () => electionRounds?.filter((electionRound) => electionRound.status === "Started"),
    [electionRounds],
  );

  const theme = useTheme();
  const insets = useSafeAreaInsets();

  return (
    <DrawerContentScrollView
      contentContainerStyle={{ flexGrow: 1, paddingBottom: insets.bottom + 32 }}
      bounces={false}
      stickyHeaderIndices={[0]}
      {...props}
    >
      <XStack paddingTop={16} paddingLeft="$md" paddingBottom="$xl">
        <Icon icon="vmObserverLogo" width={211} height={65} />
      </XStack>
      {startedElectionRounds?.map((round, index) => {
        return (
          <DrawerItem
            key={index}
            label={`${round.status} - ${round.title}`}
            focused={activeElectionRound?.id === round.id}
            activeTintColor={theme.purple5?.val}
            activeBackgroundColor={theme.yellow5?.val}
            inactiveTintColor="white"
            onPress={() => setSelectedElectionRoundId(round.id)}
            style={{
              paddingVertical: 4,
              paddingHorizontal: 16,
              marginVertical: 0,
              marginHorizontal: 0,
              borderRadius: 0,
            }}
            allowFontScaling={false}
          />
        );
      })}

      {/* app mode switch */}
      <AppModeSwitchButton switchToMode={AppMode.CITIZEN} />
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
