import React, { useState } from "react";
import OptionsSheet, { OptionsSheetProps } from "./OptionsSheet";
import { Typography } from "./Typography";
import { useTranslation } from "react-i18next";
import { XStack, YStack } from "tamagui";
import Input from "./Inputs/Input";
import { useKeyboardVisible } from "@tamagui/use-keyboard-visible";
import { Keyboard, Platform } from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import Button from "./Button";
import { Controller, useForm } from "react-hook-form";
import { useAddFeedbackMutation } from "../services/mutations/feedback/add-feedback.mutation";
import { useUserData } from "../contexts/user/UserContext.provider";
import Constants from "expo-constants";
import * as Device from "expo-device";
import Toast from "react-native-toast-message";
import { onlineManager } from "@tanstack/react-query";

const FeedbackSheet = (props: OptionsSheetProps) => {
  const { t } = useTranslation("more");
  const insets = useSafeAreaInsets();
  const keyboardIsVisible = useKeyboardVisible();
  const [writingFeedback, setWritingFeedback] = useState(false);
  const { activeElectionRound } = useUserData();

  const { mutate: addFeedback, isPending } = useAddFeedbackMutation(activeElectionRound?.id);

  const {
    control,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm({
    defaultValues: {
      userFeedback: "",
    },
  });

  const onSheetClose = () => {
    Keyboard.dismiss();
    props.setOpen(false);
    reset();
  };

  const onSubmit = ({ userFeedback }: { userFeedback: string }) => {
    const feedbackPayload = {
      electionRoundId: activeElectionRound?.id,
      userFeedback,
      metadata: {
        appVersion: Constants.expoConfig?.version,
        sentAt: new Date().toLocaleString(),
        platform: Platform.OS,
        modelName: Device.modelName,
        electionRoundId: activeElectionRound?.id,
        systemVersion: Device.osVersion,
      },
    };
    addFeedback(feedbackPayload, {
      onSuccess: () => {
        onSheetClose();
        Toast.show({
          type: "success",
          text2: t("feedback_toast.success"),
        });
      },
      onError: () => {
        Toast.show({
          type: "error",
          text2: t("feedback_toast.error"),
        });
      },
    });

    if (!onlineManager.isOnline()) {
      onSheetClose();
    }
  };

  return (
    <OptionsSheet
      {...props}
      dismissOnOverlayPress={false}
      moveOnKeyboardChange={Platform.OS === "android"}
      disableDrag={writingFeedback}
    >
      <YStack
        padding="$md"
        paddingTop="$0"
        gap="$lg"
        paddingBottom={
          // add padding if keyboard is visible
          Platform.OS === "ios" && keyboardIsVisible && Keyboard.metrics()?.height
            ? // @ts-ignore: it will not be undefined because we're checking above
              Keyboard.metrics()?.height - insets.bottom
            : 0
        }
      >
        <Typography preset="heading" fontWeight="400">
          {t("feedback_sheet.heading")}
        </Typography>
        <Typography color="$gray6">{t("feedback_sheet.p1")}</Typography>
        <Controller
          name={"userFeedback"}
          control={control}
          rules={{
            required: {
              value: true,
              message: t("feedback_toast.required"),
            },
            maxLength: {
              value: 10000,
              message: t("feedback_sheet.input.max", { value: 10000 }),
            },
          }}
          render={({ field: { value, onChange } }) => {
            return (
              <YStack gap="$xxs">
                <YStack height={100}>
                  <Input
                    type="textarea"
                    placeholder={t("feedback_sheet.placeholder")}
                    value={value}
                    height={100}
                    onChangeText={onChange}
                    // use this in order to not allow dragging of the sheet while input is focused (causes trouble on android)
                    onFocus={() => setWritingFeedback(true)}
                    onBlur={() => {
                      Keyboard.dismiss();
                      setWritingFeedback(false);
                    }}
                  />
                </YStack>
                {errors.userFeedback && (
                  <Typography color="$red12">{errors.userFeedback.message}</Typography>
                )}
              </YStack>
            );
          }}
        />

        {/* buttons */}
        <XStack gap="$md">
          <Button preset="chromeless" onPress={onSheetClose}>
            {t("feedback_sheet.cancel")}
          </Button>
          <Button flex={1} disabled={isPending} onPress={handleSubmit(onSubmit)}>
            {t("feedback_sheet.action")}
          </Button>
        </XStack>
      </YStack>
    </OptionsSheet>
  );
};

export default FeedbackSheet;
