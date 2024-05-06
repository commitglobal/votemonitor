import React, { useEffect } from "react";
import { Slot, SplashScreen, useNavigationContainerRef } from "expo-router";
import AuthContextProvider from "../contexts/auth/AuthContext.provider";
import { TamaguiProvider } from "@tamagui/core";
import { tamaguiConfig } from "../tamagui.config";
import { useFonts } from "expo-font";
import "../common/config/i18n";
import LanguageContextProvider from "../contexts/language/LanguageContext.provider";
import PersistQueryContextProvider from "../contexts/persist-query/PersistQueryContext.provider";
import { PortalProvider } from "tamagui";
import { NetInfoProvider } from "../contexts/net-info-banner/NetInfoContext";
import NetInfoBanner from "../components/NetInfoBanner";
import { EasUpdateMonitorContextProvider } from "../contexts/eas-update/EasUpdateMonitorContextProvider";
import * as Sentry from "@sentry/react-native";
import { isRunningInExpoGo } from "expo";
import { Button } from "react-native";

// Construct a new instrumentation instance. This is needed to communicate between the integration and React
const routingInstrumentation = new Sentry.ReactNavigationInstrumentation();

Sentry.init({
  dsn: process.env.EXPO_PUBLIC_SENTRY_DSN,
  debug: process.env.EXPO_PUBLIC_ENVIRONMENT === "local",
  enableNative: true,
  environment: process.env.EXPO_PUBLIC_ENVIRONMENT,
  attachScreenshot: true,
  enabled: process.env.EXPO_PUBLIC_ENVIRONMENT !== "local",
  // Set tracesSampleRate to 1.0 to capture 100%
  // of transactions for performance monitoring.
  // We recommend adjusting this value in production
  tracesSampleRate: process.env.EXPO_PUBLIC_ENVIRONMENT === "production" ? 0.2 : 0,
  integrations: [
    new Sentry.ReactNativeTracing({
      // Pass instrumentation to be used as `routingInstrumentation`
      routingInstrumentation,
      enableNativeFramesTracking: !isRunningInExpoGo(),
      // ...
    }),
  ],
});

SplashScreen.preventAutoHideAsync();

function RootLayout() {
  const [loaded] = useFonts({
    Roboto: require("../assets/fonts/Roboto-Medium.ttf"),
    RobotoRegular: require("../assets/fonts/Roboto-Regular.ttf"),
    RobotoBold: require("../assets/fonts/Roboto-Bold.ttf"),
    DMSans: require("../assets/fonts/DMSans-Medium.ttf"),
    DMSansRegular: require("../assets/fonts/DMSans-Regular.ttf"),
    DMSansBold: require("../assets/fonts/DMSans-Bold.ttf"),
  });

  // Capture the NavigationContainer ref and register it with the instrumentation.
  const ref = useNavigationContainerRef();

  React.useEffect(() => {
    if (ref) {
      routingInstrumentation.registerNavigationContainer(ref);
    }
  }, [ref]);

  useEffect(() => {
    if (loaded) {
      // can hide splash screen here
    }
  }, [loaded]);

  if (!loaded) {
    return null;
  }

  return (
    <TamaguiProvider config={tamaguiConfig}>
      <NetInfoProvider>
        <PortalProvider>
          <AuthContextProvider>
            <PersistQueryContextProvider>
              <LanguageContextProvider>
                <EasUpdateMonitorContextProvider>
                  <Slot />
                  <NetInfoBanner />
                </EasUpdateMonitorContextProvider>
              </LanguageContextProvider>
            </PersistQueryContextProvider>
          </AuthContextProvider>
        </PortalProvider>
      </NetInfoProvider>
    </TamaguiProvider>
  );
}

// Wrap the Root Layout route component with `Sentry.wrap` to capture gesture info and profiling data.
export default Sentry.wrap(RootLayout);
