import React, { ReactNode } from "react";
import { Typography } from "./Typography";
import { Dialog } from "./Dialog";
import { ScrollView, XStack, YStack } from "tamagui";
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
          <ScrollView bounces={false} showsVerticalScrollIndicator={false}>
            <YStack gap="$lg">
              {typeof description === "string" ? (
                <Typography preset="body1" color="$gray6">
                  {description}
                </Typography>
              ) : (
                description
              )}
            </YStack>
          </ScrollView>
        )
      }
      footer={
        <XStack gap="$sm" justifyContent="center" alignItems="center">
          <Button
            preset="chromeless"
            textStyle={{ color: "black", textAlign: "center" }}
            onPress={onCancel}
            height="100%"
          >
            {cancelBtnText}
          </Button>

          {actionBtnText && (
            <Button
              backgroundColor="$red10"
              height="100%"
              flex={1}
              onPress={action}
              textStyle={{ textAlign: "center" }}
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
