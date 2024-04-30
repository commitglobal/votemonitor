import React, { ReactNode } from "react";
import { Stack, YStack } from "tamagui";
import { Typography } from "../Typography";
import { Keyboard, StyleProp, TextStyle } from "react-native";
import { FieldError, FieldErrorsImpl, Merge } from "react-hook-form";

export interface FormElementProps {
  //   question title
  title?: string;
  titleProps?: StyleProp<TextStyle>;
  //   children elements
  children: ReactNode;
  // error
  error?:
    | FieldError
    | Merge<FieldError, FieldErrorsImpl<{ details: string; id: string }>>
    | string
    | undefined;
}

const FormElement: React.FC<FormElementProps> = ({ children, title, titleProps, error }) => {
  return (
    <YStack gap="$xxs" onPress={Keyboard.dismiss}>
      {/* title */}
      {title && (
        <Typography fontWeight="500" style={titleProps} color={error && "$red5"}>
          {title}
        </Typography>
      )}
      {/* input place */}
      <Stack gap="$md">{children}</Stack>
    </YStack>
  );
};

export default FormElement;
