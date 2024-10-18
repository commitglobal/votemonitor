import React from "react";
import { Tabs } from "expo-router";
import { TextStyle, ViewStyle } from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { useTheme } from "tamagui";
import { useTranslation } from "react-i18next";
import { useNetInfoContext } from "../../../../../contexts/net-info-banner/NetInfoContext";
import { Icon } from "../../../../../components/Icon";

export default function TabLayout() {
  const insets = useSafeAreaInsets();
  const theme = useTheme();
  const { t } = useTranslation("tabs");

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
        tabBarAllowFontScaling: false,
      }}
    >
      <Tabs.Screen
        name="index"
        options={{
          title: t("report"),
          tabBarIcon: ({ color }) => <Icon icon="quickReport" color={color} />,
        }}
      />
      <Tabs.Screen
        name="resources"
        options={{
          title: t("resources"),
          tabBarIcon: ({ color }) => <Icon icon="learning" color={color} />,
        }}
      />
      <Tabs.Screen
        name="updates"
        options={{
          title: t("updates"),
          tabBarIcon: ({ color }) => <Icon icon="updates" color={color} />,
        }}
      />
      <Tabs.Screen
        name="more"
        options={{
          title: t("more"),
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
