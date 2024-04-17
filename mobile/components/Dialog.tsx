import React, { ReactNode } from "react";
import { AlertDialog, AlertDialogProps, Stack } from "tamagui";

interface DialogProps extends AlertDialogProps {
  // what you press on in order to open the dialog
  trigger: ReactNode;
  // dialog header
  header?: ReactNode;
  // content inside dialog
  content?: ReactNode;
  // dialog footer
  footer: ReactNode;
}

export const Dialog: React.FC<DialogProps> = ({ trigger, header, content, footer }) => {
  return (
    <AlertDialog>
      {/* the button we press on to open the modal */}
      <AlertDialog.Trigger asChild>{trigger}</AlertDialog.Trigger>

      <AlertDialog.Portal>
        {/* backdrop for the modal */}
        <AlertDialog.Overlay
          key="overlay"
          animation="quick"
          opacity={0.7}
          enterStyle={{ opacity: 0 }}
          exitStyle={{ opacity: 0 }}
        />
        {/* the actual content inside the modal */}
        <AlertDialog.Content
          backgroundColor="white"
          paddingTop="$lg"
          paddingHorizontal="$lg"
          width="85%"
          maxHeight="80%"
          elevate
          key="content"
          animation={[
            "quick",
            {
              opacity: {
                overshootClamping: true,
              },
            },
          ]}
          enterStyle={{ x: 0, y: -20, opacity: 0, scale: 0.9 }}
          exitStyle={{ x: 0, y: 10, opacity: 0, scale: 0.95 }}
          x={0}
          scale={1}
          opacity={1}
          y={0}
          gap="$md"
        >
          {header}
          {content}
          <Stack marginTop="$sm">{footer}</Stack>
        </AlertDialog.Content>
      </AlertDialog.Portal>
    </AlertDialog>
  );
};
