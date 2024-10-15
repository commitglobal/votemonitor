import React from "react";
import { router } from "expo-router";
import { Screen } from "../../../components/Screen";
import Header from "../../../components/Header";
import { Icon } from "../../../components/Icon";
import { Typography } from "../../../components/Typography";
import { Image } from "expo-image";
import { useTranslation } from "react-i18next";
import { ScrollView, styled, View, XStack, YStack } from "tamagui";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import Constants from "expo-constants";

const AboutVoteMonitor = () => {
  const { t } = useTranslation("about_app");
  const insets = useSafeAreaInsets();
  const appVersion = Constants.expoConfig?.version;

  return (
    <Screen preset="fixed" contentContainerStyle={{ flexGrow: 1 }}>
      <Header
        title={t("title")}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      >
        <Icon icon="loginLogo" paddingVertical="$md" />
      </Header>

      <YStack flex={1} paddingBottom={insets.bottom + 24}>
        <ScrollView
          showsVerticalScrollIndicator={false}
          contentContainerStyle={{ flexGrow: 1, paddingHorizontal: 24 }}
        >
          <YStack gap="$lg">
            <Typography preset="subheading" fontWeight="500" paddingTop="$lg">
              {t("title_observer")}
            </Typography>

            <Typography preset="body1">{t("general")}</Typography>
            <Typography preset="body1">{t("purpose")}</Typography>

            <Typography preset="body1">{t("app_provides.paragraph")}</Typography>
            <YStack marginLeft="$xs" gap="$xxs">
              <ListItem text={t("app_provides.item1")} />
              <ListItem text={t("app_provides.item2")} />
              <ListItem text={t("app_provides.item3")} />
            </YStack>

            <Typography preset="body1">{t("usage")}</Typography>

            <Typography preset="body1" color="$gray3">
              {t("version", { value: appVersion })} ({Constants.expoConfig?.extra?.updateVersion})
            </Typography>
          </YStack>

          <XStack justifyContent="center" alignItems="center" minHeight={115} marginBottom="$lg">
            <Image
              source={require("../../../assets/images/commit-global-color.png")}
              contentFit="contain"
              style={{ width: "100%", height: "100%" }}
            />
          </XStack>
        </ScrollView>
      </YStack>
    </Screen>
  );
};

export default AboutVoteMonitor;

const Bullet = styled(View, {
  width: 5,
  height: 5,
  marginTop: 10,
  backgroundColor: "$gray9",
  borderRadius: 50,
});

const ListItem = ({ text }: { text: string }) => {
  return (
    <XStack gap="$md">
      <Bullet />
      <Typography preset="body1">{text}</Typography>
    </XStack>
  );
};
