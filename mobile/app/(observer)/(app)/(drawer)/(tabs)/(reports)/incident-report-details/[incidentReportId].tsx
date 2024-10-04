import { router, useLocalSearchParams } from "expo-router";
import React from "react";
import { useTranslation } from "react-i18next";
import { RefreshControl } from "react-native";
import { ScrollView, YStack } from "tamagui";
import Card from "../../../../../../../components/Card";
import Header from "../../../../../../../components/Header";
import { Icon } from "../../../../../../../components/Icon";
import { Screen } from "../../../../../../../components/Screen";
import { Typography } from "../../../../../../../components/Typography";
import { useUserData } from "../../../../../../../contexts/user/UserContext.provider";
import { useQuickReportById } from "../../../../../../../services/queries/quick-reports.query";

type SearchParamsType = {
  incidentReportId: string;
};

const QuickReportDetails = () => {
  const { incidentReportId } = useLocalSearchParams<SearchParamsType>();
  const { t } = useTranslation(["report_details", "common"]);

  if (!incidentReportId) {
    return <Typography>Incorrect page params</Typography>;
  }

  const { activeElectionRound } = useUserData();

  const {
    data: quickReport,
    isLoading: isLoadingCurrentReport,
    error: currentReportError,
    refetch: refetchQuickReport,
    isRefetching: isRefetchingQuickReport,
  } = useQuickReportById(activeElectionRound?.id, incidentReportId);

  if (isLoadingCurrentReport) {
    return <Typography>{t("loading", { ns: "common" })}</Typography>;
  }

  if (currentReportError) {
    return (
      <Screen preset="fixed" contentContainerStyle={{ flexGrow: 1 }}>
        <Header
          title={"kdkdkdkdk"}
          titleColor="white"
          barStyle="light-content"
          leftIcon={<Icon icon="chevronLeft" color="white" />}
          onLeftPress={() => router.back()}
        />
        <ScrollView
          showsVerticalScrollIndicator={false}
          contentContainerStyle={{ flex: 1, alignItems: "center" }}
          paddingVertical="$xxl"
          refreshControl={
            <RefreshControl refreshing={isRefetchingQuickReport} onRefresh={refetchQuickReport} />
          }
        >
          <Typography>{t("error")}</Typography>
        </ScrollView>
      </Screen>
    );
  }

  const attachments = quickReport?.attachments || [];
  return (
    <Screen
      preset="fixed"
      contentContainerStyle={{
        flexGrow: 1,
      }}
      backgroundColor="white"
    >
      <Header
        title={`kdkdkdkdk`}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      />
      <ScrollView
        contentContainerStyle={{ gap: 32, justifyContent: "center" }}
        paddingHorizontal={16}
        paddingTop={32}
        refreshControl={
          <RefreshControl refreshing={isRefetchingQuickReport} onRefresh={refetchQuickReport} />
        }
      >
        <YStack gap={16}>
          <Typography preset="subheading" fontWeight="500">
            {quickReport?.title}
          </Typography>
          <Typography preset="body1" lineHeight={24} color="$gray8">
            {quickReport?.description}
          </Typography>
        </YStack>

        {attachments.length === 0 ? (
          <Typography fontWeight="500">{t("no_files")}</Typography>
        ) : (
          <YStack gap={16}>
            <Typography fontWeight="500" color="$gray10">
              {t("uploaded_media")}
            </Typography>
            {attachments.map((attachment, key) => (
              <Card key={key}>
                <Typography preset="body1" fontWeight="700" key={attachment.id}>
                  {attachment.fileName}
                </Typography>
              </Card>
            ))}
          </YStack>
        )}
      </ScrollView>
    </Screen>
  );
};

export default QuickReportDetails;
