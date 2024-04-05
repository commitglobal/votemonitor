import { View } from "react-native";
import { useAuth } from "../../../../hooks/useAuth";
import OfflinePersistComponentExample from "../../../../components/OfflinePersistComponentExample";
import { StatusBar } from "react-native";
import { Button, Text } from "tamagui";
import { router } from "expo-router";

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
    </View>
  );
};

export default Index;
