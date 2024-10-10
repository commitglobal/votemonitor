import { ViewStyle } from "react-native";
import { Screen } from "../../../../components/Screen";
import { YStack } from "tamagui";
import { Typography } from "../../../../components/Typography";
import Button from "../../../../components/Button";
import { router } from "expo-router";

const CitizenFormSuccess = () => {
  return (
    <Screen
      preset="fixed"
      backgroundColor="white"
      style={$screenStyle}
      contentContainerStyle={$containerStyle}
    >
      <YStack gap="$xxs" padding="$md" flex={1} justifyContent="center" alignItems="center">
        <Typography>Submitted successfully</Typography>
        <Button onPress={() => router.back()}>Go back</Button>
      </YStack>
    </Screen>
  );
};

export default CitizenFormSuccess;

const $containerStyle: ViewStyle = {
  flex: 1,
};

const $screenStyle: ViewStyle = {
  backgroundColor: "white",
  justifyContent: "space-between",
};
