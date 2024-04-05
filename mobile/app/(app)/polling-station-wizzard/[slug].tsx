import { View } from "react-native";
import { Typography } from "../../../components/Typography";
import { router, useLocalSearchParams } from "expo-router";
import { Button } from "tamagui";

const PollingStationWizzard = () => {
  const { slug } = useLocalSearchParams();
  return (
    <View
      style={{
        flex: 1,
        justifyContent: "center",
        alignItems: "center",
        gap: 20,
      }}
    >
      <Typography>This is the wizzard, page {slug}</Typography>
      <Button onPress={() => router.replace(`/polling-station-wizzard/${+(slug || 0) + 1}`)}>
        Next
      </Button>
    </View>
  );
};

export default PollingStationWizzard;
