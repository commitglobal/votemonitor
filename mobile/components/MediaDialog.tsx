import React, { ReactNode } from "react";
import { AlertDialog, AlertDialogProps } from "tamagui";

interface DialogProps extends AlertDialogProps {
  // what you press on in order to open the dialog
  trigger?: ReactNode;
  // dialog header
  header?: ReactNode;
  // content inside dialog
  content?: ReactNode;
}

const MediaDialog: React.FC<DialogProps> = ({ header, content, trigger, ...props }) => {
  return (
    <AlertDialog {...props}>
      {trigger && <AlertDialog.Trigger asChild>{trigger}</AlertDialog.Trigger>}
      <AlertDialog.Portal>
        {/* backdrop for the modal */}
        <AlertDialog.Overlay
          key="overlay"
          animation="quick"
          opacity={1}
          enterStyle={{ opacity: 0 }}
          exitStyle={{ opacity: 0 }}
        />
        {/* the actual content inside the modal */}
        <AlertDialog.Content
          backgroundColor="white"
          style={{ padding: 0 }}
          width="90%"
          maxHeight="70%"
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
        </AlertDialog.Content>
      </AlertDialog.Portal>
    </AlertDialog>
  );
};

export default MediaDialog;
