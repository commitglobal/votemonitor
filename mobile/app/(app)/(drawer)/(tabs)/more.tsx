import React from "react";
import { View, XStack } from "tamagui";
import Card from "../../../../components/Card";
import { Screen } from "../../../../components/Screen";
import { Typography } from "../../../../components/Typography";
import { Icon } from "../../../../components/Icon";
import { useTranslation } from "react-i18next";
import * as Linking from "expo-linking";
import { tokens } from "../../../../theme/tokens";
import { router } from "expo-router";
import { useAuth } from "../../../../hooks/useAuth";

const More = () => {
  const { t } = useTranslation("more");

  const { signOut } = useAuth();

  // TODO: Change these consts
  const appVersion = "2.0.4";
  const appLanguage = "English (United States)";
  const URL = "https://www.google.com/";

  return (
    <Screen
      preset="auto"
      backgroundColor="white"
      contentContainerStyle={{
        gap: tokens.space.md.val,
        paddingHorizontal: tokens.space.md.val,
        paddingVertical: tokens.space.xl.val,
      }}
    >
      <MenuItem
        label={t("change-language")}
        helper={appLanguage}
        icon="language"
        chevronRight={true}
        onClick={() => router.push("/change-language")}
      ></MenuItem>
      <MenuItem label={t("change-password")} icon="changePassword" chevronRight={true}></MenuItem>
      <MenuItem
        label={t("terms")}
        icon="termsConds"
        chevronRight={true}
        onClick={() => {
          Linking.openURL(URL);
        }}
      ></MenuItem>
      <MenuItem
        label={t("privacy_policy")}
        icon="privacyPolicy"
        chevronRight={true}
        onClick={() => {
          Linking.openURL(URL);
        }}
      ></MenuItem>
      <MenuItem
        label={t("about")}
        helper={t("app_version", { value: appVersion })}
        icon="aboutVM"
        chevronRight={true}
      ></MenuItem>
      <MenuItem label={t("support")} icon="contactNGO"></MenuItem>
      <MenuItem label={t("feedback")} icon="feedback"></MenuItem>
      <MenuItem label={t("logout")} icon="logoutNoBackground" onClick={signOut}></MenuItem>
    </Screen>
  );
};

interface MenuItemProps {
  label: string;
  helper?: string;
  icon: string;
  chevronRight?: boolean;
  onClick?: () => void;
}

const MenuItem = ({ label, helper, icon, chevronRight, onClick }: MenuItemProps) => (
  <Card onPress={onClick}>
    <XStack alignItems="center" justifyContent="space-between">
      <XStack alignItems="center" gap="$xxs">
        <Icon size={24} icon={icon} color="black" />
        <View alignContent="center" gap="$xxxs">
          <Typography preset="body2"> {label} </Typography>
          {helper && <Typography color="$gray8"> {helper}</Typography>}
        </View>
      </XStack>

      {chevronRight && <Icon size={32} icon="chevronRight" color="$purple7" />}
    </XStack>
  </Card>
);

export default More;
