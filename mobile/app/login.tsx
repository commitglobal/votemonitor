import React, { useEffect, useRef, useState } from "react";
import { SplashScreen, router } from "expo-router";
import { useAuth } from "../hooks/useAuth";
import { ScrollView, View, XStack, YStack } from "tamagui";
import { useTranslation } from "react-i18next";
import { Screen } from "../components/Screen";
import { Animated, Keyboard, Image } from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { Icon } from "../components/Icon";
import { Typography } from "../components/Typography";
import FormInput from "../components/FormInputs/FormInput";
import { Control, Controller, FieldErrors, FieldValues, useForm } from "react-hook-form";
import AsyncStorage from "@react-native-async-storage/async-storage";
import { ASYNC_STORAGE_KEYS, SECURE_STORAGE_KEYS } from "../common/constants";
import Constants from "expo-constants";
import * as Sentry from "@sentry/react-native";
import * as SecureStore from "expo-secure-store";
import ChooseOnboardingLanguage from "../components/ChooseOnboardingLanguage";
import OnboardingViewPager from "../components/OnboardingViewPager";
import CredentialsError from "../components/CredentialsError";
import Toast from "react-native-toast-message";
import { useNetInfoContext } from "../contexts/net-info-banner/NetInfoContext";
import Header from "../components/Header";
import * as Clipboard from "expo-clipboard";
import WizzardControls from "../components/WizzardControls";

interface FormData {
  email: string;
  password: string;
}

const Login = () => {
  const { t } = useTranslation(["login", "common", "onboarding"]);
  const insets = useSafeAreaInsets();

  const { signIn } = useAuth();
  const [authError, setAuthError] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [languageSelectionApplied, setLanguageSelectionApplied] = useState(false);
  const [onboardingComplete, setOnboardingComplete] = useState(true);
  const { isOnline } = useNetInfoContext();

  const scrollOffsetAnimatedValue = React.useRef(new Animated.Value(0)).current;
  const positionAnimatedValue = React.useRef(new Animated.Value(0)).current;
  const [currentPage, setCurrentPage] = useState(0);
  const pagerViewRef = useRef(null);

  useEffect(() => {
    SplashScreen.hideAsync();

    try {
      const onboardingComplete = SecureStore.getItem(SECURE_STORAGE_KEYS.ONBOARDING_COMPLETE);
      if (onboardingComplete !== "true") {
        setOnboardingComplete(false);
      }
    } catch (err) {
      Sentry.captureException(err);
    }
  }, []);

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
  const { handleSubmit, control, formState } = useForm<FormData>();
  const { errors } = formState;

  const onOnboardingComplete = () => {
    try {
      SecureStore.setItem(SECURE_STORAGE_KEYS.ONBOARDING_COMPLETE, "true");
      setOnboardingComplete(true);
    } catch (err) {
      console.log("err", err);
      Sentry.captureException(err);
    }
  };

  const onNextButtonPress = () => {
    if (currentPage !== data.length - 1) {
      // @ts-ignore
      pagerViewRef?.current?.setPage(currentPage + 1);
    } else {
      onOnboardingComplete();
    }
  };

  // todo: refactor this (nr of pages in the view pager) @luciatugui
  const data = [1, 2, 3];

  const copyToClipboard = async () => {
    await Clipboard.setStringAsync(t("disclaimer.email"));
    return Toast.show({
      type: "success",
      text2: t("email_toast"),
      visibilityTime: 5000,
      text2Style: { textAlign: "center" },
    });
  };

  if (onboardingComplete) {
    return (
      <Screen
        preset="fixed"
        contentContainerStyle={{
          flexGrow: 1,
          flex: 1,
        }}
      >
        <Header barStyle="light-content">
          <Icon icon="loginLogo" paddingBottom="$md" />
        </Header>

        <ScrollView>
          <YStack padding="$md" gap="$md">
            <LoginForm control={control} errors={errors} authError={authError} />

            {/* info text */}
            <XStack marginTop="$md" justifyContent="flex-start" gap="$xxs">
              <Icon icon="infoCircle" size={18} color="white" style={{ marginTop: 2 }} />

              <YStack gap="$lg" maxWidth="90%">
                <Typography>{t("disclaimer.paragraph1")}</Typography>
                <Typography>
                  {t("disclaimer.paragraph2")}{" "}
                  <Typography color="$purple5" onPress={copyToClipboard}>
                    {t("disclaimer.email")}
                  </Typography>
                  .
                </Typography>
              </YStack>
            </XStack>

            {/* commit global logo */}
            <XStack justifyContent="center" alignItems="center" minHeight={115}>
              <Image
                source={require("../assets/images/commit-global-color.png")}
                resizeMode="contain"
                style={{ width: "100%", height: "100%" }}
              />
            </XStack>
          </YStack>
        </ScrollView>

        <WizzardControls
          isFirstElement
          onActionButtonPress={() => {
            Keyboard.dismiss();
            handleSubmit(onLogin)();
          }}
          actionBtnLabel={isLoading ? t("form.submit.loading") : t("form.submit.save")}
          isNextDisabled={isLoading}
        />
      </Screen>
    );
  }

  if (!languageSelectionApplied) {
    return <ChooseOnboardingLanguage setLanguageSelectionApplied={setLanguageSelectionApplied} />;
  }

  return (
    <Screen preset="fixed">
      <OnboardingViewPager
        scrollOffsetAnimatedValue={scrollOffsetAnimatedValue}
        positionAnimatedValue={positionAnimatedValue}
        pagerViewRef={pagerViewRef}
        currentPage={currentPage}
        setCurrentPage={setCurrentPage}
      />

      <XStack
        backgroundColor="$purple6"
        padding="$md"
        paddingBottom={16}
        position="absolute"
        bottom={insets.bottom}
        justifyContent="center"
        width="100%"
      >
        {/* <Pagination
          scrollOffsetAnimatedValue={scrollOffsetAnimatedValue}
          positionAnimatedValue={positionAnimatedValue}
          data={data}
          currentPage={currentPage + 1}
        /> */}
        <YStack
          width="100%"
          alignItems="flex-end"
          padding="$xxs"
          pressStyle={{ opacity: 0.5 }}
          onPress={onNextButtonPress}
        >
          <Typography color="white" preset="body2" textAlign="center">
            {currentPage !== data.length - 1
              ? t("skip", { ns: "common" })
              : t("media.save", { ns: "onboarding" })}
          </Typography>
        </YStack>
      </XStack>
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
