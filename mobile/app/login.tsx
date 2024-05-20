import React, { useEffect, useRef, useState } from "react";
import { router } from "expo-router";
import { useAuth } from "../hooks/useAuth";
import { View, XStack, YStack, styled } from "tamagui";
import { useTranslation } from "react-i18next";
import { Screen } from "../components/Screen";
import { StatusBar, Animated, StyleSheet } from "react-native";
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
import * as SecureStore from "expo-secure-store";
import ChooseOnboardingLanguage from "../components/ChooseOnboardingLanguage";
import OnboardingViewPager from "../components/OnboardingViewPager";
import Pagination from "../components/Pagination";
import CredentialsError from "../components/CredentialsError";

interface FormData {
  email: string;
  password: string;
}

const Login = () => {
  const { t } = useTranslation("login");
  const insets = useSafeAreaInsets();

  const { signIn } = useAuth();
  const [authError, setAuthError] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [languageSelectionApplied, setLanguageSelectionApplied] = useState(false);
  const [onboardingComplete, setOnboardingComplete] = useState(true);

  const scrollOffsetAnimatedValue = React.useRef(new Animated.Value(0)).current;
  const positionAnimatedValue = React.useRef(new Animated.Value(0)).current;
  const [currentPage, setCurrentPage] = useState(0);
  const pagerViewRef = useRef(null);

  useEffect(() => {
    try {
      const onboardingComplete = SecureStore.getItem("onboardingComplete");
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
  const { handleSubmit, control, formState } = useForm<FormData>();
  const { errors } = formState;

  const onOnboardingComplete = () => {
    try {
      SecureStore.setItem("onboardingComplete", "true");
      setOnboardingComplete(true);
    } catch (err) {
      console.log(err);
      Sentry.captureException(err);
    }
  };

  //todo: refactor this (nr of pages in the view pager) @luciatugui
  const data = [1, 2, 3];

  if (!onboardingComplete) {
    if (!languageSelectionApplied) {
      return <ChooseOnboardingLanguage setLanguageSelectionApplied={setLanguageSelectionApplied} />;
    }
    return (
      <>
        <OnboardingViewPager
          scrollOffsetAnimatedValue={scrollOffsetAnimatedValue}
          positionAnimatedValue={positionAnimatedValue}
          pagerViewRef={pagerViewRef}
          currentPage={currentPage}
          setCurrentPage={setCurrentPage}
        />

        <XStack
          justifyContent="center"
          alignItems="center"
          backgroundColor="$purple6"
          paddingHorizontal="$md"
          paddingBottom={insets.bottom + 32}
        >
          <XStack flex={1}></XStack>
          <XStack justifyContent="center" flex={1}>
            <Pagination
              scrollOffsetAnimatedValue={scrollOffsetAnimatedValue}
              positionAnimatedValue={positionAnimatedValue}
              data={data}
            />
          </XStack>

          {currentPage !== data.length - 1 ? (
            <XStack
              onPress={() => {
                // @ts-ignore
                currentPage !== data.length - 1 && pagerViewRef?.current?.setPage(currentPage + 1);
              }}
              pressStyle={{ opacity: 0.5 }}
              flex={1}
              justifyContent="flex-end"
            >
              <Typography color="white" preset="body2" paddingVertical="$xs" paddingRight="$md">
                {t("onboarding.skip")}
              </Typography>
            </XStack>
          ) : (
            <XStack
              onPress={() => onOnboardingComplete()}
              pressStyle={{ opacity: 0.5 }}
              flex={1}
              justifyContent="flex-end"
            >
              <Typography
                color="white"
                preset="body2"
                paddingVertical="$xs"
                paddingRight="$md"
                textAlign="center"
              >
                {/* //!this might cause problems if the translation is too long */}
                {t("onboarding.go_to_app")}
              </Typography>
            </XStack>
          )}
        </XStack>
      </>
    );
  }

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

      {authError && <CredentialsError error={t("errors.credentials")} />}

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

export default Login;
