import React from "react";
import { View, Text } from "react-native";
import { router } from "expo-router";
import { useAuth } from "../hooks/useAuth";
import { Button, useTheme } from "tamagui";
import { Typography } from "../components/Typography";
import { Card } from "../components/Card"


const Login = () => {
  const theme = useTheme()

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
        <Typography>Sign In</Typography>
      </Button>
      <Button
        paddingHorizontal="$xl"
        height={"auto"}
        paddingVertical="$lg"
        backgroundColor="$yellow2"
        onPress={() => {
          router.push("/forgot-password");
        }}
      >
        <Typography size="xl">Forgot Password</Typography>
      </Button>
      <Typography preset="heading">Heading</Typography>
      <Typography preset="subheading">Subheading</Typography>
      <Typography preset="default">default</Typography>
      <Typography preset="body1">body1</Typography>
      <Typography preset="body2">body2</Typography>
      <Typography preset="helper">helper</Typography>

      <Card
        customStyle={{backgroundColor:theme.$gray1?.val}}
        children={<View><Text>Children component</Text></View>}>
      </Card>

    </View>
  );
};

export default Login;
