import React, { ReactNode } from "react";
import { AlertDialog, TextArea, XStack, YStack } from "tamagui";
import Button from "../components/Button";
import { KeyboardAvoidingView } from "react-native";
// import { KeyboardAvoidingView } from "react-native";

export const Dialog = ({ children }: { children: ReactNode }) => {
  return (
    <AlertDialog>
      <AlertDialog.Trigger asChild>
        <Button>Show Alert</Button>
      </AlertDialog.Trigger>
      <KeyboardAvoidingView>
        <AlertDialog.Portal>
          <AlertDialog.Overlay
            key="overlay"
            animation="quick"
            opacity={0.7}
            enterStyle={{ opacity: 0 }}
            exitStyle={{ opacity: 0 }}
          />
          <AlertDialog.Content
            width="85%"
            bordered
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
          >
            <YStack>
              {children}
              <TextArea />
              <TextArea />
              <TextArea />
              <TextArea />
              <TextArea />
              <TextArea />
              <XStack>
                <AlertDialog.Cancel asChild>
                  <Button preset="chromeless">Cancel</Button>
                </AlertDialog.Cancel>
                <Button flex={1}>Save</Button>
              </XStack>
            </YStack>
          </AlertDialog.Content>
        </AlertDialog.Portal>
      </KeyboardAvoidingView>
    </AlertDialog>
  );
};
