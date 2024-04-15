import { Typography } from "../../components/Typography";
import Header from "../../components/Header";
import { Screen } from "../../components/Screen";
import { useTheme } from "tamagui";
import { Icon } from "../../components/Icon";
import { router } from "expo-router";
import { useState } from "react";
import { Sheet, View } from "tamagui";
import { ViewStyle } from "react-native";

const PollingStationQuestionnaire = () => {
  const theme = useTheme();
  const [open, setOpen] = useState(false);

  return (
    <Screen style={$screenStyle} contentContainerStyle={$containerStyle} preset="fixed">
      <Header
        title="Header"
        titleColor="white"
        backgroundColor={theme.purple5?.val}
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        rightIcon={<Icon icon="dotsVertical" color="white" />}
        onLeftPress={() => router.back()}
        onRightPress={() => setOpen(true)}
      />

      <PollingStationQuestionnaireContent open={open} setOpen={setOpen} />
    </Screen>
  );
};

interface PollingStationQuestionnaireProps {
  open: boolean;
  setOpen: (state: boolean) => void;
}

const PollingStationQuestionnaireContent = (props: PollingStationQuestionnaireProps) => {
  const { open, setOpen } = props;

  return (
    <>
      <Typography>This is the polling station questionnaire</Typography>
      <CustomSheet open={open} onClose={() => setOpen(false)}></CustomSheet>
    </>
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

const $screenStyle: ViewStyle = {
  backgroundColor: "white",
  justifyContent: "space-between",
};

const $containerStyle: ViewStyle = {
  flex: 1,
};

export default PollingStationQuestionnaire;
