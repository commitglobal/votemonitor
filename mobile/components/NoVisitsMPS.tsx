import React from "react";
import { useTranslation } from "react-i18next";
import Header from "./Header";
import { Screen } from "./Screen";
import { YStack } from "tamagui";
import { Icon } from "./Icon";
import { Typography } from "./Typography";
import { router } from "expo-router";

const NoVisitsMPS = () => {
  const { t } = useTranslation("manage_my_polling_stations");
  return (
    <Screen
      preset="scroll"
      ScrollViewProps={{
        showsVerticalScrollIndicator: false,
        stickyHeaderIndices: [0],
        bounces: false,
      }}
      contentContainerStyle={{ flexGrow: 1 }}
    >
      <Header
        title={t("general_text")}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      />
      <YStack
        flex={1}
        justifyContent="center"
        alignItems="center"
        gap="$md"
        backgroundColor="white"
      >
        <Icon icon="missingPollingStation" />
        <YStack gap="$xxxs">
          <Typography preset="subheading" textAlign="center">
            {t("empty.title")}
          </Typography>
        </YStack>
      </YStack>
    </Screen>
  );
};

export default NoVisitsMPS;
