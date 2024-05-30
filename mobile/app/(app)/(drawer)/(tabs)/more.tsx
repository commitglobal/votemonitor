import React, { useState } from "react";
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
import SelectAppLanguage from "../../../../components/SelectAppLanguage";
import i18n from "../../../../common/config/i18n";
import { useNotification } from "../../../../hooks/useNotifications";
import { useUserData } from "../../../../contexts/user/UserContext.provider";
import { ASYNC_STORAGE_KEYS } from "../../../../common/constants";
import { useNetInfoContext } from "../../../../contexts/net-info-banner/NetInfoContext";
import WarningDialog from "../../../../components/WarningDialog";
import FeedbackSheet from "../../../../components/FeedbackSheet";
import OptionsSheet from "../../../../components/OptionsSheet";
import Toast from "react-native-toast-message";

interface MenuItemProps {
  label: string;
  helper?: string;
  icon: string;
  chevronRight?: boolean;
  onClick?: () => void;
}

const MenuItem = ({ label, helper, icon, chevronRight, onClick }: MenuItemProps) => (
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
  const navigation = useNavigation();
  const queryClient = useQueryClient();
  const [isLanguageSelectSheetOpen, setIsLanguageSelectSheetOpen] = React.useState(false);
  const [logoutLoading, setLogoutLoading] = useState(false);
  const [feedbackSheetOpen, setFeedbackSheetOpen] = useState(false);
  const [optionsSheetOpen, setOptionsSheetOpen] = useState(false);
  const { activeElectionRound } = useUserData();
  const { isOnline } = useNetInfoContext();

  const [showWarningModal, setShowWarningModal] = useState(false);

  const { unsubscribe: unsubscribePushNotifications } = useNotification();

  const { t } = useTranslation(["more", "languages", "common"]);
  const { signOut } = useAuth();

  const appVersion = Constants.expoConfig?.version;
  const URL = "https://www.code4.ro/ro/privacy-policy-vote-monitor";

  const { data: currentUser } = useQuery({
    queryKey: [ASYNC_STORAGE_KEYS.CURRENT_USER_STORAGE_KEY],
    queryFn: () => AsyncStorage.getItem(ASYNC_STORAGE_KEYS.CURRENT_USER_STORAGE_KEY),
    staleTime: 0,
  });

  const logout = async () => {
    setLogoutLoading(true);
    await unsubscribePushNotifications();
    signOut(queryClient);
    setLogoutLoading(false);
    setShowWarningModal(false);
  };

  const onFeedbackPress = () => {
    //don't allow adding feedback while offline -> we won't open the feedback sheet, just display error toast
    if (!isOnline) {
      return Toast.show({
        type: "error",
        text2: t("feedback_toast.offline"),
      });
    }
    // display feedback sheet to enable adding feedback
    return setFeedbackSheetOpen(true);
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
        title={activeElectionRound?.title}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="menuAlt2" color="white" />}
        onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
        rightIcon={<Icon icon="dotsVertical" color="white" />}
        onRightPress={() => setOptionsSheetOpen(true)}
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
          onClick={() => router.push("/about-votemonitor")}
        ></MenuItem>
        <MenuItem label={t("support")} icon="contactNGO"></MenuItem>
        <MenuItem
          label={t("change-password")}
          icon="changePassword"
          chevronRight={true}
          onClick={() => router.push("/change-password")}
        ></MenuItem>
        <MenuItem label={t("feedback")} icon="feedback" onClick={onFeedbackPress}></MenuItem>
        <MenuItem
          label={t("logout")}
          icon="logoutNoBackground"
          onClick={() => {
            !isOnline ? setShowWarningModal(true) : logout();
          }}
          helper={currentUser ? t("logged_in", { user: currentUser }) : ""}
        ></MenuItem>
      </YStack>
      <FeedbackSheet open={feedbackSheetOpen} setOpen={setFeedbackSheetOpen} />
      <OptionsSheet open={optionsSheetOpen} setOpen={setOptionsSheetOpen}>
        <YStack
          paddingVertical="$xxs"
          paddingHorizontal="$sm"
          onPress={() => {
            setOptionsSheetOpen(false);
            router.push("/manage-polling-stations");
          }}
        >
          <Typography preset="body1" color="$gray7" lineHeight={24}>
            {t("options_sheet.manage_my_polling_stations")}
          </Typography>
        </YStack>
      </OptionsSheet>
      {/* 
          This element is controlled via the MenuItem change-language component.
          It is visible only when open===true as a bottom sheet. 
          Otherwise, no select element is rendered.
         */}
      <SelectAppLanguage open={isLanguageSelectSheetOpen} setOpen={setIsLanguageSelectSheetOpen} />
      {showWarningModal && (
        <WarningDialog
          title={t("warning_modal.logout.title")}
          description={t("warning_modal.logout.description")}
          actionBtnText={
            logoutLoading ? t("loading", { ns: "common" }) : t("warning_modal.logout.action")
          }
          cancelBtnText={t("warning_modal.logout.cancel")}
          action={logout}
          onCancel={() => setShowWarningModal(false)}
          actionBtnStyle={{ backgroundColor: "hsl(272, 56%, 45%)" }}
        />
      )}
    </Screen>
  );
};

export default More;
