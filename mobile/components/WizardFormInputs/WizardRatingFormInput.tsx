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

  // lower label text
  lowerLabel?: string;

  // upper label text
  upperLabel?: string;
}

const WizardRatingFormInput: React.FC<WizardRatingFormInputProps> = ({
  id,
  label,
  paragraph,
  helper,
  lowerLabel,
  upperLabel,
  ...rest
}) => {
  return (
    <WizardFormElement label={label} paragraph={paragraph} helper={helper}>
      <RatingInput id={id} lowerLabel={lowerLabel} upperLabel={upperLabel} {...rest} />
    </WizardFormElement>
  );
};

export default WizardRatingFormInput;
