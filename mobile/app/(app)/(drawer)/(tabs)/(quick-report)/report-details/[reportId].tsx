import { Screen } from "../../../../../../components/Screen";
import { router, useLocalSearchParams } from "expo-router";
import Header from "../../../../../../components/Header";
import { Icon } from "../../../../../../components/Icon";
import { Typography } from "../../../../../../components/Typography";
import { AlertDialog, YStack, Image } from "tamagui";
import { useQuickReportById } from "../../../../../../services/queries/quick-reports.query";
import { useUserData } from "../../../../../../contexts/user/UserContext.provider";
import { useTranslation } from "react-i18next";
import React from "react";
import Card from "../../../../../../components/Card";
import { QuickReportAttachmentAPIResponse } from "../../../../../../services/api/quick-report/get-quick-reports.api";
import VideoPlayer from "../../../../../../components/VideoPlayer";
import MediaDialog from "../../../../../../components/MediaDialog";
import { ListView } from "../../../../../../components/ListView";

type SearchParamsType = {
  reportId: string;
  reportTitle: string;
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
  } = useQuickReportById(activeElectionRound?.id, reportId);

  if (isLoadingCurrentReport) {
    return <Typography>{t("loading", { ns: "common" })}</Typography>;
  }

  if (currentReportError) {
    return (
      <Screen preset="fixed">
        <Header
          title={`${reportTitle}`}
          titleColor="white"
          barStyle="light-content"
          leftIcon={<Icon icon="chevronLeft" color="white" />}
          onLeftPress={() => router.back()}
        />
        <YStack paddingVertical="$xxl" alignItems="center">
          <Typography>{t("error")}</Typography>
        </YStack>
      </Screen>
    );
  }

  const attachments = quickReport?.attachments || [];
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
        title={`${reportTitle}`}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      />
      <YStack gap={32} paddingHorizontal={16} paddingTop={32} justifyContent="center">
        <YStack gap={16}>
          <Typography preset="subheading" fontWeight="500">
            {quickReport?.title}
          </Typography>
          <Typography preset="body1" lineHeight={24} color="$gray8">
            {quickReport?.description}
          </Typography>
        </YStack>

        <Attachments attachments={attachments} />
      </YStack>
    </Screen>
  );
};

interface AttachmentsProps {
  attachments: QuickReportAttachmentAPIResponse[];
}

const Attachments = (props: AttachmentsProps) => {
  const { attachments } = props;
  const { t } = useTranslation(["report_details", "common"]);

  // No attachments
  if (attachments.length === 0) {
    return (
      <Typography fontWeight="500" color="$gray10">
        {t("no_files")}
      </Typography>
    );
  }

  // Attachment preview dialog will be displayed only for the selected file
  const [selectedFile, setSelectedFile] = React.useState<QuickReportAttachmentAPIResponse | null>(
    null,
  );

  return (
    <YStack gap={16}>
      <Typography fontWeight="500" color="$gray10">
        {t("uploaded_media")}
      </Typography>
      <ListView<QuickReportAttachmentAPIResponse>
        data={attachments}
        renderItem={({ item, index }) => {
          return (
            <Card onPress={() => setSelectedFile(item)} key={index}>
              <Typography preset="body1" fontWeight="700" key={item.id}>
                {item.fileName}
              </Typography>
            </Card>
          );
        }}
        estimatedItemSize={225}
      />

      {/* Display dialog for the selected file */}
      {selectedFile && (
        <MediaDialog
          open
          header={
            <AlertDialog.Cancel>
              <Icon icon="x" alignSelf="flex-end" onPress={() => setSelectedFile(null)} />
            </AlertDialog.Cancel>
          }
          content={
            selectedFile.mimeType.includes("image") ? (
              <Image
                source={{ uri: selectedFile.presignedUrl }}
                width="100%"
                height={350}
                resizeMode="contain"
              />
            ) : selectedFile.mimeType.includes("video") ? (
              <VideoPlayer uri={selectedFile.presignedUrl} />
            ) : (
              // TODO: This might be changed to AudioPlayer in the future
              <VideoPlayer uri={selectedFile.presignedUrl} />
            )
          }
        />
      )}
    </YStack>
  );
};
export default ReportDetails;
