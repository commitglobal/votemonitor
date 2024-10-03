import React, { useEffect } from "react";
import { Redirect } from "expo-router";
import * as SecureStore from "expo-secure-store";
import { SECURE_STORAGE_KEYS } from "../common/constants";

function AppModeWrapper() {
  console.log("AppModeWrapper");
  const APP_MODE = "citizen";

  useEffect(() => {
    try {
      const onboardingComplete = SecureStore.getItem(SECURE_STORAGE_KEYS.ONBOARDING_NEW_COMPLETE);
      if (onboardingComplete !== "true") {
        // setOnboardingComplete(false);
      }
    } catch (err) {
      // Sentry.captureException(err);
    }
  }, []);

  // 1. Redirect to onboarding if not already done // TODO: add another onboarding key
  // 2. Redirect to citizen or app based on last OnBoarding step (Select AppMode)
  // 2.1. The last selection will be added in a context and here will take care of redirecting so we can do it declaratively
  return APP_MODE === "citizen" ? (
    <Redirect href="(citizen)/" />
  ) : (
    <Redirect href="(observer)/(app)" />
  );
}

export default AppModeWrapper;
