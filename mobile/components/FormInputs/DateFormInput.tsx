import React from "react";
import { DateInput, DateInputProps } from "../Inputs/DateInput";
import FormElement from "./FormElement";

interface FormInputProps extends DateInputProps {
  //   question title
  title: string;
  // placeholder
  placeholder?: string;
}

const DateFormInput: React.FC<FormInputProps> = ({ title, placeholder = "", ...rest }) => {
  return (
    <FormElement title={title}>
      <DateInput placeholder={placeholder} {...rest} />
    </FormElement>
  );
};

export default DateFormInput;
