import React from "react";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { AlertDialog, ScrollView, XStack, YStack } from "tamagui";
import { Typography } from "./Typography";
import { Dialog } from "./Dialog";
import Button from "./Button";
import { useNetInfoContext } from "../contexts/net-info-banner/NetInfoContext";
import { useTranslation } from "react-i18next";
import { AppMode, useAppMode } from "../contexts/app-mode/AppModeContext.provider";

const NetInfoBanner = () => {
  const { isOnline, shouldDisplayBanner } = useNetInfoContext();
  const { appMode } = useAppMode();
  const insets = useSafeAreaInsets();
  const { t } = useTranslation(["network_banner", "common"]);

  return (
    <>
      {isOnline ? (
        shouldDisplayBanner && (
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
              {t("online")}
            </Typography>
          </XStack>
        )
      ) : (
        <XStack
          backgroundColor="$red9"
          paddingBottom={insets.bottom}
          justifyContent="space-between"
          alignItems="center"
          width="100%"
          zIndex={100_000}
        >
          <YStack paddingLeft="$md" maxWidth="65%" paddingVertical="$xxs">
            <Typography fontWeight="500" color="white" numberOfLines={2}>
              {appMode === AppMode.CITIZEN ? t("offline_citizen") : t("offline")}
            </Typography>
          </YStack>

          <Dialog
            trigger={
              <XStack
                gap="$sm"
                justifyContent="center"
                pressStyle={{ opacity: 0.5 }}
                maxWidth="35%"
              >
                <Typography
                  fontWeight="700"
                  textAlign="center"
                  color="white"
                  paddingVertical="$xxs"
                  paddingLeft="$lg"
                  paddingRight="$md"
                >
                  {t("more_details")}
                </Typography>
              </XStack>
            }
            content={
              <ScrollView bounces={false} showsVerticalScrollIndicator={false}>
                <XStack gap="$sm" justifyContent="center">
                  <Typography preset="body1" color="$gray7" lineHeight={24}>
                    {appMode === AppMode.CITIZEN
                      ? t("offline_warning_citizen")
                      : t("offline_warning")}
                  </Typography>
                </XStack>
              </ScrollView>
            }
            footer={
              <XStack gap="$sm" justifyContent="center">
                {/*  !this 'asChild' is necessary in order to close the modal */}
                <AlertDialog.Cancel asChild>
                  <Button preset="chromeless" textStyle={{ color: "red" }}>
                    {t("cancel", { ns: "common" })}
                  </Button>
                  {/* <Button title="Cancel" />  - WITH BUTTON FROM REACT NATIVE TO SUPPRES React.ForwardRef warning */}
                </AlertDialog.Cancel>
              </XStack>
            }
          ></Dialog>
        </XStack>
      )}
    </>
  );
};
export default NetInfoBanner;
