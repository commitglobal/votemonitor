import { useIsMutating, useIsRestoring, useQueryClient } from "@tanstack/react-query";
import { SplashScreen } from "expo-router";
import { useEffect, useState } from "react";
import { Spinner, YStack } from "tamagui";
import { Typography } from "./Typography";
import { Screen } from "./Screen";
import { Icon } from "./Icon";
import { useTranslation } from "react-i18next";

export function PersistGate({ children }: React.PropsWithChildren) {
  const isRestoring = useIsRestoring();
  const queryClient = useQueryClient();

  const [pendingMutations, setPendingMutations] = useState<number>(0);

  const isMutating = useIsMutating();

  useEffect(() => {
    return queryClient.getMutationCache().subscribe((event) => {
      // You only get continue when resuming from a paused state + resumePausedMutations
      if (event.type === "updated" && event.action.type === "continue") {
        setPendingMutations(queryClient.isMutating());
      }
    });
  }, [queryClient]);

  useEffect(() => {
    if (isMutating === 0) {
      setPendingMutations(0);
    }
  }, [isMutating]);

  useEffect(() => {
    if (!isRestoring) {
      setTimeout(() => {
        SplashScreen.hideAsync();
      }, 200);
    }
  }, [isRestoring]);

  return isRestoring || pendingMutations ? <PersistGateLoadingScreen /> : children;
}

const PersistGateLoadingScreen = () => {
  const { t } = useTranslation("sync");
  const queryClient = useQueryClient();

  const runningMutations = useIsMutating();

  const [totalMutations] = useState(
    () =>
      queryClient
        .getMutationCache()
        .getAll()
        .filter((mutation) => mutation.state.isPaused || mutation.state.status === "pending")
        .length,
  );

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
            {t("pending_items")}: {runningMutations} / {totalMutations}
          </Typography>
        </YStack>
      </YStack>
    </Screen>
  );
};
