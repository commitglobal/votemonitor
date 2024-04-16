import React from "react";
import FormElement from "./FormElement";
import RatingInput, { RatingInputProps } from "../Inputs/RatingInput";

interface RatingFormInputProps extends RatingInputProps {
  //   question title
  title: string;
}

const RatingFormInput: React.FC<RatingFormInputProps> = ({ id, title, ...rest }) => {
  return (
    <FormElement title={title}>
      <RatingInput id={id} {...rest} />
    </FormElement>
  );
};

export default RatingFormInput;
