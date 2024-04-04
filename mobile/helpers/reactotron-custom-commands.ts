import Reactotron from "reactotron-react-native";
import AsyncStorage from "@react-native-async-storage/async-storage";

import { calculateSizes, clearStorage } from "./calculateAsyncStorageSize";
import {
  getPollingStationNomenclatorNodesCount,
  deleteAllRecordsPollingStationNomenclator,
  getPollingStationNomenclatorNodes,
} from "../database/DAO/PollingStationsNomenclatorDAO";
import * as ElectionRoundsDBQueries from "../database/DAO/ElectionRounds.dao";
import {
  getElectionRounds,
  getPollingStationInformation,
} from "../services/election-rounds/election-rounds.api";

Reactotron.onCustomCommand({
  title: "AsyncStorage",
  command: "asyncStorageSize",
  handler: async () => {
    console.log(await AsyncStorage.getAllKeys());
    calculateSizes();
  },
});

Reactotron.onCustomCommand({
  title: "AsyncStoragShowCache",
  command: "asyncStorageSizeShow",
  handler: async () => {
    console.log(await AsyncStorage.getItem("REACT_QUERY_OFFLINE_CACHE"));
  },
});

Reactotron.onCustomCommand({
  title: "Clear Async Storage",
  command: "clearAsyncStorage",
  handler: async () => {
    clearStorage();
  },
});

Reactotron.onCustomCommand({
  title: "Watermelon Polling Station Nodes",
  command: "getPollingStationNomenclatorNodes",
  handler: async () => {
    console.log("Calling getPollingStationNomenclatorNodes");
    console.log(
      await getPollingStationNomenclatorNodesCount(
        "1e34f72d-0fe6-415d-a123-f9d6b8fa962d"
      )
    );
  },
});

Reactotron.onCustomCommand({
  title: "Watermelon deleteAllRecordsPollingStationNomenclator",
  command: "deleteAllRecordsPollingStationNomenclator",
  handler: async () => {
    console.log("Calling deleteAllRecordsPollingStationNomenclator");
    console.log(await deleteAllRecordsPollingStationNomenclator());
  },
});

Reactotron.onCustomCommand({
  title: "Watermelon get first 10 records",
  command: "getFirst10Records",
  handler: async () => {
    console.log("Calling getPollingStationNomenclatorNodes");
    const data = await getPollingStationNomenclatorNodes();
    console.log(
      data.map((i: any) => ({
        name: i.name,
        id: i._id,
        electionRoundId: i.electionRoundId,
        parentId: i.parentId,
      }))
    );
  },
});

Reactotron.onCustomCommand({
  title: "Watermelon getElectionRounds",
  command: "getAllElectionRounds",
  handler: async () => {
    console.log("Calling getAllElectionRounds");
    console.log(await ElectionRoundsDBQueries.getElectionRounds());
  },
});

Reactotron.onCustomCommand({
  title: "Watermelon addElectionRoundsFromAPItoDB",
  command: "addElectionRoundsFromAPItoDB",
  handler: async () => {
    console.log("Calling addElectionRoundsFromAPItoDB");
    const data = await getElectionRounds();
    await ElectionRoundsDBQueries.addElectionRoundsBulk(data);
    console.log("Done addElectionRoundsFromAPItoDB");
  },
});

Reactotron.onCustomCommand({
  title: "TEst getPollingStationInformation",
  command: "getPollingStationInformation",
  handler: async () => {
    const data = await getPollingStationInformation(
      "1e34f72d-0fe6-415d-a123-f9d6b8fa962d",
      ["9133160e-d9b7-acce-436d67372722"]
    )
      .then(console.log)
      .catch((err) => console.log(err.response.data));
  },
});

export default () => {
  console.log("Reactotron commands enabled");
};
