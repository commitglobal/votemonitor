import React, { useState } from "react";
import { SplashScreen, router } from "expo-router";
import { useAuth } from "../../hooks/useAuth";
import { ScrollView, View, XStack, YStack } from "tamagui";
import { useTranslation } from "react-i18next";
import { Screen } from "../../components/Screen";
import { Keyboard } from "react-native";
import { Image } from "expo-image";
import { Icon } from "../../components/Icon";
import { Typography } from "../../components/Typography";
import FormInput from "../../components/FormInputs/FormInput";
import { Control, Controller, FieldErrors, FieldValues, useForm } from "react-hook-form";
import AsyncStorage from "@react-native-async-storage/async-storage";
import { ASYNC_STORAGE_KEYS } from "../../common/constants";
import Constants from "expo-constants";
import * as Sentry from "@sentry/react-native";
import CredentialsError from "../../components/CredentialsError";
import Toast from "react-native-toast-message";
import { useNetInfoContext } from "../../contexts/net-info-banner/NetInfoContext";
import Header from "../../components/Header";
import { FooterButtons } from "../../components/FooterButtons";

interface FormData {
  email: string;
  password: string;
}

const Login = () => {
  setTimeout(() => {
    SplashScreen.hideAsync();
  }, 500);

  const { t } = useTranslation(["login", "common", "onboarding"]);

  const { signIn } = useAuth();
  const [authError, setAuthError] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const { isOnline } = useNetInfoContext();

  const { handleSubmit, control, formState } = useForm<FormData>();
  const { errors } = formState;

  const onLogin = async (formData: FormData) => {
    try {
      setIsLoading(true);
      const email = formData.email.trim().toLocaleLowerCase();
      const password = formData.password.trim();

      if (!isOnline) {
        return Toast.show({
          type: "error",
          text2: t("form.errors.offline"),
          visibilityTime: 5000,
          text2Style: { textAlign: "center" },
        });
      }
      await signIn(email, password);
      await AsyncStorage.setItem(ASYNC_STORAGE_KEYS.CURRENT_USER_STORAGE_KEY, email);
      router.replace("/");
    } catch (err) {
      setAuthError(true);
      Sentry.captureException(err);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Screen
      preset="fixed"
      contentContainerStyle={{
        flexGrow: 1,
        flex: 1,
      }}
    >
      <Header barStyle="light-content">
        <Icon icon="vmObserverLogo" width={294} height={83} paddingBottom="$md" />
      </Header>

      <ScrollView>
        <YStack padding="$md" gap="$md">
          <LoginForm control={control} errors={errors} authError={authError} />

          {/* info text */}
          <XStack marginTop="$md" justifyContent="flex-start" gap="$xxs">
            <Icon icon="infoCircle" color="black" size={18} style={{ marginTop: 2 }} />

            <YStack gap="$lg" maxWidth="90%">
              <Typography>{t("disclaimer.paragraph1")}</Typography>
              <Typography>
                {t("disclaimer.paragraph2.slice1")}{" "}
                <Typography
                  color="$purple5"
                  textDecorationLine="underline"
                  onPress={() => router.push("/citizen/select-election-rounds")}
                  pressStyle={{ opacity: 0.5 }}
                >
                  {t("disclaimer.paragraph2.link")}{" "}
                </Typography>
                {t("disclaimer.paragraph2.slice2")}
              </Typography>
            </YStack>
          </XStack>

          {/* commit global logo */}
          <XStack justifyContent="center" alignItems="center" minHeight={115}>
            <Image
              source={require("../../assets/images/commit-global-color.png")}
              style={{ width: "100%", height: "100%" }}
              contentFit="contain"
            />
          </XStack>
        </YStack>
      </ScrollView>

      <FooterButtons
        primaryAction={() => {
          Keyboard.dismiss();
          handleSubmit(onLogin)();
        }}
        primaryActionLabel={isLoading ? t("form.submit.loading") : t("form.submit.save")}
        isPrimaryButtonDisabled={isLoading}
        handleGoBack={() => router.push("/select-app-mode")}
      />
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
        {t("heading")}
      </Typography>

      <Typography>{t("paragraph")}</Typography>

      {authError && <CredentialsError error={t("form.errors.invalid_credentials")} />}

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
            message: t("form.email.pattern"),
          },
        }}
        render={({ field: { onChange, value } }) => (
          <FormInput
            type="email-address"
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
            autoCapitalize="none"
            autoCorrect={false}
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

export default Login;
