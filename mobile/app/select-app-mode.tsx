import React, { useState } from "react";
import { Screen } from "../components/Screen";
import Header from "../components/Header";
import { Icon } from "../components/Icon";
import { Typography } from "../components/Typography";
import { ScrollView, XStack, YStack } from "tamagui";
import { useTranslation } from "react-i18next";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { Selector } from "../components/Selector";
import Button from "../components/Button";
import { AppMode, useAppMode } from "../contexts/app-mode/AppModeContext.provider";

export default function SelectAppMode() {
  const { t } = useTranslation("select_app_mode");
  const insets = useSafeAreaInsets();
  const [appMode, setAppMode] = useState<AppMode>(AppMode.CITIZEN);
  const { setAppMode: setAppModeContext } = useAppMode();

  const setAppModeToCitizen = () => {
    setAppMode(AppMode.CITIZEN);
  };

  const setAppModeToAccreditedObserver = () => {
    setAppMode(AppMode.OBSERVER);
  };

  const handleSetAppModeContext = () => {
    setAppModeContext(appMode);
  };

  return (
    <Screen
      preset="fixed"
      contentContainerStyle={{
        flex: 1,
      }}
    >
      <Header barStyle="light-content" backgroundColor="$purple6">
        <Icon icon="loginLogo" paddingBottom="$md" />
        <Typography preset="heading" color="white" fontWeight="500">
          {t("heading")}
        </Typography>
      </Header>

      <YStack
        flex={1}
        backgroundColor="$purple6"
        paddingTop="$xl"
        paddingBottom={insets.bottom + 32}
        gap="$xl"
      >
        <ScrollView
          showsVerticalScrollIndicator={false}
          contentContainerStyle={{
            gap: 32,
            flexGrow: 1,
            paddingHorizontal: 32,
          }}
        >
          <Typography textAlign="center" preset="body1" opacity={0.8} color="white">
            {t("description")}
          </Typography>

          <YStack gap="$md">
            <Selector
              title={t("accredited_observer.title")}
              description={t("accredited_observer.description")}
              selected={appMode === AppMode.OBSERVER}
              onPress={setAppModeToAccreditedObserver}
            />
            <Selector
              title={t("citizen.title")}
              description={t("citizen.description")}
              selected={appMode === AppMode.CITIZEN}
              onPress={setAppModeToCitizen}
            />
          </YStack>

          <Typography preset="body1" color="white" opacity={0.8}>
            {t("helper")}
          </Typography>
        </ScrollView>

        <XStack justifyContent="center" alignItems="center" paddingHorizontal="$xl">
          <Button flex={1} preset="yellow" onPress={handleSetAppModeContext}>
            {t("continue")}
          </Button>
        </XStack>
      </YStack>
    </Screen>
  );
}
