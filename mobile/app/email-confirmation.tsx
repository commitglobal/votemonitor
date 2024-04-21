import { Screen } from "../components/Screen";
import { useTranslation } from "react-i18next";
import { Icon } from "../components/Icon";
import { YStack, XStack, styled } from "tamagui";
import { Typography } from "../components/Typography";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { StatusBar } from "react-native";

const EmailConfirmation = () => {
  const { t } = useTranslation("email-confirmation");

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
        <Icon icon="emailSent" size={126} marginBottom="$md" />
        <Typography preset="heading" fontWeight="700">
          {t("title")}
        </Typography>
        <Typography>{t("paragraph")}</Typography>
      </YStack>
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

export default EmailConfirmation;
