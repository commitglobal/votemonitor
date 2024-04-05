import { View, Text, StatusBar } from "react-native";
import { useAuth } from "../../../../hooks/useAuth";
import OfflinePersistComponentExample from "../../../../components/OfflinePersistComponentExample";
import { Button } from "tamagui";
import { router } from "expo-router";
import Card from "../../../../components/Card";
import Input from "../../../../components/Inputs/Input";
import CheckboxInput from "../../../../components/Inputs/CheckboxInput";

const Index = () => {
  const { signOut } = useAuth();

  return (
    <View style={{ gap: 20 }}>
      <Card>
        <Input />
        <CheckboxInput label="hello" marginTop={20} id="1" />
        <CheckboxInput label="hello2" marginTop={10} id="2" />
      </Card>

      <StatusBar barStyle="light-content" />
      <OfflinePersistComponentExample></OfflinePersistComponentExample>
      <Button onPress={() => router.push("/polling-station-wizzard/1")}>
        Go To Polling station wizzard
      </Button>
      <Button onPress={() => router.push("/form-questionnaire/1")}>Go Form wizzard</Button>
      <Button onPress={() => router.push("/polling-station-questionnaire")}>
        Go To Polling Station Qustionnaire
      </Button>
      <Text onPress={signOut}>Logout</Text>
    </View>
  );
};

export default Index;
