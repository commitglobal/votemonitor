import { Screen } from "../../../../../../../components/Screen";
import { router, useLocalSearchParams } from "expo-router";
import Header from "../../../../../../../components/Header";
import { Icon } from "../../../../../../../components/Icon";
import { Typography } from "../../../../../../../components/Typography";
import { ScrollView, YStack } from "tamagui";
import { useQuickReportById } from "../../../../../../../services/queries/quick-reports.query";
import { useUserData } from "../../../../../../../contexts/user/UserContext.provider";
import Card from "../../../../../../../components/Card";
import { useTranslation } from "react-i18next";
import { RefreshControl } from "react-native";
import { useMemo, useState } from "react";
import { MediaDialog } from "../../../../../../../components/MediaDialog";
import { AttachmentMimeType } from "../../../../../../../services/api/get-attachments.api";
import { QuickReportAttachmentAPIResponse } from "../../../../../../../services/api/quick-report/get-quick-reports.api";
import { localizeIncidentCategory } from "../../../../../../../helpers/translationHelper";

type SearchParamsType = {
  reportId: string;
  reportTitle: string;
};

const ReportDetails = () => {
  const { reportTitle, reportId } = useLocalSearchParams<SearchParamsType>();
  const { t } = useTranslation(["report_details", "common"]);

  const [previewAttachment, setPreviewAttachment] =
    useState<QuickReportAttachmentAPIResponse | null>(null);

  if (!reportId) {
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

  const incidentCategory = useMemo(() => {
    if (quickReport?.incidentCategory) {
      return localizeIncidentCategory(quickReport.incidentCategory);
    }

    return "";
  }, [quickReport?.incidentCategory]);

  if (isLoadingCurrentReport) {
    return <Typography>{t("loading", { ns: "common" })}</Typography>;
  }

  if (currentReportError) {
    return (
      <Screen preset="fixed" contentContainerStyle={{ flexGrow: 1 }}>
        <Header
          title={`${reportTitle ?? t("title")}`}
          leftIcon={<Icon icon="chevronLeft" color="white" />}
          onLeftPress={() => router.back()}
        />
        <YStack flex={1}>
          <ScrollView
            showsVerticalScrollIndicator={false}
            contentContainerStyle={{ flex: 1, alignItems: "center", flexGrow: 1 }}
            paddingVertical="$xxl"
            refreshControl={
              <RefreshControl refreshing={isRefetchingQuickReport} onRefresh={refetchQuickReport} />
            }
          >
            <Typography>{t("error")}</Typography>
          </ScrollView>
        </YStack>
      </Screen>
    );
  }

  const attachments = useMemo(() => quickReport?.attachments || [], [quickReport?.attachments]);

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
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      />

      <YStack flex={1}>
        <ScrollView
          contentContainerStyle={{
            paddingVertical: 32,
            gap: 32,
            flexGrow: 1,
            paddingHorizontal: 16,
          }}
          showsVerticalScrollIndicator={false}
          refreshControl={
            <RefreshControl refreshing={isRefetchingQuickReport} onRefresh={refetchQuickReport} />
          }
        >
          <YStack gap={16}>
            <Typography preset="subheading" fontWeight="500">
              {quickReport?.title}
            </Typography>
            <Typography preset="body1" lineHeight={24} color="$gray8">
              {incidentCategory}
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
                <Card key={key} onPress={() => setPreviewAttachment(attachment)}>
                  <Typography preset="body1" fontWeight="700" key={attachment.id}>
                    {attachment.fileName}
                  </Typography>
                </Card>
              ))}
            </YStack>
          )}
          {previewAttachment && previewAttachment.mimeType === AttachmentMimeType.IMG && (
            <MediaDialog
              media={{ type: previewAttachment.mimeType, src: previewAttachment.presignedUrl }}
              onClose={() => setPreviewAttachment(null)}
            />
          )}
        </ScrollView>
      </YStack>
    </Screen>
  );
};

export default ReportDetails;
