import React from "react";
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
      <Input type={type} marginBottom="$xxs" {...rest} placeholder={helper || ""} />
    </FormElement>
  );
};

export default FormInput;
