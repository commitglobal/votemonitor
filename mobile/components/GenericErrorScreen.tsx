import { Stack, YStack } from "tamagui";
import { Screen } from "./Screen";
import { Icon } from "./Icon";
import { Typography } from "./Typography";
import { reloadAsync } from "expo-updates";
import Button from "./Button";
import { useTranslation } from "react-i18next";

const GenericErrorScreen = () => {
  const { t } = useTranslation("generic_error_screen");
  return (
    <Screen preset="fixed">
      <Stack height="100%" backgroundColor="white" justifyContent="center" alignItems="center">
        <YStack width={312} alignItems="center">
          <Icon icon="loadingScreenDevice" marginBottom="$md" />
          <Typography preset="subheading" textAlign="center" marginBottom="$xxxs">
            {t("paragraph1")}
          </Typography>
          <Typography preset="body1" textAlign="center" color="$gray5">
            {t("paragraph2")}
          </Typography>
          <Button style={{ marginTop: 10 }} onPress={() => reloadAsync().catch((_error) => {})}>
            {t("retry")}
          </Button>
        </YStack>
      </Stack>
    </Screen>
  );
};

export default GenericErrorScreen;
