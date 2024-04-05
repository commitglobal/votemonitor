import React, { useState } from "react";
import { Checkbox, Label, XStack, CheckboxProps, CheckedState } from "tamagui";
import { Icon } from "../Icon";
import { Typography } from "../Typography";

interface CheckboxInputProps extends CheckboxProps {
  id: string;
  label: string;
}

const CheckboxInput: React.FC<CheckboxInputProps> = ({ label, id, ...rest }) => {
  const [isChecked, setIsChecked] = useState<CheckedState>(false);
  return (
    <XStack
      height={42}
      alignItems="center"
      borderColor={isChecked ? "$purple5" : "$gray3"}
      borderWidth={1}
      gap="$xs"
      paddingHorizontal={14}
      paddingVertical="$xs"
      borderRadius={8}
      {...rest}
    >
      <Checkbox
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

      <Label htmlFor={id} padding="$0" margin="$0" lineHeight={24} flex={1}>
        <Typography>{label}</Typography>
      </Label>
    </XStack>
  );
};

export default CheckboxInput;
