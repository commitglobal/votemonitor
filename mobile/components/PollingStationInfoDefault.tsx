import React from "react";
import { YStack, Text, Button } from "tamagui";

const PollingStationInfoDefault = () => {
  return (
    <YStack
      backgroundColor="$gray1"
      borderRadius={3}
      paddingVertical="$md"
      paddingHorizontal={35}
      alignItems="center"
      gap={8}
    >
      {/* //TODO: typography here */}
      <Text textAlign="center" color="$gray7">
        {/* //TODO: add translation here */}
        Answer a few quick questions about the polling station.
      </Text>
      {/* //TODO: use custom button */}
      <Button variant="outlined">Answer Question</Button>
    </YStack>
  );
};

export default PollingStationInfoDefault;
