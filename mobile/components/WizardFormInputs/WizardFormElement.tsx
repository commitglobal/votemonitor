import React, { ReactNode } from "react";
import { Stack, XStack, YStack } from "tamagui";
import { Typography } from "../Typography";
import { Keyboard } from "react-native";
import { Icon } from "../Icon";

export interface FormElementProps {
  //   question title
  label: string;
  //   question subtitle
  paragraph?: string;
  // helper text
  helper?: string;
  //   children elements
  children: ReactNode;

  allowAttachment?: boolean;
}

const WizardFormElement: React.FC<FormElementProps> = ({
  children,
  label,
  paragraph,
  helper,
  allowAttachment,
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

      {/* attach element */}
      {allowAttachment && (
        <XStack alignItems="center" paddingTop="$md" onPress={() => console.log("attach")}>
          <Icon icon="attachment" />
          <Typography color="$purple5" marginLeft="$xs">
            Add Note, Photo or Video
          </Typography>
        </XStack>
      )}
    </YStack>
  );
};

export default WizardFormElement;
