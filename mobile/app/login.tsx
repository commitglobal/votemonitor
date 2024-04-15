import React from "react";
import { View } from "react-native";
import { router } from "expo-router";
import { useAuth } from "../hooks/useAuth";
import { Button as TamaguiButton } from "tamagui";
import { Typography } from "../components/Typography";
import { useTranslation } from "react-i18next";

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
      <TamaguiButton onPress={onLogin}>
        <Typography>{t("submit")}</Typography>
      </TamaguiButton>
    </View>
  );
};

export default Login;
