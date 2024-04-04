import { Database } from "@nozbe/watermelondb";
import SQLiteAdapter from "@nozbe/watermelondb/adapters/sqlite";
import { PollingStationsNom } from "./models/polling-stations-nomenclator.model";
import { schema } from "./schemas";
import { ElectionRoundModelDB } from "./models/election-rounds.model";
import { PollingStationInformationFormDBModel } from "./models/polling-station-information-form.model";

const adapter = new SQLiteAdapter({
  schema,
  jsi: true /* enable if Platform.OS === 'ios' */,
});

const database = new Database({
  adapter,
  modelClasses: [
    PollingStationsNom,
    ElectionRoundModelDB,
    PollingStationInformationFormDBModel,
  ],
});

export { database };
