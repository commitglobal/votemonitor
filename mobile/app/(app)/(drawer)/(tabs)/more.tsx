import React, { useState, useMemo } from "react";
import { View, XStack, YStack, Sheet, Adapt, Select, styled, Input } from "tamagui";
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
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { Keyboard } from "react-native";

const languages = [
  "Romanian",
  "English",
  "French",
  "German",
  "Spanish",
  "Italian",
  "Portuguese",
  "Russian",
  "Romanian",
  "English",
  "French",
  "German",
  "Spanish",
  "Italian",
  "Portuguese",
  "Russian",
];

const More = () => {
  const navigation = useNavigation();
  const queryClient = useQueryClient();
  const [openLanguageSheet, setOpenLanguageSheet] = React.useState(false);
  const [language, setLanguage] = useState("");

  const { t } = useTranslation("more");
  const { signOut } = useAuth();

  // TODO: Change these consts
  const appVersion = Constants.expoConfig?.version;
  const URL = "https://www.google.com/";

  const { data: currentUser } = useQuery({
    queryKey: [CURRENT_USER_STORAGE_KEY],
    queryFn: () => AsyncStorage.getItem(CURRENT_USER_STORAGE_KEY),
  });

  const logout = () => {
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
        {/* 
          This element is controlled via the MenuItem change-language component.
          It is visible only when open===true as a bottom sheet. 
          Otherwise, no select element is rendered.
         */}
        <SelectLanguage
          open={openLanguageSheet}
          setOpen={setOpenLanguageSheet}
          options={languages}
          language={language}
          setLanguage={setLanguage}
        />

        <MenuItem
          label={t("change-language")}
          icon="language"
          onClick={() => {
            setOpenLanguageSheet(!openLanguageSheet);
          }}
          helper={language}
        ></MenuItem>

        <MenuItem
          label={t("change-password")}
          icon="changePassword"
          chevronRight={true}
          onClick={() => router.push("/forgot-password")}
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
          helper={t("app_version", { value: appVersion })}
          icon="aboutVM"
          chevronRight={true}
        ></MenuItem>
        <MenuItem label={t("support")} icon="contactNGO"></MenuItem>
        <MenuItem label={t("feedback")} icon="feedback"></MenuItem>
        <MenuItem
          label={t("logout")}
          icon="logoutNoBackground"
          onClick={logout}
          helper={currentUser ? `Logged in as ${currentUser}` : ""}
        ></MenuItem>
      </YStack>
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

interface SelectLanguageProps {
  open: boolean;
  setOpen: (state: boolean) => void;
  options: string[];
  language: string;
  setLanguage: (state: string) => void;
}

const SelectLanguage = (props: SelectLanguageProps) => {
  const { open, setOpen, options, language, setLanguage } = props;
  console.log("language: ", language);

  return (
    <Select open={open} onOpenChange={setOpen} onValueChange={setLanguage}>
      <Adapt platform="touch">
        <Sheet
          native
          modal
          snapPoints={[40]}
          open={open}
          moveOnKeyboardChange={open || Keyboard.isVisible()}
        >
          <Sheet.Frame>
            <Sheet.ScrollView paddingHorizontal="$sm">
              <Adapt.Contents />
            </Sheet.ScrollView>
          </Sheet.Frame>
          <Sheet.Overlay />
        </Sheet>
      </Adapt>

      <Select.Content>
        <Select.Viewport>
          <Select.Group>
            {useMemo(
              () =>
                options?.map((entry, i) => {
                  return (
                    <Select.Item
                      index={i}
                      key={`${entry}_${i}`}
                      value={entry}
                      gap="$3"
                      paddingBottom="$sm"
                    >
                      <Select.ItemText>{entry}</Select.ItemText>
                      <Select.ItemIndicator>
                        <Icon icon="chevronLeft" />
                      </Select.ItemIndicator>
                    </Select.Item>
                  );
                }),
              [options, language],
            )}
          </Select.Group>
        </Select.Viewport>
      </Select.Content>
    </Select>
  );
};

export default More;
