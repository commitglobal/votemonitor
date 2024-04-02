import { View, Text } from "react-native";
import { useAuth } from "../../../hooks/useAuth";
import OfflinePersistComponentExample from "../../../components/OfflinePersistComponentExample";
import { Icon } from "../../../components/Icon";

const Index = () => {
  const { signOut } = useAuth();

  return (
    <View style={{ gap: 20 }}>
      <Text>Observation</Text>
      <Icon
        icon="eyeOff"
        size={24}
        color="$purple5"
        backgroundColor="$yellow2"
      />
      <OfflinePersistComponentExample></OfflinePersistComponentExample>
      <Text onPress={signOut}>Logout</Text>
    </View>
  );
};

export default Index;
