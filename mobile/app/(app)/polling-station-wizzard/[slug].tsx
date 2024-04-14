import { router, useLocalSearchParams } from "expo-router";
import { Screen } from "../../../components/Screen";
import { Typography } from "../../../components/Typography";
import Button from "../../../components/Button";
import { useContext } from "react";
import { OpenContext } from "./_layout";
import { Sheet, View } from "tamagui";
import { Icon } from "../../../components/Icon";

const PollingStationWizzard = () => {
  const { slug } = useLocalSearchParams();
  const { open, setOpen } = useContext(OpenContext);

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

      <CustomSheet open={open} onClose={() => setOpen(false)}></CustomSheet>
    </Screen>
  );
};

export const CustomSheet = ({ open, onClose }: { open: boolean; onClose: any }) => {
  console.log("Open:" + open);
  return (
    <>
      <Sheet
        open={open}
        onOpenChange={onClose}
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
