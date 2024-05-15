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
  index: number;
  numberOfQuestions: number;
  status: QuestionStatus;
  question: string;
  onClick: () => void;
}

const FormQuestionListItem = ({
  index,
  numberOfQuestions,
  status,
  question,
  onClick,
}: FormQuestionListItemProps) => {
  const { t } = useTranslation("form_overview");

  const QuestionStatusToTextWrapper = {
    [QuestionStatus.ANSWERED]: t("questions.answered"),
    [QuestionStatus.NOT_ANSWERED]: t("questions.not_answered"),
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

      {/* TODO: change this text in case there are attached notes */}
      <CardFooter text={t("questions.no_notes")}></CardFooter>
    </Card>
  );
};

export default FormQuestionListItem;
