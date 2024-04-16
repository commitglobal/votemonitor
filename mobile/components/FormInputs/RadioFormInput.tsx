import React from "react";
import FormElement from "./FormElement";
import { RadioGroup, RadioGroupProps } from "tamagui";
import RadioInput from "../Inputs/RadioInput";

interface RadioFormInputProps extends RadioGroupProps {
  //   question title
  title: string;
  value: string;
  options: { id: string; value: string; label: string }[];
}

const RadioFormInput: React.FC<RadioFormInputProps> = ({ title, options, value, ...rest }) => {
  return (
    <FormElement title={title}>
      <RadioGroup gap="$md" {...rest}>
        {options.map(({ id, value: optionValue, label }) => (
          <RadioInput id={id} value={optionValue} label={label} selectedValue={value} key={id} />
        ))}
      </RadioGroup>
    </FormElement>
  );
};

export default RadioFormInput;
