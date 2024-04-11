import React from "react";
import { YStack } from "tamagui";
import { Typography } from "../Typography";
import Input, { InputProps } from "../Inputs/Input";

interface FormInputProps extends InputProps {
  //   question title
  label: string;
  //   question subtitle
  paragraph?: string;
  // helper text
  helper?: string;
}

const FormInput: React.FC<FormInputProps> = ({ type, label, paragraph, helper, ...rest }) => {
  return (
    <YStack>
      <YStack gap="$xxs" marginBottom="$lg">
        <Typography preset="subheading" fontWeight="500">
          {label}
        </Typography>
        {paragraph && <Typography color="$gray7">{paragraph}</Typography>}
      </YStack>
      <Input type={type} marginBottom="$xxs" {...rest} />
      {helper && <Typography color="$gray5">{helper}</Typography>}
    </YStack>
  );
};

export default FormInput;
