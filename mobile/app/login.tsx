import { View } from "react-native";
import { router } from "expo-router";
import { useAuth } from "../hooks/useAuth";
import { Button as TamaguiButton } from "tamagui";
import { Typography } from "../components/Typography";
import { Button } from "../components/Button";

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
      <TamaguiButton
        onPress={() => {
          signIn();
          // Navigate after signing in. You may want to tweak this to ensure sign-in is
          // successful before navigating.
          router.replace("/");
        }}
      >
        Sign In
      </TamaguiButton>
      <TamaguiButton
        onPress={() => {
          router.push("/forgot-password");
        }}
      >
        Forgot Password
      </TamaguiButton>
      <Typography preset="heading" size="sm">Heading</Typography>
      <Typography preset="subheading">Subheading</Typography>
      <Typography preset="default">default</Typography>
      <Typography preset="body1">body1</Typography>
      <Typography preset="body2" >body2</Typography>
      <Typography preset="helper">helper</Typography>


      <Button
        onPress={() => console.log("Default pressed")}> Default (filled) </Button>

      <Button
        preset="outlined"
        onPress={() => console.log("Outlined pressed")}>
          Outlined button
      </Button>

      <Button
        preset="chromeless"
        onPress={() => console.log("Chromeless pressed")}>
          Chromeless
      </Button>
      
    </View>
  );
};

export default Login;
