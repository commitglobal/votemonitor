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
import Toast from "react-native-toast-message";
import { toastConfig } from "../toast.config";
import * as Notifications from "expo-notifications";
import AppModeContextProvider from "../contexts/app-mode/AppModeContext.provider";

// Construct a new instrumentation instance. This is needed to communicate between the integration and React
const routingInstrumentation = new Sentry.ReactNavigationInstrumentation();

// replace console.* for disable log on production
if (process.env.NODE_ENV === "production") {
  console.log = () => {};
  console.error = () => {};
  console.debug = () => {};
}

Notifications.setNotificationHandler({
  handleNotification: async () => ({
    shouldShowAlert: true,
    shouldPlaySound: true,
    shouldSetBadge: false,
  }),
});

Sentry.init({
  dsn: process.env.EXPO_PUBLIC_SENTRY_DSN,
  debug: __DEV__,
  enableNative: true,
  environment: process.env.EXPO_PUBLIC_ENVIRONMENT,
  attachScreenshot: true,
  enabled: false,
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

  // ! https://docs.expo.dev/develop/file-based-routing/#root-layout
  // !: With Expo Router, any React providers defined inside app/_layout.tsx are accessible by any route in your app.
  // !: To improve performance and cause fewer renders, try to reduce the scope of your providers to only the routes that need them.

  return (
    <TamaguiProvider config={tamaguiConfig}>
      <NetInfoProvider>
        <AuthContextProvider>
          <PersistQueryContextProvider>
            <PortalProvider>
              <LanguageContextProvider>
                <EasUpdateMonitorContextProvider>
                  <AppModeContextProvider>
                    <Slot />
                    <Toast config={toastConfig} position="top" />
                    <NetInfoBanner />
                  </AppModeContextProvider>
                </EasUpdateMonitorContextProvider>
              </LanguageContextProvider>
            </PortalProvider>
          </PersistQueryContextProvider>
        </AuthContextProvider>
      </NetInfoProvider>
    </TamaguiProvider>
  );
}

// Wrap the Root Layout route component with `Sentry.wrap` to capture gesture info and profiling data.
export default Sentry.wrap(RootLayout);
