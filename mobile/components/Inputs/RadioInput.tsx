import React from "react";
import { Label, RadioGroup, XStack, RadioGroupItemProps } from "tamagui";
import { Typography } from "../Typography";

export interface RadioInputProps extends RadioGroupItemProps {
  id: string;
  value: string;
  label: string;
  selectedValue: string;
}

const RadioInput: React.FC<RadioInputProps> = ({ id, value, label, selectedValue, ...rest }) => {
  const isSelected = selectedValue === value;

  return (
    <XStack
      alignItems="center"
      height={42}
      borderWidth={1}
      backgroundColor="white"
      borderColor={isSelected ? "$purple5" : "$gray3"}
      gap="$xs"
      paddingHorizontal={14}
      borderRadius={8}
      {...rest}
    >
      <RadioGroup.Item
        height="$md"
        width="$md"
        value={value}
        id={id}
        backgroundColor={isSelected ? "$purple5" : "white"}
        borderColor={isSelected ? "$purple5" : "$gray3"}
      >
        {isSelected && (
          <RadioGroup.Indicator forceMount={true} backgroundColor="white"></RadioGroup.Indicator>
        )}
      </RadioGroup.Item>

      <Label htmlFor={id} flex={1} lineHeight={20} paddingVertical="$xs">
        <Typography preset="body1">{label}</Typography>
      </Label>
    </XStack>
  );
};

export default RadioInput;
