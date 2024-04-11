import React from "react";
import FormElement from "./FormElement";
import { RadioGroup , RadioGroupProps } from "tamagui";
import RadioInput from "../Inputs/RadioInput";

interface RadioFormInputProps extends RadioGroupProps {
  //   question title
  label: string;
  //   question subtitle
  paragraph?: string;
  // helper text
  helper?: string;
  value: string;
  options: { id: string; value: string; label: string }[];
}

const RadioFormInput: React.FC<RadioFormInputProps> = ({
  label,
  paragraph,
  helper,
  options,
  value,
  ...rest
}) => {
  return (
    <FormElement label={label} paragraph={paragraph} helper={helper}>
      <RadioGroup gap="$md" {...rest}>
        {options.map(({ id, value: optionValue, label }) => (
          <RadioInput id={id} value={optionValue} label={label} selectedValue={value} key={id}/>
        ))}
      </RadioGroup>
    </FormElement>
  );
};

export default RadioFormInput;
