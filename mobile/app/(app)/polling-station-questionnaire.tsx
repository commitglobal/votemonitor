import { Typography } from "../../components/Typography";
import Header from "../../components/Header";
import { Screen } from "../../components/Screen";
import { useTheme, Sheet, View } from "tamagui";
import { Icon } from "../../components/Icon";
import { router } from "expo-router";
import { useState } from "react";
import { ViewStyle } from "react-native";
import { useTranslation } from "react-i18next";

const PollingStationQuestionnaire = () => {
  const theme = useTheme();
  const { t } = useTranslation("polling_station_information");
  const [open, setOpen] = useState(false);

  return (
    <Screen style={$screenStyle} contentContainerStyle={$containerStyle} preset="fixed">
      <Header
        title={t("header.title")}
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
  /* The current state of the sheet */
  open: boolean;

  /* Control the state for sheet */
  setOpen: (state: boolean) => void;
}

const PollingStationQuestionnaireContent = (props: PollingStationQuestionnaireProps) => {
  const { open, setOpen } = props;

  return (
    <View>
      <Typography>This is the polling station questionnaire</Typography>
      <ButtomSheet open={open} setOpen={setOpen}></ButtomSheet>
    </View>
  );
};

interface ButtomSheetProps {
  /* The current state of the sheet */
  open: boolean;

  /* Control the state of the sheet */
  setOpen: (state: boolean) => void;

  /* For future: Triggered action for pressing "Clear form" */
  action?: () => void;
}

export const ButtomSheet = (props: ButtomSheetProps) => {
  const { open, setOpen } = props;
  const { t } = useTranslation("bottom_sheets");

  return (
    <Sheet
      open={open}
      onOpenChange={() => setOpen(false)}
      snapPointsMode="fit"
      modal={true}
      dismissOnSnapToBottom
    >
      <Sheet.Overlay animation="lazy" enterStyle={{ opacity: 0 }} exitStyle={{ opacity: 0 }} />
      <Sheet.Frame borderRadius={28} gap="$sm" paddingHorizontal="$md" paddingBottom="$xl">
        <Icon paddingVertical="$md" alignSelf="center" icon="dragHandle"></Icon>

        <View paddingVertical="$xxs" paddingHorizontal="$sm">
          <Typography preset="body1" color="$gray7" lineHeight={24}>
            {t("observations.actions.clear_form")}
          </Typography>
        </View>
      </Sheet.Frame>
    </Sheet>
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
