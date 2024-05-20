import { Stack, YStack } from "tamagui";
import { Icon } from "./Icon";
import { Typography } from "./Typography";
import Button from "./Button";
import { useQueryClient } from "@tanstack/react-query";

const FormListErrorScreen = () => {
  const queryClient = useQueryClient();

  return (
    <Stack height="100%" justifyContent="center" alignItems="center">
      <YStack width={312} alignItems="center">
        <Icon icon="loadingScreenDevice" marginBottom="$md" />
        <Typography preset="subheading" textAlign="center" marginBottom="$xxxs">
          Something went wrong!
        </Typography>
        <Typography preset="body1" textAlign="center" color="$gray5">
          We could not fetch the data. Press the retry button to try again!
        </Typography>
        <Button style={{ marginTop: 10 }} onPress={() => queryClient.invalidateQueries()}>
          Retry
        </Button>
      </YStack>
    </Stack>
  );
};

export default FormListErrorScreen;
