import React, { ReactNode } from "react";
import { Stack, YStack } from "tamagui";
import { Typography } from "../Typography";
import { Keyboard } from "react-native";
// import AddAttachment from "../AddAttachment";

export interface FormElementProps {
  //   question title
  label: string;
  //   question subtitle
  paragraph?: string;
  // helper text
  helper?: string;
  //   children elements
  children: ReactNode;

  // allowAttachment?: boolean;
}

const WizardFormElement: React.FC<FormElementProps> = ({
  children,
  label,
  paragraph,
  helper,
  // allowAttachment,
}) => {
  return (
    <YStack gap="$xxs" onPress={Keyboard.dismiss}>
      {/* header */}
      <Typography preset="subheading" fontWeight="500">
        {label}
      </Typography>
      {/* subheader */}
      {paragraph && <Typography color="$gray7">{paragraph}</Typography>}
      {/* input place */}
      <Stack paddingTop="$md" gap="$md">
        {children}
      </Stack>
      {/* helper text */}
      {helper && <Typography color="$gray5">{helper}</Typography>}
    </YStack>
  );
};

export default WizardFormElement;
