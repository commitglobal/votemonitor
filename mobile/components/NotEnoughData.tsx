import { Button, Stack, YStack } from "tamagui";
import { Screen } from "./Screen";
import { Icon } from "./Icon";
import { Typography } from "./Typography";
import { useQueryClient } from "@tanstack/react-query";

const NotEnoughData = () => {
  const queryClient = useQueryClient();

  return (
    <Screen preset="fixed">
      <Stack height="100%" backgroundColor="white" justifyContent="center" alignItems="center">
        <YStack width={312} alignItems="center">
          <Icon icon="loadingScreenDevice" marginBottom="$md" />
          <Typography preset="subheading" textAlign="center" marginBottom="$xxxs">
            Oops, we couldn't fetch the data
          </Typography>
          <Typography preset="body1" textAlign="center" color="$gray5">
            To be able to use the app we need to load more data. Check your internet connection and
            press retry!
          </Typography>
          <Button onPress={() => queryClient.invalidateQueries()}>Retry</Button>
        </YStack>
      </Stack>
    </Screen>
  );
};

export default NotEnoughData;
