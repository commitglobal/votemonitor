import { View, Text } from "react-native";
import { useAuth } from "../../../hooks/useAuth";
import OfflinePersistComponentExample from "../../../components/OfflinePersistComponentExample";

const Index = () => {
  const { signOut } = useAuth();

  return (
    <View style={{ gap: 20 }}>
      <Text>Observation</Text>
      <OfflinePersistComponentExample></OfflinePersistComponentExample>
      <Text onPress={signOut}>Logout</Text>
    </View>
  );
};

export default Index;
