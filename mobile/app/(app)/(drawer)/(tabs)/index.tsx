import { View, Text } from "react-native";
import { useAuth } from "../../../../hooks/useAuth";
import OfflinePersistComponentExample from "../../../../components/OfflinePersistComponentExample";
import { StatusBar } from "react-native";
import { Button } from "tamagui";
import { router } from "expo-router";

const Index = () => {
  const { signOut } = useAuth();

  return (
    <View style={{ gap: 20 }}>
      <StatusBar barStyle="light-content" />
      <Text>Observation</Text>
      <OfflinePersistComponentExample></OfflinePersistComponentExample>
      <Button onPress={() => router.push("/wizzard")}>Go To Wizzard</Button>
      <Text onPress={signOut}>Logout</Text>
    </View>
  );
};

export default Index;
