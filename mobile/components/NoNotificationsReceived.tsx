import React from "react";
import { Icon } from "./Icon";
import { ScrollView, YStack } from "tamagui";
import { Typography } from "./Typography";
import { useTranslation } from "react-i18next";
import Header from "./Header";
import { useNavigation } from "expo-router";
import { DrawerActions } from "@react-navigation/native";
import { Screen } from "./Screen";
import { useUserData } from "../contexts/user/UserContext.provider";
import { useNotifications } from "../services/queries/notifications.query";
import { RefreshControl } from "react-native";

const NoNotificationsReceived = () => {
  const navigation = useNavigation();
  const { t } = useTranslation("inbox");

  const { activeElectionRound } = useUserData();
  const { isRefetching: isRefetchingNotifications, refetch: refetchNotifications } =
    useNotifications(activeElectionRound?.id);

  return (
    <Screen
      preset="fixed"
      contentContainerStyle={{
        flexGrow: 1,
      }}
    >
      <Header
        title={activeElectionRound?.title}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="menuAlt2" color="white" />}
        onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
      />
      <ScrollView
        contentContainerStyle={{ alignItems: "center", justifyContent: "center", gap: 16, flex: 1 }}
        backgroundColor="white"
        showsVerticalScrollIndicator={false}
        refreshControl={
          <RefreshControl refreshing={isRefetchingNotifications} onRefresh={refetchNotifications} />
        }
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
      </ScrollView>
    </Screen>
  );
};

export default NoNotificationsReceived;
