import React from "react";
import { YStack } from "tamagui";
import { Typography } from "../Typography";
import Input, { InputProps } from "../Inputs/Input";
import FormElement from "./FormElement";

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
    <FormElement label={label} paragraph={paragraph} helper={helper}>
      <Input type={type} marginBottom="$xxs" {...rest} />
    </FormElement>
  );
};

export default FormInput;
