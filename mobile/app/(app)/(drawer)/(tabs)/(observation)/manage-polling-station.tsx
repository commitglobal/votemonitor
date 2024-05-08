import { Screen } from "../../../../../components/Screen";
import Header from "../../../../../components/Header";
import { Icon } from "../../../../../components/Icon";
import { YStack, XStack } from "tamagui";
import { Typography } from "../../../../../components/Typography";
import { router } from "expo-router";
import Card from "../../../../../components/Card";
import { useUserData } from "../../../../../contexts/user/UserContext.provider";

const ManagePollingStation = () => {
  const { visits } = useUserData();
  if (visits === undefined || visits.length === 0) {
    return <Typography>No visits</Typography>;
  }

  console.log(visits[0]);

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
        {visits.map((visit) => (
          <PollingStationCard key={visit.pollingStationId} visit={visit} />
        ))}
      </YStack>
    </Screen>
  );
};

interface PollingStationCardProps {
  visit: any;
}

const PollingStationCard = (props: PollingStationCardProps) => {
  const { visit } = props;

  return (
    <Card>
      <YStack gap={16}>
        <XStack justifyContent="space-between" alignItems="center">
          <Typography preset="body1" fontWeight="700">
            Polling station #: {visit.number}
          </Typography>
          <Icon icon="bin" color="white"></Icon>
        </XStack>

        <Typography>
          [Location L1]: <Typography fontWeight="500">{visit.level1}</Typography>{" "}
        </Typography>
        <Typography>
          [Location L2]: <Typography fontWeight="500"> {visit.level2} </Typography>{" "}
        </Typography>
        <Typography>
          [Location L3]: <Typography fontWeight="500">{visit.level3}</Typography>
        </Typography>
        <Typography>
          [Street]: <Typography fontWeight="500">{visit.address}</Typography>
        </Typography>
        <Typography>
          Polling station number: <Typography fontWeight="500">{visit.number}</Typography>
        </Typography>
      </YStack>
    </Card>
  );
};

export default ManagePollingStation;
