import { Text } from "react-native";
import { Button } from "tamagui";
import { router } from "expo-router";
import * as ReactotronCommands from "../../../../helpers/reactotron-custom-commands";
import { useAuth } from "../../../../hooks/useAuth";
import { Screen } from "../../../../components/Screen";
import { useUserData } from "../../../../contexts/user/UserContext.provider";

ReactotronCommands.default();
const Index = () => {
  const { signOut } = useAuth();
  const { visits, electionRounds } = useUserData();
  console.log("visits", visits);
  console.log("electionRounds", electionRounds);
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

  return (
    <Screen preset="fixed" contentContainerStyle={{ gap: 20 }} safeAreaEdges={["top"]}>
      <Button onPress={() => router.push("/polling-station-wizzard/1")}>
        Go To Polling station wizzard
      </Button>
      <Button onPress={() => router.push("/form-questionnaire/1")}>Go Form wizzard</Button>
      <Button onPress={() => router.push("/polling-station-questionnaire")}>
        Go To Polling Station Qustionnaire
      </Button>
      {/* <Button onPress={update}>Update</Button> */}
      <Text onPress={signOut}>Logout</Text>
    </Screen>
  );
};

export default Index;
