import React from "react";
import FormElement from "./FormElement";
import RatingInput, { RatingInputProps } from "../Inputs/RatingInput";

interface RatingFormInputProps extends RatingInputProps {
  //   question title
  label: string;
  //   question subtitle
  paragraph?: string;
  // helper text
  helper?: string;
}

const RatingFormInput: React.FC<RatingFormInputProps> = ({
  id,
  label,
  paragraph,
  helper,
  ...rest
}) => {
  return (
    <FormElement label={label} paragraph={paragraph} helper={helper}>
      <RatingInput id={id} {...rest} />
    </FormElement>
  );
};

export default RatingFormInput;
