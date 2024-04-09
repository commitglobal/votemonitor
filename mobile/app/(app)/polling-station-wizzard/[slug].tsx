import { Typography } from "../../../components/Typography";
import { router, useLocalSearchParams } from "expo-router";
import { Button } from "tamagui";
import { Screen } from "../../../components/Screen";

const PollingStationWizzard = () => {
  const { slug } = useLocalSearchParams();
  return (
    <Screen
      contentContainerStyle={{
        gap: 20,
      }}
      statusBarStyle="dark"
      safeAreaEdges={["top"]}
    >
      <Typography>This is the wizzard, page {slug}</Typography>
      <Button onPress={() => router.replace(`/polling-station-wizzard/${+(slug || 0) + 1}`)}>
        Next
      </Button>
    </Screen>
  );
};

export default PollingStationWizzard;
