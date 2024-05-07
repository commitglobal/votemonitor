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
import { useQuickReports } from "../../../../../services/queries/quick-reports.query";
import { useUserData } from "../../../../../contexts/user/UserContext.provider";
import { QuickReportsAPIResponse } from "../../../../../services/definitions.api";
import ReportCard from "../../../../../components/ReportCard";

const QuickReport = () => {
  const navigation = useNavigation();
  const [openContextualMenu, setOpenContextualMenu] = useState(false);
  const { activeElectionRound } = useUserData();

  const { data: quickReports } = useQuickReports(activeElectionRound?.id);

  console.log(quickReports);

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

      {quickReports === null && <NoQuickReportsExist />}
      {quickReports && quickReports.length === 0 && <NoQuickReportsExist />}
      {quickReports && quickReports.length > 0 && <QuickReportsList quickReports={quickReports} />}

      <OptionsSheet open={openContextualMenu} setOpen={setOpenContextualMenu}>
        {/* //TODO: what do we need to add here? */}
        <OptionsSheetContent />
      </OptionsSheet>
    </Screen>
  );
};

interface listProps {
  quickReports: Array<QuickReportsAPIResponse>;
}

const QuickReportsList = (props: listProps) => {
  const { quickReports } = props;

  return (
    <YStack flex={1} alignItems="center" justifyContent="center" gap="$xs">
      {quickReports.map((report) => (
        <ReportCard
          key={report.id}
          title={report.title}
          description={report.description}
          onPress={() => router.push(`/report-details/${report.id}?reportTitle=${report.title}`)}
        />
      ))}
    </YStack>
  );
};

/**
 * This component is displayed when there are no quick reports available.
 */
const NoQuickReportsExist = () => {
  return (
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
