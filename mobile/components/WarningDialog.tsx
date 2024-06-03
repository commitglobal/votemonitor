import React, { ReactNode } from "react";
import { Typography } from "./Typography";
import { Dialog } from "./Dialog";
import { XStack, YStack } from "tamagui";
import Button from "./Button";
import { StyleProp, TextStyle } from "react-native";

type WarningDialogProps = {
  title: string;
  description: string | ReactNode;
  actionBtnText: string;
  cancelBtnText: string;
  onCancel: () => void;
  action: () => void;
  actionBtnStyle?: object;
  titleProps?: StyleProp<TextStyle>;
};

const WarningDialog = ({
  title,
  description,
  actionBtnText,
  cancelBtnText,
  action,
  onCancel,
  actionBtnStyle,
  titleProps,
}: WarningDialogProps) => {
  return (
    <Dialog
      open
      header={
        <Typography preset="heading" style={titleProps}>
          {title}
        </Typography>
      }
      content={
        description && (
          <YStack gap="$lg">
            {typeof description === "string" ? (
              <Typography preset="body1" color="$gray6">
                {description}
              </Typography>
            ) : (
              description
            )}
          </YStack>
        )
      }
      footer={
        <XStack gap="$sm" justifyContent="center" alignItems="center">
          <Button preset="chromeless" textStyle={{ color: "black" }} onPress={onCancel}>
            {cancelBtnText}
          </Button>

          {actionBtnText && (
            <Button
              backgroundColor="$red10"
              flex={1}
              onPress={action}
              style={{ ...actionBtnStyle }}
            >
              {actionBtnText}
            </Button>
          )}
        </XStack>
      }
    />
  );
};

export default WarningDialog;
