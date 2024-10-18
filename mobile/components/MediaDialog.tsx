import { Image } from "expo-image";
import React, { ReactNode, useEffect, useState } from "react";
import { AlertDialog, AlertDialogProps, Spinner, YStack } from "tamagui";
import { Typography } from "./Typography";
import { useTranslation } from "react-i18next";
import { BackHandler } from "react-native";

interface MediaDialogProps extends AlertDialogProps {
  // what you press on in order to open the dialog
  trigger?: ReactNode;
  // dialog header
  media?: { type: string; src: string };
  onClose: () => void;
}

export const MediaDialog: React.FC<MediaDialogProps> = ({ trigger, media, onClose, ...props }) => {
  const { t } = useTranslation("common");
  const [isLoaded, setIsLoaded] = useState(false);

  const handleHardwareBackPress = () => {
    onClose();
    return true;
  };

  useEffect(() => {
    const backHandler = BackHandler.addEventListener("hardwareBackPress", handleHardwareBackPress);

    return () => backHandler.remove();
  }, []);

  return (
    <AlertDialog open {...props}>
      {trigger && <AlertDialog.Trigger asChild>{trigger}</AlertDialog.Trigger>}
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
          paddingTop="$xl"
          width="95%"
          height="65%"
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
          <YStack flex={1} backgroundColor={"white"}>
            {media && (
              <>
                {!isLoaded && (
                  <YStack flex={1} justifyContent="center" alignItems="center">
                    <Spinner size="large" color="$purple5" />
                  </YStack>
                )}

                <Image
                  source={media.src}
                  contentFit="contain"
                  style={{
                    position: "absolute",
                    width: "100%",
                    height: "100%",
                    opacity: isLoaded ? 1 : 0,
                  }}
                  onLoad={() => setIsLoaded(true)}
                />
              </>
            )}
          </YStack>

          <YStack
            padding="$xxs"
            justifyContent="center"
            alignItems="center"
            paddingVertical="$md"
            paddingTop="$xl"
            onPress={onClose}
            pressStyle={{ opacity: 0.5 }}
          >
            <Typography preset="body2" color="$purple5">
              {t("cancel")}
            </Typography>
          </YStack>
        </AlertDialog.Content>
      </AlertDialog.Portal>
    </AlertDialog>
  );
};
