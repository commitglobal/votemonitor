import React from "react";
import Card from "./Card";
import { XStack } from "tamagui";
import { Typography } from "./Typography";
import Badge from "./Badge";
import CardFooter from "./CardFooter";

interface QuestionCardProps {
  // TODO: question type
  question: any;
  index: number;
  onPress: () => void;
}

const QuestionCard: React.FC<QuestionCardProps> = ({ question, index, onPress }) => {
  const { numberOfCompletedQuestions, numberOfQuestions, status, text } = question;
  return (
    <Card onPress={onPress}>
      <XStack alignItems="center" justifyContent="space-between">
        <Typography color="$gray5" fontWeight="500">
          {index}/{numberOfQuestions}
        </Typography>
        {/* //TODO: status is not alright */}
        <Badge status={status} />
      </XStack>

      <Typography preset="body2" marginTop="$xxs">
        {text}
      </Typography>

      <CardFooter text="No attached notes" marginTop="$md" />
    </Card>
  );
};

export default QuestionCard;
