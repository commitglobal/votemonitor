import { XStack, YStack } from "tamagui";
import Card from "./Card";
import { Typography } from "./Typography";
import Badge from "./Badge";
import CardFooter from "./CardFooter";
import { useTranslation } from "react-i18next";

export enum QuestionStatus {
  ANSWERED = "answered",
  NOT_ANSWERED = "not answered",
}

export interface FormQuestionListItemProps {
  id: string;
  index: number;
  numberOfQuestions: number;
  status: QuestionStatus;
  question: string;
  numberOfNotes: number;
  numberOfAttachments: number;
  onClick: () => void;
}

const FormQuestionListItem = ({
  index,
  numberOfQuestions,
  numberOfNotes,
  numberOfAttachments,
  status,
  question,
  onClick,
}: FormQuestionListItemProps) => {
  const { t } = useTranslation("form_overview");

  const QuestionStatusToTextWrapper = {
    [QuestionStatus.ANSWERED]: t("questions.answered"),
    [QuestionStatus.NOT_ANSWERED]: t("questions.not_answered"),
  };

  const footerText = () => {
    const note = numberOfNotes > 0 ? `${numberOfNotes} Note(s)` : "";
    const attachments = numberOfAttachments > 0 ? `${numberOfAttachments} Media File(s)` : "";

    return [note, attachments].filter(Boolean).join(", ") || t("questions.no_notes");
  };

  return (
    <Card gap="$md" padding="$md" marginBottom="$xxs" onPress={onClick}>
      <YStack gap="$xxs">
        <XStack justifyContent="space-between">
          <Typography color="$gray5">{`${index}/${numberOfQuestions}`}</Typography>
          <Badge status={status}>{QuestionStatusToTextWrapper[status]}</Badge>
        </XStack>
        <Typography preset="body2">{question}</Typography>
      </YStack>

      <CardFooter text={footerText()}></CardFooter>
    </Card>
  );
};

export default FormQuestionListItem;
