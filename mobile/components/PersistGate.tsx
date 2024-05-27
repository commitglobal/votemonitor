import { useIsMutating, useIsRestoring } from "@tanstack/react-query";
import { SplashScreen } from "expo-router";
import { useEffect } from "react";
import { Spinner, YStack } from "tamagui";
import { Typography } from "./Typography";
import { Screen } from "./Screen";
import { Icon } from "./Icon";
import { useTranslation } from "react-i18next";

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
  const { t } = useTranslation("sync");

  const runningMutations = useIsMutating();

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
            {t("sync_data")}
          </Typography>
          <Typography preset="body2" color="white" textAlign="center" marginTop="$sm">
            {t("warning")}
          </Typography>
          <Typography preset="body2" color="white" textAlign="center" marginTop="$sm">
            Items left: {runningMutations}
          </Typography>
        </YStack>
      </YStack>
    </Screen>
  );
};
