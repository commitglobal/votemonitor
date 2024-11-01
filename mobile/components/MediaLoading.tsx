import { useTranslation } from "react-i18next";
import { Spinner, XStack, YStack } from "tamagui";
import { Typography } from "./Typography";
import Button from "./Button";
import { useNetInfoContext } from "../contexts/net-info-banner/NetInfoContext";
import { useState } from "react";
const MediaLoading = ({
  progress,
  isUploading,
  uploadedAttachments,
  onAbortUpload,
  confirmAbort
}: {
  progress?: string;
  isUploading?: boolean;
  uploadedAttachments?: number;
  onAbortUpload?: () => void;
  confirmAbort?: boolean
}) => {
  const { isOnline } = useNetInfoContext();
  const { t } = useTranslation("polling_station_form_wizard");
  const [promptConfirmAbort, setPromptConfirmAbort] = useState(false);

  return (
    <YStack alignItems="center" gap="$lg" paddingHorizontal="$lg">
      <Spinner size="large" color="$purple5" />
      <Typography preset="subheading" fontWeight="500" color="$purple5">
        {progress || t("attachments.loading")}
      </Typography>
      {!isOnline && isUploading && onAbortUpload && (
        <>
          <Typography preset="body1" fontWeight="500" color="$purple5">
            {t("attachments.upload.abort_offline")}
          </Typography>
          {uploadedAttachments && uploadedAttachments > 0 && (
            <Typography preset="body1" fontWeight="500" color="$purple5">
              {t("attachments.upload.uploaded", { value: uploadedAttachments })}
            </Typography>
          )}

          {!promptConfirmAbort && (
            <Button preset="red" onPress={() => confirmAbort ? setPromptConfirmAbort(true) : onAbortUpload()}>
              {t("attachments.upload.abort_offline_button")}
            </Button>
          )}

          {promptConfirmAbort && (
            <>
              <Typography preset="body1" fontWeight="500" color="$purple5">
                {t("attachments.upload.abort_offline_confirm")}
              </Typography>
              <XStack gap="$xs">
                <Button preset="red" onPress={onAbortUpload}>
                  {t("attachments.upload.abort_offline_button")}
                </Button>
                <Button preset="outlined" onPress={() => setPromptConfirmAbort(false)}>
                  {t("attachments.upload.abort_offline_cancel")}
                </Button>
              </XStack>
            </>
          )}
        </>
      )}
    </YStack>
  );
};

export default MediaLoading;
