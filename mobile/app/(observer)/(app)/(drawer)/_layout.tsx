import React from "react";
import { GestureHandlerRootView } from "react-native-gesture-handler";
import { Drawer } from "expo-router/drawer";
import { ScrollViewProps } from "react-native";
import { DrawerContentScrollView, DrawerItem } from "@react-navigation/drawer";
import { useTheme, XStack } from "tamagui";
import { useUserData } from "../../../../contexts/user/UserContext.provider";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { Icon } from "../../../../components/Icon";
import { Typography } from "../../../../components/Typography";
import { useTranslation } from "react-i18next";
import { useAppMode } from "../../../../contexts/app-mode/AppModeContext.provider";
import { router } from "expo-router";

type DrawerContentProps = ScrollViewProps & {
  children?: React.ReactNode;
  backgroundColor: string;
};

export const DrawerContent = (props: DrawerContentProps) => {
  const { electionRounds } = useUserData();
  const { t } = useTranslation("drawer");
  const { setAppMode } = useAppMode();

  const theme = useTheme();
  const insets = useSafeAreaInsets();

  const handleSwitchAppModeToCitizen = () => {
    setAppMode("citizen");
    router.push("(main)");
  };

  return (
    <DrawerContentScrollView
      contentContainerStyle={{ flexGrow: 1, paddingBottom: insets.bottom + 32 }}
      {...props}
    >
      {electionRounds?.map((round, index) => (
        <DrawerItem
          key={index}
          label={`${round.status} - ${round.title}`}
          inactiveTintColor={theme.yellow6?.val}
          onPress={() => console.log("")}
          allowFontScaling={false}
        />
      ))}

      {/* app mode switch */}
      <XStack
        marginTop="auto"
        alignItems="center"
        gap="$xxs"
        paddingHorizontal="$md"
        paddingVertical="$xxxs"
        pressStyle={{ opacity: 0.5 }}
        onPress={handleSwitchAppModeToCitizen}
      >
        <Icon icon="appModeSwitch" color="white" size={32} />
        <Typography color="white" textDecorationLine="underline">
          {t("report_as", { value: t("citizen") })}
        </Typography>
      </XStack>
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
