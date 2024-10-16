import React, { useState } from "react";
import { XStack, YStack } from "tamagui";
import Card from "../../../../../components/Card";
import { Screen } from "../../../../../components/Screen";
import { Typography } from "../../../../../components/Typography";
import { Icon } from "../../../../../components/Icon";
import { useTranslation } from "react-i18next";
import * as Linking from "expo-linking";
import { router, useNavigation } from "expo-router";
import Header from "../../../../../components/Header";
import { DrawerActions } from "@react-navigation/native";
import Constants from "expo-constants";
import SelectAppLanguage from "../../../../../components/SelectAppLanguage";
import i18n from "../../../../../common/config/i18n";
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

  const navigation = useNavigation();

  const [isLanguageSelectSheetOpen, setIsLanguageSelectSheetOpen] = React.useState(false);
  const [feedbackSheetOpen, setFeedbackSheetOpen] = useState(false);

  const appVersion = Constants.expoConfig?.version;
  const URL = "https://www.code4.ro/ro/privacy-policy-vote-monitor";

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
          onClick={() => router.push("/citizen/main/about-votemonitor")}
        ></MoreMenuItem>
        {/* <MoreMenuItem
          label={t("feedback")}
          icon="feedback"
          onClick={() => setFeedbackSheetOpen(true)}
        ></MoreMenuItem> */}
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
    </Screen>
  );
};

export default More;
