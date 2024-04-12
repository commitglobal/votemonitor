import { Stack, YStack } from "tamagui";
import { Screen } from "./Screen";
import { Icon } from "./Icon";
import { Typography } from "./Typography";

const NoElectionRounds = () => (
  <Screen preset="fixed">
    <Stack height="100%" backgroundColor="white" justifyContent="center" alignItems="center">
      <YStack width={312} alignItems="center">
        <Icon icon="peopleAddingVote" marginBottom="$md" />
        <Typography preset="subheading" textAlign="center" marginBottom="$xxxs">
          No election event to observe yet
        </Typography>
        <Typography preset="body1" textAlign="center" color="$gray5">
          You will be able to use the app once you will be assigned to an election event by your
          organization
        </Typography>
      </YStack>
    </Stack>
  </Screen>
);

export default NoElectionRounds;
