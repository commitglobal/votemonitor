import { Model } from "@nozbe/watermelondb";
import { field, text } from "@nozbe/watermelondb/decorators";
import { DB_TABLE_NAMES } from "../schemas";

export class PollingStationsNom extends Model {
  static table = DB_TABLE_NAMES.POLLING_STATIONS_NOMENCLATOR;
  // @ts-ignore
  @field("_id") _id: number; // binds a table column to a model property
  // @ts-ignore
  @text("name") name: string;

  // @ts-ignore
  @text("election_round_id") electionRoundId: string;

  // @ts-ignore
  @text("polling_station_id") pollingStationId?: string;

  // @ts-ignore
  @field("polling_station_number") pollingStationNumber?: string;

  // @ts-ignore
  @field("parent_id") parentId?: number;
}
