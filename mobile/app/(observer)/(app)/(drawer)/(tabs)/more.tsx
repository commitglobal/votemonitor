import React, { useState } from "react";
import { XStack, YStack } from "tamagui";
import Card from "../../../../../components/Card";
import { Screen } from "../../../../../components/Screen";
import { Typography } from "../../../../../components/Typography";
import { Icon } from "../../../../../components/Icon";
import { useTranslation } from "react-i18next";
import * as Linking from "expo-linking";
import { router, useNavigation } from "expo-router";
import { useAuth } from "../../../../../hooks/useAuth";
import Header from "../../../../../components/Header";
import { DrawerActions } from "@react-navigation/native";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import Constants from "expo-constants";
import AsyncStorage from "@react-native-async-storage/async-storage";
import SelectAppLanguage from "../../../../../components/SelectAppLanguage";
import i18n from "../../../../../common/config/i18n";
import { useNotification } from "../../../../../hooks/useNotifications";
import { ASYNC_STORAGE_KEYS } from "../../../../../common/constants";
import { useNetInfoContext } from "../../../../../contexts/net-info-banner/NetInfoContext";
import WarningDialog from "../../../../../components/WarningDialog";
import FeedbackSheet from "../../../../../components/FeedbackSheet";

interface MenuItemProps {
  label: string;
  helper?: string;
  icon: string;
  chevronRight?: boolean;
  onClick?: () => void;
}

export const MoreMenuItem = ({ label, helper, icon, chevronRight, onClick }: MenuItemProps) => (
  <Card onPress={onClick} style={{ minHeight: 64, justifyContent: "center" }}>
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
  const { t } = useTranslation(["more", "languages", "common"]);

  const { isOnline } = useNetInfoContext();
  const { signOut } = useAuth();

  const navigation = useNavigation();
  const queryClient = useQueryClient();

  const [isLanguageSelectSheetOpen, setIsLanguageSelectSheetOpen] = React.useState(false);
  const [logoutLoading, setLogoutLoading] = useState(false);
  const [feedbackSheetOpen, setFeedbackSheetOpen] = useState(false);
  const [showWarningModal, setShowWarningModal] = useState(false);

  const appVersion = Constants.expoConfig?.version;
  const URL = "https://www.code4.ro/ro/privacy-policy-vote-monitor";
  const HOTLINE_URL = "https://www.commitglobal.org/en/elections-callcenters";

  const { data: currentUser } = useQuery({
    queryKey: [ASYNC_STORAGE_KEYS.CURRENT_USER_STORAGE_KEY],
    queryFn: () => AsyncStorage.getItem(ASYNC_STORAGE_KEYS.CURRENT_USER_STORAGE_KEY),
    staleTime: 0,
  });

  const { unsubscribe: unsubscribePushNotifications } = useNotification();

  const logout = async () => {
    setLogoutLoading(true);
    await unsubscribePushNotifications();
    signOut(queryClient);
    setLogoutLoading(false);
    setShowWarningModal(false);
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
        title={t("title")}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="menuAlt2" color="white" />}
        onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
      />
      <YStack paddingHorizontal="$md" paddingVertical="$xl" gap="$md">
        <MoreMenuItem
          label={t("change-language")}
          icon="language"
          onClick={setIsLanguageSelectSheetOpen.bind(null, true)}
          helper={t(i18n.language, { ns: "languages" })}
        ></MoreMenuItem>
        <MoreMenuItem
          label={t("terms")}
          icon="termsConds"
          chevronRight={true}
          onClick={() => {
            Linking.openURL(URL);
          }}
        ></MoreMenuItem>
        <MoreMenuItem
          label={t("privacy_policy")}
          icon="privacyPolicy"
          chevronRight={true}
          onClick={() => {
            Linking.openURL(URL);
          }}
        ></MoreMenuItem>
        <MoreMenuItem
          label={t("about")}
          helper={`${t("app_version", { value: appVersion })} (${Constants.expoConfig?.extra?.updateVersion}) ${
            process.env.EXPO_PUBLIC_ENVIRONMENT !== "production"
              ? process.env.EXPO_PUBLIC_ENVIRONMENT
              : ""
          }`}
          icon="aboutVM"
          chevronRight={true}
          onClick={() => router.push("/about-votemonitor")}
        ></MoreMenuItem>
        <MoreMenuItem
          label={t("support")}
          icon="contactNGO"
          onClick={() => Linking.openURL(HOTLINE_URL)}
        ></MoreMenuItem>
        <MoreMenuItem
          label={t("change-password")}
          icon="changePassword"
          chevronRight={true}
          onClick={() => router.push("/change-password")}
        ></MoreMenuItem>
        <MoreMenuItem
          label={t("feedback")}
          icon="feedback"
          onClick={() => setFeedbackSheetOpen(true)}
        ></MoreMenuItem>
        <MoreMenuItem
          label={t("logout")}
          icon="logoutNoBackground"
          onClick={() => setShowWarningModal(true)}
          helper={currentUser ? t("logged_in", { user: currentUser }) : ""}
        ></MoreMenuItem>
      </YStack>
      {feedbackSheetOpen && <FeedbackSheet open setOpen={setFeedbackSheetOpen} />}
      {/* 
          This element is controlled via the MenuItem change-language component.
          It is visible only when open===true as a bottom sheet. 
          Otherwise, no select element is rendered.
         */}
      {isLanguageSelectSheetOpen && (
        <SelectAppLanguage
          open={isLanguageSelectSheetOpen}
          setOpen={setIsLanguageSelectSheetOpen}
        />
      )}
      {showWarningModal && (
        <WarningDialog
          theme="info"
          title={
            isOnline
              ? t("warning_modal.logout_online.title")
              : t("warning_modal.logout_offline.title")
          }
          titleProps={isOnline && { textAlign: "center" }}
          description={isOnline ? "" : t("warning_modal.logout_offline.description")}
          actionBtnText={
            logoutLoading
              ? t("loading", { ns: "common" })
              : t("warning_modal.logout_offline.action")
          }
          cancelBtnText={t("warning_modal.logout_online.cancel")}
          action={logout}
          onCancel={() => setShowWarningModal(false)}
        />
      )}
    </Screen>
  );
};

export default More;
