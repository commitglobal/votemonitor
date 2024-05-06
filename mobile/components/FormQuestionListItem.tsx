import { XStack, YStack } from "tamagui";
import Card from "./Card";
import { Typography } from "./Typography";
import Badge from "./Badge";
import CardFooter from "./CardFooter";

export enum QuestionStatus {
  ANSWERED = "answered",
  NOT_ANSWERED = "not answered",
}

const QuestionStatusToTextWrapper = {
  [QuestionStatus.ANSWERED]: "Answered",
  [QuestionStatus.NOT_ANSWERED]: "Not Answered",
};
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
}: FormQuestionListItemProps) => (
  <Card gap="$md" padding="$md" marginBottom="$xxs" onPress={onClick}>
    <YStack gap="$xxs">
      <XStack justifyContent="space-between">
        <Typography color="$gray5">{`${index}/${numberOfQuestions}`}</Typography>
        <Badge status={status}>{QuestionStatusToTextWrapper[status]}</Badge>
      </XStack>
      <Typography preset="body2">{question}</Typography>
    </YStack>
    <CardFooter text="No attached notes"></CardFooter>
  </Card>
);

export default FormQuestionListItem;
