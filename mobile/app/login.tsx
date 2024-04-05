import React, { useContext } from "react";
import { View } from "react-native";
import { router } from "expo-router";
import { useAuth } from "../hooks/useAuth";
import { Button as TamaguiButton } from "tamagui";
import { Typography } from "../components/Typography";
import Card from "../components/Card";
import { useIsRestoring } from "@tanstack/react-query";
import { useTranslation } from "react-i18next";
import { LanguageContext } from "../contexts/language/LanguageContext.provider";
import Button from "../components/Button";

const Login = () => {
  // https://tanstack.com/query/latest/docs/framework/react/plugins/persistQueryClient#useisrestoring
  const isRestoring = useIsRestoring();
  console.log("isRestoring persistQueryClient", isRestoring);

  const { signIn } = useAuth();
  const { t } = useTranslation("login");
  const { changeLanguage } = useContext(LanguageContext);

  const switchToEnglish = () => {
    changeLanguage("en");
  };

  const switchToRomanian = () => {
    changeLanguage("ro");
  };

  return (
    <View
      style={{
        flex: 1,
        justifyContent: "center",
        alignItems: "center",
        gap: 20,
      }}
    >
      <TamaguiButton
        onPress={() => {
          signIn();
          // Navigate after signing in. You may want to tweak this to ensure sign-in is
          // successful before navigating.
          router.replace("/");
        }}
      >
        <Typography>{t("submit")}</Typography>
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
      </TamaguiButton>
      <Button onPress={switchToEnglish}>English</Button>
      <Button onPress={switchToRomanian}>Romanian</Button>
    </View>
  );
};

export default Login;
