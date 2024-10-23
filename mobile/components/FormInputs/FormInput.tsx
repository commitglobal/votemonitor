import React, { forwardRef } from "react";
import Input, { InputProps } from "../Inputs/Input";
import FormElement, { FormElementProps } from "./FormElement";

export interface FormInputProps extends InputProps, Omit<FormElementProps, "children"> {
  placeholder?: string;
}

const FormInput: React.FC<FormInputProps> = forwardRef(
  ({ type, title, placeholder = "", titleProps, error, helper, ...rest }, ref) => {
    return (
      <FormElement title={title} titleProps={titleProps} error={error} helper={helper}>
        <Input
          ref={ref}
          type={type}
          placeholder={placeholder}
          borderColor={error ? "$red7" : undefined}
          {...rest}
        />
      </FormElement>
    );
  },
);

export default FormInput;
