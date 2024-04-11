import React from "react";
import { View } from "react-native";
import { router } from "expo-router";
import { useAuth } from "../hooks/useAuth";
import { Button as TamaguiButton } from "tamagui";
import { Typography } from "../components/Typography";
import { useTranslation } from "react-i18next";
import { useLanguage } from "../contexts/language/LanguageContext.provider";
import Button from "../components/Button";

const Login = () => {
  const { signIn } = useAuth();
  const { t } = useTranslation("login");
  const { changeLanguage } = useLanguage();

  const switchToEnglish = () => {
    changeLanguage("en");
  };

  const switchToRomanian = () => {
    changeLanguage("ro");
  };

  const onLogin = async () => {
    try {
      await signIn();
      router.replace("/");
    } catch (err) {
      console.error("Error while logging in...");
    }
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
      <TamaguiButton onPress={onLogin}>
        <Typography>{t("submit")}</Typography>
      </TamaguiButton>
      <Button onPress={switchToEnglish}>English</Button>
      <Button onPress={switchToRomanian}>Romanian</Button>
    </View>
  );
};

export default Login;
