import React from "react";
import { DateInput, DateInputProps } from "../Inputs/DateInput";
import WizardFormElement from "./WizardFormElement";

interface WizardFormInputProps extends DateInputProps {
  //   question title
  label: string;
  //   question subtitle
  paragraph?: string;
  // helper text
  helper?: string;
}

const WizardDateFormInput: React.FC<WizardFormInputProps> = ({
  label,
  paragraph,
  helper,
  ...rest
}) => {
  return (
    <WizardFormElement label={label} paragraph={paragraph} helper={helper}>
      <DateInput {...rest} />
    </WizardFormElement>
  );
};

export default WizardDateFormInput;
