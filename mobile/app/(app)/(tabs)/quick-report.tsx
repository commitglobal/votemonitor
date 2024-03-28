import React from "react";
import { View, Text, Platform } from "react-native";
import PollingStationAddressSelect from "../../../components/polling-station-info/PollingStationAddressSelect";
import { Sheet, Button, XStack, YStack, H2, SizableText } from "tamagui";

import { useState } from "react";
import GeneralCard from "../../../components/GeneralCard";
import TimeCard from "../../../components/polling-station-info/TimeCard";

const QuickReport = () => {
  const [open, setOpen] = useState(false);

  return (
    <View>
      <Text>Quick Report</Text>
      <PollingStationAddressSelect />
      <XStack padding="$4" gap="$4">
        <TimeCard flex={1} />
        <TimeCard flex={1} />
      </XStack>
    </View>
  );
};

export default QuickReport;
