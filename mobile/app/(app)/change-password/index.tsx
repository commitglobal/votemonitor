import { ViewStyle } from "react-native";
import { Screen } from "../../../components/Screen";
import Header from "../../../components/Header";
import { Icon } from "../../../components/Icon";
import { router } from "expo-router";
import { YStack } from "tamagui";
import Card from "../../../components/Card";
import Button from "../../../components/Button";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { Control, Controller, FieldErrors, FieldValues, useForm } from "react-hook-form";
import React from "react";
import { Typography } from "../../../components/Typography";
import FormInput from "../../../components/FormInputs/FormInput";
import { useTranslation } from "react-i18next";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";

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
      currentPassword: z.string(),
      newPassword: z
        .string()
        .min(8, t("form.new_password.helper"))
        .regex(/^(?=.*[A-Z])(?=.*[0-9])(?=.*[^a-zA-Z0-9]).*$/, t("form.new_password.pattern")),
      confirmPassword: z.string(),
    })
    .refine((data) => data.newPassword === data.confirmPassword, {
      message: t("form.new_password.no_match"),
      path: ["confirmPassword"],
    });

  // React Hook Form setup
  const { handleSubmit, control, formState } = useForm<FormData>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      currentPassword: "",
      newPassword: "Votemonitor1",
      confirmPassword: "Votemonitor1",
    },
  });
  const { errors } = formState;

  // TODO: Add API call to change password
  const onSubmit = (data: FormData) => {
    console.log("Change password: ", data);
  };

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

      <Form control={control} errors={errors}></Form>

      <Card width="100%" paddingBottom={16 + insets.bottom} marginTop="auto">
        <Button onPress={handleSubmit(onSubmit)}>{t("form.actions.save_password")}</Button>
      </Card>
    </Screen>
  );
};

const Form = ({
  control,
  errors,
}: {
  control: Control<FormData, any>;
  errors: FieldErrors<FieldValues>;
}) => {
  const [passSecureEntry, setPassSecureEntry] = React.useState(false);
  const passIcon = passSecureEntry === false ? "eye" : "eyeOff";

  const [newPassSecureEntry, setNewPassSecureEntry] = React.useState(false);
  const newPassIcon = newPassSecureEntry === false ? "eye" : "eyeOff";

  const [confirmPassSecureEntry, setConfirmPassSecureEntry] = React.useState(false);
  const confirmPassIcon = confirmPassSecureEntry === false ? "eye" : "eyeOff";

  const { t } = useTranslation("change_password");

  return (
    <YStack gap={32} paddingHorizontal={16} paddingVertical={32}>
      <Controller
        key="currentPassword"
        name="currentPassword"
        control={control}
        render={({ field: { onChange, value } }) => (
          <YStack>
            <FormInput
              type="password"
              secureTextEntry={passSecureEntry}
              title={t("form.current_password.label")}
              placeholder={t("form.current_password.placeholder")}
              value={value}
              onChangeText={onChange}
              iconRight={<Icon icon={passIcon} size={20} color="$gray11" />}
              onIconRightPress={() => {
                setPassSecureEntry(!passSecureEntry);
              }}
            ></FormInput>
          </YStack>
        )}
      />

      <Controller
        key="newPassword"
        name="newPassword"
        control={control}
        render={({ field: { onChange, value } }) => (
          <YStack>
            <FormInput
              type="password"
              secureTextEntry={newPassSecureEntry}
              title={t("form.new_password.label")}
              placeholder={t("form.new_password.placeholder")}
              value={value}
              onChangeText={onChange}
              iconRight={<Icon icon={passIcon} size={20} color="$gray11" />}
              onIconRightPress={() => {
                setNewPassSecureEntry(!newPassSecureEntry);
              }}
            ></FormInput>

            {(errors?.confirmPassword?.message &&
              helperText({
                error: true,
                message: errors?.newPassword?.message?.toString() ?? "",
              })) ||
              helperText({ error: false, message: t("form.new_password.helper") })}
          </YStack>
        )}
      />

      <Controller
        key="confirmPassword"
        name="confirmPassword"
        control={control}
        render={({ field: { onChange, value } }) => (
          <YStack>
            <FormInput
              type="password"
              secureTextEntry={confirmPassSecureEntry}
              title={t("form.confirm_password.label")}
              placeholder={t("form.confirm_password.placeholder")}
              value={value}
              onChangeText={onChange}
              iconRight={<Icon icon={passIcon} size={20} color="$gray11" />}
              onIconRightPress={() => {
                setConfirmPassSecureEntry(!confirmPassSecureEntry);
              }}
            ></FormInput>

            {(errors?.confirmPassword?.message &&
              helperText({
                error: true,
                message: errors?.confirmPassword?.message?.toString() ?? "",
              })) ||
              helperText({ error: false, message: t("form.confirm_password.helper") })}
          </YStack>
        )}
      />
    </YStack>
  );
};

const helperText = ({ error, message }: { error: boolean; message: string }) => {
  return (
    <Typography color={error ? "$red5" : "gray"} marginTop="$xs">
      {message}
    </Typography>
  );
};

const $containerStyle: ViewStyle = {
  flexGrow: 1,
};

export default ChangePassowrd;
