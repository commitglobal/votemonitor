import React from "react";
import { Checkbox, Label, XStack, CheckboxProps, styled } from "tamagui";
import { Icon } from "../Icon";
import { Typography } from "../Typography";

interface CheckboxInputProps extends CheckboxProps {
  id: string;
  label: string;
}

const StyledCheckbox = styled(Checkbox, {
  width: "$md",
  height: "$md",
  borderRadius: 4,
  // onCheckedChange={(checked) => console.log(checked)}
  // backgroundColor={this.checked ? "$purple5" : "white"}
  // borderColor={checked ? "$purple5" : "$gray3"}
});

const CheckboxInput: React.FC<CheckboxInputProps> = ({
  label,
  id,
  defaultChecked = false,
  checked,
  ...rest
}) => {
  console.log(checked);
  return (
    <XStack
      height={42}
      alignItems="center"
      borderWidth={1}
      borderColor={checked ? "$purple5" : "$gray3"}
      gap="$xs"
      paddingHorizontal={14}
      paddingVertical="$xs"
      borderRadius={8}
      {...rest}
    >
      <StyledCheckbox id={id}>
        <Checkbox.Indicator>
          <Icon icon="check" color="black" />
        </Checkbox.Indicator>
      </StyledCheckbox>

      <Label htmlFor={id} padding="$0" margin="$0" flex={1}>
        <Typography preset="body1">{label}</Typography>
      </Label>
    </XStack>
  );
};

export default CheckboxInput;
