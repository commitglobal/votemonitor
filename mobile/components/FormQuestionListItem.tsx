import { XStack, YStack } from "tamagui";
import Card from "./Card";
import { Typography } from "./Typography";
import Badge from "./Badge";
import { useTranslation } from "react-i18next";
import { ApiFormAnswer } from "../services/interfaces/answer.type";
import QuestionCardFooter from "./QuestionCardFooter";
import { AttachmentMimeType } from "../services/api/get-attachments.api";
import { getAnswerDisplay } from "../common/utils/answers";

export enum QuestionStatus {
  ANSWERED = "answered",
  NOT_ANSWERED = "not answered",
}

export interface FormQuestionListItemProps {
  id: string;
  index: number;
  numberOfQuestions: number;
  status: QuestionStatus;
  code: string;
  question: string;
  numberOfNotes: number;
  numberOfAttachments: number;
  answer?: ApiFormAnswer;
  lastNoteText?: string;
  attachmentTypes?: AttachmentMimeType[];
  onClick: () => void;
}

const FormQuestionListItem = ({
  index,
  numberOfQuestions,
  numberOfNotes,
  numberOfAttachments,
  attachmentTypes,
  status,
  code,
  question,
  answer,
  lastNoteText,
  onClick,
}: FormQuestionListItemProps) => {
  const { t } = useTranslation("form_overview");

  const QuestionStatusToTextWrapper = {
    [QuestionStatus.ANSWERED]: t("questions.answered"),
    [QuestionStatus.NOT_ANSWERED]: t("questions.not_answered"),
  };

  return (
    <Card gap="$xxs" padding="$md" marginBottom="$xxs" onPress={onClick}>
      <YStack gap="$xxs">
        <XStack justifyContent="space-between">
          <Typography color="$gray5">{`${index}/${numberOfQuestions}`}</Typography>
          <Badge status={status}>{QuestionStatusToTextWrapper[status]}</Badge>
        </XStack>
        <Typography preset="body2">
          {code} - {question}
        </Typography>
      </YStack>

      {answer && (
        <Typography preset="body1" color="$gray6" numberOfLines={1} width="90%">
          {getAnswerDisplay(answer)}
        </Typography>
      )}
      <QuestionCardFooter
        numberOfAttachments={numberOfAttachments}
        numberOfNotes={numberOfNotes}
        lastNoteText={lastNoteText}
        attachmentTypes={attachmentTypes}
      ></QuestionCardFooter>
    </Card>
  );
};

export default FormQuestionListItem;
