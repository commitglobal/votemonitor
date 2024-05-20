import { useIsRestoring } from "@tanstack/react-query";
import { SplashScreen } from "expo-router";
import { useEffect } from "react";
import { Spinner, YStack } from "tamagui";
import { Typography } from "./Typography";
import { Screen } from "./Screen";
import { Icon } from "./Icon";

export function PersistGate({ children }: React.PropsWithChildren) {
  const isRestoring = useIsRestoring();

  useEffect(() => {
    if (!isRestoring) {
      SplashScreen.hideAsync();
    }
  }, [isRestoring]);

  return isRestoring ? <PersistGateLoadingScreen /> : children;
}

const PersistGateLoadingScreen = () => {
  return (
    <Screen
      preset="fixed"
      backgroundColor="#FDD20C"
      contentContainerStyle={{
        flexGrow: 1,
      }}
    >
      <YStack justifyContent="center" alignItems="center" flexGrow={1} marginHorizontal={45}>
        <Icon icon="splashLogo" marginTop={200} />
        <YStack height={200} paddingTop="$lg">
          <Spinner size="large" color="white" />
          <Typography preset="body2" color="white" textAlign="center" marginTop="$lg">
            Synchronizing data...
          </Typography>
          <Typography preset="body2" color="white" textAlign="center" marginTop="$sm">
            Please DO NOT CLOSE the app or the data will be lost.
          </Typography>
        </YStack>
      </YStack>
    </Screen>
  );
};
