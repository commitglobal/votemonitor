import { router } from "expo-router";
import * as ReactotronCommands from "../../../../helpers/reactotron-custom-commands";
import { Screen } from "../../../../components/Screen";
import { useUserData } from "../../../../contexts/user/UserContext.provider";
import { Typography } from "../../../../components/Typography";
import Button from "../../../../components/Button";
import { Stack, YStack } from "tamagui";
import { Icon } from "../../../../components/Icon";

ReactotronCommands.default();

const MissingElectionRounds = () => (
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

const MissingVisits = () => (
  <Screen preset="fixed">
    <Stack height="100%" backgroundColor="white" justifyContent="center" alignItems="center">
      <YStack width={312} alignItems="center" gap="$md">
        <Icon icon="missingPollingStation" />
        <YStack gap="$xxxs">
          <Typography preset="subheading" textAlign="center">
            No visited polling stations yet
          </Typography>
          <Typography preset="body1" textAlign="center" color="$gray5">
            Start configuring your first polling station before completing observation forms.
          </Typography>
        </YStack>
        <Button
          preset="outlined"
          backgroundColor="white"
          width="100%"
          onPress={router.push.bind(null, "/polling-station-wizzard")}
        >
          Add your first polling station
        </Button>
      </YStack>
    </Stack>
  </Screen>
);

const Index = () => {
  const { isAssignedToEllectionRound, visits } = useUserData();

  // // Station ID: d3e6d2e9-0341-4dde-a58a-142a3f2dd19a
  // const update = () => {
  //   mutate({
  //     electionRoundId: "43b91c74-6d05-4fd1-bd93-dfe203c83c53",
  //     pollingStationId: "d3e6d2e9-0341-4dde-a58a-142a3f2dd19a",
  //     arrivalTime: new Date().toISOString(),
  //     departureTime: new Date().toISOString(),
  //     answers: [],
  //   });
  // };

  // const { mutate } = upsertPollingStationGeneralInformationMutation();

  if (isAssignedToEllectionRound) {
    return <MissingElectionRounds />;
  }

  if (visits.length !== 0) {
    return <MissingVisits />;
  }

  return (
    <Screen preset="fixed" contentContainerStyle={{ gap: 20 }} safeAreaEdges={["top"]}>
      <Button onPress={() => router.push("/form-questionnaire/1")}>Go Form wizzard</Button>
      <Button onPress={() => router.push("/polling-station-questionnaire")}>
        Go To Polling Station Qustionnaire
      </Button>
    </Screen>
  );
};

export default Index;
