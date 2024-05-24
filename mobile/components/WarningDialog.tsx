import React from "react";
import { Typography } from "./Typography";
import { Dialog } from "./Dialog";
import { XStack, YStack } from "tamagui";
import Button from "./Button";

type WarningDialogProps = {
  title: string;
  description: string;
  actionBtnText: string;
  cancelBtnText: string;
  onCancel: () => void;
  action: () => void;
};

const WarningDialog = ({
  title,
  description,
  actionBtnText,
  cancelBtnText,
  action,
  onCancel,
}: WarningDialogProps) => {
  return (
    <Dialog
      open
      header={<Typography preset="heading">{title}</Typography>}
      content={
        <YStack gap="$lg">
          <Typography preset="body1" color="$gray6">
            {description}
          </Typography>
        </YStack>
      }
      footer={
        <XStack gap="$sm" justifyContent="center">
          <Button preset="chromeless" textStyle={{ color: "black" }} onPress={onCancel}>
            {cancelBtnText}
          </Button>

          <Button backgroundColor="$red10" flex={1} onPress={action}>
            {actionBtnText}
          </Button>
        </XStack>
      }
    />
  );
};

export default WarningDialog;
