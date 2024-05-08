import React, { ReactNode } from "react";
import { Stack, YStack } from "tamagui";
import { Typography } from "../Typography";
import { Keyboard, StyleProp, TextStyle } from "react-native";

export interface FormElementProps {
  title?: string;
  titleProps?: StyleProp<TextStyle>;
  children: ReactNode;
  error?: string;
  helper?: string;
}

const FormElement: React.FC<FormElementProps> = ({
  children,
  title,
  titleProps,
  error,
  helper,
}) => {
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
      {helper && !error && <Typography color="gray">{helper}</Typography>}
      {error && <Typography color="$red7">{error}</Typography>}
    </YStack>
  );
};

export default FormElement;
