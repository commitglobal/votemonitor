import React from "react";
import Input, { InputProps } from "../Inputs/Input";
import FormElement, { FormElementProps } from "./FormElement";

export interface FormInputProps extends InputProps, Omit<FormElementProps, "children"> {
  placeholder?: string;
}

const FormInput: React.FC<FormInputProps> = ({
  type,
  title,
  placeholder = "",
  titleProps,
  error,
  helper,
  ...rest
}) => {
  return (
    <FormElement title={title} titleProps={titleProps} error={error} helper={helper}>
      <Input type={type} placeholder={placeholder} borderColor={error && "$red7"} {...rest} />
    </FormElement>
  );
};

export default FormInput;
