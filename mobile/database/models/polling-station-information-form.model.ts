import { Model } from "@nozbe/watermelondb";
import {
  field,
  immutableRelation,
  json,
  text,
} from "@nozbe/watermelondb/decorators";
import { DB_TABLE_NAMES } from "../schemas";

const sanitizeLanguages = (rawLanguages: any) => {
  return Array.isArray(rawLanguages) ? rawLanguages.map(String) : [];
};

const sanitizeQuestions = (json: any | undefined) => (json ? json : {});

export class PollingStationInformationFormDBModel extends Model {
  static table = DB_TABLE_NAMES.POLLING_STATION_INFORMATION_FORM;

  @field("_id") _id!: string;

  @json("languages", sanitizeLanguages) languages!: string[];

  @json("questions", sanitizeQuestions) questions: any;

  @text("created_on") createdOn!: string;

  @text("last_modified_on") lastModifiedOn!: string;

  @text("election_round_id") electionRoundId!: string;

  // @immutableRelation(
  //   DB_TABLE_NAMES.POLLING_STATION_INFORMATION_FORM,
  //   "election_round_id"
  // )
  // electionRoundId!: string;
}
