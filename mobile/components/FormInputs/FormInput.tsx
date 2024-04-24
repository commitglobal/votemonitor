import React from "react";
import Input, { InputProps } from "../Inputs/Input";
import FormElement from "./FormElement";
import { StyleProp, TextStyle } from "react-native";

interface FormInputProps extends InputProps {
  //   question title
  title: string;
  // title props
  titleProps?: StyleProp<TextStyle>;
  // placeholder
  placeholder?: string;
}

const FormInput: React.FC<FormInputProps> = ({
  type,
  title,
  placeholder = "",
  titleProps,
  ...rest
}) => {
  return (
    <FormElement title={title} titleProps={titleProps}>
      <Input type={type} placeholder={placeholder} {...rest} />
    </FormElement>
  );
};

export default FormInput;
