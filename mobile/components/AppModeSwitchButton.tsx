import React from "react";
import { Icon } from "./Icon";
import { XStack, XStackProps } from "tamagui";
import { useTranslation } from "react-i18next";
import { AppMode, useAppMode } from "../contexts/app-mode/AppModeContext.provider";
import { Typography } from "./Typography";
import { router } from "expo-router";

interface AppModeSwitchButtonProps extends XStackProps {
  switchToMode: AppMode;
  color?: string;
}

export const AppModeSwitchButton = ({
  switchToMode,
  color = "white",
  ...rest
}: AppModeSwitchButtonProps) => {
  const { t } = useTranslation("drawer");

  const { setAppMode } = useAppMode();

  const handleSwitchAppMode = () => {
    switch (switchToMode) {
      case AppMode.CITIZEN:
        setAppMode(AppMode.CITIZEN);
        router.replace("citizen");
        break;
      case AppMode.OBSERVER:
        setAppMode(AppMode.OBSERVER);
        router.replace("(observer)");
        break;
    }
  };

  const renderModeToSwitchTo = () => {
    switch (switchToMode) {
      case AppMode.CITIZEN:
        return t("report_as_citizen");
      case AppMode.OBSERVER:
        return t("report_as_accredited_observer");
    }
  };

  return (
    <XStack
      marginTop="auto"
      gap="$xxs"
      alignItems="center"
      paddingHorizontal="$md"
      paddingVertical="$xxxs"
      pressStyle={{ opacity: 0.5 }}
      onPress={handleSwitchAppMode}
      {...rest}
    >
      <Icon icon="appModeSwitch" color={color} size={32} />
      <Typography color={color} preset="body2" textDecorationLine="underline" flex={1}>
        {renderModeToSwitchTo()}
      </Typography>
    </XStack>
  );
};
