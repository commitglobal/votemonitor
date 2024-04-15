import React from "react";
import { DateInput, DateInputProps } from "../Inputs/DateInput";
import FormElement from "./FormElement";

interface FormInputProps extends DateInputProps {
  //   question title
  label: string;
  //   question subtitle
  paragraph?: string;
  // helper text
  helper?: string;
}

const DateFormInput: React.FC<FormInputProps> = ({ label, paragraph, helper, ...rest }) => {
  return (
    <FormElement label={label} paragraph={paragraph} helper={helper}>
      <DateInput {...rest} />
    </FormElement>
  );
};

export default DateFormInput;
