import React from "react";
import { Typography } from "../../../../../../components/Typography";
import { Spinner, YStack } from "tamagui";
import { Icon } from "../../../../../../components/Icon";
import { Screen } from "../../../../../../components/Screen";
import { router, useNavigation } from "expo-router";
import Header from "../../../../../../components/Header";
import { DrawerActions } from "@react-navigation/native";
import Button from "../../../../../../components/Button";
import { useQuickReports } from "../../../../../../services/queries/quick-reports.query";
import { useUserData } from "../../../../../../contexts/user/UserContext.provider";
import { ListView } from "../../../../../../components/ListView";
import ReportCard from "../../../../../../components/ReportCard";
import { RefreshControl, useWindowDimensions, ViewStyle } from "react-native";
import { QuickReportsAPIResponse } from "../../../../../../services/api/quick-report/get-quick-reports.api";
import { useTranslation } from "react-i18next";
import { ElectionRoundVM } from "../../../../../../common/models/election-round.model";

const QuickReport = () => {
  const { t } = useTranslation("quick_report");
  const navigation = useNavigation();

  const { activeElectionRound } = useUserData();
  const { data: quickReports, isLoading, error } = useQuickReports(activeElectionRound?.id);

  return (
    <>
      <Screen preset="fixed" contentContainerStyle={$containerStyle}>
        <Header
          title={"Quick report"}
          titleColor="white"
          barStyle="light-content"
          leftIcon={<Icon icon="menuAlt2" color="white" />}
          onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
        />
        {quickReports ? (
          <QuickReportContent
            quickReports={quickReports}
            isLoading={isLoading}
            error={error}
            activeElectionRound={activeElectionRound}
          />
        ) : (
          false
        )}
      </Screen>
      {quickReports?.length ? (
        <YStack width="100%" paddingHorizontal="$md" marginVertical="$xxs">
          <Button
            preset="outlined"
            backgroundColor="white"
            onPress={router.push.bind(null, "/report-issue")}
          >
            {t("list.add")}
          </Button>
        </YStack>
      ) : (
        false
      )}
    </>
  );
};

interface QuickReportContentProps {
  quickReports: QuickReportsAPIResponse[];
  isLoading: boolean;
  error: Error | null;
  activeElectionRound: ElectionRoundVM | undefined;
}

const ESTIMATED_ITEM_SIZE = 200;

const QuickReportContent = ({
  quickReports,
  isLoading,
  error,
  activeElectionRound,
}: QuickReportContentProps) => {
  const { t } = useTranslation(["quick_report", "common"]);
  const { width } = useWindowDimensions();

  const { refetch, isRefetching } = useQuickReports(activeElectionRound?.id);

  if (isLoading) {
    return (
      <YStack justifyContent="center" alignItems="center" flex={1}>
        <Spinner size="large" color="$purple5" />
      </YStack>
    );
  }

  if (error) {
    return <Typography>{t("list.error")}</Typography>;
  }

  return (
    <YStack padding="$md" flex={1}>
      <ListView<any>
        data={quickReports}
        showsVerticalScrollIndicator={false}
        ListEmptyComponent={
          <YStack alignItems="center" justifyContent="center" gap="$md" marginTop="40%">
            <Icon icon="undrawFlag" />
            <YStack gap="$md" paddingHorizontal="$xl">
              <Typography preset="body1" textAlign="center" color="$gray12" lineHeight={24}>
                {t("list.empty")}
              </Typography>
              <Button
                preset="outlined"
                onPress={router.push.bind(null, "/report-issue")}
                backgroundColor="white"
              >
                {t("list.add")}
              </Button>
            </YStack>
          </YStack>
        }
        bounces={true}
        renderItem={({ item, index }) => (
          <ReportCard
            key={`${index}`}
            title={item.title}
            description={item.description}
            numberOfAttachments={item.attachments.length}
            onPress={() => router.push(`/report-details/${item.id}?reportTitle=${item.title}`)}
          />
        )}
        estimatedItemSize={ESTIMATED_ITEM_SIZE}
        estimatedListSize={
          !quickReports.length
            ? undefined
            : {
                height: ESTIMATED_ITEM_SIZE * 5,
                width: width - 32,
              }
        }
        refreshControl={<RefreshControl refreshing={isRefetching} onRefresh={refetch} />}
      />
    </YStack>
  );
};

const $containerStyle: ViewStyle = {
  flexGrow: 1,
};

export default QuickReport;
