import React from "react";
import { View, Text } from "react-native";
import { Typography } from "../../../components/Typography";
import { getTokens } from "@tamagui/core";
import { Stack } from "tamagui";

const QuickReport = () => {
  return (
    <View>
      <Text>Quick Report</Text>

      {/* example of using default spacing for padding*/}
      <Stack backgroundColor={"$purple5"} padding="$sm">
        <Typography size="xl">Hello typo</Typography>
      </Stack>
    </View>
  );
};

export default QuickReport;
