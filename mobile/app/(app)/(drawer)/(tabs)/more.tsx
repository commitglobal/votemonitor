import React, { useState } from "react";
import { View, XStack } from "tamagui";
import Card from "../../../../components/Card";
import { Screen } from "../../../../components/Screen";
import { Typography } from "../../../../components/Typography";
import { Icon } from "../../../../components/Icon";
import { useTranslation } from "react-i18next";
import * as Linking from "expo-linking";
import { tokens } from "../../../../theme/tokens";

const More = () => {
  const { t } = useTranslation("more");

  // TODO: Change these consts
  const appVersion = "2.0.4";
  const appLanguage = "English (United States)";
  const URL = "https://www.google.com/";

  return (
    <Screen
      preset="auto"
      backgroundColor="white"
      contentContainerStyle={{
        gap: tokens.space.md.val,
        paddingHorizontal: tokens.space.md.val,
        paddingVertical: tokens.space.xl.val,
      }}
    >
      <Section
        headerText={t("change-language")}
        subHeaderText={appLanguage}
        iconName="check"
        chevronRight={true}
      ></Section>
      <Section headerText={t("change-password")} iconName="check" chevronRight={true}></Section>
      <Section
        headerText={t("terms")}
        iconName="check"
        chevronRight={true}
        action={() => {
          Linking.openURL(URL);
        }}
      ></Section>
      <Section
        headerText={t("privacy_policy")}
        iconName="check"
        chevronRight={true}
        action={() => {
          Linking.openURL(URL);
        }}
      ></Section>
      <Section
        headerText={t("about")}
        subHeaderText={t("app_version", { value: appVersion })}
        iconName="check"
        chevronRight={true}
      ></Section>
      <Section headerText={t("support")} iconName="check"></Section>
      <Section headerText={t("feedback")} iconName="check"></Section>
      <Section headerText={t("logout")} iconName="check"></Section>
    </Screen>
  );
};

interface SectionProps {
  headerText: string;
  subHeaderText?: string;
  iconName: string;
  chevronRight?: boolean;
  action?: () => void;
}

const Section = (props: SectionProps) => {
  const { headerText, subHeaderText, iconName, chevronRight, action } = props;
  const hasSubHeader = subHeaderText ? true : false;
  const hasChevronRight = chevronRight ? true : false;

  const [isPressed, setIsPressed] = useState(false);

  return (
    <Card
      onPress={action}
      onPressIn={() => setIsPressed(true)}
      onPressOut={() => setIsPressed(false)}
      opacity={isPressed ? 0.5 : 1}
    >
      <XStack alignItems="center" justifyContent="space-between">
        <XStack alignItems="center" gap="$xxs">
          <Icon size={24} icon={iconName} color="black" />
          <View alignContent="center" gap="$xxxs">
            <Typography preset="body2"> {headerText} </Typography>
            {hasSubHeader && <Typography color="$gray8"> {subHeaderText}</Typography>}
          </View>
        </XStack>

        {hasChevronRight && <Icon size={24} icon="chevronRight" color="$purple7" />}
      </XStack>
    </Card>
  );
};

export default More;
