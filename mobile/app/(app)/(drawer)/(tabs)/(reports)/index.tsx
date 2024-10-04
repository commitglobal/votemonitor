import { DrawerActions } from "@react-navigation/native";
import { router, useNavigation } from "expo-router";
import React, { useMemo } from "react";
import { useTranslation } from "react-i18next";
import { RefreshControl, useWindowDimensions, ViewStyle } from "react-native";
import { Spinner, YStack } from "tamagui";
import { ElectionRoundVM } from "../../../../../common/models/election-round.model";
import Button from "../../../../../components/Button";
import Header from "../../../../../components/Header";
import { Icon } from "../../../../../components/Icon";
import { ListView } from "../../../../../components/ListView";
import ReportCard from "../../../../../components/ReportCard";
import { Screen } from "../../../../../components/Screen";
import { Typography } from "../../../../../components/Typography";
import { useUserData } from "../../../../../contexts/user/UserContext.provider";
import { IncidentReport } from "../../../../../services/api/incident-report/get-incident-reports.api";
import { QuickReportsAPIResponse } from "../../../../../services/api/quick-report/get-quick-reports.api";
import {
  FormAPIModel,
  isIncidentReport,
  isQuickReport,
  ReportType,
} from "../../../../../services/definitions.api";
import { useElectionRoundAllForms } from "../../../../../services/queries/forms.query";
import { useIncidentReports } from "../../../../../services/queries/incident-reports.query";
import { useQuickReports } from "../../../../../services/queries/quick-reports.query";

const Report = () => {
  const { t } = useTranslation("quick_report");
  const navigation = useNavigation();

  const { activeElectionRound } = useUserData();
  const { data: quickReports, isLoading: quickReportsIsLoading } = useQuickReports(
    activeElectionRound?.id,
  );

  const { data: incidentReportsData, isLoading: incidentReportsIsLoading } = useIncidentReports(
    activeElectionRound?.id,
  );

  const reports = useMemo(() => {
    return [...(quickReports ?? []), ...(incidentReportsData?.incidentReports ?? [])].sort(
      (a, b) => +new Date(a.timestamp) - +new Date(b.timestamp),
    );
  }, [quickReports, incidentReportsData]);

  const { data: forms, isLoading: isLoadingForms } = useElectionRoundAllForms(
    activeElectionRound?.id,
  );

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

        {incidentReportsData ? (
          <ReportContent
            reports={reports}
            isLoading={quickReportsIsLoading || incidentReportsIsLoading || isLoadingForms}
            activeElectionRound={activeElectionRound}
            forms={forms?.forms ?? []}
          />
        ) : (
          false
        )}
      </Screen>

      {reports?.length ? (
        <YStack width="100%" paddingHorizontal="$md" marginVertical="$xxs">
          <Button
            preset="outlined"
            backgroundColor="white"
            onPress={router.push.bind(null, "/new-report")}
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

interface ReportContentProps {
  reports: (QuickReportsAPIResponse | IncidentReport)[];
  isLoading: boolean;
  activeElectionRound: ElectionRoundVM | undefined;
  forms: FormAPIModel[];
}

interface FormDetails {
  id: string;
  code: string;
  name: string;
}

const ESTIMATED_ITEM_SIZE = 200;

const ReportContent = ({ reports, isLoading, activeElectionRound, forms }: ReportContentProps) => {
  const { t } = useTranslation(["quick_report", "common"]);
  const { width } = useWindowDimensions();

  const { refetch, isRefetching } = useQuickReports(activeElectionRound?.id);

  const formDetails = useMemo(() => {
    return forms
      ?.map<FormDetails>((f) => ({
        id: f.id,
        code: f.code,
        name: f.name[f.defaultLanguage],
      }))
      ?.reduce((acc: Record<string, FormDetails>, current: FormDetails) => {
        acc[current.id] = current;

        return acc;
      }, {});
  }, [forms]);


  if (isLoading) {
    return (
      <YStack justifyContent="center" alignItems="center" flex={1}>
        <Spinner size="large" color="$purple5" />
      </YStack>
    );
  }

  return (
    <YStack padding="$md" flex={1}>
      <ListView<QuickReportsAPIResponse | IncidentReport>
        data={reports}
        showsVerticalScrollIndicator={false}
        ListHeaderComponent={
          reports.length > 0 ? (
            <Typography
              marginBottom="$xxs"
              preset="body1"
              textAlign="left"
              color="$gray7"
              fontWeight="700"
            >
              {t("list.heading")}
            </Typography>
          ) : (
            <></>
          )
        }
        ListEmptyComponent={
          <YStack alignItems="center" justifyContent="center" gap="$md" marginTop="40%">
            <Icon icon="undrawFlag" />
            <YStack gap="$md" paddingHorizontal="$xl">
              <Typography preset="body1" textAlign="center" color="$gray12" lineHeight={24}>
                {t("list.empty")}
              </Typography>
              <Button
                preset="outlined"
                onPress={router.push.bind(null, "/new-report")}
                backgroundColor="white"
              >
                {t("list.add")}
              </Button>
            </YStack>
          </YStack>
        }
        bounces={true}
        renderItem={({ item, index }) => {
          if (isQuickReport(item)) {
            return (
              <>
                <ReportCard
                  key={`${index}`}
                  title={item.title}
                  description={item.description}
                  numberOfAttachments={item.attachments.length}
                  onPress={() =>
                    router.push(`/quick-report-details/${item.id}?reportTitle=${item.title}`)
                  }
                  reportType={ReportType.QuickReport}
                  marginBottom="$md"
                />
              </>
            );
          }

          if (isIncidentReport(item)) {
            return (
              <>
                <ReportCard
                  key={`${index}`}
                  title={formDetails[item.formId].name}
                  description={item.timestamp}
                  numberOfAttachments={item.attachments.length}
                  onPress={() => router.push(`/incident-report-details/${item.id}`)}
                  reportType={ReportType.IncidentReport}
                  marginBottom="$md"
                />
              </>
            );
          }

          return null;
        }}
        estimatedItemSize={ESTIMATED_ITEM_SIZE}
        estimatedListSize={
          !reports.length
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

export default Report;
