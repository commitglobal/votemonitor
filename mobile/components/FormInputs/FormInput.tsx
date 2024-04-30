import React from "react";
import Input, { InputProps } from "../Inputs/Input";
import FormElement from "./FormElement";
import { StyleProp, TextStyle } from "react-native";
import { FieldError, FieldErrorsImpl, Merge } from "react-hook-form";

interface FormInputProps extends InputProps {
  //   question title
  title: string;
  // title props
  titleProps?: StyleProp<TextStyle>;
  // placeholder
  placeholder?: string;
  // error
  error?:
    | FieldError
    | Merge<FieldError, FieldErrorsImpl<{ details: string; id: string }>>
    | string
    | undefined;
}

const FormInput: React.FC<FormInputProps> = ({
  type,
  title,
  placeholder = "",
  titleProps,
  error,
  ...rest
}) => {
  return (
    <FormElement title={title} titleProps={titleProps} error={error}>
      <Input type={type} placeholder={placeholder} borderColor={error && "$red7"} {...rest} />
    </FormElement>
  );
};

export default FormInput;
