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
import { Controller, useForm } from "react-hook-form";

const Login = () => {
  const { t } = useTranslation("login");

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
    </Screen>
  );
};

const LoginForm = () => {
  const { handleSubmit, control } = useForm();

  const { signIn } = useAuth();
  const onLogin = async (email: string, password: string) => {
    try {
      await signIn(email, password);
      router.replace("/");
    } catch (err) {
      console.error("Error while logging in...");
    }
  };

  const onSubmit = (data: any) => {
    console.log(data);
    onLogin(data.email, data.password);
  };

  return (
    <YStack>
      <Typography> Log in </Typography>

      <Typography>
        Please use the email address with which you registered as an independent observer:
      </Typography>

      <Controller
        key="email"
        name="email"
        control={control}
        render={({ field: { onChange, value } }) => (
          <FormInput
            type="text"
            title="email"
            placeholder="email"
            value={value}
            onChangeText={onChange}
          ></FormInput>
        )}
      />

      <Controller
        key="password"
        name="password"
        control={control}
        render={({ field: { onChange, value } }) => (
          <FormInput
            type="text"
            title="password"
            placeholder="password"
            value={value}
            onChangeText={onChange}
          ></FormInput>
        )}
      />

      <Button onPress={handleSubmit(onSubmit)}>Log in</Button>
    </YStack>
  );
};

// const SmallHeader = () => {
//   const insets = useSafeAreaInsets();
//   const StyledWrapper = styled(XStack, {
//     name: "StyledWrapper",
//     backgroundColor: "$purple5",
//     height: insets.top,
//     paddingTop: insets.top,
//   });

//   return (
//     <StyledWrapper>
//       <StatusBar barStyle="light-content"></StatusBar>
//     </StyledWrapper>
//   );
// };

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
