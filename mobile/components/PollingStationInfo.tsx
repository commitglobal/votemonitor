import React from "react";
import { XStack } from "tamagui";
import { Typography } from "./Typography";
import Badge from "./Badge";
import { FormProgress } from "./Badge";

interface PollingStationInfoProps {
  nrOfAnswers: number | undefined;
  nrOfQuestions: number | undefined;
  status: string;
}

const PollingStationInfo: React.FC<PollingStationInfoProps> = ({
  nrOfAnswers = 0,
  nrOfQuestions = 0,
  status = "not started",
}) => {
  return (
    <>
      <XStack justifyContent="space-between">
        <Typography preset="heading" fontWeight="500">
          {nrOfAnswers}/{nrOfQuestions} questions
        </Typography>
        <Badge status={status} />
      </XStack>
    </>
  );
};

export default PollingStationInfo;
