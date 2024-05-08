import { Screen } from "../../../../../../components/Screen";
import { router, useLocalSearchParams } from "expo-router";
import Header from "../../../../../../components/Header";
import { Icon } from "../../../../../../components/Icon";
import { Typography } from "../../../../../../components/Typography";
import { YStack } from "tamagui";
import { useQuickReportById } from "../../../../../../services/queries/quick-reports.query";
import { useUserData } from "../../../../../../contexts/user/UserContext.provider";

type SearchParamsType = {
  reportId: string;
  reportTitle: string;
};

const ReportDetails = () => {
  const { reportTitle, reportId } = useLocalSearchParams<SearchParamsType>();
  console.log(reportTitle);
  console.log(reportId);

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
    return <Typography>Loading</Typography>;
  }

  if (currentReportError) {
    return <Typography>Report Error</Typography>;
  }

  console.log(quickReport);

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
      <YStack gap={16} paddingHorizontal={16} paddingTop={32} justifyContent="center">
        <Typography preset="subheading" fontWeight="500">
          {quickReport?.title}
        </Typography>
        <Typography preset="body1" lineHeight={24} color="$gray8">
          {quickReport?.description}
        </Typography>

        {quickReport?.attachments?.length == 0 && (
          <Typography fontWeight="500"> No attached files </Typography>
        )}
      </YStack>
    </Screen>
  );
};

export default ReportDetails;
