import { Text } from "react-native";
import { useAuth } from "../../../../hooks/useAuth";
import OfflinePersistComponentExample from "../../../../components/OfflinePersistComponentExample";
import { Button } from "tamagui";
import { router } from "expo-router";
import { Screen } from "../../../../components/Screen";

const Index = () => {
  const { signOut } = useAuth();

  return (
    <Screen preset="fixed" contentContainerStyle={{ gap: 20 }} safeAreaEdges={["top"]}>
      <OfflinePersistComponentExample></OfflinePersistComponentExample>
      <Button onPress={() => router.push("/polling-station-wizzard/1")}>
        Go To Polling station wizzard
      </Button>
      <Button onPress={() => router.push("/form-questionnaire/1")}>Go Form wizzard</Button>
      <Button onPress={() => router.push("/polling-station-questionnaire")}>
        Go To Polling Station Qustionnaire
      </Button>
      <Text onPress={signOut}>Logout</Text>
    </Screen>
  );
};

export default Index;
