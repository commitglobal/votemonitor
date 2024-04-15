import React, { ReactNode } from "react";
import { Stack, YStack } from "tamagui";
import { Typography } from "../Typography";
import { Keyboard } from "react-native";

export interface FormElementProps {
  //   question title
  title: string;
  //   children elements
  children: ReactNode;
}

const FormElement: React.FC<FormElementProps> = ({ children, title }) => {
  return (
    <YStack gap="$xxs" onPress={Keyboard.dismiss}>
      {/* title */}
      <Typography fontWeight="500">{title}</Typography>
      {/* input place */}
      <Stack gap="$md">{children}</Stack>
    </YStack>
  );
};

export default FormElement;
