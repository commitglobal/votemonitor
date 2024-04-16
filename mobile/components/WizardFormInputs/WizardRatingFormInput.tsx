import React from "react";
import WizardFormElement from "./WizardFormElement";
import RatingInput, { RatingInputProps } from "../Inputs/RatingInput";

interface WizardRatingFormInputProps extends RatingInputProps {
  //   question title
  label: string;
  //   question subtitle
  paragraph?: string;
  // helper text
  helper?: string;
}

const WizardRatingFormInput: React.FC<WizardRatingFormInputProps> = ({
  id,
  label,
  paragraph,
  helper,
  ...rest
}) => {
  return (
    <WizardFormElement label={label} paragraph={paragraph} helper={helper}>
      <RatingInput id={id} {...rest} />
    </WizardFormElement>
  );
};

export default WizardRatingFormInput;
