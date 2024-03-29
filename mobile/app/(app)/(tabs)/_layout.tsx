import React from "react";
import { Tabs } from "expo-router";
import ObservationSVG from "../../../assets/icons/observation.svg";
import QuickReportSVG from "../../../assets/icons/quick-report.svg";
import GuidesSVG from "../../../assets/icons/Learning.svg";
import InboxSVG from "../../../assets/icons/Inbox.svg";
import MoreSVG from "../../../assets/icons/More.svg";
import { TextStyle, ViewStyle } from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { useTheme } from "tamagui";

export default function TabLayout() {
  const insets = useSafeAreaInsets();
  const theme = useTheme();

  return (
    <Tabs
      screenOptions={{
        tabBarActiveTintColor: theme.purple5.val,
        tabBarHideOnKeyboard: true,
        headerShown: false,
        tabBarStyle: [$tabBar, { height: insets.bottom + 60 }],
        tabBarLabelStyle: $tabBarLabel,
      }}
    >
      <Tabs.Screen
        name="index"
        options={{
          title: "Observation",
          tabBarIcon: ({ color }) => <ObservationSVG fill={color} />,
        }}
      />
      <Tabs.Screen
        name="quick-report"
        options={{
          title: "Quick Report",
          tabBarIcon: ({ color }) => <QuickReportSVG fill={color} />,
        }}
      />
      <Tabs.Screen
        name="guides"
        options={{
          title: "Guides",
          tabBarIcon: ({ color }) => <GuidesSVG fill={color} />,
        }}
      />
      <Tabs.Screen
        name="inbox"
        options={{
          title: "Inbox",
          tabBarIcon: ({ color }) => <InboxSVG fill={color} />,
        }}
      />
      <Tabs.Screen
        name="more"
        options={{
          title: "More",
          tabBarIcon: ({ color }) => <MoreSVG fill={color} />,
        }}
      />
    </Tabs>
  );
}

const $tabBar: ViewStyle = {
  backgroundColor: "white",
};

const $tabBarLabel: TextStyle = {
  marginBottom: 4,
  marginTop: -12,
  fontFamily: "Roboto",
  fontSize: 12,
};
