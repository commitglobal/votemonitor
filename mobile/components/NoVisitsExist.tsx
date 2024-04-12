import React from "react";
import { router } from "expo-router";
import { Stack, YStack } from "tamagui";
import { Screen } from "./Screen";
import { Icon } from "./Icon";
import { Typography } from "./Typography";
import Button from "./Button";

const NoVisitsExist = () => (
  <Screen preset="fixed">
    <Stack height="100%" backgroundColor="white" justifyContent="center" alignItems="center">
      <YStack width={312} alignItems="center" gap="$md">
        <Icon icon="missingPollingStation" />
        <YStack gap="$xxxs">
          <Typography preset="subheading" textAlign="center">
            No visited polling stations yet
          </Typography>
          <Typography preset="body1" textAlign="center" color="$gray5">
            Start configuring your first polling station before completing observation forms.
          </Typography>
        </YStack>
        <Button
          preset="outlined"
          backgroundColor="white"
          width="100%"
          onPress={router.push.bind(null, "/polling-station-wizzard")}
        >
          Add your first polling station
        </Button>
      </YStack>
    </Stack>
  </Screen>
);

export default NoVisitsExist;
