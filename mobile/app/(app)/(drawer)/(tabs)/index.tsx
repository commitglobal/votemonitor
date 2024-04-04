import { View, Text } from "react-native";
import { useAuth } from "../../../../hooks/useAuth";
import OfflinePersistComponentExample from "../../../../components/OfflinePersistComponentExample";
import { StatusBar } from "react-native";
import { Button, XStack, YStack } from "tamagui";
import { router } from "expo-router";
import SelectPollingStation from "../../../../components/SelectPollingStation";
import { Typography } from "../../../../components/Typography";
import TimeSelect from "../../../../components/TimeSelect";
import Card from "../../../../components/Card";

const Index = () => {
  const { signOut } = useAuth();

  return (
    <View style={{ gap: 20 }}>
      <StatusBar barStyle="light-content" />
      <SelectPollingStation />
      <XStack gap={8}>
        <Card flex={1}>
          <TimeSelect />
        </Card>
        <Card flex={1}>
          <TimeSelect />
        </Card>
      </XStack>

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
