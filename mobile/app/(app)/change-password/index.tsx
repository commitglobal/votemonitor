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

const formSchema = z
  .object({
    currentPassword: z.string(),
    newPassword: z
      .string()
      .min(8, "Password must be at least 8 characters long")
      .regex(
        /^(?=.*[A-Z])(?=.*[0-9])(?=.*[^a-zA-Z0-9]).*$/,
        "Password should have at least one uppercase character, one number, one special character, and be minimum 8 characters long without spaces",
      ),
    confirmPassword: z.string(),
  })
  .refine((data) => data.newPassword === data.confirmPassword, {
    message: "Passwords do not match",
    path: ["confirmPassword"],
  });

const ChangePassowrd = () => {
  const insets = useSafeAreaInsets();

  const { handleSubmit, control, formState } = useForm<FormData>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      currentPassword: "",
      newPassword: "Votemonitor1*",
      confirmPassword: "Votemonitor1",
    },
  });
  const { errors } = formState;
  const { t } = useTranslation("change_password");

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
  const [secureTextEntry, setSecureTextEntry] = React.useState(false);
  const passIcon = secureTextEntry === false ? "eye" : "eyeOff";
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
              secureTextEntry={secureTextEntry}
              title={t("form.current_password.label")}
              placeholder={t("form.current_password.placeholder")}
              value={value}
              onChangeText={onChange}
              iconRight={<Icon icon={passIcon} size={20} color="$gray11" />}
              onIconRightPress={() => {
                setSecureTextEntry(!secureTextEntry);
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
              secureTextEntry={secureTextEntry}
              title={t("form.new_password.label")}
              placeholder={t("form.new_password.placeholder")}
              value={value}
              onChangeText={onChange}
              iconRight={<Icon icon={passIcon} size={20} color="$gray11" />}
              onIconRightPress={() => {
                setSecureTextEntry(!secureTextEntry);
              }}
            ></FormInput>

            {(errors?.confirmPassword?.message &&
              helperText({
                error: true,
                message: errors?.newPassword?.message?.toString() ?? "",
              })) ||
              helperText({ error: false, message: "Must be at least 8 characters" })}
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
              secureTextEntry={secureTextEntry}
              title={t("form.confirm_password.label")}
              placeholder={t("form.confirm_password.placeholder")}
              value={value}
              onChangeText={onChange}
              iconRight={<Icon icon={passIcon} size={20} color="$gray11" />}
              onIconRightPress={() => {
                setSecureTextEntry(!secureTextEntry);
              }}
            ></FormInput>

            {(errors?.confirmPassword?.message &&
              helperText({
                error: true,
                message: errors?.confirmPassword?.message?.toString() ?? "",
              })) ||
              helperText({ error: false, message: "Both passwords must match." })}
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
