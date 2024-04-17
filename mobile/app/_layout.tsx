import React, { useEffect, useState } from "react";
import { Slot } from "expo-router";
import AuthContextProvider from "../contexts/auth/AuthContext.provider";
import { TamaguiProvider } from "@tamagui/core";
import { tamaguiConfig } from "../tamagui.config";
import { useFonts } from "expo-font";
import "../common/config/i18n";
import LanguageContextProvider from "../contexts/language/LanguageContext.provider";
import PersistQueryContextProvider from "../contexts/persist-query/PersistQueryContext.provider";

import { XStack } from "tamagui";
import { Typography } from "../components/Typography";
import NetInfo from "@react-native-community/netinfo";
import { useSafeAreaInsets } from "react-native-safe-area-context";

export default function Root() {
  const [loaded] = useFonts({
    Roboto: require("../assets/fonts/Roboto-Medium.ttf"),
    RobotoRegular: require("../assets/fonts/Roboto-Regular.ttf"),
    RobotoBold: require("../assets/fonts/Roboto-Bold.ttf"),
    DMSans: require("../assets/fonts/DMSans-Medium.ttf"),
    DMSansRegular: require("../assets/fonts/DMSans-Regular.ttf"),
    DMSansBold: require("../assets/fonts/DMSans-Bold.ttf"),
  });

  const [isOnline, setIsOnline] = useState(false);
  const [showNetInfoBanner, setShowNetInfoBanner] = useState(true);
  const insets = useSafeAreaInsets();

  useEffect(() => {
    const unsubscribe = NetInfo.addEventListener((state) => {
      const status = !!state.isConnected;
      setIsOnline(status);
    });
    return unsubscribe();
  }, []);

  // show online banner again after user is connected again
  useEffect(() => {
    if (isOnline) setShowNetInfoBanner(true);
  }, [isOnline]);

  // remove online banner after 3 seconds
  useEffect(() => {
    if (showNetInfoBanner) {
      const timer = setTimeout(() => {
        setShowNetInfoBanner(false);
      }, 3000);
      return () => clearTimeout(timer);
    }
    return;
  }, [showNetInfoBanner]);

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
      <AuthContextProvider>
        <PersistQueryContextProvider>
          <LanguageContextProvider>
            <Slot />
            {isOnline ? (
              showNetInfoBanner && (
                <XStack
                  backgroundColor="$green7"
                  padding="$xxs"
                  justifyContent="center"
                  alignItems="center"
                  width="100%"
                  zIndex={100_000}
                >
                  <Typography
                    fontWeight="500"
                    color="$gray7"
                    paddingBottom={insets.bottom}
                    textAlign="center"
                  >
                    App online. All answers sent to server.
                  </Typography>
                </XStack>
              )
            ) : (
              <XStack
                backgroundColor="$red9"
                padding="$xxs"
                justifyContent="center"
                alignItems="center"
                width="100%"
                zIndex={100_000}
              >
                <Typography fontWeight="500" color="white" paddingBottom={insets.bottom}>
                  Offline mode. Saving answers locally.
                </Typography>
              </XStack>
            )}
          </LanguageContextProvider>
        </PersistQueryContextProvider>
      </AuthContextProvider>
    </TamaguiProvider>
  );
}
