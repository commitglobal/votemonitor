import { Model } from "@nozbe/watermelondb";
import { DB_TABLE_NAMES } from "../schemas";
import { field, text } from "@nozbe/watermelondb/decorators";

export class CitizenLocation extends Model {
  static table = DB_TABLE_NAMES.CITIZEN_LOCATIONS;

  // @ts-ignore
  @field("_id") _id: number;
  // @ts-ignore
  @text("election_round_id") electionRoundId: string;
  // @ts-ignore
  @text("name") name: string;
  // @ts-ignore
  @field("depth") depth: number;
  // @ts-ignore
  @field("display_order") displayOrder?: number;
  // @ts-ignore
  @field("parent_id") parentId?: number;
  // @ts-ignore
  @field("location_id") locationId?: string;
}
