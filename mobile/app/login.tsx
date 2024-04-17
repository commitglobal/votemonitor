import React from "react";
import { router } from "expo-router";
import { useAuth } from "../hooks/useAuth";
import { View, XStack, styled } from "tamagui";
import { useTranslation } from "react-i18next";
import { Screen } from "../components/Screen";
import { StatusBar, ViewStyle } from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { Icon } from "../components/Icon";
import { tokens } from "../theme/tokens";
import { Typography } from "../components/Typography";
import Button from "../components/Button";
import FormInput from "../components/FormInputs/FormInput";
import { Control, Controller, FieldValues, useForm } from "react-hook-form";
import Card from "../components/Card";

const Login = () => {
  const { t } = useTranslation("login");
  const { signIn } = useAuth();
  const onLogin = async (formData: Record<string, string>) => {
    try {
      await signIn(formData.email, formData.password);
      router.replace("/");
    } catch (err) {
      console.error("Error while logging in...");
    }
  };
  const { handleSubmit, control } = useForm({
    // defaultValues: {
    //   email: "alice@example.com",
    //   password: "string",
    // },
  });

  const ContentContainer = styled(View, {
    name: "Container",
    flex: 1,
    gap: 40,
    paddingHorizontal: tokens.space.md.val,
    paddingTop: tokens.space.xl.val,
  });

  return (
    <Screen preset="auto" contentContainerStyle={$containerStyle}>
      <BigHeader />

      <ContentContainer>
        <View flexDirection="row" gap={8}>
          <Icon icon="infoCircle" size={18} color="white" style={{ marginTop: 2 }} />
          <Typography>{t("informative-text")}</Typography>
        </View>

        <LoginForm control={control} />
      </ContentContainer>

      <Card width="100%">
        <Button onPress={handleSubmit(onLogin)}>Log in</Button>
      </Card>
    </Screen>
  );
};

const LoginForm = ({ control }: { control: Control<FieldValues, any> }) => {
  const { t } = useTranslation("login");

  return (
    <View gap={12}>
      <Typography preset="heading" fontWeight="700">
        {t("title")}
      </Typography>

      <Typography>{t("paragraph")}</Typography>

      <Controller
        key="email"
        name="email"
        control={control}
        render={({ field: { onChange, value } }) => (
          <FormInput
            type="text"
            title={t("form.email.label")}
            placeholder={t("form.email.placeholder")}
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
            title={t("form.password.label")}
            placeholder={t("form.password.placeholder")}
            value={value}
            onChangeText={onChange}
          ></FormInput>
        )}
      />
    </View>
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

const $containerStyle: ViewStyle = {
  flex: 1,
};

export default Login;
