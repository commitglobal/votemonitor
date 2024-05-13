import { Screen } from "../components/Screen";
import { useTranslation } from "react-i18next";
import { Icon } from "../components/Icon";
import { YStack, XStack, styled } from "tamagui";
import { Typography } from "../components/Typography";
import { StatusBar } from "react-native";
import Card from "../components/Card";
import Button from "../components/Button";
import { router } from "expo-router";
import { useSafeAreaInsets } from "react-native-safe-area-context";

interface ChangePasswordConfirmationProps {
  emailConfirmation: boolean;
}

const ChangePasswordConfirmation = (props: ChangePasswordConfirmationProps) => {
  const { emailConfirmation } = props;
  const { t } = useTranslation(["change_password", "email-confirmation"]);
  const insets = useSafeAreaInsets();

  const icon = emailConfirmation === true ? "emailSent" : "passwordConfirmation";
  const header =
    emailConfirmation === true ? t("title", { ns: "email-confirmation" }) : t("success_page.title");
  const paragraph =
    emailConfirmation === true
      ? t("paragraph", { ns: "email-confirmation" })
      : t("success_page.paragraph");

  return (
    <Screen
      preset="auto"
      backgroundColor="white"
      ScrollViewProps={{
        bounces: false,
      }}
      contentContainerStyle={{
        flexGrow: 1,
      }}
    >
      <Header />
      <YStack
        paddingHorizontal="$xxxl"
        justifyContent="center"
        alignItems="center"
        flexGrow={1}
        gap="$xxs"
      >
        <Icon icon={icon} size={126} marginBottom="$md" color="yellow" />
        <Typography preset="heading" fontWeight="700" color="$purple5">
          {header}
        </Typography>
        <Typography preset="body1" color="black" textAlign="center">
          {paragraph}
        </Typography>
      </YStack>

      <Card width="100%" paddingBottom={16 + insets.bottom} marginTop="auto">
        <Button onPress={() => router.back()}>{t("success_page.actions.back")}</Button>
      </Card>
    </Screen>
  );
};

const Header = () => {
  const insets = useSafeAreaInsets();
  const StyledWrapper = styled(XStack, {
    name: "StyledWrapper",
    backgroundColor: "$purple5",
    height: "5%",
    paddingTop: insets.top,
    alignItems: "center",
    justifyContent: "center",
  });

  return (
    <StyledWrapper>
      <StatusBar barStyle="light-content"></StatusBar>
    </StyledWrapper>
  );
};

export default ChangePasswordConfirmation;
