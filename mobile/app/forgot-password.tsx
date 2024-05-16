import Header from "../components/Header";
import { Screen } from "../components/Screen";
import { useTranslation } from "react-i18next";
import { Icon } from "../components/Icon";
import { router } from "expo-router";
import { XStack, YStack } from "tamagui";
import { Typography } from "../components/Typography";
import { useForm, Controller } from "react-hook-form";
import FormInput from "../components/FormInputs/FormInput";
import Card from "../components/Card";
import Button from "../components/Button";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { useState } from "react";
import ChangePasswordConfirmation from "../components/ChangePasswordConfirmation";
import { ForgotPasswwordPayload, forgotPassword } from "../services/definitions.api";
import * as Sentry from "@sentry/react-native";

type FormData = {
  email: string;
};

const ForgotPassword = () => {
  const insets = useSafeAreaInsets();
  const { t } = useTranslation("reset");
  const [emailConfirmation, setEmailConfirmation] = useState(false);
  const [authError, setAuthError] = useState(false);

  // React Hook form
  const { handleSubmit, control, formState } = useForm<FormData>({
    defaultValues: { email: "avant.arnez@milkgitter.com" },
  });
  const { errors } = formState;

  // Submit handler - forgot password
  const onSubmit = async (data: FormData) => {
    try {
      const payload: ForgotPasswwordPayload = { email: data.email };
      await forgotPassword(payload);
      setEmailConfirmation(true);
    } catch (error) {
      Sentry.captureException(error);
      setAuthError(true);
    }
  };

  if (emailConfirmation) {
    return <ChangePasswordConfirmation emailConfirmation={true} />;
  }

  return (
    <Screen
      preset="auto"
      ScrollViewProps={{
        bounces: false,
      }}
      contentContainerStyle={{
        flexGrow: 1,
      }}
    >
      <Header
        title={t("header.title")}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      />

      <YStack paddingHorizontal="$md" gap="$md" paddingTop={10 + insets.top}>
        <Typography preset="heading" fontWeight="700">
          {t("title")}
        </Typography>

        <Typography>{t("paragraph")}</Typography>
        {authError && <CredentialsError />}

        <Controller
          key="email"
          name="email"
          control={control}
          rules={{
            required: {
              value: true,
              message: t("form.email.required"),
            },
            pattern: {
              value: /\S+@\S+\.\S+/,
              message: t("form.email.format"),
            },
          }}
          render={({ field: { onChange, value } }) => (
            <FormInput
              type="text"
              title={t("form.email.label")}
              placeholder={t("form.email.placeholder")}
              value={value}
              onChangeText={onChange}
              error={errors?.email?.message?.toString()}
            />
          )}
        />
      </YStack>

      <Card width="100%" paddingBottom={16 + insets.bottom} marginTop="auto">
        <Button onPress={handleSubmit(onSubmit)}>{t("actions.send")}</Button>
      </Card>
    </Screen>
  );
};

const CredentialsError = () => {
  const { t } = useTranslation("login");
  return (
    <XStack
      backgroundColor="$red1"
      borderRadius={6}
      justifyContent="flex-start"
      padding="$md"
      alignItems="flex-start"
    >
      <Icon icon="loginError" size={20} />
      <Typography paddingHorizontal="$md" color="$red6" fontWeight="500">
        Email incorect
      </Typography>
    </XStack>
  );
};

export default ForgotPassword;
