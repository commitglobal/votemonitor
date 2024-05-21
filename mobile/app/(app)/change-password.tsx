import { ViewStyle } from "react-native";
import { Screen } from "../../components/Screen";
import Header from "../../components/Header";
import { Icon } from "../../components/Icon";
import { router } from "expo-router";
import { YStack } from "tamagui";
import Card from "../../components/Card";
import Button from "../../components/Button";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { Controller, useForm } from "react-hook-form";
import React, { useState } from "react";
import FormInput, { FormInputProps } from "../../components/FormInputs/FormInput";
import { useTranslation } from "react-i18next";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { useChangePasswordMutation } from "../../services/mutations/change-password.mutation";
import { ChangePasswordPayload } from "../../services/definitions.api";
import ChangePasswordConfirmation from "../../components/ChangePasswordConfirmation";

interface FormData {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

const PasswordInput = (props: FormInputProps): JSX.Element => {
  const [secureTextEntry, setSecureTextEntry] = useState(true);
  return (
    <FormInput
      {...props}
      secureTextEntry={secureTextEntry}
      iconRight={
        <Icon icon={secureTextEntry === false ? "eye" : "eyeOff"} size={20} color="$gray11" />
      }
      onIconRightPress={() => {
        setSecureTextEntry(!secureTextEntry);
      }}
    />
  );
};

const ChangePassword = () => {
  const insets = useSafeAreaInsets();
  const { t } = useTranslation("change_password");
  // Form validation schema
  const formSchema = z
    .object({
      currentPassword: z.string({
        required_error: t("form.current_password.required"),
      }),
      newPassword: z
        .string({
          required_error: t("form.new_password.required"),
        })
        .min(8, t("form.new_password.helper"))
        .regex(/^(?=.*[A-Z])(?=.*[0-9])(?=.*[^a-zA-Z0-9]).*$/, t("form.new_password.pattern")),
      confirmPassword: z.string({
        required_error: t("form.confirm_password.required"),
      }),
    })
    .refine((data) => data.newPassword === data.confirmPassword, {
      message: t("form.confirm_password.no_match"),
      path: ["confirmPassword"],
    });

  // React Hook Form setup
  const {
    handleSubmit,
    control,
    formState: { errors },
  } = useForm<FormData>({
    resolver: zodResolver(formSchema),
  });

  // Submit handler - change password
  const { mutate: updatePassword, isSuccess: successfullyChanged } = useChangePasswordMutation();

  const onSubmit = async (data: FormData) => {
    const payload: ChangePasswordPayload = {
      password: data.currentPassword.trim(),
      newPassword: data.newPassword.trim(),
      confirmNewPassword: data.confirmPassword.trim(),
    };
    updatePassword(payload, {
      onError(error) {
        console.error("error", error);
      },
    });
  };

  if (successfullyChanged) {
    return <ChangePasswordConfirmation emailConfirmation={false} />;
  }

  // Render either form or confirmation screen
  return (
    <Screen
      preset="auto"
      backgroundColor="white"
      ScrollViewProps={{
        bounces: false,
      }}
      contentContainerStyle={$containerStyle}
    >
      <Header
        title={t("header.title")}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      />
      <YStack gap="$xl" paddingHorizontal="$md" paddingTop={40}>
        <Controller
          key="currentPassword"
          name="currentPassword"
          control={control}
          render={({ field: { onChange, value } }) => (
            <PasswordInput
              error={errors.currentPassword?.message}
              key="currentPassword"
              type="password"
              title={t("form.current_password.label")}
              placeholder={t("form.current_password.placeholder")}
              value={value}
              onChangeText={onChange}
            />
          )}
        />
        <Controller
          key="newPassword"
          name="newPassword"
          control={control}
          render={({ field: { onChange, value } }) => (
            <PasswordInput
              error={errors.newPassword?.message}
              key="newPassword"
              type="password"
              title={t("form.new_password.label")}
              placeholder={t("form.new_password.placeholder")}
              helper={t("form.new_password.helper")}
              value={value}
              onChangeText={onChange}
            />
          )}
        />
        <Controller
          key="confirmPassword"
          name="confirmPassword"
          control={control}
          render={({ field: { onChange, value } }) => (
            <PasswordInput
              error={errors.confirmPassword?.message}
              key="confirmPassword"
              type="password"
              title={t("form.confirm_password.label")}
              placeholder={t("form.confirm_password.placeholder")}
              helper={t("form.confirm_password.helper")}
              value={value}
              onChangeText={onChange}
            />
          )}
        />
      </YStack>
      <Card width="100%" paddingBottom={16 + insets.bottom} marginTop="auto">
        <Button onPress={handleSubmit(onSubmit)}>{t("form.actions.save_password")}</Button>
      </Card>
    </Screen>
  );
};

const $containerStyle: ViewStyle = {
  flexGrow: 1,
};

export default ChangePassword;
