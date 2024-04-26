import React, { useEffect } from "react";
import { Slot, SplashScreen } from "expo-router";
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

SplashScreen.preventAutoHideAsync();

export default function Root() {
  const [loaded] = useFonts({
    Roboto: require("../assets/fonts/Roboto-Medium.ttf"),
    RobotoRegular: require("../assets/fonts/Roboto-Regular.ttf"),
    RobotoBold: require("../assets/fonts/Roboto-Bold.ttf"),
    DMSans: require("../assets/fonts/DMSans-Medium.ttf"),
    DMSansRegular: require("../assets/fonts/DMSans-Regular.ttf"),
    DMSansBold: require("../assets/fonts/DMSans-Bold.ttf"),
  });

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
                <Slot />
                <NetInfoBanner />
              </LanguageContextProvider>
            </PersistQueryContextProvider>
          </AuthContextProvider>
        </PortalProvider>
      </NetInfoProvider>
    </TamaguiProvider>
  );
}
