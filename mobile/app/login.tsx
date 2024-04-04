import React from "react";
import { View, Text } from "react-native";
import { router } from "expo-router";
import { useAuth } from "../hooks/useAuth";
import { Button } from "tamagui";
import { Typography } from "../components/Typography";
import { Badge } from "../components/Badge";
import Card from "../components/Card";
// import { Badge, BadgeProps } from "../components/Badge";
import { FormCard } from "../components/FormCard";

const Login = () => {
  const { signIn } = useAuth();
  return (
    <View
      style={{
        flex: 1,
        justifyContent: "center",
        alignItems: "center",
        gap: 20,
        paddingHorizontal: 16,
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
      <Typography preset="heading" color="$red12">
        Heading
      </Typography>

      <Typography color="$purple5" numberOfLines={3}>
        SubheadingSubheadingSubheadingSubheadingSubheadingSubheadingSubheadingSubheadingSubheadingSubheadingSubheadingSubheadingSubheading
      </Typography>

      <Typography preset="default">default</Typography>
      <Typography preset="body1">body1</Typography>
      <Typography preset="body2">body2</Typography>
      <Typography preset="helper">helper</Typography>

      <FormCard header="A - Opening (EN)" footer="0/33 questions" />
      <FormCard
        header="A - Opening"
        subHeader="Available in RO, BG, EN"
        footer="0/33 questions"
        badgePreset="warning"
      />
      {/* 
      <Badge></Badge>
      <Badge preset="success"></Badge>
      <Badge preset="warning"></Badge>
      <Badge preset="danger"></Badge> */}
    </View>
  );
};

export default Login;
