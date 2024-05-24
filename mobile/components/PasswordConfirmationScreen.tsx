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

interface PasswordConfirmationScreenProps {
  translationKey: string;
  icon: string;
}

const PasswordConfirmationScreen = ({ translationKey, icon }: PasswordConfirmationScreenProps) => {
  const { t } = useTranslation(translationKey);
  const insets = useSafeAreaInsets();
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
          {t("confirmation.heading")}
        </Typography>
        <Typography preset="body1" color="black" textAlign="center">
          {t("confirmation.paragraph")}
        </Typography>
      </YStack>

      <Card width="100%" paddingBottom={16 + insets.bottom} marginTop="auto">
        <Button onPress={() => router.back()}>{t("confirmation.confirm")}</Button>
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

export default PasswordConfirmationScreen;