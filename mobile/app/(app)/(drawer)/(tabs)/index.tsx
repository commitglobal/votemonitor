import { View, Text } from "react-native";
import { useAuth } from "../../../../hooks/useAuth";
import { StatusBar } from "react-native";
import { Button } from "tamagui";
import { router } from "expo-router";
import {
  upsertPollingStationGeneralInformationMutation,
  useElectionRoundsQuery,
  usePollingStationById,
  usePollingStationsNomenclatorQuery,
  usePollingStationsVisits,
} from "../../../../services/queries.service";

import * as ReactotronCommands from "../../../../helpers/reactotron-custom-commands";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import {
  PollingStationInformationAPIPayload,
  upsertPollingStationGeneralInformation,
} from "../../../../services/definitions.api";

ReactotronCommands.default();
const Index = () => {
  const { signOut } = useAuth();

  const { data: rounds } = useElectionRoundsQuery();
  // console.log("📍 ROUND ", rounds ? rounds.electionRounds[0].id : "");
  // const { data: visits } = usePollingStationsVisits(
  //   rounds ? rounds.electionRounds[0].id : ""
  // );
  const {} = usePollingStationsNomenclatorQuery(
    rounds ? rounds.electionRounds[0].id : ""
  );

  // const { data: station } = usePollingStationById(25902);
  // // Station ID: d3e6d2e9-0341-4dde-a58a-142a3f2dd19a

  const update = () => {
    mutate({
      electionRoundId: "43b91c74-6d05-4fd1-bd93-dfe203c83c53",
      pollingStationId: "d3e6d2e9-0341-4dde-a58a-142a3f2dd19a",
      arrivalTime: new Date().toISOString(),
      departureTime: new Date().toISOString(),
      answers: [],
    });
  };

  const { mutate, error } = upsertPollingStationGeneralInformationMutation();

  return (
    <View style={{ gap: 20 }}>
      <StatusBar barStyle="light-content" />
      <Button onPress={() => router.push("/polling-station-wizzard/1")}>
        Go To Polling station wizzard
      </Button>
      <Button onPress={() => router.push("/form-questionnaire/1")}>
        Go Form wizzard
      </Button>
      <Button onPress={() => router.push("/polling-station-questionnaire")}>
        Go To Polling Station Qustionnaire
      </Button>
      <Text onPress={signOut}>Logout</Text>
      <Text onPress={() => update()}>Update</Text>
    </View>
  );
};

export default Index;
