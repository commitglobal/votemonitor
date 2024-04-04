import { ElectionRoundVM } from "../../common/models/election-round.model";
import { ElectionRoundsAPIResponse } from "../../services/election-rounds/election-rounds.api";
import { database } from "../db";
import { ElectionRoundModelDB } from "../models/election-rounds.model";
import { DB_TABLE_NAMES } from "../schemas";

export const getElectionRounds = async (): Promise<ElectionRoundVM[]> => {
  try {
    const data = await database
      .get<ElectionRoundModelDB>(DB_TABLE_NAMES.ELECTION_ROUNDS)
      .query()
      .fetch();
    return (
      data?.map((item) => {
        return {
          id: item._id,
          countryId: item.countryId,
          country: item.country,
          title: item.title,
          englishTitle: item.englishTitle,
          startDate: item.startDate,
          status: item.status,
          createdOn: item.createdOn,
          lastModifiedOn: item.lastModifiedOn,
        };
      }) || []
    );
  } catch (err) {
    console.log(err);
    return [];
  }
};

export const addElectionRoundsBulk = async (
  data: ElectionRoundsAPIResponse
) => {
  console.log("addElectionRoundsBulk");
  const startTime = performance.now();

  if (!data?.electionRounds?.length) {
    console.log("Nothing to add: addElectionRoundsBulk");
    return;
  }

  try {
    await database.write(async () => {
      const newElectionRounds = data.electionRounds.map((node) =>
        database
          .get<ElectionRoundModelDB>(DB_TABLE_NAMES.ELECTION_ROUNDS)
          .prepareCreate((electionRound) => {
            electionRound._id = node.id;
            electionRound.countryId = node.countryId;
            electionRound.country = node.country;
            electionRound.title = node.title;
            electionRound.englishTitle = node.englishTitle;
            electionRound.startDate = node.startDate;
            electionRound.status = node.status;
            electionRound.createdOn = node.createdOn;
            electionRound.lastModifiedOn = node.lastModifiedOn;
          })
      );
      await database.batch(newElectionRounds);
      const endTime = performance.now();
      console.log(
        `addElectionRoundsBulk took ${endTime - startTime} milliseconds.`
      );
    });
  } catch (err) {
    console.log(err);
  }
};
