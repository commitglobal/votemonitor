import { Screen } from "../../../../../../components/Screen";
import { router, useLocalSearchParams } from "expo-router";
import Header from "../../../../../../components/Header";
import { Icon } from "../../../../../../components/Icon";
import { Typography } from "../../../../../../components/Typography";
import { YStack } from "tamagui";
import { useQuickReportById } from "../../../../../../services/queries/quick-reports.query";
import { useUserData } from "../../../../../../contexts/user/UserContext.provider";
import { QuickReportAttachmentAPIResponse } from "../../../../../../services/definitions.api";
import Card from "../../../../../../components/Card";

type SearchParamsType = {
  reportId: string;
  reportTitle: string;
};

const dummyAttachments: QuickReportAttachmentAPIResponse[] = [
  {
    id: "1",
    quickReportId: "1",
    electionRoundId: "1",
    fileName: "Attachment 1.jpg",
    mimeType: "image/jpeg",
    presignedUrl: "https://via.placeholder.com/150",
    urlValidityInSeconds: 3600,
  },
  {
    id: "2",
    quickReportId: "1",
    electionRoundId: "1",
    fileName: "Attachment 1.jpg",
    mimeType: "image/jpeg",
    presignedUrl: "https://via.placeholder.com/150",
    urlValidityInSeconds: 3600,
  },
];

const ReportDetails = () => {
  const { reportTitle, reportId } = useLocalSearchParams<SearchParamsType>();

  if (!reportId || !reportTitle) {
    return <Typography>Incorrect page params</Typography>;
  }

  const { activeElectionRound } = useUserData();
  const {
    data: quickReport,
    isLoading: isLoadingCurrentReport,
    error: currentReportError,
  } = useQuickReportById(activeElectionRound?.id, reportId);

  const attachments = dummyAttachments;

  if (isLoadingCurrentReport) {
    return <Typography>Loading</Typography>;
  }

  if (currentReportError) {
    return <Typography>Report Error</Typography>;
  }

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

        {attachments.length === 0 ? (
          <Typography fontWeight="500">No attached files</Typography>
        ) : (
          <YStack gap={16}>
            <Typography fontWeight="500" color="$gray10">
              Uploaded media
            </Typography>
            {attachments.map((attachment) => (
              <Card>
                <Typography preset="body1" fontWeight="700" key={attachment.id}>
                  {attachment.fileName}
                </Typography>
              </Card>
            ))}
          </YStack>
        )}
      </YStack>
    </Screen>
  );
};

export default ReportDetails;
