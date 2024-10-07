import React from "react";
import { Redirect } from "expo-router";
import { AppMode, useAppMode } from "../contexts/app-mode/AppModeContext.provider";

function AppModeWrapper() {
  console.log("AppModeWrapper");

  const { appMode } = useAppMode();
  console.log("appMode", appMode);

  // 1. Redirect to citizen or app based on last OnBoarding step (Select AppMode)
  if (appMode === AppMode.CITIZEN) {
    return <Redirect href={`(citizen)`} />;
  }
  // 2.1. The last selection will be added in a context and here will take care of redirecting so we can do it declaratively
  if (appMode === AppMode.OBSERVER) {
    return <Redirect href="(observer)/(app)" />;
  }

  // 3. Redirect to onboarding if not already done // TODO: add another onboarding key
  // TODO: do we actually need an onboarding screen?
  return <Redirect href="/select-app-mode" />;
}

export default AppModeWrapper;
