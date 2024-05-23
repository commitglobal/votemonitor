import React from "react";
import { Icon } from "./Icon";
import { YStack } from "tamagui";
import { Typography } from "./Typography";
import { useTranslation } from "react-i18next";
import Header from "./Header";
import { useNavigation } from "expo-router";
import { DrawerActions } from "@react-navigation/native";
import { Screen } from "./Screen";

const NoNotificationsReceived = () => {
  const navigation = useNavigation();
  const { t } = useTranslation("inbox");

  return (
    <Screen
      preset="auto"
      ScrollViewProps={{
        bounces: false,
      }}
      contentContainerStyle={{
        flexGrow: 1,
      }}
    >
      <Header
        title={t("title")}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="menuAlt2" color="white" />}
        onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
        rightIcon={<Icon icon="dotsVertical" color="white" />}
        onRightPress={() => {
          console.log("Right icon pressed");
        }}
      />
      <YStack
        flex={1}
        alignItems="center"
        justifyContent="center"
        gap="$md"
        backgroundColor="white"
      >
        <Icon icon="undrawInbox" size={190} />

        <YStack gap="$xxxs" paddingHorizontal="$lg">
          <Typography preset="subheading" textAlign="center">
            {t("empty.heading")}
          </Typography>
          <Typography preset="body1" textAlign="center" color="$gray12">
            {t("empty.paragraph")}
          </Typography>
        </YStack>
      </YStack>
    </Screen>
  );
};

export default NoNotificationsReceived;
