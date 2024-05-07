import React, { useState } from "react";
import { Typography } from "../../../../../components/Typography";
import { View, YStack } from "tamagui";
import { Icon } from "../../../../../components/Icon";
import { Screen } from "../../../../../components/Screen";
import { router, useNavigation } from "expo-router";
import Header from "../../../../../components/Header";
import { DrawerActions } from "@react-navigation/native";
import Button from "../../../../../components/Button";
import OptionsSheet from "../../../../../components/OptionsSheet";
import {
  useQuickReports,
  useQuickReportById,
} from "../../../../../services/queries/quick-reports.query";
import { useUserData } from "../../../../../contexts/user/UserContext.provider";
import ReportCard from "../../../../../components/ReportCard";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { use } from "i18next";

const QuickReport = () => {
  const insets = useSafeAreaInsets();
  const navigation = useNavigation();
  const [openContextualMenu, setOpenContextualMenu] = useState(false);
  const { activeElectionRound } = useUserData();
  const { data: quickReports } = useQuickReports(activeElectionRound?.id);

  if (quickReports === undefined || (quickReports && quickReports.length === 0)) {
    return <NoQuickReportsExist />;
  }

  console.log(quickReports[0].id);

  const quickReport = useQuickReportById(activeElectionRound?.id, quickReports[0].id);
  console.log(quickReport);

  const id = 1;

  return (
    <Screen
      preset="scroll"
      ScrollViewProps={{
        showsVerticalScrollIndicator: false,
        stickyHeaderIndices: [0],
        bounces: false,
      }}
      contentContainerStyle={{
        flexGrow: 1,
      }}
      backgroundColor="white"
    >
      <Header
        title={"Quick Report"}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="menuAlt2" color="white" />}
        onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
        rightIcon={<Icon icon="dotsVertical" color="white" />}
        onRightPress={() => {
          setOpenContextualMenu(true);
        }}
      />

      <YStack paddingHorizontal={16} gap={8} justifyContent="center" paddingVertical={32}>
        <Typography preset="body1" textAlign="left" color="$gray7" fontWeight="700">
          My reported issues{" "}
        </Typography>

        {quickReports.map((report) => (
          <ReportCard
            key={report.id}
            report={report}
            onPress={() => router.push(`/report-details/${id}?reportTitle=${report.title}`)}
          />
        ))}
      </YStack>

      <View width="100%" paddingBottom={8 + insets.bottom} marginTop="auto" paddingHorizontal={32}>
        <Button preset="outlined" onPress={router.push.bind(null, "/report-issue")}>
          Report new issue
        </Button>
      </View>

      <OptionsSheet open={openContextualMenu} setOpen={setOpenContextualMenu}>
        {/* //TODO: what do we need to add here? */}
        <OptionsSheetContent />
      </OptionsSheet>
    </Screen>
  );
};

/**
 * This component is displayed when there are no quick reports available.
 */
const NoQuickReportsExist = () => {
  const navigation = useNavigation();
  const [openContextualMenu, setOpenContextualMenu] = useState(false);

  return (
    <Screen
      preset="auto"
      ScrollViewProps={{
        bounces: false,
      }}
      contentContainerStyle={{
        flexGrow: 1,
      }}
    >
      <Header
        title={"Quick Report"}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="menuAlt2" color="white" />}
        onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
        rightIcon={<Icon icon="dotsVertical" color="white" />}
        onRightPress={() => {
          setOpenContextualMenu(true);
        }}
      />

      <YStack flex={1} alignItems="center" justifyContent="center" gap="$md">
        <Icon icon="undrawFlag" />

        <YStack gap="$md" paddingHorizontal="$xl">
          <Typography preset="body1" textAlign="center" color="$gray12" lineHeight={24}>
            Start sending quick reports to the organization if you notice irregularities inside,
            outside the polling station or whenever needed.
          </Typography>
          <Button
            preset="outlined"
            onPress={router.push.bind(null, "/report-issue")}
            backgroundColor="white"
          >
            Report new issue
          </Button>
        </YStack>
      </YStack>
      <OptionsSheet open={openContextualMenu} setOpen={setOpenContextualMenu}>
        {/* //TODO: what do we need to add here? */}
        <OptionsSheetContent />
      </OptionsSheet>
    </Screen>
  );
};

const OptionsSheetContent = () => {
  return (
    <View paddingVertical="$xxs" paddingHorizontal="$sm">
      <Typography preset="body1" color="$gray7" lineHeight={24}>
        Option
      </Typography>
    </View>
  );
};

export default QuickReport;
