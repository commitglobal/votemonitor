import React from "react";
import { DateInput, DateInputProps } from "../Inputs/DateInput";
import FormElement from "./FormElement";

interface FormInputProps extends DateInputProps {
  //   question title
  title: string;
  // placeholder
  placeholder?: string;
}

const DateFormInput: React.FC<FormInputProps> = ({ title, placeholder = "", error, ...rest }) => {
  return (
    <FormElement title={title} error={error}>
      <DateInput placeholder={placeholder} error={error} {...rest} />
    </FormElement>
  );
};

export default DateFormInput;
