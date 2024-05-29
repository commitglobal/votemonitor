import { Stack, YStack } from "tamagui";
import { Screen } from "./Screen";
import { Icon } from "./Icon";
import { Typography } from "./Typography";
import { useTranslation } from "react-i18next";

const LoadingScreen = () => {
  const { t } = useTranslation("loading_screen");
  return (
    <Screen preset="fixed">
      <Stack height="100%" backgroundColor="white" justifyContent="center" alignItems="center">
        <YStack width={312} alignItems="center">
          <Icon icon="loadingScreenDevice" marginBottom="$md" />
          <Typography preset="subheading" textAlign="center" marginBottom="$xxxs">
            {t("heading")}
          </Typography>
          <Typography preset="body1" textAlign="center" color="$gray5">
            {t("paragraph")}
          </Typography>
        </YStack>
      </Stack>
    </Screen>
  );
};

export default LoadingScreen;
