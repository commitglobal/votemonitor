import React, { ReactNode } from "react";
import { YStack } from "tamagui";
import { Typography } from "../Typography";

export interface FormElementProps {
  //   question title
  label: string;
  //   question subtitle
  paragraph?: string;
  // helper text
  helper?: string;
  //   children elements
  children: ReactNode;
}

const FormElement: React.FC<FormElementProps> = ({ children, label, paragraph, helper }) => {
  return (
    <YStack gap="$xxs" marginBottom="$lg">
      {/* header */}
      <Typography preset="subheading" fontWeight="500">
        {label}
      </Typography>
      {/* subheader */}
      {paragraph && <Typography color="$gray7">{paragraph}</Typography>}
      {/* input place */}
      {children}
      {/* helper text */}
      {helper && <Typography color="$gray5">{helper}</Typography>}
    </YStack>
  );
};

export default FormElement;
