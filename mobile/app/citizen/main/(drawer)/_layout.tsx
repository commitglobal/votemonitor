import React from "react";
import { GestureHandlerRootView } from "react-native-gesture-handler";
import { Drawer } from "expo-router/drawer";
import { ScrollViewProps } from "react-native";
import { ScrollView, Spinner, useTheme, XStack, YStack } from "tamagui";

import { useSafeAreaInsets } from "react-native-safe-area-context";

import { DrawerActions } from "@react-navigation/native";
import { useNavigation } from "expo-router";
import { AppModeSwitchButton } from "../../../../components/AppModeSwitchButton";
import { Icon } from "../../../../components/Icon";
import { Typography } from "../../../../components/Typography";
import { AppMode } from "../../../../contexts/app-mode/AppModeContext.provider";
import { useCitizenUserData } from "../../../../contexts/citizen-user/CitizenUserContext.provider";
import { useGetCitizenElectionEvents } from "../../../../services/queries/citizen.query";
type DrawerContentProps = ScrollViewProps & {
  children?: React.ReactNode;
  backgroundColor: string;
};

export const DrawerContent = (props: DrawerContentProps) => {
  const insets = useSafeAreaInsets();
  const navigation = useNavigation();

  const { setSelectedElectionRound, selectedElectionRound } = useCitizenUserData();
  const { data: electionEvents, isLoading: isLoadingElectionEvents } =
    useGetCitizenElectionEvents();

  const handleSelectElectionRound = (electionRoundId: string) => {
    setSelectedElectionRound(electionRoundId);
    navigation.dispatch(DrawerActions.closeDrawer());
  };

  return (
    <YStack flex={1} backgroundColor="$purple25">
      <YStack flex={1} paddingTop={insets.top}>
        <ScrollView
          {...props}
          contentContainerStyle={{ flexGrow: 1, paddingBottom: 16 }}
          stickyHeaderIndices={[0]}
        >
          <XStack backgroundColor="$purple25" paddingTop={16} paddingLeft="$md">
            <Icon icon="vmCitizenLogo" width={211} height={65} />
          </XStack>

          <YStack marginTop="$lg">
            {isLoadingElectionEvents ? (
              <Spinner size="large" color="$purple5" marginTop="$xl" />
            ) : (
              <>
                {electionEvents?.electionRounds.map((electionEvent, index) => (
                  <XStack
                    key={index}
                    paddingVertical="$md"
                    paddingHorizontal="$lg"
                    pressStyle={{ opacity: 0.5 }}
                    onPress={() => handleSelectElectionRound(electionEvent.id)}
                    backgroundColor={
                      selectedElectionRound === electionEvent.id ? "$purple5" : "transparent"
                    }
                  >
                    <Typography
                      preset="body2"
                      color={selectedElectionRound === electionEvent.id ? "white" : "$purple5"}
                    >
                      {electionEvent.title}
                    </Typography>
                  </XStack>
                ))}
              </>
            )}
          </YStack>
        </ScrollView>
      </YStack>

      <AppModeSwitchButton
        switchToMode={AppMode.OBSERVER}
        paddingBottom={insets.bottom + 16}
        color="$purple5"
      />
    </YStack>
  );
};

export default function DrawerLayout() {
  const theme = useTheme();

  return (
    <GestureHandlerRootView style={{ flex: 1 }}>
      <Drawer
        drawerContent={() => (
          <DrawerContent
            backgroundColor={theme.purple25?.val}
            showsVerticalScrollIndicator={false}
          />
        )}
        screenOptions={{
          drawerType: "front",
          headerShown: false,
        }}
      >
        <Drawer.Screen name="(tabs)" options={{ headerShown: false }} />
      </Drawer>
    </GestureHandlerRootView>
  );
}