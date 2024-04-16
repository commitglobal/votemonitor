import React from "react";
import WizardFormElement from "./WizardFormElement";
import { RadioGroup, RadioGroupProps } from "tamagui";
import RadioInput from "../Inputs/RadioInput";

interface WizardRadioFormInputProps extends RadioGroupProps {
  //   question title
  label: string;
  //   question subtitle
  paragraph?: string;
  // helper text
  helper?: string;
  value: string;
  options: { id: string; value: string; label: string }[];
}

const WizardRadioFormInput: React.FC<WizardRadioFormInputProps> = ({
  label,
  paragraph,
  helper,
  options,
  value,
  ...rest
}) => {
  return (
    <WizardFormElement label={label} paragraph={paragraph} helper={helper}>
      <RadioGroup gap="$md" {...rest}>
        {options.map(({ id, value: optionValue, label }) => (
          <RadioInput id={id} value={optionValue} label={label} selectedValue={value} key={id} />
        ))}
      </RadioGroup>
    </WizardFormElement>
  );
};

export default WizardRadioFormInput;
