import { View, Text, Button } from "react-native";
import { useAuth } from "../../../hooks/useAuth";
import {
  upsertPollingStationGeneralInformationMutation,
  useCustomQueryToSaveData,
  useElectionRoundsQuery,
  usePollingStationById,
  usePollingStationByParentID,
  usePollingStationInformation,
  usePollingStationInformationForm,
  usePollingStationsNomenclatorQuery,
  usePollingStationsVisits,
} from "../../../services/election-rounds/election-rounds.service";

import ReactotronCommands from "../../../helpers/reactotron-custom-commands";

import { withObservables } from "@nozbe/watermelondb/react";
import { DB_TABLE_NAMES } from "../../../database/schemas";
import { database } from "../../../database/db";
import { useQueryClient } from "@tanstack/react-query";

ReactotronCommands();

const Index = () => {
  const { signOut } = useAuth();

  const queryClient = useQueryClient();

  // // 1. Get Election Rounds, for start we can use the index 0 as active election round
  const { data: electionRounds, fetchStatus: electionRoundFetchStatus } =
    useElectionRoundsQuery();
  // console.log(electionRounds);

  // // 2. Get polling stations nomenclature for the given election round (from DB or API if doesn't exist)
  // // Will not be available in memory, make use of:
  // //    ------------ usePollingStationByParentID for wizard
  // //    ------------ usePollingStationById to select a particular one, and set it in Context/DB/Whatever
  const { fetchStatus: nomenclatorFetchStatus } =
    usePollingStationsNomenclatorQuery(
      electionRounds ? electionRounds[0]?.id : ""
    );

  // // 3. Get my visits, so I can pick a Polling Station to be displayed, also for the top dropdown
  // const { data: myVisits, fetchStatus: visitsFetchStatus } =
  //   usePollingStationsVisits(electionRounds![0]?.id);
  // console.log("myVisits", myVisits);

  // // 4. Polling Stations by level for the wizard
  // const { data: pollingStationsByParent } = usePollingStationByParentID(-1);
  // console.log("pollingStationsByParent", pollingStationsByParent?.length);

  // // 5. Select one polling station by id
  // const { data: pollingStationsById } = usePollingStationById(5);
  // console.log(
  //   "pollingStationsById",
  //   pollingStationsById ? pollingStationsById[0] : "Nu exista"
  // );

  // // 6. Mutation: UPSERT polling station information (arrival,departure time + general survey)
  // const { mutate: updatePollingInfo } =
  //   upsertPollingStationGeneralInformationMutation();

  // const { data } = usePollingStationInformationForm(
  //   electionRounds ? electionRounds[0]?.id : ""
  // );

  const { data } = usePollingStationInformation(
    electionRounds ? electionRounds[0]?.id : ""
  );

  const { data: dragos } = useCustomQueryToSaveData();

  const updateQueryData = () => {
    queryClient.setQueryData(["test-data", 1], (oldData: any) => {
      return {
        ...oldData,
        andrew: "Radulescu",
      };
    });
  };

  console.log(dragos);

  return (
    <View style={{ gap: 20 }}>
      <Text>Observation</Text>
      {/* <Text>{nomenclator?.nodes?.length}</Text> */}
      {/* <Text>Election Rounds: {electionRoundFetchStatus}</Text>
      <Text>Polling Stations Nomenclator: {nomenclatorFetchStatus}</Text>
      <Text>Polling Stations Visits: {visitsFetchStatus}</Text>
      <Button
        onPress={() =>
          updatePollingInfo({
            electionRoundId: electionRounds![0]?.id,
            pollingStationId: `${pollingStationsById![0]?.pollingStationId}`,
            arrivalTime: new Date().toISOString(),
            answers: [],
            departureTime: new Date().toISOString(),
          })
        }
        title="Update here"
      ></Button> */}
      {/* <OfflinePersistComponentExample></OfflinePersistComponentExample> */}
      {/* <EnhancedElectionRoundsComponent
        electionRoundId={"Ka79pCXVJkTBmH8o"}
      ></EnhancedElectionRoundsComponent> */}
      <Text onPress={signOut}>Logout</Text>
      <Text onPress={updateQueryData}>Update query data</Text>
    </View>
  );
};

const enhance = withObservables([`electionRoundId`], ({ electionRoundId }) => ({
  electionRound: database
    .get(DB_TABLE_NAMES.ELECTION_ROUNDS)
    .findAndObserve(electionRoundId),
}));

const ElectionRoundsComponent = ({ electionRoundId, electionRound }: any) => {
  return (
    <Text>
      {electionRoundId} and {electionRound._id}
    </Text>
  );
};

const EnhancedElectionRoundsComponent = enhance(ElectionRoundsComponent);

export default Index;
