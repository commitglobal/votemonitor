import React, { useContext } from "react";
import { LanguageContext } from "../../../contexts/language/LanguageContext.provider";
import Button from "../../../components/Button";
import { Screen } from "../../../components/Screen";
import Header from "../../../components/Header";
import { Icon } from "../../../components/Icon";
import { router } from "expo-router";
import { YStack } from "tamagui";
import { useTranslation } from "react-i18next";

const ChangeLanguage = () => {
  const { changeLanguage } = useContext(LanguageContext);
  const { t } = useTranslation("more");

  const switchToEnglish = () => {
    changeLanguage("en");
  };

  const switchToRomanian = () => {
    changeLanguage("ro");
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
      contentContainerStyle={{
        flexGrow: 1,
      }}
    >
      <Header
        title={t("change-language")}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
        rightIcon={<Icon icon="dotsVertical" color="white" />}
        onRightPress={() => {
          console.log("Right icon pressed");
        }}
      />

      <YStack flex={1} alignItems="center" justifyContent="center" gap={10}>
        <Button onPress={switchToEnglish}>English</Button>
        <Button onPress={switchToRomanian}>Romanian</Button>
      </YStack>
    </Screen>
  );
};

export default ChangeLanguage;
