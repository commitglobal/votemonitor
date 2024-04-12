import { useState } from "react";
import { router, useLocalSearchParams } from "expo-router";
import { Sheet, View } from "tamagui";
import { Screen } from "../../../components/Screen";
import { Typography } from "../../../components/Typography";
import Button from "../../../components/Button";
import { Icon } from "../../../components/Icon";

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

      <CustomSheet></CustomSheet>
    </Screen>
  );
};

const CustomSheet = () => {
  const [open, setOpen] = useState(false);
  console.log("Open:" + open);
  return (
    <>
      <Button onPress={() => setOpen(!open)}>Open Sheet</Button>
      <Sheet
        open={open}
        onOpenChange={setOpen}
        snapPointsMode="fit"
        modal={true}
        dismissOnSnapToBottom
      >
        <Sheet.Overlay animation="lazy" enterStyle={{ opacity: 0 }} exitStyle={{ opacity: 0 }} />

        <Sheet.Frame borderRadius={28} gap={12} paddingHorizontal={16} paddingBottom={32}>
          <Icon paddingVertical={16} alignSelf="center" icon="dragHandle"></Icon>

          <View paddingVertical={8} paddingHorizontal={12}>
            <Typography preset="body1" color="$gray7" lineHeight={24}>
              Clear form (delete all answers)
            </Typography>
          </View>
        </Sheet.Frame>
      </Sheet>
    </>
  );
};

export default PollingStationWizzard;
