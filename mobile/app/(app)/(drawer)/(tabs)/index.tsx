import { View } from "react-native";
import { useAuth } from "../../../../hooks/useAuth";
import OfflinePersistComponentExample from "../../../../components/OfflinePersistComponentExample";
import { StatusBar } from "react-native";
import { Button, Text } from "tamagui";
import { router } from "expo-router";
import Card from "../../../../components/Card";
import Select from "../../../../components/Select";

const Index = () => {
  const { signOut } = useAuth();

  return (
    <View style={{ gap: 20 }}>
      <StatusBar barStyle="light-content" />
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
      <Card>
        <Select placeholder=" Select region" selectionData={regionData} />
      </Card>
      <Card>
        <Select placeholder=" Select region" selectionData={countryData} />
      </Card>
    </View>
  );
};

const regionData = [
  { id: 1, value: "North" },
  { id: 2, value: "North-West" },
  { id: 3, value: "North-East" },
  { id: 4, value: "West" },
  { id: 5, value: "East" },
  { id: 6, value: "South-West" },
  { id: 7, value: "South" },
];

const countryData = [
  { id: 3, value: "Russia" },
  { id: 4, value: "France" },
  { id: 5, value: "China" },
  { id: 6, value: "Brazil" },
  { id: 7, value: "Australia" },
];

export default Index;
