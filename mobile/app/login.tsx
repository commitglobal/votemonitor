import React from "react";
import { View } from "react-native";
import { router } from "expo-router";
import { useAuth } from "../hooks/useAuth";
import { Button as TamaguiButton } from "tamagui";
import { Typography } from "../components/Typography";
import { Button } from "../components/Button";
import InboxSVG from "/home/madalina/votemonitor/mobile/assets/icons/Inbox.svg";

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
      {/* <TamaguiButton
        onPress={() => {
          signIn();
          // Navigate after signing in. You may want to tweak this to ensure sign-in is
          // successful before navigating.
          router.replace("/");
        }}
      >
        <Typography>Sign In</Typography>
      </TamaguiButton>
      <TamaguiButton
        paddingHorizontal="$xl"
        height={"auto"}
        paddingVertical="$lg"
        backgroundColor="$yellow2"
        onPress={() => {
          router.push("/forgot-password");
        }}
      >
        <Typography size="xl">Forgot Password</Typography>
      </TamaguiButton> */}
      <Typography preset="heading">Heading</Typography>
      <Typography preset="subheading">Subheading</Typography>
      <Typography preset="default">default</Typography>
      <Typography preset="body1">body1</Typography>
      <Typography preset="body2" >body2</Typography>
      <Typography preset="helper">helper</Typography>


      <Button
        text="Default (filled)"
        onPress={() => console.log("Default pressed")} />

      <Button
        text="Outlined pressed"
        preset="outlined"
        icon={<InboxSVG fill={'#7833B3'}></InboxSVG>}
        onPress={() => console.log("Outlined pressed")} />
  

      <Button
        text="Chromeless"
        preset="chromeless"
        iconAfter={<InboxSVG fill={'#7833B3'}></InboxSVG>}
        onPress={() => console.log("Chromeless pressed")}/>
       

      <Button
        text="Clear form"
        preset="red"
        onPress={() => console.log("Red pressed")}/>
    
    </View>
  );
};

export default Login;
