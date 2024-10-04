import React from "react";
import { Tabs } from "expo-router";
import { TextStyle, ViewStyle } from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { useTheme } from "tamagui";
import { Icon } from "../../../../components/Icon";
import { useUserData } from "../../../../contexts/user/UserContext.provider";
import { useNetInfoContext } from "../../../../contexts/net-info-banner/NetInfoContext";
import { useTranslation } from "react-i18next";

export default function TabLayout() {
  const insets = useSafeAreaInsets();
  const theme = useTheme();
  const { t } = useTranslation("tabs");

  const { activeElectionRound } = useUserData();
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
        name="(observation)"
        options={{
          title: t("observation"),
          tabBarIcon: ({ color }) => <Icon icon="observation" color={color} />,
        }}
      />
      <Tabs.Screen
        name="(reports)"
        options={{
          title: t("reports"),
          tabBarIcon: ({ color }) => <Icon icon="quickReport" color={color} />,
        }}
      />

      <Tabs.Screen
        name="guides"
        options={{
          title: t("guides"),
          tabBarIcon: ({ color }) => <Icon icon="learning" color={color} />,
          href: activeElectionRound ? "/guides" : null,
        }}
      />
      <Tabs.Screen
        name="inbox"
        options={{
          title: t("inbox"),
          tabBarIcon: ({ color }) => <Icon icon="inbox" color={color} />,
          href: activeElectionRound ? "/inbox" : null,
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
