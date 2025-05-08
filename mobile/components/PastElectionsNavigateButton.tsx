import { router } from "expo-router";
import React from "react";
import { useTranslation } from "react-i18next";
import { XStack, XStackProps } from "tamagui";
import { Icon } from "./Icon";
import { Typography } from "./Typography";

interface PastElectionsNavigateButtonProps extends XStackProps {
  color?: string;
}

export const PastElectionsNavigateButton = ({
  color = "white",
  ...rest
}: PastElectionsNavigateButtonProps) => {
  const { t } = useTranslation("drawer");

  return (
    <XStack
      marginTop="auto"
      gap="$xxs"
      alignItems="center"
      paddingHorizontal="$md"
      paddingVertical="$xxxs"
      pressStyle={{ opacity: 0.5 }}
      onPress={() => router.push("/past-elections")}
      {...rest}
    >
      <Icon icon="archive" color={color} size={32} />
      <Typography color={color} preset="body2" textDecorationLine="underline" flex={1}>
        {t("past_elections")}
      </Typography>
    </XStack>
  );
};
