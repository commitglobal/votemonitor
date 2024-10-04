import React from "react";
import { GestureHandlerRootView } from "react-native-gesture-handler";
import { Drawer } from "expo-router/drawer";
import { ScrollViewProps } from "react-native";
import { DrawerContentScrollView } from "@react-navigation/drawer";
import { useTheme, XStack, YStack } from "tamagui";
import { Icon } from "../../../../components/Icon";
import { useGetCitizenElectionEvents } from "../../../../services/queries/citizen.query";
import { Typography } from "../../../../components/Typography";
import { useSafeAreaInsets } from "react-native-safe-area-context";

type DrawerContentProps = ScrollViewProps & {
  children?: React.ReactNode;
  backgroundColor: string;
};

export const DrawerContent = (props: DrawerContentProps) => {
  const insets = useSafeAreaInsets();
  const { data: electionEvents, isLoading: isLoadingElectionEvents } =
    useGetCitizenElectionEvents();

  // todo: delete
  const mockElectionEvents = {
    electionRounds: Array(15)
      .fill(null)
      .map((_, index) => ({
        id: `event-${index + 1}`,
        countryCode: `CC${index + 1}`,
        countryName: `Country ${index + 1}`,
        countryFullName: `Full Country Name ${index + 1}`,
        startDate: `2023-${(index % 12) + 1}-${(index % 28) + 1}`,
        title: `Election Event ${index + 1}`,
      })),
  };

  // todo
  const selectedElectionEvent = mockElectionEvents?.electionRounds[0];

  return (
    <>
      <YStack flex={1}>
        <DrawerContentScrollView
          {...props}
          contentContainerStyle={{ flexGrow: 1, paddingBottom: 16 }}
        >
          <Icon icon="vmCitizenLogo" width={211} height={65} paddingLeft="$md" />

          <YStack marginTop="$lg">
            {mockElectionEvents.electionRounds.map((electionEvent, index) => (
              <XStack
                key={index}
                paddingVertical="$md"
                paddingHorizontal="$lg"
                pressStyle={{ opacity: 0.5 }}
                backgroundColor={
                  selectedElectionEvent?.id === electionEvent.id ? "$purple5" : "transparent"
                }
              >
                <Typography
                  preset="body2"
                  color={selectedElectionEvent?.id === electionEvent.id ? "white" : "$purple5"}
                >
                  {electionEvent.title}
                </Typography>
              </XStack>
            ))}
          </YStack>
        </DrawerContentScrollView>
      </YStack>
      <XStack borderWidth={2} paddingBottom={insets.bottom + 16}>
        <Typography marginTop="auto">Report an issue</Typography>
      </XStack>
    </>
  );
};

export default function DrawerLayout() {
  const theme = useTheme();

  console.log("DrawerLayout");

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
        <Drawer.Screen name="(tabs)" />
      </Drawer>
    </GestureHandlerRootView>
  );
}
