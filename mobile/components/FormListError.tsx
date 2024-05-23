import { Stack, YStack } from "tamagui";
import { Icon } from "./Icon";
import { Typography } from "./Typography";
import Button from "./Button";
import { useQueryClient } from "@tanstack/react-query";
import { useTranslation } from "react-i18next";

const FormListErrorScreen = () => {
  const queryClient = useQueryClient();
  const { t } = useTranslation("observation");

  return (
    <Stack height="100%" justifyContent="center" alignItems="center">
      <YStack width={312} alignItems="center">
        <Icon icon="loadingScreenDevice" marginBottom="$md" />
        <Typography preset="subheading" textAlign="center" marginBottom="$xxxs">
          {t("forms.list.error.paragraph1")}
        </Typography>
        <Typography preset="body1" textAlign="center" color="$gray5">
          {t("forms.list.error.paragraph2")}
        </Typography>
        <Button style={{ marginTop: 10 }} onPress={() => queryClient.invalidateQueries()}>
          {t("forms.list.error.retry")}
        </Button>
      </YStack>
    </Stack>
  );
};

export default FormListErrorScreen;
