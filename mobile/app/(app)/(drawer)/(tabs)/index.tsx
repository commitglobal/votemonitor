import { router } from "expo-router";
import * as ReactotronCommands from "../../../../helpers/reactotron-custom-commands";
import { Screen } from "../../../../components/Screen";
import { useUserData } from "../../../../contexts/user/UserContext.provider";
import { Typography } from "../../../../components/Typography";
import Button from "../../../../components/Button";

ReactotronCommands.default();

const MissingElectionRounds = () => (
  <Screen preset="fixed">
    <Typography>This will be the missing election round screen</Typography>
  </Screen>
);

const MissingVisits = () => (
  <Screen preset="fixed">
    <Typography>This will be the missing visited polling stations screen</Typography>
    <Button
      preset="outlined"
      backgroundColor="white"
      onPress={router.push.bind(null, "/polling-station-wizzard/-1")}
    >
      Add your first polling station
    </Button>
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

  if (!isAssignedToEllectionRound) {
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
