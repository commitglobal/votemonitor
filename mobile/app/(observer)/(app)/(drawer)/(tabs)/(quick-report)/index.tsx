import React, { useMemo, useState } from "react";
import { Typography } from "../../../../../../components/Typography";
import { Spinner, XStack, YStack } from "tamagui";
import { Icon } from "../../../../../../components/Icon";
import { Screen } from "../../../../../../components/Screen";
import { router, useNavigation } from "expo-router";
import Header from "../../../../../../components/Header";
import { DrawerActions } from "@react-navigation/native";
import Button from "../../../../../../components/Button";
import { useQuickReports } from "../../../../../../services/queries/quick-reports.query";
import {
  useIncidentReportingForms,
  useIncidentReports,
} from "../../../../../../services/queries/incident-reports.query";
import { useUserData } from "../../../../../../contexts/user/UserContext.provider";
import { ListView } from "../../../../../../components/ListView";
import ReportCard from "../../../../../../components/ReportCard";
import IncidentReportCard from "../../../../../../components/IncidentReportCard";
import OptionsSheet from "../../../../../../components/OptionsSheet";
import { RefreshControl, ViewStyle } from "react-native";
import { QuickReportsAPIResponse } from "../../../../../../services/api/quick-report/get-quick-reports.api";
import { IncidentReportAPIResponse } from "../../../../../../services/api/incident-report/incident-report.api";
import { FormAPIModel } from "../../../../../../services/definitions.api";
import { useTranslation } from "react-i18next";
import InfoModal from "../../../../../../components/InfoModal";
import SelectFormLanguageDialog from "../../../../../../components/SelectFormLanguageDialog";
import {
  getFormLanguagePreference,
  setFormLanguagePreference,
} from "../../../../../../common/language.preferences";
import { useNetInfoContext } from "../../../../../../contexts/net-info-banner/NetInfoContext";
import {
  getFormDisplayName,
  getFormLanguage,
  getIncidentReportLocationLabel,
} from "../../../../../../helpers/incidentReport";

type ReportListItem =
  | { kind: "quick-report"; timestamp?: string; report: QuickReportsAPIResponse }
  | { kind: "incident-report"; timestamp?: string; report: IncidentReportAPIResponse };

// newest first; reports not synched yet (no timestamp) stay on top
const byTimestampDesc = (a: ReportListItem, b: ReportListItem) => {
  const aTime = a.timestamp ? new Date(a.timestamp).getTime() : Number.MAX_SAFE_INTEGER;
  const bTime = b.timestamp ? new Date(b.timestamp).getTime() : Number.MAX_SAFE_INTEGER;
  return bTime - aTime;
};

const QuickReport = () => {
  const { t } = useTranslation(["quick_report", "incident_report"]);
  const navigation = useNavigation();

  const [isOpenInfoModal, setIsOpenInfoModal] = useState(false);
  const [isReportTypeSheetOpen, setIsReportTypeSheetOpen] = useState(false);
  const [formToSelectLanguage, setFormToSelectLanguage] = useState<FormAPIModel | null>(null);

  const { activeElectionRound } = useUserData();
  const { data: quickReports, isLoading, error } = useQuickReports(activeElectionRound?.id);
  const { data: incidentReportingForms } = useIncidentReportingForms(activeElectionRound?.id);
  const { data: incidentReports } = useIncidentReports(activeElectionRound?.id);

  // with no incident reporting forms there is nothing to choose from: open the quick report directly
  const handleAddReport = () => {
    if (incidentReportingForms?.length) {
      setIsReportTypeSheetOpen(true);
    } else {
      router.push("/report-issue");
    }
  };

  const openQuickReport = () => {
    setIsReportTypeSheetOpen(false);
    router.push("/report-issue");
  };

  const openIncidentReport = (form: FormAPIModel, language: string) => {
    setFormLanguagePreference({ formId: form.id, language });
    setFormToSelectLanguage(null);
    router.push(`/incident-report?formId=${form.id}&language=${language}`);
  };

  // same as the observation forms: the saved language, the only language, or ask
  const selectIncidentForm = async (form: FormAPIModel) => {
    setIsReportTypeSheetOpen(false);
    const preferredLanguage = await getFormLanguagePreference({ formId: form.id });

    if (preferredLanguage && form.languages.includes(preferredLanguage)) {
      openIncidentReport(form, preferredLanguage);
    } else if (form.languages.length === 1) {
      openIncidentReport(form, form.languages[0]);
    } else {
      setFormToSelectLanguage(form);
    }
  };

  return (
    <>
      <Screen preset="fixed" contentContainerStyle={$containerStyle}>
        <Header
          title={t("header_title")}
          titleColor="white"
          barStyle="light-content"
          leftIcon={<Icon icon="menuAlt2" color="white" />}
          onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
          rightIcon={<Icon icon="infoCircle" color="white" />}
          onRightPress={() => setIsOpenInfoModal(true)}
        />
        {quickReports ? (
          <QuickReportContent
            quickReports={quickReports}
            incidentReportingForms={incidentReportingForms}
            isLoading={isLoading}
            error={error}
            electionRoundId={activeElectionRound?.id}
            onAddReport={handleAddReport}
          />
        ) : (
          false
        )}
      </Screen>

      {quickReports?.length || incidentReports?.length ? (
        <YStack width="100%" paddingHorizontal="$md" marginVertical="$xxs">
          <Button preset="outlined" backgroundColor="white" onPress={handleAddReport}>
            {t("list.add")}
          </Button>
        </YStack>
      ) : (
        false
      )}

      {isReportTypeSheetOpen && (
        <OptionsSheet open setOpen={setIsReportTypeSheetOpen}>
          <YStack paddingHorizontal="$sm" gap="$xxs">
            <Typography preset="body2" paddingVertical="$xs">
              {t("report_type_sheet.title", { ns: "incident_report" })}
            </Typography>
            {/* quick report is always the first option */}
            <Typography
              preset="body1"
              paddingVertical="$md"
              pressStyle={{ color: "$purple5" }}
              onPress={openQuickReport}
            >
              {t("report_type_sheet.quick_report", { ns: "incident_report" })}
            </Typography>
            {incidentReportingForms?.map((form: FormAPIModel) => (
              <Typography
                key={form.id}
                preset="body1"
                paddingVertical="$md"
                pressStyle={{ color: "$purple5" }}
                onPress={() => selectIncidentForm(form)}
              >
                {getFormDisplayName(form)}
              </Typography>
            ))}
          </YStack>
        </OptionsSheet>
      )}

      {formToSelectLanguage && (
        <SelectFormLanguageDialog
          languages={formToSelectLanguage.languages}
          onConfirm={(language) => openIncidentReport(formToSelectLanguage, language)}
          onCancel={() => setFormToSelectLanguage(null)}
        />
      )}

      {isOpenInfoModal && (
        <InfoModal
          paragraphs={[t("info_modal.p1"), t("info_modal.p2")]}
          handleCloseInfoModal={() => setIsOpenInfoModal(false)}
        />
      )}
    </>
  );
};

interface QuickReportContentProps {
  quickReports: QuickReportsAPIResponse[];
  incidentReportingForms: FormAPIModel[] | undefined;
  isLoading: boolean;
  error: Error | null;
  electionRoundId: string | undefined;
  onAddReport: () => void;
}

const QuickReportContent = ({
  quickReports,
  incidentReportingForms,
  isLoading,
  error,
  electionRoundId,
  onAddReport,
}: QuickReportContentProps) => {
  const { t } = useTranslation(["quick_report", "common"]);
  const { isOnline } = useNetInfoContext();
  const { visits } = useUserData();

  const [isRefreshing, setIsRefreshing] = useState(false);

  const { refetch: refetchQuickReports } = useQuickReports(electionRoundId);
  // a failing incident reports request should not hide the quick reports
  const { data: incidentReports, refetch: refetchIncidentReports } =
    useIncidentReports(electionRoundId);

  // a sent report is locked and only viewed, a draft is continued in the language it was started in
  const openListedIncidentReport = async (report: IncidentReportAPIResponse) => {
    if (report.isCompleted) {
      router.push(`/incident-report-details/${report.id}`);
      return;
    }
    const form = incidentReportingForms?.find((f) => f.id === report.formId);
    const preferredLanguage = await getFormLanguagePreference({ formId: report.formId });
    const language =
      preferredLanguage && form?.languages.includes(preferredLanguage)
        ? preferredLanguage
        : getFormLanguage(form);
    router.push(
      `/incident-report?formId=${report.formId}&incidentReportId=${report.id}&language=${language}`,
    );
  };

  const reports: ReportListItem[] = useMemo(
    () =>
      [
        ...quickReports.map((report): ReportListItem => ({
          kind: "quick-report",
          timestamp: report.timestamp,
          report,
        })),
        ...(incidentReports || []).map((report): ReportListItem => ({
          kind: "incident-report",
          timestamp: report.isNotSynched ? undefined : report.timestamp,
          report,
        })),
      ].sort(byTimestampDesc),
    [quickReports, incidentReports],
  );

  const handleRefetch = () => {
    setIsRefreshing(true);
    Promise.all([refetchQuickReports(), refetchIncidentReports()]).then(() => {
      setIsRefreshing(false);
    });
  };

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
    <YStack flex={1}>
      <ListView<ReportListItem>
        data={reports}
        showsVerticalScrollIndicator={false}
        contentContainerStyle={{ padding: 16 }}
        ListEmptyComponent={
          <YStack alignItems="center" justifyContent="center" gap="$md" marginTop="40%">
            <Icon icon="undrawFlag" />
            <YStack gap="$md" paddingHorizontal="$xl">
              <Typography preset="body1" textAlign="center" color="$gray12" lineHeight={24}>
                {t("list.empty")}
              </Typography>
              <XStack>
                <Button
                  preset="outlined"
                  width="100%"
                  height="100%"
                  textStyle={{ textAlign: "center" }}
                  onPress={onAddReport}
                  backgroundColor="white"
                >
                  {t("list.add")}
                </Button>
              </XStack>
            </YStack>
          </YStack>
        }
        bounces={isOnline}
        renderItem={({ item }) =>
          item.kind === "quick-report" ? (
            <ReportCard
              key={item.report.id}
              title={item.report.title}
              description={item.report.description}
              incidentCategory={item.report.incidentCategory}
              numberOfAttachments={item.report.attachments.length}
              onPress={() =>
                router.push(`/report-details/${item.report.id}?reportTitle=${item.report.title}`)
              }
            />
          ) : (
            <IncidentReportCard
              key={item.report.id}
              formName={getFormDisplayName(
                incidentReportingForms?.find((form) => form.id === item.report.formId),
              )}
              location={getIncidentReportLocationLabel(item.report, visits)}
              numberOfAnswers={item.report.answers.length}
              numberOfAttachments={item.report.attachments.length}
              isCompleted={item.report.isCompleted}
              timestamp={item.report.timestamp}
              onPress={() => openListedIncidentReport(item.report)}
            />
          )
        }
        refreshControl={<RefreshControl refreshing={isRefreshing} onRefresh={handleRefetch} />}
      />
    </YStack>
  );
};

const $containerStyle: ViewStyle = {
  flexGrow: 1,
};

export default QuickReport;
