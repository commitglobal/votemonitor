import React, { useState } from "react";
import { router } from "expo-router";
import { useAuth } from "../hooks/useAuth";
import { View, XStack, YStack, styled } from "tamagui";
import { useTranslation } from "react-i18next";
import { Screen } from "../components/Screen";
import { StatusBar } from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { Icon } from "../components/Icon";
import { Typography } from "../components/Typography";
import Button from "../components/Button";
import FormInput from "../components/FormInputs/FormInput";
import { Control, Controller, FieldErrors, FieldValues, useForm } from "react-hook-form";
import Card from "../components/Card";
import AsyncStorage from "@react-native-async-storage/async-storage";
import { CURRENT_USER_STORAGE_KEY } from "../common/constants";
import Constants from "expo-constants";
import * as Sentry from "@sentry/react-native";

interface FormData {
  email: string;
  password: string;
}

const Login = () => {
  const { t } = useTranslation("login");
  const { signIn } = useAuth();
  const [authError, setAuthError] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const onLogin = async (formData: FormData) => {
    try {
      setIsLoading(true);
      const email = formData.email.trim().toLocaleLowerCase();
      const password = formData.password.trim();

      await signIn(email, password);
      await AsyncStorage.setItem(CURRENT_USER_STORAGE_KEY, email);
      router.replace("/");
    } catch (err) {
      setAuthError(true);
      Sentry.captureException(err);
    } finally {
      setIsLoading(false);
    }
  };
  const { handleSubmit, control, formState } = useForm({
    defaultValues: { email: "alice@example.com", password: "string" },
  });
  const { errors } = formState;

  const insets = useSafeAreaInsets();

  return (
    <Screen
      preset="auto"
      ScrollViewProps={{
        bounces: false,
      }}
      contentContainerStyle={{
        flexGrow: 1,
      }}
    >
      <Header />

      <YStack paddingHorizontal="$md" gap="$md">
        <XStack marginTop="$md" justifyContent="flex-start" gap="$xxs">
          <Icon icon="infoCircle" size={18} color="white" style={{ marginTop: 2 }} />
          <XStack flex={1}>
            <Typography>{t("informative-text")}</Typography>
          </XStack>
        </XStack>
        <LoginForm control={control} errors={errors} authError={authError} />
      </YStack>
      <Card width="100%" paddingBottom={16 + insets.bottom} marginTop="auto">
        <Button onPress={handleSubmit(onLogin)} disabled={isLoading}>
          {isLoading ? "Log in..." : "Log in"}
        </Button>
      </Card>
    </Screen>
  );
};

const LoginForm = ({
  control,
  errors,
  authError,
}: {
  control: Control<FormData, any>;
  errors: FieldErrors<FieldValues>;
  authError: boolean;
}) => {
  const { t } = useTranslation("login");
  const [secureTextEntry, setSecureTextEntry] = React.useState(true);
  const passIcon = secureTextEntry === false ? "eye" : "eyeOff";

  return (
    <View gap="$sm">
      <Typography preset="heading" fontWeight="700">
        {t("title")}
      </Typography>

      <Typography>{t("paragraph")}</Typography>

      {authError && <CredentialsError />}

      <Controller
        key="email"
        name="email"
        control={control}
        rules={{
          required: {
            value: true,
            message: t("form.email.required"),
          },
          pattern: {
            value: /\S+@\S+\.\S+/,
            message: t("form.email.format"),
          },
        }}
        render={({ field: { onChange, value } }) => (
          <FormInput
            type="text"
            title={t("form.email.label")}
            placeholder={t("form.email.placeholder")}
            value={value}
            onChangeText={onChange}
            error={errors?.email?.message?.toString()}
          />
        )}
      />

      <Controller
        key="password"
        name="password"
        control={control}
        rules={{
          required: {
            value: true,
            message: t("form.password.required"),
          },
        }}
        render={({ field: { onChange, value } }) => (
          <FormInput
            type="password"
            secureTextEntry={secureTextEntry}
            title={t("form.password.label")}
            placeholder={t("form.password.placeholder")}
            value={value}
            onChangeText={onChange}
            iconRight={<Icon icon={passIcon} size={20} color="$gray11" />}
            onIconRightPress={() => {
              setSecureTextEntry(!secureTextEntry);
            }}
            error={errors?.password?.message?.toString()}
          />
        )}
      />

      <View
        alignSelf="flex-end"
        paddingTop="$sm"
        paddingLeft="$lg"
        onPress={() => {
          router.push("./forgot-password");
        }}
        pressStyle={{ opacity: 0.5 }}
      >
        <Typography color="$purple5" textDecorationLine="underline">
          {t("actions.forgot_password")}
        </Typography>
      </View>
      <Typography fontSize={"$1"} style={{ position: "absolute", bottom: 0 }}>
        {`v${Constants.expoConfig?.version}(${Constants.expoConfig?.extra?.updateVersion}) `}
        {process.env.EXPO_PUBLIC_ENVIRONMENT !== "production"
          ? process.env.EXPO_PUBLIC_ENVIRONMENT
          : ""}
      </Typography>
    </View>
  );
};

const Header = () => {
  const insets = useSafeAreaInsets();
  const StyledWrapper = styled(XStack, {
    name: "StyledWrapper",
    backgroundColor: "$purple5",
    height: "20%",
    paddingTop: insets.top,
    alignItems: "center",
    justifyContent: "center",
  });

  return (
    <StyledWrapper>
      <StatusBar barStyle="light-content"></StatusBar>
      <Icon icon="loginLogo" />
    </StyledWrapper>
  );
};

const CredentialsError = () => {
  const { t } = useTranslation("login");
  return (
    <XStack
      backgroundColor="$red1"
      borderRadius={6}
      justifyContent="center"
      padding="$md"
      alignItems="flex-start"
    >
      <Icon icon="loginError" size={20} />
      <Typography paddingHorizontal="$md" color="$red6" fontWeight="500">
        {t("errors.credentials")}
      </Typography>
    </XStack>
  );
};

export default Login;
