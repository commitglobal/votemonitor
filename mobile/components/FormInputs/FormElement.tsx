import React, { ReactNode } from "react";
import { Stack, YStack } from "tamagui";
import { Typography } from "../Typography";
import { Keyboard, StyleProp, TextStyle } from "react-native";

export interface FormElementProps {
  //   question title
  title?: string;
  titleProps?: StyleProp<TextStyle>;
  //   children elements
  children: ReactNode;
}

const FormElement: React.FC<FormElementProps> = ({ children, title, titleProps }) => {
  return (
    <YStack gap="$xxs" onPress={Keyboard.dismiss}>
      {/* title */}
      {title && (
        <Typography fontWeight="500" style={titleProps}>
          {title}
        </Typography>
      )}
      {/* input place */}
      <Stack gap="$md">{children}</Stack>
    </YStack>
  );
};

export default FormElement;
