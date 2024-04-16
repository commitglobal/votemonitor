import React from "react";
import { XStack } from "tamagui";
import { Typography } from "./Typography";
import Badge from "./Badge";

interface PollingStationInfoProps {
  nrOfAnswers: number | undefined;
  nrOfQuestions: number | undefined;
}

const PollingStationInfo: React.FC<PollingStationInfoProps> = ({
  nrOfAnswers = 0,
  nrOfQuestions = 0,
}) => {
  return (
    <>
      <XStack justifyContent="space-between">
        <Typography preset="heading" fontWeight="500">
          {nrOfAnswers}/{nrOfQuestions} questions
        </Typography>
        <Badge status={nrOfAnswers === nrOfQuestions ? "completed" : "in progress"} />
      </XStack>
    </>
  );
};

export default PollingStationInfo;
