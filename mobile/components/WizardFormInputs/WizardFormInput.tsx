import React from "react";
import Input, { InputProps } from "../Inputs/Input";
import WizardFormElement from "./WizardFormElement";

interface WizardFormInputProps extends InputProps {
  //   question title
  label: string;
  //   question subtitle
  paragraph?: string;
  // placeholder
  placeholder?: string;
  // helper text
  helper?: string;
  // ref
  ref?: any;
}

const WizardFormInput = ({ type, label, paragraph, helper, placeholder = "", ref, ...rest }: WizardFormInputProps) => {
  return (
    <WizardFormElement label={label} paragraph={paragraph} helper={helper}>
      <Input type={type} {...rest} placeholder={placeholder} ref={ref} />
    </WizardFormElement>
  );
};

export default WizardFormInput;
