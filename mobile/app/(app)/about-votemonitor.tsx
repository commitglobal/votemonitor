import React from "react";
import { router } from "expo-router";
import { Screen } from "../../components/Screen";
import Header from "../../components/Header";
import { Icon } from "../../components/Icon";
import { ViewStyle } from "react-native";
import { useTranslation } from "react-i18next";
import { styled, View, XStack, YStack } from "tamagui";
import { Typography } from "../../components/Typography";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import Constants from "expo-constants";

const AboutVoteMonitor = () => {
  const { t } = useTranslation("about_app");
  const insets = useSafeAreaInsets();
  const appVersion = Constants.expoConfig?.version;

  return (
    <Screen
      preset="auto"
      backgroundColor="white"
      ScrollViewProps={{
        bounces: false,
      }}
      contentContainerStyle={$containerStyle}
    >
      {/* header */}
      <Header
        title={t("title")}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      >
        <Icon icon="loginLogo" paddingBottom="$md" />
      </Header>

      {/* main content */}
      <YStack padding="$lg" gap="$lg" paddingBottom={insets.bottom}>
        <YStack gap="$lg">
          {/* title */}
          <Typography preset="subheading" fontWeight="500">
            {t("title")}
          </Typography>

          {/* about */}
          <Typography preset="body1">{t("general")}</Typography>
          <Typography preset="body1">{t("purpose")}</Typography>

          {/* bullet list */}
          <Typography preset="body1">{t("app_provides.paragraph")}</Typography>
          <YStack marginLeft="$xs" gap="$xxs">
            <ListItem text={t("app_provides.item1")} />
            <ListItem text={t("app_provides.item2")} />
            <ListItem text={t("app_provides.item3")} />
          </YStack>

          {/* usage */}
          <Typography preset="body1">{t("usage")}</Typography>

          {/* app version */}
          <Typography preset="body1" color="$gray3">
            {t("version", { value: appVersion })} ({Constants.expoConfig?.extra?.updateVersion})
          </Typography>
        </YStack>
        {/* logo */}
        <XStack justifyContent="center" marginBottom="$lg">
          <Icon icon="commitGlobal" />
        </XStack>
      </YStack>
    </Screen>
  );
};

export default AboutVoteMonitor;

const $containerStyle: ViewStyle = {
  flexGrow: 1,
};

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
