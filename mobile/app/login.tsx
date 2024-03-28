import { View } from "react-native";
import { router } from "expo-router";
import { useAuth } from "../hooks/useAuth";
import { Button, Text } from "tamagui";

const Login = () => {
  const { signIn } = useAuth();
  return (
    <View
      style={{
        flex: 1,
        justifyContent: "center",
        alignItems: "center",
        gap: 20,
      }}
    >
      <Button
        onPress={() => {
          signIn();
          // Navigate after signing in. You may want to tweak this to ensure sign-in is
          // successful before navigating.
          router.replace("/");
        }}
      >
        Sign In
      </Button>
      <Button
        onPress={() => {
          router.push("/forgot-password");
        }}
      >
        Forgot Password
      </Button>
      <Text fontFamily={"$heading"} fontSize={24}>
        Roboto
      </Text>
      <Text fontFamily={"$body"} fontSize={24}>
        DMSans
      </Text>
    </View>
  );
};

export default Login;
