import { View, Text } from "react-native";
import { useAuth } from "../../../../hooks/useAuth";
import Button from "../../../../components/Button";

const More = () => {
  const { signOut } = useAuth();

  return (
    <View>
      <Text>More</Text>
      <Button onPress={signOut}>Logout</Button>
    </View>
  );
};

export default More;
