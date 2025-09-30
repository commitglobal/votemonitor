import { format } from "date-fns";
import { useTranslation } from "react-i18next";
import { XStack, YStack } from "tamagui";
import { SubmissionStateToTextMapper } from "../common/utils/form-submissions";
import { FormSubmission } from "../services/definitions.api";
import { SubmissionStatus } from "../services/form.parser";
import Badge from "./Badge";
import Card from "./Card";
import { Icon } from "./Icon";
import { Typography } from "./Typography";
export type FormSubmissionDetails = FormSubmission & {
  submissionNumber: number;
  numberOfQuestions: number;
  numberOfCompletedQuestions: number;
  numberOfNotes: number;
  numberOfAttachments: number;
  formStatus: SubmissionStatus;
};

type FormSubmissionListItemProps = FormSubmissionDetails & {
  onClick: () => void;
  onDeleteSubmission: () => void;
};

const FormSubmissionListItem = ({
  submissionNumber,
  numberOfCompletedQuestions,
  numberOfQuestions,
  numberOfNotes,
  numberOfAttachments,
  formStatus,
  lastUpdatedAt,
  onClick,
  onDeleteSubmission,
}: FormSubmissionListItemProps) => {
  const { t } = useTranslation(["form_submission_card", "common"]);

  return (
    <Card gap="$xxs" padding="$md" marginBottom="$xxs">
      <XStack justifyContent="space-between" alignItems="center">
        <Typography preset="body1" fontWeight="700" flexShrink={1}>
          {t("header", {
            value: submissionNumber,
          })}
        </Typography>

        <YStack
          onPress={onDeleteSubmission}
          pressStyle={{ opacity: 0.5 }}
          justifyContent="center"
          alignItems="center"
        >
          <Icon icon="bin" color="$red10" size={24} />
        </YStack>
      </XStack>

      <YStack onPress={onClick} gap="$xxs">
        <XStack justifyContent="space-between">
          <Typography preset="body2">{t("last_updated_at")}</Typography>
          <Typography preset="body1">{format(lastUpdatedAt, "yyyy-MM-dd HH:mm")}</Typography>
        </XStack>
        <XStack justifyContent="space-between">
          <Typography preset="body2">{t("form_status")}</Typography>
          <Badge status={formStatus}>
            {t(SubmissionStateToTextMapper[formStatus], { ns: "common" })}
          </Badge>
        </XStack>
        <XStack justifyContent="space-between">
          <Typography preset="body2">{t("answered_questions")}</Typography>
          <Typography preset="body1">
            {numberOfCompletedQuestions} / {numberOfQuestions}
          </Typography>
        </XStack>
        {numberOfAttachments ? (
          <XStack justifyContent="space-between">
            <Typography preset="body2">{t("attachments")}</Typography>
            <Typography preset="body1">{numberOfAttachments}</Typography>
          </XStack>
        ) : null}
        {numberOfNotes ? (
          <XStack justifyContent="space-between">
            <Typography preset="body2">{t("notes")}</Typography>
            <Typography preset="body1">{numberOfNotes}</Typography>
          </XStack>
        ) : null}
      </YStack>
    </Card>
  );
};

export default FormSubmissionListItem;
