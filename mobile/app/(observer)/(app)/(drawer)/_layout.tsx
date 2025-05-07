import { DrawerContentScrollView, DrawerItem } from "@react-navigation/drawer";
import { Drawer } from "expo-router/drawer";
import React, { useMemo } from "react";
import { ScrollViewProps } from "react-native";
import { GestureHandlerRootView } from "react-native-gesture-handler";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { useTheme, XStack, YStack } from "tamagui";
import { AppMode } from "../../../../contexts/app-mode/AppModeContext.provider";
import { useUserData } from "../../../../contexts/user/UserContext.provider";

import { AppModeSwitchButton } from "../../../../components/AppModeSwitchButton";
import { Icon } from "../../../../components/Icon";
import { Typography } from "../../../../components/Typography";
import { electionRoundSorter } from "../../../../helpers/election-rounds";
import { useRouter } from "expo-router";

type DrawerContentProps = ScrollViewProps & {
  children?: React.ReactNode;
  backgroundColor: string;
};

export const DrawerContent = (props: DrawerContentProps) => {
  const { electionRounds, activeElectionRound, setSelectedElectionRoundId } = useUserData();
  const router = useRouter();
  const startedElectionRounds = useMemo(
    () =>
      electionRounds
        ?.filter((electionRound) => electionRound.status === "Started")
        .sort(electionRoundSorter),
    [electionRounds],
  );

  const theme = useTheme();
  const insets = useSafeAreaInsets();

  return (
    <YStack flex={1}>
      <DrawerContentScrollView
        contentContainerStyle={{ flexGrow: 1 }}
        bounces={false}
        stickyHeaderIndices={[0]}
        {...props}
      >
        <YStack paddingLeft="$md" backgroundColor={theme.purple5?.val}>
          <Icon icon="vmObserverLogo" width={211} height={65} />
          <Typography preset="subheading" color="white">
            Active elections
          </Typography>
        </YStack>
        {startedElectionRounds?.map((round, index) => {
          return (
            <DrawerItem
              key={index}
              // use a custom component for the label, as the default one only displays one line of text
              label={({ color }) => (
                <Typography preset="body2" color={color}>
                  {`${round.status} - ${round.title}`}
                </Typography>
              )}
              focused={activeElectionRound?.id === round.id}
              activeTintColor={theme.purple5?.val}
              activeBackgroundColor={theme.yellow5?.val}
              inactiveTintColor="white"
              onPress={() => {
                if (activeElectionRound?.id !== round.id) {
                  setSelectedElectionRoundId(round.id);
                }
              }}
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
      </DrawerContentScrollView>

      {/* app mode switch */}
      <YStack padding="$md" backgroundColor={theme.purple5?.val}>
        <XStack
          marginTop="auto"
          gap="$xxs"
          alignItems="center"
          paddingHorizontal="$md"
          paddingVertical="$xxxs"
          pressStyle={{ opacity: 0.5 }}
          onPress={() => router.push("/past-elections")}
        >
          <Icon icon="appModeSwitch" color={theme.purple5?.val} size={32} />
          <Typography color="white" preset="body2" textDecorationLine="underline" flex={1}>
            See past elections data
          </Typography>
        </XStack>
      </YStack>
      {/* app mode switch */}
      <YStack padding="$md" backgroundColor={theme.purple5?.val}>
        <AppModeSwitchButton switchToMode={AppMode.CITIZEN} />
      </YStack>
    </YStack>
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
