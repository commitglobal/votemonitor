import { Q } from "@nozbe/watermelondb";
import { PollingStationInformationFormAPIResponse } from "../../services/election-rounds/election-rounds.api";
import { database } from "../db";
import { PollingStationInformationFormDBModel } from "../models/polling-station-information-form.model";
import { DB_TABLE_NAMES } from "../schemas";

export const add = async (
  electionRoundId: string,
  data: PollingStationInformationFormAPIResponse
): Promise<PollingStationInformationFormDBModel | null> => {
  try {
    let form: PollingStationInformationFormDBModel | null = null;

    await database.write(async () => {
      const createdForm = await database
        .get<PollingStationInformationFormDBModel>(
          DB_TABLE_NAMES.POLLING_STATION_INFORMATION_FORM
        )
        .create((record) => {
          record._id = data.id;
          record.createdOn = data.createdOn;
          record.lastModifiedOn = data.lastModifiedOn;

          record.languages = data.languages;
          record.questions = data.questions;

          record.electionRoundId = electionRoundId;
        });
      form = createdForm;
    });

    return form;
  } catch (err) {
    console.log(err);
    return null;
  }
};

export const get = async (
  electionRoundId: string
): Promise<PollingStationInformationFormDBModel | null> => {
  try {
    const data = await database
      .get<PollingStationInformationFormDBModel>(
        DB_TABLE_NAMES.POLLING_STATION_INFORMATION_FORM
      )
      .query(Q.where("election_round_id", electionRoundId), Q.take(1));

    return data[0];
  } catch (err) {
    console.log(err);
    return null;
  }
};
