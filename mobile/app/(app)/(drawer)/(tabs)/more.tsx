import React from "react";
import { XStack, YStack } from "tamagui";
import Card from "../../../../components/Card";
import { Screen } from "../../../../components/Screen";
import { Typography } from "../../../../components/Typography";
import { Icon } from "../../../../components/Icon";
import { useTranslation } from "react-i18next";
import * as Linking from "expo-linking";
import { router, useNavigation } from "expo-router";
import { useAuth } from "../../../../hooks/useAuth";
import Header from "../../../../components/Header";
import { DrawerActions } from "@react-navigation/native";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import Constants from "expo-constants";
import AsyncStorage from "@react-native-async-storage/async-storage";
import { CURRENT_USER_STORAGE_KEY } from "../../../../common/constants";
import SelectAppLanguage from "../../../../components/SelectAppLanguage";
import i18n from "../../../../common/config/i18n";
import { useNotification } from "../../../../hooks/useNotifications";

interface MenuItemProps {
  label: string;
  helper?: string;
  icon: string;
  chevronRight?: boolean;
  onClick?: () => void;
}

const MenuItem = ({ label, helper, icon, chevronRight, onClick }: MenuItemProps) => (
  <Card onPress={onClick}>
    <XStack alignItems="center" justifyContent="space-between" gap="$xxxs">
      <XStack alignItems="center" gap="$xxs" maxWidth="80%">
        <Icon size={24} icon={icon} color="black" />
        <YStack alignContent="center" gap="$xxxs">
          <Typography preset="body2">{label} </Typography>
          {helper && <Typography color="$gray8">{helper}</Typography>}
        </YStack>
      </XStack>

      {chevronRight && <Icon size={32} icon="chevronRight" color="$purple7" />}
    </XStack>
  </Card>
);

const More = () => {
  const navigation = useNavigation();
  const queryClient = useQueryClient();
  const [isLanguageSelectSheetOpen, setIsLanguageSelectSheetOpen] = React.useState(false);

  const { unsubscribe: unsubscribePushNotifications } = useNotification();

  const { t } = useTranslation(["more", "languages"]);
  const { signOut } = useAuth();

  const appVersion = Constants.expoConfig?.version;
  const URL = "https://www.code4.ro/ro/privacy-policy-vote-monitor";

  const { data: currentUser } = useQuery({
    queryKey: [CURRENT_USER_STORAGE_KEY],
    queryFn: () => AsyncStorage.getItem(CURRENT_USER_STORAGE_KEY),
  });

  const logout = async () => {
    await unsubscribePushNotifications();
    signOut(queryClient);
  };

  return (
    <Screen
      preset="auto"
      backgroundColor="white"
      ScrollViewProps={{
        stickyHeaderIndices: [0],
        bounces: false,
        showsVerticalScrollIndicator: false,
      }}
    >
      <Header
        title={"More"}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="menuAlt2" color="white" />}
        onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
      />
      <YStack paddingHorizontal="$md" paddingVertical="$xl" gap="$md">
        <MenuItem
          label={t("change-language")}
          icon="language"
          onClick={setIsLanguageSelectSheetOpen.bind(null, true)}
          helper={t(i18n.language, { ns: "languages" })}
        ></MenuItem>
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
          helper={`${t("app_version", { value: appVersion })} (${Constants.expoConfig?.extra?.updateVersion}) ${
            process.env.EXPO_PUBLIC_ENVIRONMENT !== "production"
              ? process.env.EXPO_PUBLIC_ENVIRONMENT
              : ""
          }`}
          icon="aboutVM"
          chevronRight={true}
        ></MenuItem>
        <MenuItem label={t("support")} icon="contactNGO"></MenuItem>
        <MenuItem
          label={t("change-password")}
          icon="changePassword"
          chevronRight={true}
          onClick={() => router.push("/change-password")}
        ></MenuItem>
        <MenuItem label={t("feedback")} icon="feedback"></MenuItem>
        <MenuItem
          label={t("logout")}
          icon="logoutNoBackground"
          onClick={logout}
          helper={currentUser ? `Logged in as ${currentUser}` : ""}
        ></MenuItem>
      </YStack>
      {/* 
          This element is controlled via the MenuItem change-language component.
          It is visible only when open===true as a bottom sheet. 
          Otherwise, no select element is rendered.
         */}
      <SelectAppLanguage open={isLanguageSelectSheetOpen} setOpen={setIsLanguageSelectSheetOpen} />
    </Screen>
  );
};

export default More;
