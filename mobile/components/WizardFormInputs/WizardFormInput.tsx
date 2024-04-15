import React from "react";
import Input, { InputProps } from "../Inputs/Input";
import WizardFormElement from "./WizardFormElement";

interface WizardFormInputProps extends InputProps {
  //   question title
  label: string;
  //   question subtitle
  paragraph?: string;
  // helper text
  helper?: string;
}

const WizardFormInput: React.FC<WizardFormInputProps> = ({
  type,
  label,
  paragraph,
  helper,
  ...rest
}) => {
  return (
    <WizardFormElement label={label} paragraph={paragraph} helper={helper}>
      <Input type={type} marginBottom="$xxs" {...rest} placeholder={helper || ""} />
    </WizardFormElement>
  );
};

export default WizardFormInput;
