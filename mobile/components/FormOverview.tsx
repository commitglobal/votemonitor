import React from "react";
import { XStack, YStack } from "tamagui";
import { Typography } from "./Typography";
import CircularProgress from "./CircularProgress";
import Button from "./Button";

const mockForm = {
  id: "5043260e-017b-4e48-bb31-6e8bcdd870f0",
  name: "A1 - formular de test",
  numberOfCompletedQuestions: 0,
  numberOfQuestions: 6,
  options: "Available in EN, RO",
  status: "not started",
};

const FormStatus = () => {
  return (
    <XStack alignItems="center" justifyContent="space-between">
      <YStack gap="$sm">
        <Typography fontWeight="500" color="$gray5">
          Form status:{" "}
          <Typography fontWeight="700">
            {mockForm.status.charAt(0).toUpperCase() + mockForm.status.slice(1)}
          </Typography>
        </Typography>
        <Typography fontWeight="500" color="$gray5">
          Answered questions:{" "}
          <Typography fontWeight="700">
            {mockForm.numberOfCompletedQuestions}/{mockForm.numberOfQuestions}
          </Typography>
        </Typography>
      </YStack>

      <CircularProgress progress={90} size={98} />
    </XStack>
  );
};

const FormOverview = () => {
  return (
    <YStack>
      {/* //TODO: translations */}
      {/* title */}
      <Typography preset="body1" fontWeight="700">
        Form overview
      </Typography>
      {/* completed status */}
      <FormStatus />
      <Button preset="outlined" marginTop="$md">
        Start form
      </Button>
    </YStack>
  );
};

export default FormOverview;
