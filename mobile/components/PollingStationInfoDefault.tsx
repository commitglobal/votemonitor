import React from "react";
import { YStack } from "tamagui";
import Button from "./Button";
import { Typography } from "./Typography";

const PollingStationInfoDefault = ({ onPress }: { onPress: () => void }) => {
  return (
    <YStack
      backgroundColor="$gray1"
      borderRadius={3}
      paddingVertical="$md"
      paddingHorizontal={35}
      alignItems="center"
      gap={8}
    >
      <Typography textAlign="center" fontWeight="500" color="$gray5">
        {/* //TODO: add translation here */}
        Answer a few quick questions about the polling station.
      </Typography>
      <Button preset="outlined" backgroundColor="white" onPress={onPress}>
        Answer questions
      </Button>
    </YStack>
  );
};

export default PollingStationInfoDefault;
