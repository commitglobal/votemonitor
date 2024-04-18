import React from "react";
import { router } from "expo-router";
import { useAuth } from "../hooks/useAuth";
import { View, XStack, YStack, styled } from "tamagui";
import { useTranslation } from "react-i18next";
import { Screen } from "../components/Screen";
import { StatusBar } from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { Icon } from "../components/Icon";
import { tokens } from "../theme/tokens";
import { Typography } from "../components/Typography";
import Button from "../components/Button";
import FormInput from "../components/FormInputs/FormInput";
import { Control, Controller, FieldErrors, FieldValues, useForm } from "react-hook-form";
import Card from "../components/Card";

const Login = () => {
  const { t } = useTranslation("login");
  const { signIn } = useAuth();
  const onLogin = async (formData: Record<string, string>) => {
    try {
      await signIn(formData.email, formData.password);
      router.replace("/");
    } catch (err) {
      console.error("Error while logging in...");
    }
  };
  const { handleSubmit, control, formState } = useForm({});
  const { errors } = formState;

  const ContentContainer = styled(YStack, {
    name: "Container",
    gap: 40,
    paddingHorizontal: tokens.space.md.val,
    paddingTop: tokens.space.xl.val,
  });

  return (
    <Screen preset="auto">
      <Header />

      <ContentContainer>
        <View flexDirection="row" gap={tokens.space.xxs}>
          <Icon icon="infoCircle" size={18} color="white" style={{ marginTop: 2 }} />
          <Typography>{t("informative-text")}</Typography>
        </View>

        <LoginForm control={control} errors={errors} />
      </ContentContainer>

      <Card width="100%">
        <Button onPress={handleSubmit(onLogin)}>Log in</Button>
      </Card>
    </Screen>
  );
};

const LoginForm = ({
  control,
  errors,
}: {
  control: Control<FieldValues, any>;
  errors: FieldErrors<FieldValues>;
}) => {
  const { t } = useTranslation("login");
  const [secureTextEntry, setSecureTextEntry] = React.useState(false);
  const passIcon = secureTextEntry === false ? "eye" : "eyeOff";

  return (
    <View gap={12}>
      <Typography preset="heading" fontWeight="700">
        {t("title")}
      </Typography>

      <Typography>{t("paragraph")}</Typography>

      <Controller
        key="email"
        name="email"
        control={control}
        rules={{
          required: {
            value: true,
            message: t("form.email.required"),
          },
        }}
        render={({ field: { onChange, value } }) => (
          <YStack>
            <FormInput
              type="text"
              title={t("form.email.label")}
              placeholder={t("form.email.placeholder")}
              value={value}
              onChangeText={onChange}
            ></FormInput>

            <Typography color="$red5">{errors?.email?.message?.toString() ?? ""}</Typography>
          </YStack>
        )}
      />

      <Controller
        key="password"
        name="password"
        control={control}
        rules={{
          required: {
            value: true,
            message: t("form.password.required"),
          },
        }}
        render={({ field: { onChange, value } }) => (
          <YStack>
            <FormInput
              type="password"
              secureTextEntry={secureTextEntry}
              title={t("form.password.label")}
              placeholder={t("form.password.placeholder")}
              value={value}
              onChangeText={onChange}
              iconRight={<Icon icon={passIcon} size={20} color="$gray11" />}
              onIconRightPress={() => {
                setSecureTextEntry(!secureTextEntry);
              }}
            ></FormInput>

            <Typography color="$red5">{errors?.password?.message?.toString() ?? ""}</Typography>
          </YStack>
        )}
      />

      <Typography textAlign="right" color="$purple5">
        {t("actions.forgot_password")}
      </Typography>
    </View>
  );
};

const Header = () => {
  const insets = useSafeAreaInsets();
  const StyledWrapper = styled(XStack, {
    name: "StyledWrapper",
    backgroundColor: "$purple5",
    height: 160 + insets.top,
    paddingTop: insets.top,
    alignItems: "center",
    justifyContent: "center",
  });

  return (
    <StyledWrapper>
      <StatusBar barStyle="light-content"></StatusBar>
      <Icon icon="loginLogo" size={300} />
    </StyledWrapper>
  );
};

export default Login;
