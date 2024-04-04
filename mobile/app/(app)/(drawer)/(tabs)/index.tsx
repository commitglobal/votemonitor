import { View } from "react-native";
import { useAuth } from "../../../../hooks/useAuth";
import OfflinePersistComponentExample from "../../../../components/OfflinePersistComponentExample";
import { StatusBar } from "react-native";
import { Button, Card, YStack, Text, XStack } from "tamagui";
import { router } from "expo-router";
import SelectPollingStation from "../../../../components/SelectPollingStation";
import { Typography } from "../../../../components/Typography";
import TimeSelect from "../../../../components/TimeSelect";
import PollingStationInfoDefault from "../../../../components/PollingStationInfoDefault";
import { Icon } from "../../../../components/Icon";

const Index = () => {
  const { signOut } = useAuth();

  return (
    <View style={{ gap: 20 }}>
      <StatusBar barStyle="light-content" />
      <SelectPollingStation />
      {/* time selection card */}
      <Card>
        <TimeSelect />
      </Card>
      {/* polling station info card */}
      <Card width="90%" padding="$5" backgroundColor="white">
        <PollingStationInfoDefault />
        <XStack
          alignItems="center"
          justifyContent="space-between"
          marginTop="$md"
        >
          <Text color="$gray7">Polling station information</Text>
          <Icon icon="chevronRight" />
        </XStack>
      </Card>
      <Text>Observation</Text>
      <OfflinePersistComponentExample></OfflinePersistComponentExample>
      <Button onPress={() => router.push("/polling-station-wizzard/1")}>
        Go To Polling station wizzard
      </Button>
      <Button onPress={() => router.push("/form-questionnaire/1")}>
        Go Form wizzard
      </Button>
      <Button onPress={() => router.push("/polling-station-questionnaire")}>
        Go To Polling Station Qustionnaire
      </Button>
      <Text onPress={signOut}>Logout</Text>
    </View>
  );
};

export default Index;
