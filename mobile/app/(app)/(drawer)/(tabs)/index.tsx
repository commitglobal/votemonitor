import { View } from "react-native";
import { useAuth } from "../../../../hooks/useAuth";
import OfflinePersistComponentExample from "../../../../components/OfflinePersistComponentExample";
import { StatusBar } from "react-native";
import { Button, Card, Text } from "tamagui";
import { router } from "expo-router";
import SelectPollingStation from "../../../../components/SelectPollingStation";
import TimeSelect from "../../../../components/TimeSelect";
import PollingStationInfoDefault from "../../../../components/PollingStationInfoDefault";
import CardFooter from "../../../../components/CardFooter";

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
        <CardFooter
          text="Polling station information"
          action={() => console.log("hello")}
          marginTop={20}
        />
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
