import React from "react";
import { Label, RadioGroup, XStack, RadioGroupItemProps } from "tamagui";
import { Typography } from "../Typography";

export interface RadioInputProps extends RadioGroupItemProps {
  id: string;
  value: string;
  label: string;
  selectedValue: string;
  onValueChange: ((value: string) => void) | undefined;
}

const RadioInput: React.FC<RadioInputProps> = ({
  id,
  value,
  label,
  selectedValue,
  onValueChange,
  ...rest
}) => {
  const isSelected = selectedValue === value;

  // created a function to handle the selection of the radio input in order to allow selecting from multiple components, such as the XStack (not only the RadioGroup.Item and Label, which do this by default without the need of a specific function)
  const handlePress = () => {
    onValueChange && onValueChange(value);
  };

  return (
    <XStack
      alignItems="center"
      minHeight={42}
      borderWidth={1}
      backgroundColor="white"
      borderColor={isSelected ? "$purple5" : "$gray3"}
      borderRadius={8}
      paddingLeft={14}
      onPress={handlePress}
      {...rest}
    >
      <RadioGroup.Item
        height="$md"
        width="$md"
        value={value}
        id={id}
        backgroundColor={isSelected ? "$purple5" : "white"}
        borderColor={isSelected ? "$purple5" : "$gray3"}
        onPress={handlePress}
      >
        {isSelected && (
          <RadioGroup.Indicator forceMount={true} backgroundColor="white"></RadioGroup.Indicator>
        )}
      </RadioGroup.Item>

      <Label
        htmlFor={id}
        flex={1}
        lineHeight={20}
        paddingVertical="$xs"
        paddingLeft="$xs"
        onPress={handlePress}
      >
        <Typography preset="body1">{label}</Typography>
      </Label>
    </XStack>
  );
};

export default RadioInput;
