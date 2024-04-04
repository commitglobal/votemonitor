import { Model } from "@nozbe/watermelondb";
import { text } from "@nozbe/watermelondb/decorators";
import { DB_TABLE_NAMES } from "../schemas";

export class ElectionRoundModelDB extends Model {
  static table = DB_TABLE_NAMES.ELECTION_ROUNDS;
  // @ts-ignore
  @text("_id") _id;

  // @ts-ignore
  @text("status") status;

  // @ts-ignore
  @text("title") title;
  // @ts-ignore
  @text("english_title") englishTitle;

  // @ts-ignore
  @text("country_id") countryId;
  // @ts-ignore
  @text("country") country;

  // @ts-ignore
  @text("start_date") startDate;
  // @ts-ignore
  @text("end_date") endDate;

  // @ts-ignore
  @text("created_on") createdOn;
  // @ts-ignore
  @text("last_modified_on") lastModifiedOn;
}
