import React from "react";
import Input, { InputProps } from "../Inputs/Input";
import FormElement from "./FormElement";

interface FormInputProps extends InputProps {
  //   question title
  title: string;
}

const FormInput: React.FC<FormInputProps> = ({ type, title, placeholder, ...rest }) => {
  return (
    <FormElement title={title}>
      <Input type={type} marginBottom="$xxs" placeholder={placeholder || ""} {...rest} />
    </FormElement>
  );
};

export default FormInput;
