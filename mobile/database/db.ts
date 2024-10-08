import { Database } from "@nozbe/watermelondb";
import SQLiteAdapter from "@nozbe/watermelondb/adapters/sqlite";
import { PollingStationsNom } from "./models/polling-stations-nomenclator.model";
import { schema } from "./schemas";
import { CitizenLocation } from "./models/citizen-locations.model";

const adapter = new SQLiteAdapter({
  schema,
  jsi: true /* enable if Platform.OS === 'ios' */,
});

const database = new Database({
  adapter,
  modelClasses: [PollingStationsNom, CitizenLocation],
});

export { database };
