import { Screen } from "../../../../../../components/Screen";
import { router, useLocalSearchParams } from "expo-router";
import Header from "../../../../../../components/Header";
import { Icon } from "../../../../../../components/Icon";
import { Typography } from "../../../../../../components/Typography";
import { ScrollView, YStack } from "tamagui";
import { useQuickReportById } from "../../../../../../services/queries/quick-reports.query";
import { useUserData } from "../../../../../../contexts/user/UserContext.provider";
import Card from "../../../../../../components/Card";
import { useTranslation } from "react-i18next";
import { RefreshControl } from "react-native";
import i18n from "../../../../../../common/config/i18n";
import { useMemo } from "react";
import {
  QuickReportIssueType,
  QuickReportOfficialComplaintFilingStatus,
} from "../../../../../../services/api/quick-report/post-quick-report.api";

type SearchParamsType = {
  reportId: string;
  reportTitle: string;
};

const issueTypes: Record<QuickReportIssueType, string> = {
  [QuickReportIssueType.A]: i18n.t("form.issue_type.options.issue_type_a", {
    ns: "report_new_issue",
  }),
  [QuickReportIssueType.B]: i18n.t("form.issue_type.options.issue_type_b", {
    ns: "report_new_issue",
  }),
  [QuickReportIssueType.C]: i18n.t("form.issue_type.options.issue_type_c", {
    ns: "report_new_issue",
  }),
  [QuickReportIssueType.D]: i18n.t("form.issue_type.options.issue_type_d", {
    ns: "report_new_issue",
  }),
};

const officialComplaintFilingStatuses: Record<QuickReportOfficialComplaintFilingStatus, string> = {
  [QuickReportOfficialComplaintFilingStatus.Yes]: i18n.t(
    "form.official_complaint_filing_status.options.yes",
    { ns: "report_new_issue" },
  ),
  [QuickReportOfficialComplaintFilingStatus.NoButPlanningToFile]: i18n.t(
    "form.official_complaint_filing_status.options.noButPlanningToFile",
    { ns: "report_new_issue" },
  ),
  [QuickReportOfficialComplaintFilingStatus.NoAndNotPlanningToFile]: i18n.t(
    "form.official_complaint_filing_status.options.noAndNotPlanningToFile",
    { ns: "report_new_issue" },
  ),
  [QuickReportOfficialComplaintFilingStatus.DoesNotApplyOrOther]: i18n.t(
    "form.official_complaint_filing_status.options.doesNotApplyOrOther",
    { ns: "report_new_issue" },
  ),
};

const ReportDetails = () => {
  const { reportTitle, reportId } = useLocalSearchParams<SearchParamsType>();
  const { t } = useTranslation(["report_details", "common"]);

  if (!reportId || !reportTitle) {
    return <Typography>Incorrect page params</Typography>;
  }

  const { activeElectionRound } = useUserData();

  const {
    data: quickReport,
    isLoading: isLoadingCurrentReport,
    error: currentReportError,
    refetch: refetchQuickReport,
    isRefetching: isRefetchingQuickReport,
  } = useQuickReportById(activeElectionRound?.id, reportId);

  if (isLoadingCurrentReport) {
    return <Typography>{t("loading", { ns: "common" })}</Typography>;
  }

  if (currentReportError) {
    return (
      <Screen preset="fixed" contentContainerStyle={{ flexGrow: 1 }}>
        <Header
          title={`${reportTitle}`}
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
        title={`${reportTitle}`}
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
            {t("issue_type")}
          </Typography>
          <Typography preset="body1" fontWeight="500">
            {quickReport?.issueType}
          </Typography>
          <Typography preset="subheading" fontWeight="500">
            {t("official_complaint_filing_status")}
          </Typography>
          <Typography preset="body1" fontWeight="500">
            {quickReport?.officialComplaintFilingStatus}
          </Typography>

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

export default ReportDetails;
