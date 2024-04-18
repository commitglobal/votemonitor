import { Screen } from "../components/Screen";
import { useTranslation } from "react-i18next";
import { Icon } from "../components/Icon";
import { YStack, XStack, styled } from "tamagui";
import { Typography } from "../components/Typography";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { StatusBar } from "react-native";

const EmailConfirmation = () => {
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
      <YStack justifyContent="center" alignItems="center" flexGrow={1} gap={8}>
        <Icon icon="emailSent" size={126} color="$purple5" marginBottom={16} />
        <Typography preset="heading" fontWeight="700">
          Email sent
        </Typography>
        <Typography>Email sent</Typography>
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
