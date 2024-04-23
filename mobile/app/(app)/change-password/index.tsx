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

interface FormData {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

const ChangePassowrd = () => {
  const insets = useSafeAreaInsets();

  const { handleSubmit, control, formState } = useForm<FormData>({});
  const { errors } = formState;

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
        title={"Change Password"}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      />

      <Form control={control} errors={errors}></Form>

      <Card width="100%" paddingBottom={16 + insets.bottom} marginTop="auto">
        <Button onPress={() => console.log("Save new password")}>Save new pass</Button>
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
              title={"Password"}
              placeholder={"Password"}
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
              title={"Password"}
              placeholder={"Password"}
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
        key="confirmPassword"
        name="confirmPassword"
        control={control}
        render={({ field: { onChange, value } }) => (
          <YStack>
            <FormInput
              type="password"
              secureTextEntry={secureTextEntry}
              title={"Password"}
              placeholder={"Password"}
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
    </YStack>
  );
};

const $containerStyle: ViewStyle = {
  flexGrow: 1,
};

export default ChangePassowrd;
