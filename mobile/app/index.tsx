import React from "react";
import { Redirect } from "expo-router";
import { AppMode, useAppMode } from "../contexts/app-mode/AppModeContext.provider";
import { Typography } from "../components/Typography";
import { Screen } from "../components/Screen";
import { YStack } from "tamagui";
import Button from "../components/Button";

function AppModeWrapper() {
  console.log("AppModeWrapper");

  const { appMode, setAppMode } = useAppMode();
  console.log("appMode", appMode);

  // 1. Redirect to citizen or app based on last OnBoarding step (Select AppMode)
  if (appMode === "citizen") {
    return <Redirect href={`(citizen)`} />;
  }
  // 2.1. The last selection will be added in a context and here will take care of redirecting so we can do it declaratively
  if (appMode === "observer") {
    return <Redirect href="(observer)/(app)" />;
  }

  // 3. Redirect to onboarding if not already done // TODO: add another onboarding key
  // TODO: do we actually need an onboarding screen?
  return (
    <Screen>
      <Typography>Onboarding</Typography>
      <YStack style={{ paddingVertical: 100, gap: 25 }}>
        <Button onPress={() => setAppMode(AppMode.CITIZEN)}>Go to Citizen</Button>
        <Button onPress={() => setAppMode(AppMode.OBSERVER)}>Go to Observer</Button>
      </YStack>
    </Screen>
  );
}

export default AppModeWrapper;
