import React from "react";
import { router } from "expo-router";
import { useAuth } from "../hooks/useAuth";
import { View, XStack, YStack, styled } from "tamagui";
import { useTranslation } from "react-i18next";
import { Screen } from "../components/Screen";
import { StatusBar } from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { Icon } from "../components/Icon";
import { tokens } from "../theme/tokens";
import { Typography } from "../components/Typography";
import Button from "../components/Button";
import FormInput from "../components/FormInputs/FormInput";

const Login = () => {
  const { signIn } = useAuth();
  const { t } = useTranslation("login");

  const onLogin = async () => {
    try {
      await signIn();
      router.replace("/");
    } catch (err) {
      console.error("Error while logging in...");
    }
  };

  const ContentContainer = styled(View, {
    name: "Container",
    gap: 40,
    paddingHorizontal: tokens.space.md.val,
    paddingVertical: tokens.space.xl.val,
  });

  return (
    <Screen preset="auto" backgroundColor="white">
      <BigHeader />

      <ContentContainer>
        <Typography> {t("informative-text")}</Typography>
        <LoginForm />
      </ContentContainer>

      <Button> Log in</Button>
    </Screen>
  );
};

const LoginForm = () => {
  return (
    <YStack>
      <Typography> Log in </Typography>

      <Typography>
        Please use the email address with which you registered as an independent observer:
      </Typography>

      <FormInput type="text" title="email" placeholder="email"></FormInput>
      <FormInput type="text" title="password" placeholder="password"></FormInput>
    </YStack>
  );
};

const SmallHeader = () => {
  const insets = useSafeAreaInsets();
  const StyledWrapper = styled(XStack, {
    name: "StyledWrapper",
    backgroundColor: "$purple5",
    height: insets.top,
    paddingTop: insets.top,
  });

  return (
    <StyledWrapper>
      <StatusBar barStyle="light-content"></StatusBar>
    </StyledWrapper>
  );
};

const BigHeader = () => {
  const insets = useSafeAreaInsets();
  const StyledWrapper = styled(XStack, {
    name: "StyledWrapper",
    backgroundColor: "$purple5",
    height: 160 + insets.top,
    paddingTop: insets.top,
    alignItems: "center",
    justifyContent: "center",
  });

  return (
    <StyledWrapper>
      <StatusBar barStyle="light-content"></StatusBar>
      <Icon icon="loginLogo" size={300} />
    </StyledWrapper>
  );
};

export default Login;
