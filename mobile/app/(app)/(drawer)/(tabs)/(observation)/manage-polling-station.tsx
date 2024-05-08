import { Screen } from "../../../../../components/Screen";
import Header from "../../../../../components/Header";
import { Icon } from "../../../../../components/Icon";
import { YStack, XStack } from "tamagui";
import { Typography } from "../../../../../components/Typography";
import { router } from "expo-router";
import Card from "../../../../../components/Card";

const ManagePollingStation = () => {
  return (
    <Screen
      preset="scroll"
      ScrollViewProps={{
        showsVerticalScrollIndicator: false,
        stickyHeaderIndices: [0],
        bounces: false,
      }}
      backgroundColor="white"
    >
      <Header
        title={"Manage my polling station"}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      />

      <YStack gap={24} paddingTop={24} paddingHorizontal={16}>
        <PollingStationCard />
        <PollingStationCard />
        <PollingStationCard />
        <PollingStationCard />
      </YStack>
    </Screen>
  );
};

const PollingStationCard = () => {
  return (
    <Card>
      <YStack gap={16}>
        <XStack justifyContent="space-between" alignItems="center">
          <Typography preset="body1" fontWeight="700">
            Polling station #:{" "}
          </Typography>
          <Icon icon="bin" color="white"></Icon>
        </XStack>

        <Typography>[Location L1]: </Typography>
        <Typography>[Location L2]: </Typography>
        <Typography>[Location L3]: </Typography>
        <Typography>[Street]: </Typography>
        <Typography> Polling station number: </Typography>
      </YStack>
    </Card>
  );
};

export default ManagePollingStation;
