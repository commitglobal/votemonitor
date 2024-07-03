import React from "react";
import { Checkbox, Label, XStack, CheckboxProps } from "tamagui";
import { Icon } from "../Icon";
import { Typography } from "../Typography";

export interface CheckboxInputProps extends CheckboxProps {
  id: string;
  label: string;
}

const CheckboxInput: React.FC<CheckboxInputProps> = ({ label, id, checked, onCheckedChange }) => {
  // function to allow changing the pressed state from the checkbox container
  const handlePress = () => {
    if (onCheckedChange) {
      onCheckedChange(!checked);
    }
  };

  return (
    <XStack
      minHeight={42}
      alignItems="center"
      backgroundColor="white"
      borderWidth={1}
      borderColor={checked ? "$purple5" : "$gray3"}
      gap="$xs"
      paddingHorizontal={14}
      borderRadius={8}
      onPress={handlePress}
    >
      <Checkbox
        width="$md"
        height="$md"
        borderRadius={4}
        id={id}
        checked={checked}
        onCheckedChange={onCheckedChange}
        backgroundColor={checked ? "$purple5" : "white"}
        borderColor={checked ? "$purple5" : "$gray3"}
      >
        <Checkbox.Indicator>
          <Icon icon="check" color="white" />
        </Checkbox.Indicator>
      </Checkbox>

      <Label htmlFor={id} paddingVertical="$xs" flex={1} lineHeight={20}>
        <Typography preset="body1">{label}</Typography>
      </Label>
    </XStack>
  );
};

export default CheckboxInput;
