import React, { useEffect, useMemo } from "react";
import { OptionsSheetProps } from "./OptionsSheet";
import { Typography } from "./Typography";
import { useTranslation } from "react-i18next";
import { Sheet, XStack, YStack } from "tamagui";
import Input from "./Inputs/Input";
import { Animated, BackHandler, Keyboard, Platform } from "react-native";
import Button from "./Button";
import { Controller, useForm } from "react-hook-form";
import { useAddFeedbackMutation } from "../services/mutations/feedback/add-feedback.mutation";
import { useUserData } from "../contexts/user/UserContext.provider";
import Constants from "expo-constants";
import * as Device from "expo-device";
import Toast from "react-native-toast-message";
import { onlineManager } from "@tanstack/react-query";
import useAnimatedKeyboardPadding from "../hooks/useAnimatedKeyboardPadding";
import { Icon } from "./Icon";
import { KeyboardAwareScrollView } from "react-native-keyboard-aware-scroll-view";

const FeedbackSheet = (props: OptionsSheetProps) => {
  const { t } = useTranslation("more");
  const { activeElectionRound } = useUserData();
  const { mutate: addFeedback, isPending } = useAddFeedbackMutation(activeElectionRound?.id);

  const {
    control,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm({
    // reValidateMode: "onSubmit",
    defaultValues: {
      userFeedback: "",
    },
  });

  const onSheetClose = () => {
    Keyboard.dismiss();
    props.setOpen(false);
    reset();
  };

  // on Android back button press, if the feedbacksheet is open, we first close the sheet
  // and on the 2nd press we will navigate back
  useEffect(() => {
    if (Platform.OS !== "android") {
      return;
    }
    const onBackPress = () => {
      // close sheet
      if (props.open) {
        props.setOpen(false);
        return true;
      } else {
        // navigate back
        return false;
      }
    };
    const subscription = BackHandler.addEventListener("hardwareBackPress", onBackPress);
    return () => subscription.remove();
  }, [props.open, props.setOpen]);

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

  const paddingBottom = useAnimatedKeyboardPadding(16);
  const AnimatedYStack = useMemo(() => Animated.createAnimatedComponent(YStack), []);

  return (
    <Sheet
      modal
      {...props}
      zIndex={100_001}
      snapPointsMode="fit"
      dismissOnOverlayPress={false}
      disableDrag
    >
      <Sheet.Overlay />
      <Sheet.Frame
        borderTopLeftRadius={28}
        borderTopRightRadius={28}
        gap="$sm"
        paddingHorizontal="$md"
      >
        <Icon paddingVertical="$md" alignSelf="center" icon="dragHandle" />
        <KeyboardAwareScrollView keyboardShouldPersistTaps="handled">
          <AnimatedYStack padding="$md" paddingTop="$0" gap="$lg" paddingBottom={paddingBottom}>
            <Typography preset="heading" fontWeight="400">
              {t("feedback_sheet.heading")}
            </Typography>
            <Typography color="$gray6">{t("feedback_sheet.p1")}</Typography>
            <Controller
              key="userFeedback"
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
              <Button
                flex={1}
                disabled={isPending}
                onPress={() => {
                  Keyboard.dismiss();
                  handleSubmit(onSubmit)();
                }}
              >
                {t("feedback_sheet.action")}
              </Button>
            </XStack>
          </AnimatedYStack>
        </KeyboardAwareScrollView>
      </Sheet.Frame>
    </Sheet>
  );
};

export default FeedbackSheet;
