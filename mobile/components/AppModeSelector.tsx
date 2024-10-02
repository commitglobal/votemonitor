import React from "react";
import { XStack, YStack } from "tamagui";
import { Typography } from "./Typography";
import { Icon } from "./Icon";

interface IAppModeSelectorProps {
  title: string;
  description: string;
  selected?: boolean;
  onPress?: () => void;
}

export const AppModeSelector = ({
  title,
  description,
  selected,
  onPress,
}: IAppModeSelectorProps) => {
  return (
    <YStack
      gap="$xxxs"
      borderWidth={1}
      borderColor="white"
      padding="$md"
      borderRadius={8}
      backgroundColor={selected ? "$purple25" : "transparent"}
      onPress={onPress}
      pressStyle={selected ? { backgroundColor: "$purple3" } : { backgroundColor: "$purple55" }}
    >
      <XStack justifyContent="space-between">
        <Typography
          preset="subheading"
          fontSize={18}
          color={selected ? "$purple6" : "white"}
          maxWidth="80%"
        >
          {title}
        </Typography>
        {selected && <Icon icon="checkCircle" color="white" size={20} />}
      </XStack>

      <Typography preset="body2" color={selected ? "$purple6" : "white"}>
        {description}
      </Typography>
    </YStack>
  );
};
