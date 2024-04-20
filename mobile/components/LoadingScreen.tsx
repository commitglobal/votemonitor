import { Stack, YStack } from "tamagui";
import { Screen } from "./Screen";
import { Icon } from "./Icon";
import { Typography } from "./Typography";

const LoadingScreen = () => (
  <Screen preset="fixed">
    <Stack height="100%" backgroundColor="white" justifyContent="center" alignItems="center">
      <YStack width={312} alignItems="center">
        <Icon icon="loadingScreenDevice" marginBottom="$md" />
        <Typography preset="subheading" textAlign="center" marginBottom="$xxxs">
          Loading...
        </Typography>
        <Typography preset="body1" textAlign="center" color="$gray5">
          We are preparing all the data, please wait!
        </Typography>
      </YStack>
    </Stack>
  </Screen>
);

export default LoadingScreen;
