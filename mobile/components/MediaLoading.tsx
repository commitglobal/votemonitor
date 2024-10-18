import { useTranslation } from "react-i18next";
import { Spinner, YStack } from "tamagui";
import { Typography } from "./Typography";
import Button from "./Button";
import { useNetInfoContext } from "../contexts/net-info-banner/NetInfoContext";

const MediaLoading = ({ progress, isUploading, onOfflineCallback }: { progress?: string, isUploading?: boolean, onOfflineCallback?: () => void }) => {
  const { isOnline } = useNetInfoContext();
  const { t } = useTranslation("polling_station_form_wizard");
  return (
    <YStack alignItems="center" gap="$lg" paddingHorizontal="$lg">
      <Spinner size="large" color="$purple5" />
      <Typography preset="subheading" fontWeight="500" color="$purple5">
        {progress || t("attachments.loading")}
      </Typography>
      {!isOnline && isUploading && (
        <>
          <Typography preset="body1" fontWeight="500" color="$purple5">
            {t("attachments.upload.abort_offline")}
          </Typography>
          <Button preset="red" onPress={onOfflineCallback}>
            {t("attachments.upload.abort_offline_button")}
          </Button>
        </>
      )}
    </YStack>
  );
};

export default MediaLoading;
