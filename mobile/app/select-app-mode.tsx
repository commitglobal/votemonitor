import React, { useState } from "react";
import { Screen } from "../components/Screen";
import Header from "../components/Header";
import { Icon } from "../components/Icon";
import { Typography } from "../components/Typography";
import { ScrollView, YStack } from "tamagui";
import { useTranslation } from "react-i18next";
import { Selector } from "../components/Selector";
import { AppMode, useAppMode } from "../contexts/app-mode/AppModeContext.provider";
import { useRouter } from "expo-router";
import WizzardControls from "../components/WizzardControls";

export default function SelectAppMode() {
  const { t } = useTranslation("select_app_mode");
  const { appMode, setAppMode: setAppModeContext } = useAppMode();
  const [appModeLocal, setAppModeLocal] = useState<AppMode>(appMode || AppMode.CITIZEN);
  const router = useRouter();

  const setAppModeToCitizen = () => {
    setAppModeLocal(AppMode.CITIZEN);
  };

  const setAppModeToAccreditedObserver = () => {
    setAppModeLocal(AppMode.OBSERVER);
  };

  const handleSetAppModeContext = () => {
    setAppModeContext(appModeLocal);
    router.replace(appModeLocal === AppMode.CITIZEN ? "citizen" : "(observer)/(app)");
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

      <YStack flex={1} backgroundColor="$purple6" paddingTop="$xl" gap="$xl">
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
              selected={appModeLocal === AppMode.OBSERVER}
              onPress={setAppModeToAccreditedObserver}
            />
            <Selector
              title={t("citizen.title")}
              description={t("citizen.description")}
              selected={appModeLocal === AppMode.CITIZEN}
              onPress={setAppModeToCitizen}
            />
          </YStack>

          <Typography preset="body1" color="white" opacity={0.8}>
            {t("helper")}
          </Typography>
        </ScrollView>

        <WizzardControls
          actionBtnPreset="yellow"
          onActionButtonPress={handleSetAppModeContext}
          actionBtnLabel={t("continue")}
          isFirstElement
          backgroundColor="$purple6"
        />
      </YStack>
    </Screen>
  );
}
