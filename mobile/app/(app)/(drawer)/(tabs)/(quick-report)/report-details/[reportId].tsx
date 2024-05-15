import { Screen } from "../../../../../../components/Screen";
import { router, useLocalSearchParams } from "expo-router";
import Header from "../../../../../../components/Header";
import { Icon } from "../../../../../../components/Icon";
import { Typography } from "../../../../../../components/Typography";
import { Image, View, YStack } from "tamagui";
import { useQuickReportById } from "../../../../../../services/queries/quick-reports.query";
import { useUserData } from "../../../../../../contexts/user/UserContext.provider";
import Card from "../../../../../../components/Card";
import { useState } from "react";

type SearchParamsType = {
  reportId: string;
  reportTitle: string;
};

const ReportDetails = () => {
  const { reportTitle, reportId } = useLocalSearchParams<SearchParamsType>();
  const [displayAttachment, setDisplayAttachment] = useState(false);
  console.log("displayAttachment", displayAttachment);

  if (!reportId || !reportTitle) {
    return <Typography>Incorrect page params</Typography>;
  }

  const { activeElectionRound } = useUserData();

  const {
    data: quickReport,
    isLoading: isLoadingCurrentReport,
    error: currentReportError,
  } = useQuickReportById(activeElectionRound?.id, reportId);

  // console.log("data", quickReport);

  if (isLoadingCurrentReport) {
    return <Typography>Loading</Typography>;
  }

  if (currentReportError) {
    return <Typography>Report Error</Typography>;
  }

  const attachments = quickReport?.attachments || [];
  // if (attachments.length !== 0) {
  //   console.log("attachments", attachments[0]);
  //   console.log("attachments", attachments[1]);
  // }

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
            {attachments.map((attachment, key) => (
              <Card
                key={key}
                onPressIn={() => setDisplayAttachment(true)}
                onPressOut={() => setDisplayAttachment(false)}
              >
                <Typography preset="body1" fontWeight="700" key={attachment.id}>
                  {attachment.fileName}
                </Typography>
              </Card>
            ))}
          </YStack>
        )}
      </YStack>

      {displayAttachment === true && <ImageAttachment presignedUrl={attachments[0].presignedUrl} />}
    </Screen>
  );
};

interface AttachmentProps {
  presignedUrl: string;
}

const ImageAttachment = (props: AttachmentProps) => {
  const { presignedUrl } = props;

  return (
    <View>
      <Image
        source={{ uri: presignedUrl, width: 100, height: 100 }}
        width="100%"
        resizeMode="contain"
      />
    </View>
  );
};

export default ReportDetails;
