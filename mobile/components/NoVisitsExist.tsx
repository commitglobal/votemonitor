import React from "react";
import { router, useNavigation } from "expo-router";
import { YStack } from "tamagui";
import { Screen } from "./Screen";
import { Icon } from "./Icon";
import { Typography } from "./Typography";
import Button from "./Button";
import Header from "./Header";
import { DrawerActions } from "@react-navigation/native";
import { useTranslation } from "react-i18next";

const NoVisitsExist = () => {
  const navigation = useNavigation();
  const { t } = useTranslation("observation");

  return (
    <Screen preset="fixed" contentContainerStyle={{ flexGrow: 1 }}>
      <Header
        title={""}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="menuAlt2" color="white" />}
        onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
      />
      <YStack
        width="100%"
        display="flex"
        backgroundColor="white"
        alignItems="center"
        justifyContent="center"
        gap="$md"
        paddingHorizontal="$lg"
        flex={1}
      >
        <Icon icon="missingPollingStation" />
        <YStack gap="$xxxs">
          <Typography preset="subheading" textAlign="center">
            {t("no_visited_polling_stations.heading")}
          </Typography>
          <Typography preset="body1" textAlign="center" color="$gray5">
            {t("no_visited_polling_stations.paragraph")}
          </Typography>
        </YStack>
        <Button
          preset="outlined"
          backgroundColor="white"
          width="100%"
          onPress={router.push.bind(null, "/polling-station-wizzard")}
        >
          {t("no_visited_polling_stations.add")}
        </Button>
      </YStack>
    </Screen>
  );
};

export default NoVisitsExist;
