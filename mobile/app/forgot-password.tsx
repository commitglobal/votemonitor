import { View, Text } from "react-native";
import { router } from "expo-router";

const ForgotPassword = () => {
  return (
    <View style={{ flex: 1, justifyContent: "center", alignItems: "center" }}>
      <Text
        onPress={() => {
          router.back();
        }}
      >
        Go Back
      </Text>
    </View>
  );
};

export default ForgotPassword;
