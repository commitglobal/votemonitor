import { YStack } from "tamagui";
import { Typography } from "./Typography";
import Card from "./Card";
import { Icon } from "./Icon";
import { useAttachments } from "../services/queries/attachments.query";
import { useDeleteAttachment } from "../services/mutations/attachments/delete-attachment.mutation";
import { useTranslation } from "react-i18next";
import { Keyboard } from "react-native";
import { useState } from "react";
import WarningDialog from "./WarningDialog";
import { AttachmentApiResponse, AttachmentMimeType } from "../services/api/get-attachments.api";
import { MediaDialog } from "./MediaDialog";
import AttachmentsSkeleton from "./SkeletonLoaders/AttachmentsSkeleton";

interface QuestionAttachmentsProps {
  electionRoundId: string;
  pollingStationId: string;
  formId: string;
  questionId: string;
}

const QuestionAttachments: React.FC<QuestionAttachmentsProps> = ({
  electionRoundId,
  pollingStationId,
  formId,
  questionId,
}) => {
  const { t } = useTranslation("polling_station_form_wizard");
  const { data: attachments, isLoading: isLoadingAttachments } = useAttachments(
    electionRoundId,
    pollingStationId,
    formId,
  );
  const [selectedAttachmentToDelete, setSelectedAttachmentToDelete] =
    useState<AttachmentApiResponse | null>();
  const [previewAttachment, setPreviewAttachment] = useState<AttachmentApiResponse | null>(null);

  const { mutate: deleteAttachment } = useDeleteAttachment(
    electionRoundId,
    pollingStationId,
    formId,
    `Attachment_${questionId}_${pollingStationId}_${formId}_${questionId}`,
  );

  if (isLoadingAttachments) {
    return <AttachmentsSkeleton />;
  }

  return (
    attachments?.[questionId]?.length && (
      <YStack marginTop="$lg" gap="$xxs">
        <Typography fontWeight="500">{t("attachments.heading")}</Typography>
        <YStack gap="$xxs">
          {attachments[questionId]?.map((attachment) => {
            return (
              <Card
                padding="$0"
                paddingLeft="$md"
                key={attachment.id}
                flexDirection="row"
                justifyContent="space-between"
                alignItems="center"
                onPress={() => setPreviewAttachment(attachment)}
              >
                <Typography preset="body1" fontWeight="700" maxWidth="80%" numberOfLines={1}>
                  {attachment.fileName}
                </Typography>
                <YStack
                  padding="$md"
                  pressStyle={{ opacity: 0.5 }}
                  onPress={() => {
                    Keyboard.dismiss();
                    setSelectedAttachmentToDelete(attachment);
                  }}
                >
                  <Icon icon="xCircle" size={24} color="$gray5" />
                </YStack>
              </Card>
            );
          })}
        </YStack>
        {selectedAttachmentToDelete && (
          <WarningDialog
            title={t("warning_modal.attachment.title")}
            description={t("warning_modal.attachment.description")}
            actionBtnText={t("warning_modal.attachment.actions.clear")}
            cancelBtnText={t("warning_modal.attachment.actions.cancel")}
            action={() => {
              deleteAttachment(selectedAttachmentToDelete);
              setSelectedAttachmentToDelete(null);
            }}
            onCancel={() => setSelectedAttachmentToDelete(null)}
          />
        )}

        {/* image preview dialog */}
        {previewAttachment && previewAttachment.mimeType === AttachmentMimeType.IMG && (
          <MediaDialog
            media={{ type: previewAttachment.mimeType, src: previewAttachment.presignedUrl }}
            onClose={() => setPreviewAttachment(null)}
          />
        )}
      </YStack>
    )
  );
};

export default QuestionAttachments;
