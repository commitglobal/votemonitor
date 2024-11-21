import { ICitizenElectionRoundLocation } from "../../services/api/citizen/get-election-round-locations";
import { database } from "../db";
import { DB_TABLE_NAMES } from "../schemas";
import { CitizenLocation } from "../models/citizen-locations.model";
import { Q } from "@nozbe/watermelondb";

export const addCitizenLocationsBulk = async (
  electionRoundId: string,
  data: ICitizenElectionRoundLocation[],
) => {
  const startTime = performance.now();

  await database.write(async () => {
    const newLocations = data.map((location) =>
      database
        .get<CitizenLocation>(DB_TABLE_NAMES.CITIZEN_LOCATIONS)
        .prepareCreate((citizenLocation) => {
          citizenLocation._id = location.id;
          citizenLocation.electionRoundId = electionRoundId;
          citizenLocation.name = location.name;
          citizenLocation.depth = location.depth;
          citizenLocation.parentId = location.parentId || -1;
          citizenLocation.displayOrder = location.displayOrder;
          citizenLocation.locationId = location.locationId;
        }),
    );
    await database.batch(newLocations);

    const endTime = performance.now();
    console.log(`addCitizenLocationsBulk took ${endTime - startTime} milliseconds.`);
  });
};

export const getOne = async (electionRoundId: string) => {
  const data = await database
    .get<CitizenLocation>(DB_TABLE_NAMES.CITIZEN_LOCATIONS)
    .query(Q.where("election_round_id", electionRoundId), Q.take(1));

  return data?.length ? data[0] : null;
};

/**
 * @param {number} [parentId=-1] we save top-level nodes with -1
 * @returns {Promise<CitizenLocation[]>}
 */
export const getCitizenLocationsByParentId = (parentId: number, electionRoundId: string) => {
  return database
    .get<CitizenLocation>(DB_TABLE_NAMES.CITIZEN_LOCATIONS)
    .query(Q.where("parent_id", parentId), Q.where("election_round_id", electionRoundId));
};

export const getCitizenLocationByLocationId = async (locationId: string) => {
  const data = await database
    .get<CitizenLocation>(DB_TABLE_NAMES.CITIZEN_LOCATIONS)
    .query(Q.where("location_id", locationId));

  return data?.length ? data[0] : null;
};

export const deleteAllCitizenLocations = (electionRoundId: string) => {
  return database.write(async () => {
    const data = await database
      .get(DB_TABLE_NAMES.CITIZEN_LOCATIONS)
      .query(Q.where("election_round_id", electionRoundId))
      .fetch();

    if (!data.length) {
      return;
    }

    const deleted = data.map((item) => item.prepareDestroyPermanently());

    await database.batch(deleted);
  });
};
