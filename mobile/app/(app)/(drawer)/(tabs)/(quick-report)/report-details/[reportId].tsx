import { Screen } from "../../../../../../components/Screen";
import { router, useLocalSearchParams } from "expo-router";
import Header from "../../../../../../components/Header";
import { Icon } from "../../../../../../components/Icon";

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
        stickyHeaderIndices: [0],
        bounces: false,
        showsVerticalScrollIndicator: false,
      }}
    >
      <Header
        title={`${reportTitle}`}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      />
    </Screen>
  );
};

export default ReportDetails;
