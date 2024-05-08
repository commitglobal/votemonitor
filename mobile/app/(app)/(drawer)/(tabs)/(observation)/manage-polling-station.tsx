import { Screen } from "../../../../../components/Screen";
import Header from "../../../../../components/Header";
import { Icon } from "../../../../../components/Icon";
import { YStack } from "tamagui";
import { Typography } from "../../../../../components/Typography";
import { router } from "expo-router";

const ManagePollingStation = () => {
  return (
    <Screen
      preset="scroll"
      ScrollViewProps={{
        showsVerticalScrollIndicator: false,
        stickyHeaderIndices: [0],
        bounces: false,
      }}
    >
      <Header
        title={"Manage my polling station"}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      />

      <YStack>
        <Typography>My polling stations!</Typography>
      </YStack>
    </Screen>
  );
};

export default ManagePollingStation;
