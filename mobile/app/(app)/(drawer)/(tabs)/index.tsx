import React from "react";
import { Text } from "react-native";
import { Button } from "tamagui";
import { router } from "expo-router";
import SelectPollingStation from "../../../../components/SelectPollingStation";
import {
  // upsertPollingStationGeneralInformationMutation,
  useElectionRoundsQuery,
  usePollingStationsNomenclatorQuery,
} from "../../../../services/queries.service";

import * as ReactotronCommands from "../../../../helpers/reactotron-custom-commands";
import { useAuth } from "../../../../hooks/useAuth";
import { Screen } from "../../../../components/Screen";

ReactotronCommands.default();
const Index = () => {
  const { signOut } = useAuth();

  const { data: rounds } = useElectionRoundsQuery();
  // console.log("📍 ROUND ", rounds ? rounds.electionRounds[0].id : "");
  // const { data: visits } = usePollingStationsVisits(
  //   rounds ? rounds.electionRounds[0].id : ""
  // );
  const { data } = usePollingStationsNomenclatorQuery(rounds ? rounds.electionRounds[0].id : "");
  console.log(data);

  // const { data: station } = usePollingStationById(25902);
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

  // const { mutate, error } = upsertPollingStationGeneralInformationMutation();

  return (
    <Screen preset="fixed" contentContainerStyle={{ gap: 20 }} safeAreaEdges={["top"]}>
      <Button onPress={() => router.push("/polling-station-wizzard/1")}>
        Go To Polling station wizzard
      </Button>
      <Button onPress={() => router.push("/form-questionnaire/1")}>Go Form wizzard</Button>
      <Button onPress={() => router.push("/polling-station-questionnaire")}>
        Go To Polling Station Qustionnaire
      </Button>
      <Text onPress={signOut}>Logout</Text>
    </Screen>
  );
};

export default Index;
