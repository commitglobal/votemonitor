import { useTranslation } from "react-i18next";
import { Spinner, YStack } from "tamagui";
import { Typography } from "./Typography";

const MediaLoading = ({ progress }: { progress?: string }) => {
  const { t } = useTranslation("polling_station_form_wizard");
  return (
    <YStack alignItems="center" gap="$lg" paddingHorizontal="$lg">
      <Spinner size="large" color="$purple5" />
      <Typography preset="subheading" fontWeight="500" color="$purple5">
        {progress || t("attachments.loading")}
      </Typography>
    </YStack>
  );
};

export default MediaLoading;
