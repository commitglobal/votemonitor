import Reactotron from "reactotron-react-native";
import AsyncStorage from "@react-native-async-storage/async-storage";

import { calculateSizes, clearStorage } from "./calculateAsyncStorageSize";
import {
  getPollingStationNomenclatorNodesCount,
  getPollingStationNomenclatorNodes,
  deleteEverything,
} from "../database/DAO/PollingStationsNomenclatorDAO";

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
      await getPollingStationNomenclatorNodesCount("43b91c74-6d05-4fd1-bd93-dfe203c83c53"),
    );
  },
});

Reactotron.onCustomCommand({
  title: "Watermelon deleteAllRecordsPollingStationNomenclator",
  command: "deleteAllRecordsPollingStationNomenclator",
  handler: async () => {
    console.log("Calling deleteAllRecordsPollingStationNomenclator");
    console.log(await deleteEverything());
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
      })),
    );
  },
});

export default () => {
  console.log("Reactotron commands enabled");
};
