import { useIsMutating, useIsRestoring } from "@tanstack/react-query";
import { SplashScreen } from "expo-router";
import { useEffect, useState } from "react";
import { YStack } from "tamagui";
import { Typography } from "./Typography";
import { Screen } from "./Screen";
import { Icon } from "./Icon";
import { useTranslation } from "react-i18next";
import CircularProgress from "./CircularProgress";
import { AttachmentsKeys } from "../services/queries/attachments.query";

export function PersistGate({ children }: React.PropsWithChildren) {
  const isRestoring = useIsRestoring();

  const [totalMutations, setTotalMutations] = useState(0);

  const isMutating = useIsMutating();

  useEffect(() => {
    if (isMutating === 0 || totalMutations === 0) setTotalMutations(isMutating);
  }, [isMutating]);

  useEffect(() => {
    setTimeout(() => {
      SplashScreen.hideAsync();
    }, 500);
  }, []);

  return isRestoring ? <PersistGateLoadingScreen totalMutations={totalMutations} /> : children;
}

const PersistGateLoadingScreen = ({ totalMutations }: { totalMutations: number }) => {
  const { t } = useTranslation("sync");

  const runningMutations = useIsMutating();

  const runningFileMutations = useIsMutating({ mutationKey: AttachmentsKeys.all });

  return (
    <Screen
      preset="fixed"
      backgroundColor="#FDD20C"
      contentContainerStyle={{
        flexGrow: 1,
      }}
    >
      <YStack
        justifyContent="center"
        gap={50}
        alignItems="center"
        flexGrow={1}
        marginHorizontal={45}
      >
        <YStack>
          <Typography
            preset="body2"
            color="white"
            textAlign="center"
            marginTop="$lg"
            marginBottom="$md"
          >
            {t("sync_data")}
          </Typography>

          <CircularProgress
            progress={
              totalMutations ? ((totalMutations - runningMutations) / totalMutations) * 100 : 0
            }
            size={98}
            progressCircleColors={["#fff", "#A16207", "#10B981"]}
            backgroundCircleColors={["#fff", "#fff", "#fff"]}
          />
        </YStack>
        <Icon icon="splashLogo" marginTop={0} />
        <YStack height={200} paddingTop="$lg">
          <Typography preset="body2" color="white" textAlign="center" marginTop="$sm">
            {t("warning")}
          </Typography>

          {runningFileMutations > 0 && (
            <Typography preset="body2" color="white" textAlign="center" marginTop="$sm">
              {t("files")}
            </Typography>
          )}
        </YStack>
      </YStack>
    </Screen>
  );
};
