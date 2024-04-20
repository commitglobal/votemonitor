import { Stack, YStack } from "tamagui";
import { Screen } from "./Screen";
import { Icon } from "./Icon";
import { Typography } from "./Typography";
import { reloadAsync } from "expo-updates";
import Button from "./Button";

const GenericErrorScreen = () => {
  return (
    <Screen preset="fixed">
      <Stack height="100%" backgroundColor="white" justifyContent="center" alignItems="center">
        <YStack width={312} alignItems="center">
          <Icon icon="loadingScreenDevice" marginBottom="$md" />
          <Typography preset="subheading" textAlign="center" marginBottom="$xxxs">
            Oops, something went wrong!
          </Typography>
          <Typography preset="body1" textAlign="center" color="$gray5">
            We could not recover after this error. Please restart the application!
          </Typography>
          <Button onPress={() => reloadAsync().catch((_error) => {})}>Retry</Button>
        </YStack>
      </Stack>
    </Screen>
  );
};

export default GenericErrorScreen;
