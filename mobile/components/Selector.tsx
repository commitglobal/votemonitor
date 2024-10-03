import React from "react";
import { XStack, YStack, YStackProps } from "tamagui";
import { Typography } from "./Typography";
import { Icon } from "./Icon";

interface ISelectorProps extends YStackProps {
  title?: string;
  description?: string;
  selected?: boolean;
  displayMode?: "dark" | "light";
  onPress?: () => void;
}

export const Selector = ({
  title,
  description,
  selected,
  displayMode = "dark",
  onPress,
  ...rest
}: ISelectorProps) => {
  const theme = {
    dark: {
      background: selected ? "$purple25" : "transparent",
      textColor: selected ? "$purple6" : "white",
      pressStyle: selected ? { backgroundColor: "$purple3" } : { backgroundColor: "$purple55" },
      borderColor: "white",
    },
    light: {
      background: selected ? "$purple2" : "transparent",
      textColor: "$purple5",
      pressStyle: { backgroundColor: "$purple2" },
      borderColor: "$purple5",
    },
  };

  const currentTheme = theme[displayMode];

  return (
    <XStack
      borderWidth={1}
      borderColor={currentTheme.borderColor}
      padding="$md"
      borderRadius={8}
      backgroundColor={currentTheme.background}
      onPress={onPress}
      pressStyle={currentTheme.pressStyle}
      width="100%"
      {...rest}
      justifyContent="space-between"
    >
      <YStack flex={0.9}>
        {title && (
          <Typography preset="subheading" fontSize={18} color={currentTheme.textColor}>
            {title}
          </Typography>
        )}
        {description && (
          <Typography preset="body2" color={currentTheme.textColor}>
            {description}
          </Typography>
        )}
      </YStack>

      {selected && (
        <Icon icon="checkCircle" color="white" size={20} flex={0.1} alignItems="flex-end" />
      )}
    </XStack>
  );
};
