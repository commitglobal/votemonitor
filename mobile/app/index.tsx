import React from "react";
import { Redirect } from "expo-router";
import { AppMode, useAppMode } from "../contexts/app-mode/AppModeContext.provider";
import * as SecureStore from "expo-secure-store";
import { SECURE_STORAGE_KEYS } from "../common/constants";

function RootIndex() {
  const onboardingComplete = SecureStore.getItem(SECURE_STORAGE_KEYS.ONBOARDING_NEW_COMPLETE);
  const { appMode } = useAppMode();

  if (!onboardingComplete) {
    return <Redirect href="/onboarding" />;
  }

  // 1. Redirect to citizen or app based on last OnBoarding step (Select AppMode)
  if (appMode === AppMode.CITIZEN) {
    return <Redirect href={`citizen`} />;
  }
  // 2.1. The last selection will be added in a context and here will take care of redirecting so we can do it declaratively
  if (appMode === AppMode.OBSERVER) {
    return <Redirect href="(observer)/(app)" />;
  }

  // 3. Redirect to SelectAppMode if no app mode is selected
  return <Redirect href="/select-app-mode" />;
}

export default RootIndex;
