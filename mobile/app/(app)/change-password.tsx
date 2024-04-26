import { ViewStyle } from "react-native";
import { Screen } from "../../components/Screen";
import Header from "../../components/Header";
import { Icon } from "../../components/Icon";
import { router } from "expo-router";
import { YStack } from "tamagui";
import Card from "../../components/Card";
import Button from "../../components/Button";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { Control, Controller, useForm } from "react-hook-form";
import React from "react";
import { Typography } from "../../components/Typography";
import FormInput from "../../components/FormInputs/FormInput";
import { useTranslation } from "react-i18next";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import ChangePasswordConfirmation from "./change-password-confirmation";
import { useChangePasswordMutation } from "../../services/mutations/change-password.mutation";
import { ChangePasswordPayload } from "../../services/definitions.api";

interface FormData {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

const ChangePassowrd = () => {
  const insets = useSafeAreaInsets();
  const { t } = useTranslation("change_password");

  // Form validation schema
  const formSchema = z
    .object({
      currentPassword: z.string().min(1, t("form.current_password.required")),
      newPassword: z
        .string()
        .min(8, t("form.new_password.helper"))
        .regex(/^(?=.*[A-Z])(?=.*[0-9])(?=.*[^a-zA-Z0-9]).*$/, t("form.new_password.pattern")),
      confirmPassword: z.string(),
    })
    .refine((data) => data.newPassword === data.confirmPassword, {
      message: t("form.confirm_password.no_match"),
      path: ["confirmPassword"],
    });

  // React Hook Form setup
  const { handleSubmit, control, formState } = useForm<FormData>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      currentPassword: "Votemonitor1*",
      newPassword: "Votemonitor1*",
      confirmPassword: "Votemonitor1*",
    },
  });
  const { errors } = formState;

  // Submit handler - change password
  const {
    mutate: updatePassword,
    isError: credentialsError,
    isSuccess: succesfullyChanged,
  } = useChangePasswordMutation();

  const onSubmit = async (data: FormData) => {
    const payload: ChangePasswordPayload = {
      password: data.currentPassword,
      newPassword: data.newPassword,
      confirmNewPassword: data.confirmPassword,
    };
    updatePassword(payload);
  };

  // Render either form or confirmation screen
  return succesfullyChanged ? (
    <ChangePasswordConfirmation />
  ) : (
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

      <YStack gap="$xl" paddingHorizontal={16} paddingVertical={40}>
        <PasswordInput
          formKey="currentPassword"
          control={control}
          helper={""}
          error={
            (errors?.currentPassword?.message?.toString() ?? "") ||
            (credentialsError ? t("form.current_password.credentials_error") : "")
          }
          label={t("form.current_password.label")}
          placeholder={t("form.current_password.placeholder")}
          hasError={!!errors?.currentPassword || credentialsError}
          name="currentPassword"
        />

        <PasswordInput
          formKey="newPassword"
          control={control}
          helper={t("form.new_password.helper")}
          error={errors?.newPassword?.message?.toString() ?? ""}
          label={t("form.new_password.label")}
          placeholder={t("form.new_password.placeholder")}
          hasError={!!errors?.newPassword}
          name="newPassword"
        />

        <PasswordInput
          formKey="confirmPassword"
          control={control}
          helper={t("form.confirm_password.helper")}
          error={errors?.confirmPassword?.message?.toString() ?? ""}
          label={t("form.confirm_password.label")}
          placeholder={t("form.confirm_password.placeholder")}
          hasError={!!errors?.confirmPassword}
          name="confirmPassword"
        />
      </YStack>

      <Card width="100%" paddingBottom={16 + insets.bottom} marginTop="auto">
        <Button onPress={handleSubmit(onSubmit)}>{t("form.actions.save_password")}</Button>
      </Card>
    </Screen>
  );
};

interface PasswordInputProps {
  control: Control<FormData, any>;
  helper: string;
  error: string;
  label: string;
  placeholder: string;
  hasError: boolean;
  name: keyof FormData;
  formKey: string;
}

const PasswordInput = (props: PasswordInputProps) => {
  const { control, helper, error, label, placeholder, hasError, name, formKey } = props;

  const [secureTextEntry, setSecureTextEntry] = React.useState(false);
  const passIcon = secureTextEntry === false ? "eye" : "eyeOff";

  return (
    <Controller
      key={formKey}
      name={name}
      control={control}
      render={({ field: { onChange, value } }) => (
        <YStack>
          <FormInput
            borderColor={hasError ? "$red7" : "$gray11"}
            key="currentPassword"
            type="password"
            secureTextEntry={secureTextEntry}
            title={label}
            placeholder={placeholder}
            value={value}
            onChangeText={onChange}
            iconRight={<Icon icon={passIcon} size={20} color="$gray11" />}
            onIconRightPress={() => {
              setSecureTextEntry(!secureTextEntry);
            }}
          ></FormInput>

          {!hasError && <Typography color="gray">{helper}</Typography>}
          {hasError && <Typography color="$red7">{error}</Typography>}
        </YStack>
      )}
    />
  );
};

const $containerStyle: ViewStyle = {
  flexGrow: 1,
};

export default ChangePassowrd;
