import Header from "../../components/Header";
import { Screen } from "../../components/Screen";
import { useTranslation } from "react-i18next";
import { Icon } from "../../components/Icon";
import { router } from "expo-router";
import { ScrollView, YStack } from "tamagui";
import { Typography } from "../../components/Typography";
import { useForm, Controller } from "react-hook-form";
import FormInput from "../../components/FormInputs/FormInput";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { useState } from "react";
import PasswordConfirmationScreen from "../../components/PasswordConfirmationScreen";
import { ForgotPasswwordPayload, forgotPassword } from "../../services/definitions.api";
import * as Sentry from "@sentry/react-native";
import CredentialsError from "../../components/CredentialsError";
import WizzardControls from "../../components/WizzardControls";

type FormData = {
  email: string;
};

const ForgotPassword = () => {
  const insets = useSafeAreaInsets();
  const { t } = useTranslation(["forgot_password", "generic_error_screen"]);
  const [isLoading, setIsLoading] = useState(false);
  const [emailConfirmation, setEmailConfirmation] = useState(false);
  const [authError, setAuthError] = useState(false);

  // React Hook form
  const {
    handleSubmit,
    control,
    formState: { errors },
  } = useForm<FormData>({});

  // Submit handler - forgot password
  const onSubmit = async (data: FormData) => {
    try {
      setIsLoading(true);
      const payload: ForgotPasswwordPayload = { email: data.email };
      await forgotPassword(payload);
      setEmailConfirmation(true);
    } catch (error) {
      Sentry.captureException(error);
      setAuthError(true);
    } finally {
      setIsLoading(false);
    }
  };

  if (emailConfirmation) {
    return <PasswordConfirmationScreen icon="emailSent" translationKey="forgot_password" />;
  }

  return (
    <Screen
      preset="fixed"
      contentContainerStyle={{
        flexGrow: 1,
        flex: 1,
      }}
    >
      <Header
        title={t("header.title")}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      />

      <ScrollView>
        <YStack paddingHorizontal="$md" gap="$md" paddingTop={10 + insets.top}>
          <Typography preset="heading" fontWeight="700">
            {t("heading")}
          </Typography>

          <Typography>{t("paragraph")}</Typography>
          {authError && (
            <CredentialsError error={t("paragraph1", { ns: "generic_error_screen" })} />
          )}

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
                message: t("form.email.pattern"),
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
      </ScrollView>

      <WizzardControls
        isFirstElement
        onActionButtonPress={handleSubmit(onSubmit)}
        isNextDisabled={isLoading}
        actionBtnLabel={isLoading ? t("form.submit.loading") : t("form.submit.save")}
        marginTop="auto"
      />
    </Screen>
  );
};

export default ForgotPassword;
