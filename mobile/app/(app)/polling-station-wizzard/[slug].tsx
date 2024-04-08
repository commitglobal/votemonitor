import { router } from "expo-router";
import { Screen } from "../../../components/Screen";
import Header from "../../../components/Header";
import { Icon } from "../../../components/Icon";
import { TextStyle, ViewStyle } from "react-native";
import { XStack, YStack } from "tamagui";
import LinearProgress from "../../../components/LinearProgress";
import { Typography } from "../../../components/Typography";
import Select from "../../../components/Select";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import Button from "../../../components/Button";

const PollingStationWizzard = () => {
  const insets = useSafeAreaInsets();
  // const { slug } = useLocalSearchParams();
  // () => router.replace(`/polling-station-wizzard/${+(slug || 0) + 1}

  return (
    <Screen style={$screenStyle} contentContainerStyle={$containerStyle} preset="fixed">
      <Header
        title={"Add polling station"}
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      />
      <YStack style={$containerStyle} justifyContent="space-between">
        <YStack paddingHorizontal="$md" paddingTop="$xl">
          <YStack gap="$md" minHeight="$xxl">
            <LinearProgress total={5} current={2} />
            <Typography color="$gray5">Add polling station from the North-West Region</Typography>
          </YStack>
          <YStack paddingTop={140} gap="$lg" justifyContent="center">
            <Typography preset="body2" style={$labelStyle}>
              Select the [Location - L1] of the polling station
            </Typography>
            <Select options={regionData} placeholder="Select Region" />
          </YStack>
        </YStack>
        <XStack
          elevation={2}
          backgroundColor="white"
          gap="$sm"
          padding="$md"
          paddingBottom={insets.bottom}
        >
          <XStack flex={0.25}>
            <Button
              width="100%"
              icon={() => <Icon icon="chevronLeft" color="$purple5" />}
              preset="chromeless"
            >
              Back
            </Button>
          </XStack>
          <XStack flex={0.75}>
            <Button width="100%">Next Step</Button>
          </XStack>
        </XStack>
      </YStack>
    </Screen>
  );
};

const $screenStyle: ViewStyle = {
  backgroundColor: "white",
  justifyContent: "space-between",
};

const $containerStyle: ViewStyle = {
  flex: 1,
};

const $labelStyle: TextStyle = {
  color: "black",
  fontWeight: "700",
};

export default PollingStationWizzard;

const regionData = [
  { id: 1, value: "North" },
  { id: 2, value: "North-West" },
  { id: 3, value: "North-East" },
  { id: 4, value: "West" },
  { id: 5, value: "East" },
  { id: 6, value: "South-West" },
  { id: 7, value: "South" },
  { id: 8, value: "South" },
  { id: 9, value: "South" },
  { id: 10, value: "South" },
  { id: 11, value: "South" },
];
