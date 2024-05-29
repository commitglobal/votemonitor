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

const FeedbackSheet = (props: OptionsSheetProps) => {
  const { t } = useTranslation("more");
  const insets = useSafeAreaInsets();
  const keyboardIsVisible = useKeyboardVisible();
  const [writingFeedback, setWritingFeedback] = useState(false);

  const {
    control,
    handleSubmit,
    formState: { errors },
  } = useForm({
    defaultValues: {
      feedbackText: "",
    },
  });

  return (
    <OptionsSheet
      {...props}
      onOpenChange={(open: boolean) => {
        if (!open) Keyboard.dismiss();
        props.setOpen(open);
      }}
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
        {/* <Input type="textarea" /> */}
        <Controller
          name={"feedbackText"}
          control={control}
          rules={{
            maxLength: {
              value: 2,
              message: t("feedback_sheet.input.max", { value: 2 }),
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
                    //use this in order to not allow dragging of the sheet while input is focused (causes trouble on android)
                    onFocus={() => setWritingFeedback(true)}
                    onBlur={() => setWritingFeedback(false)}
                  />
                </YStack>
                {errors.feedbackText && (
                  <Typography color="$red12">{errors.feedbackText.message}</Typography>
                )}
              </YStack>
            );
          }}
        />

        {/* buttons */}
        <XStack gap="$md">
          <Button
            preset="chromeless"
            onPress={() => {
              Keyboard.dismiss();
              props.setOpen(false);
            }}
          >
            {t("feedback_sheet.cancel")}
          </Button>
          <Button flex={1} onPress={handleSubmit(() => console.log("send to backend"))}>
            {t("feedback_sheet.action")}
          </Button>
        </XStack>
      </YStack>
    </OptionsSheet>
  );
};

export default FeedbackSheet;
