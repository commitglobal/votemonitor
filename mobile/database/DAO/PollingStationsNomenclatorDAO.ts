import { Q, Query } from "@nozbe/watermelondb";
import { PollingStationNomenclatorNodeAPIResponse } from "../../services/definitions.api";
import { database } from "../db";
import { PollingStationsNom } from "../models/polling-stations-nomenclator.model";
import { DB_TABLE_NAMES } from "../schemas";

export const addPollingStationsNomenclatureBulk = async (
  electionRoundId: string,
  data: PollingStationNomenclatorNodeAPIResponse[],
) => {
  console.log("addPollingStationsNomenclatureBulk");
  const startTime = performance.now();

  await database.write(async () => {
    const newNomenclature = data.map((node) =>
      database.get<PollingStationsNom>("polling_stations_nom").prepareCreate((pollingStation) => {
        pollingStation._id = node.id;
        pollingStation.name = node.name;
        pollingStation.electionRoundId = electionRoundId;
        pollingStation.pollingStationId = node.pollingStationId;
        pollingStation.pollingStationNumber = node.number;
        pollingStation.parentId = node.parentId || -1;
      }),
    );
    await database.batch(newNomenclature);

    const endTime = performance.now();
    console.log(`addPollingStationsNomenclatureBulk took ${endTime - startTime} milliseconds.`);
  });
};

export const getOne = async (electionRoundId: string) => {
  const data = await database
    .get<PollingStationsNom>(DB_TABLE_NAMES.POLLING_STATIONS_NOMENCLATOR)
    .query(Q.where("election_round_id", electionRoundId), Q.take(1));

  return data?.length ? data[0] : null;
};

export const getPollingStationNomenclatorNodesCount = (
  electionRoundId: string,
  // take?: number
) => {
  // TODO: instead of counting, we can try to take 1 and if exists, we know all are there
  console.log("[DATABASE CALL] getPollingStationNomenclatorNodesCount");
  return database
    .get("polling_stations_nom")
    .query(Q.where("election_round_id", electionRoundId))
    .fetchCount();
};

export const getPollingStationNomenclatorNodes = (): Query<PollingStationsNom> => {
  return database.get<PollingStationsNom>("polling_stations_nom").query(Q.take(10));
};

export const deleteAll = (electionRoundId: string) => {
  console.log("deleteAllRecordsPollingStationNomenclator");
  return database.write(async () => {
    const data = await database
      .get(DB_TABLE_NAMES.POLLING_STATIONS_NOMENCLATOR)
      .query(Q.where("election_round_id", electionRoundId))
      .fetch();

    if (!data.length) {
      return;
    }

    const deleted = data.map((item) => item.prepareDestroyPermanently());

    await database.batch(deleted);
    console.log("deleteAllRecordsPollingStationNomenclator END");
  });
};

export const deleteEverything = () => {
  return database.write(async () => {
    const data = await database.get(DB_TABLE_NAMES.POLLING_STATIONS_NOMENCLATOR).query().fetch();

    if (!data.length) {
      return;
    }

    const deleted = data.map((item) => item.prepareDestroyPermanently());

    await database.batch(deleted);
    console.log("deleteAllRecordsPollingStationNomenclator END");
  });
};

/**
 * @param {number} [parentId=-1] we save top-level nodes with -1
 * @returns {Promise<PollingStationsNom[]>}
 */
export const getPollingStationsByParentId = (
  parentId: number | null = -1,
  electionRoundId: string,
) => {
  return database
    .get<PollingStationsNom>("polling_stations_nom")
    .query(Q.where("parent_id", parentId), Q.where("election_round_id", electionRoundId))
    .fetch();
};

export const getPollingStationById = async (id: string): Promise<PollingStationsNom | null> => {
  const data = await database
    .get<PollingStationsNom>("polling_stations_nom")
    .query(Q.where("polling_station_id", id));

  return data?.length ? data[0] : null;
};
