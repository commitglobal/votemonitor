import { router, useLocalSearchParams } from "expo-router";
import { Screen } from "../../../components/Screen";
import { Typography } from "../../../components/Typography";
import Button from "../../../components/Button";
import { useState } from "react";
import { CustomSheet } from "./modal";

const PollingStationWizzard = () => {
  const { slug } = useLocalSearchParams();
  const [open, setOpen] = useState(false);

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

      <Button onPress={() => setOpen(true)}> Open clear form modal </Button>
      <CustomSheet open={open} onClose={() => setOpen(false)}></CustomSheet>
    </Screen>
  );
};

export default PollingStationWizzard;
