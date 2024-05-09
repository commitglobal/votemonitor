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
import { ListView } from "../../../../../components/ListView";
import ReportCard from "../../../../../components/ReportCard";
import { Dimensions, ViewStyle } from "react-native";
import { QuickReportsAPIResponse } from "../../../../../services/api/quick-report/get-quick-reports.api";

const QuickReport = () => {
  const navigation = useNavigation();
  const [openContextualMenu, setOpenContextualMenu] = useState(false);
  const { activeElectionRound } = useUserData();
  const { data: quickReports, isLoading, error } = useQuickReports(activeElectionRound?.id);

  return (
    <>
      <Screen
        preset="scroll"
        ScrollViewProps={{
          showsVerticalScrollIndicator: false,
          stickyHeaderIndices: [0],
          bounces: false,
        }}
        contentContainerStyle={$containerStyle}
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
        <QuickReportContent quickReports={quickReports} isLoading={isLoading} error={error} />
        <OptionsSheet open={openContextualMenu} setOpen={setOpenContextualMenu}>
          {/* //TODO: what do we need to add here? */}
          <View paddingVertical="$xxs" paddingHorizontal="$sm">
            <Typography preset="body1" color="$gray7" lineHeight={24}>
              Option
            </Typography>
          </View>
        </OptionsSheet>
      </Screen>
      {quickReports.length > 0 && (
        <YStack width="100%" paddingHorizontal="$md" marginVertical="$xxs">
          <Button
            preset="outlined"
            backgroundColor="white"
            onPress={router.push.bind(null, "/report-issue")}
          >
            Report new issue
          </Button>
        </YStack>
      )}
    </>
  );
};

interface QuickReportContentProps {
  quickReports: QuickReportsAPIResponse[];
  isLoading: boolean;
  error: Error | null;
}

const QuickReportContent = ({ quickReports, isLoading, error }: QuickReportContentProps) => {
  if (isLoading) {
    return <Typography>Loading...</Typography>;
  }

  if (error) {
    return <Typography>Error...</Typography>;
  }

  return (
    <YStack padding="$md" height={Dimensions.get("screen").height * 1.4}>
      <ListView<any>
        data={quickReports}
        showsVerticalScrollIndicator={false}
        ListHeaderComponent={
          quickReports.length > 0 ? (
            <Typography
              marginBottom="$xxs"
              preset="body1"
              textAlign="left"
              color="$gray7"
              fontWeight="700"
            >
              My reported issues
            </Typography>
          ) : (
            <></>
          )
        }
        ListEmptyComponent={
          <YStack flex={1} alignItems="center" justifyContent="center" gap="$md" marginTop="50%">
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
        }
        bounces={false}
        renderItem={({ item, index }) => (
          <ReportCard
            key={`${index}`}
            title={item.title}
            description={item.description}
            numberOfAttachments={item.attachments.length}
            onPress={() => router.push(`/report-details/${item.id}?reportTitle=${item.title}`)}
          />
        )}
        estimatedItemSize={100}
      />
    </YStack>
  );
};

const $containerStyle: ViewStyle = {
  flexGrow: 1,
};

export default QuickReport;
