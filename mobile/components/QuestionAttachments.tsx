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
import { AttachmentApiResponse } from "../services/api/get-attachments.api";

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
  const { data: attachments } = useAttachments(electionRoundId, pollingStationId, formId);
  const [selectedAttachment, setSelectedAttachment] = useState<AttachmentApiResponse | null>();

  const { mutate: deleteAttachment } = useDeleteAttachment(
    electionRoundId,
    pollingStationId,
    formId,
    `Attachment_${questionId}_${pollingStationId}_${formId}_${questionId}`,
  );

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
              >
                <Typography preset="body1" fontWeight="700" maxWidth="85%" numberOfLines={1}>
                  {attachment.fileName}
                </Typography>
                <YStack
                  padding="$md"
                  pressStyle={{ opacity: 0.5 }}
                  onPress={() => {
                    Keyboard.dismiss();
                    setSelectedAttachment(attachment);
                  }}
                >
                  <Icon icon="xCircle" size={24} color="$gray5" />
                </YStack>
              </Card>
            );
          })}
        </YStack>
        {selectedAttachment && (
          <WarningDialog
            title={t("warning_modal.attachment.title")}
            description={t("warning_modal.attachment.description")}
            actionBtnText={t("warning_modal.attachment.actions.clear")}
            cancelBtnText={t("warning_modal.attachment.actions.cancel")}
            action={() => {
              deleteAttachment(selectedAttachment);
              setSelectedAttachment(null);
            }}
            onCancel={() => setSelectedAttachment(null)}
          />
        )}
      </YStack>
    )
  );
};

export default QuestionAttachments;
