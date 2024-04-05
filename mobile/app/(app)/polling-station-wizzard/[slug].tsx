import { Typography } from "../../../components/Typography";
import { router, useLocalSearchParams } from "expo-router";
import { Button } from "tamagui";
import { Screen } from "../../../components/Screen";
import Header from "../../../components/Header";
import { Icon } from "../../../components/Icon";

const PollingStationWizzard = () => {
  const { slug } = useLocalSearchParams();
  return (
    <Screen
      contentContainerStyle={{
        gap: 20,
      }}
      statusBarStyle="light"
    >
      <Header
        title={"Add polling station"}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      />
      <Typography>This is the wizzard, page {slug}</Typography>
      <Button onPress={() => router.replace(`/polling-station-wizzard/${+(slug || 0) + 1}`)}>
        Next
      </Button>
    </Screen>
  );
};

export default PollingStationWizzard;
