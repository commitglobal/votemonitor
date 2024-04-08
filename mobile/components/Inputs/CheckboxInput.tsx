import React, { useState } from "react";
import { Checkbox, Label, XStack, CheckboxProps } from "tamagui";
import { Icon } from "../Icon";
import { Typography } from "../Typography";

interface CheckboxInputProps extends CheckboxProps {
  id: string;
  label: string;
}

const CheckboxInput: React.FC<CheckboxInputProps> = ({
  label,
  id,
  defaultChecked = false,
  ...rest
}) => {
  const [isChecked, setIsChecked] = useState(defaultChecked);

  return (
    <XStack
      height={42}
      alignItems="center"
      borderWidth={1}
      borderColor={isChecked ? "$purple5" : "$gray3"}
      gap="$xs"
      paddingHorizontal={14}
      borderRadius={8}
      {...rest}
    >
      <Checkbox
        defaultChecked={defaultChecked}
        width="$md"
        height="$md"
        borderRadius={4}
        id={id}
        onCheckedChange={(checked) => setIsChecked(checked)}
        backgroundColor={isChecked ? "$purple5" : "white"}
        borderColor={isChecked ? "$purple5" : "$gray3"}
      >
        <Checkbox.Indicator>
          <Icon icon="check" color="white" />
        </Checkbox.Indicator>
      </Checkbox>

      <Label htmlFor={id} paddingVertical="$xs" margin="$0" flex={1}>
        <Typography preset="body1">{label}</Typography>
      </Label>
    </XStack>
  );
};

export default CheckboxInput;
