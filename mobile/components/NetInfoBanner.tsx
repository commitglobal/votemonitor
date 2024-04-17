import React from "react";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { AlertDialog, XStack } from "tamagui";
import { Typography } from "./Typography";
import { Dialog } from "./Dialog";
import Button from "./Button";
import { useNetInfoContext } from "../contexts/net-info-banner/NetInfoContext";

const NetInfoBanner = () => {
  const { isOnline, shouldDisplayBanner } = useNetInfoContext();
  const insets = useSafeAreaInsets();

  return (
    <>
      {shouldDisplayBanner &&
        (isOnline ? (
          <XStack
            backgroundColor="$green7"
            padding="$xxs"
            justifyContent="center"
            alignItems="center"
            width="100%"
            zIndex={100_000}
          >
            <Typography
              fontWeight="500"
              color="$gray7"
              paddingBottom={insets.bottom}
              textAlign="center"
            >
              App online. All answers sent to server.
            </Typography>
          </XStack>
        ) : (
          <XStack
            backgroundColor="$red9"
            paddingBottom={insets.bottom}
            justifyContent="space-between"
            alignItems="center"
            width="100%"
            zIndex={100_000}
          >
            <Typography fontWeight="500" color="white" paddingLeft="$md">
              Offline mode. Saving answers locally.
            </Typography>

            <Dialog
              trigger={
                <XStack gap="$sm" justifyContent="center" onPress={() => console.log("open modal")}>
                  <Typography
                    fontWeight="700"
                    color="white"
                    paddingVertical="$xxs"
                    paddingLeft="$lg"
                    paddingRight="$md"
                  >
                    More Details
                  </Typography>
                </XStack>
              }
              content={
                <XStack gap="$sm" justifyContent="center">
                  <Typography preset="body1" color="$gray7" lineHeight={24}>
                    It looks like you're currently offline. Don't worry, your answers are safely
                    stored locally and will be synced with our servers once your connection is
                    restored. Feel free to continue filling out the observation forms and submitting
                    quick reports.
                  </Typography>
                </XStack>
              }
              footer={
                <XStack gap="$sm" justifyContent="center">
                  {/*  !this 'asChild' is necessary in order to close the modal */}
                  <AlertDialog.Cancel asChild>
                    <Button preset="chromeless" textStyle={{ color: "red" }}>
                      Cancel
                    </Button>
                    {/* <Button title="Cancel" />  - WITH BUTTON FROM REACT NATIVE TO SUPPRES React.ForwardRef warning */}
                  </AlertDialog.Cancel>
                </XStack>
              }
            ></Dialog>
          </XStack>
        ))}
    </>
  );
};
export default NetInfoBanner;
