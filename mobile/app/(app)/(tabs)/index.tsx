import { View, Text } from "react-native";
import { useAuth } from "../../../hooks/useAuth";

const Index = () => {
  const { signOut } = useAuth();

  return (
    <View style={{ gap: 20 }}>
      <Text>Observation</Text>
      <Text onPress={signOut}>Logout</Text>
    </View>
  );
};

export default Index;
