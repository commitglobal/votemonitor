import { Screen } from "../../../../../../components/Screen";
import { router, useLocalSearchParams } from "expo-router";
import Header from "../../../../../../components/Header";
import { Icon } from "../../../../../../components/Icon";
import { Typography } from "../../../../../../components/Typography";
import { YStack } from "tamagui";

type SearchParamsType = {
  reportTitle: string;
};

const ReportDetails = () => {
  const { reportTitle } = useLocalSearchParams<SearchParamsType>();
  console.log(reportTitle);

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
      <YStack gap={32} paddingHorizontal={16}>
        <Typography> Title of the issue </Typography>
        <Typography> Description of the issue </Typography>
        <Typography> Attachments </Typography>
      </YStack>
    </Screen>
  );
};

export default ReportDetails;
