import React, { useEffect, useState } from "react";
import { Slot } from "expo-router";
import AuthContextProvider from "../contexts/auth/AuthContext.provider";
import { TamaguiProvider } from "@tamagui/core";
import { tamaguiConfig } from "../tamagui.config";
import { useFonts } from "expo-font";
import "../common/config/i18n";
import LanguageContextProvider from "../contexts/language/LanguageContext.provider";
import PersistQueryContextProvider from "../contexts/persist-query/PersistQueryContext.provider";

import { Stack, XStack } from "tamagui";
import { Icon } from "../components/Icon";
import { Typography } from "../components/Typography";
import NetInfo from "@react-native-community/netinfo";

export default function Root() {
  const [loaded] = useFonts({
    Roboto: require("../assets/fonts/Roboto-Medium.ttf"),
    RobotoRegular: require("../assets/fonts/Roboto-Regular.ttf"),
    RobotoBold: require("../assets/fonts/Roboto-Bold.ttf"),
    DMSans: require("../assets/fonts/DMSans-Medium.ttf"),
    DMSansRegular: require("../assets/fonts/DMSans-Regular.ttf"),
    DMSansBold: require("../assets/fonts/DMSans-Bold.ttf"),
  });

  const [isOnline, setIsOnline] = useState(true);
  const [showNetInfoBanner, setShowNetInfoBanner] = useState(true);

  useEffect(() => {
    const unsubscribe = NetInfo.addEventListener((state) => {
      const status = !!state.isConnected;
      setIsOnline(!status);
    });
    return unsubscribe();
  }, []);

  // show online banner again after user is connected again
  useEffect(() => {
    if (isOnline) setShowNetInfoBanner(true);
  }, [isOnline]);

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
                  paddingLeft={20}
                  justifyContent="space-between"
                  alignItems="center"
                  // position="absolute"
                  width="100%"
                  // bottom={50}
                  zIndex={100_000}
                >
                  <Typography fontWeight="500" color="$gray7" paddingBottom={50}>
                    App online. All answers sent to server.
                  </Typography>
                  <Stack
                    onPress={() => setShowNetInfoBanner(false)}
                    paddingVertical="$xxs"
                    paddingHorizontal={20}
                  >
                    <Icon icon="x" size={16} />
                  </Stack>
                </XStack>
              )
            ) : (
              <XStack
                backgroundColor="$red6"
                paddingVertical="$xxs"
                paddingHorizontal={20}
                width="100%"
              >
                <Typography fontWeight="500" color="$gray7" paddingBottom={10}>
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
