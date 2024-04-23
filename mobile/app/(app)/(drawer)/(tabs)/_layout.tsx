import React from "react";
import { Tabs } from "expo-router";
import { TextStyle, ViewStyle } from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { useTheme } from "tamagui";
import { Icon } from "../../../../components/Icon";
import { useUserData } from "../../../../contexts/user/UserContext.provider";
import { useNetInfoContext } from "../../../../contexts/net-info-banner/NetInfoContext";

export default function TabLayout() {
  const insets = useSafeAreaInsets();
  const theme = useTheme();

  const { electionRounds } = useUserData();
  const { shouldDisplayBanner } = useNetInfoContext();

  return (
    <Tabs
      screenOptions={{
        tabBarActiveTintColor: theme.purple5?.val,
        tabBarHideOnKeyboard: true,
        headerShown: false,
        tabBarStyle: [
          $tabBar,
          {
            height: insets.bottom + 60,
            ...(shouldDisplayBanner && insets?.bottom ? { marginBottom: -insets.bottom + 10 } : {}),
          },
        ],
        tabBarLabelStyle: $tabBarLabel,
      }}
    >
      <Tabs.Screen
        name="(observation)"
        options={{
          title: "Observation",
          tabBarIcon: ({ color }) => <Icon icon="observation" color={color} />,
        }}
      />
      <Tabs.Screen
        name="quick-report"
        options={{
          title: "Quick Report",
          tabBarIcon: ({ color }) => <Icon icon="quickReport" color={color} />,
          href: electionRounds?.length ? "/quick-report" : null,
        }}
      />
      <Tabs.Screen
        name="guides"
        options={{
          title: "Guides",
          tabBarIcon: ({ color }) => <Icon icon="learning" color={color} />,
          href: electionRounds?.length ? "/guides" : null,
        }}
      />
      <Tabs.Screen
        name="inbox"
        options={{
          title: "Inbox",
          tabBarIcon: ({ color }) => <Icon icon="inbox" color={color} />,
          href: electionRounds?.length ? "/inbox" : null,
        }}
      />
      <Tabs.Screen
        name="more"
        options={{
          title: "More",
          tabBarIcon: ({ color }) => <Icon icon="more" color={color} />,
        }}
      />
    </Tabs>
  );
}

const $tabBar: ViewStyle = {
  backgroundColor: "white",
};

const $tabBarLabel: TextStyle = {
  marginBottom: 11,
  marginTop: -5,
  fontFamily: "Roboto",
  fontSize: 12,
};
