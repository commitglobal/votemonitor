import React from "react";
import { View, Text } from "react-native";
import { Typography } from "../../../../components/Typography";
import { Stack } from "tamagui";
import Select from "../../../../components/Select";

const QuickReport = () => {
  return (
    <View>
      <Text>Quick Report</Text>

      {/* example of using default spacing for padding*/}
      <Stack backgroundColor="$purple5" padding="$sm">
        <Typography size="xl">Hello typo</Typography>
      </Stack>
      <Stack gap="$xs" padding="$sm">
        <Typography preset="subheading">Select</Typography>
        <Select
          options={regionData}
          placeholder="Select option"
          defaultValue={"West"}
        />
        <Select options={countryData} placeholder="Select option" />
      </Stack>
    </View>
  );
};

const regionData = [
  { id: 1, value: "North" },
  { id: 2, value: "North-West" },
  { id: 3, value: "North-East" },
  { id: 4, value: "West" },
  { id: 5, value: "East" },
  { id: 6, value: "South-West" },
  { id: 7, value: "South" },
];

const countryData = [
  { id: 3, value: "Russia" },
  { id: 4, value: "France" },
  { id: 5, value: "China" },
  { id: 6, value: "Brazil" },
  { id: 7, value: "Australia" },
];

export default QuickReport;
