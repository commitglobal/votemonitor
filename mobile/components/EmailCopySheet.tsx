import React, { Dispatch, SetStateAction, useState } from "react";
import { Sheet, XStack, YStack } from "tamagui";
import { Icon } from "./Icon";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { useTranslation } from "react-i18next";
import { Typography } from "./Typography";
import { Controller, useForm } from "react-hook-form";
import FormInput from "./FormInputs/FormInput";
import Button from "./Button";
import { useCitizenUserData } from "../contexts/citizen-user/CitizenUserContext.provider";
import { usePutCitizenReportCopyMutation } from "../services/mutations/citizen/put-citizen-report-copy..mutation";

export const EmailCopySheet = ({
  submissionId,
  formId,
  setIsEmailSheetOpen,
  setCopySent,
}: {
  submissionId: string | undefined;
  formId: string | undefined;
  setIsEmailSheetOpen: Dispatch<SetStateAction<boolean>>;
  setCopySent: Dispatch<SetStateAction<boolean>>;
}) => {
  const { t } = useTranslation("citizen_form");

  const insets = useSafeAreaInsets();

  const [isSheetDraggable, setIsSheetDraggable] = useState(true);

  const { selectedElectionRound } = useCitizenUserData();
  const { mutate: sendEmailCopy, isPending } = usePutCitizenReportCopyMutation();

  const {
    control,
    handleSubmit,
    formState: { errors },
  } = useForm();

  const onSubmit = (formValues: any) => {
    if (!selectedElectionRound || !submissionId || !formId) {
      console.log("⛔️ Missing data for sending email copy. ⛔️");
      return;
    }

    sendEmailCopy(
      {
        electionRoundId: selectedElectionRound,
        citizenReportId: submissionId,
        reqBody: {
          formId,
          email: formValues.email,
        },
      },
      {
        onSuccess: () => {
          setCopySent(true);
        },
        onSettled: () => {
          setIsEmailSheetOpen(false);
        },
      },
    );
  };

  return (
    <Sheet
      modal
      open
      zIndex={100_001}
      snapPoints={[50]}
      dismissOnOverlayPress
      moveOnKeyboardChange
      onOpenChange={(open: boolean) => {
        if (!open) {
          setIsEmailSheetOpen(false);
        }
      }}
      disableDrag={!isSheetDraggable}
    >
      <Sheet.Overlay />
      <Sheet.Frame>
        <Icon paddingVertical="$md" alignSelf="center" icon="dragHandle" />
        <YStack flex={1} marginBottom={insets.bottom + 16}>
          <Sheet.ScrollView
            contentContainerStyle={{ flexGrow: 1, paddingHorizontal: 24, gap: 16 }}
            bounces={false}
            showsVerticalScrollIndicator={false}
            keyboardShouldPersistTaps="handled"
          >
            <Typography preset="heading">{t("email_copy.title")}</Typography>
            <Typography color="$gray5">{t("email_copy.description")}</Typography>

            <Controller
              control={control}
              name="email"
              rules={{
                required: {
                  value: true,
                  message: t("email_copy.required"),
                },
                // todo: maybe use a schema validation for this
                pattern: {
                  value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i,
                  message: t("email_copy.invalid"),
                },
              }}
              render={({ field: { onChange, value } }) => (
                <FormInput
                  type="email-address"
                  value={value}
                  onChangeText={onChange}
                  placeholder={t("email_copy.placeholder")}
                  error={errors.email?.message as string | undefined}
                  onFocus={() => setIsSheetDraggable(false)}
                  onBlur={() => setIsSheetDraggable(true)}
                />
              )}
            />

            <XStack marginTop="auto">
              <Button preset="chromeless" onPress={() => setIsEmailSheetOpen(false)}>
                {t("email_copy.cancel")}
              </Button>
              <Button flex={1} onPress={handleSubmit(onSubmit)} disabled={isPending}>
                {t("email_copy.send")}
              </Button>
            </XStack>
          </Sheet.ScrollView>
        </YStack>
      </Sheet.Frame>
    </Sheet>
  );
};
