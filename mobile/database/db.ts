import { Database } from "@nozbe/watermelondb";
import SQLiteAdapter from "@nozbe/watermelondb/adapters/sqlite";
import { PollingStationsNom } from "./models/polling-stations-nomenclator.model";
import { schema } from "./schemas";
import { CitizenLocation } from "./models/citizen-locations.model";
import * as Sentry from "@sentry/react-native";
import { Platform } from "react-native";

const adapter = new SQLiteAdapter({
  schema,
  jsi: Platform?.OS === "ios", // enable if Platform.OS === 'ios',
  onSetUpError: (error) => {
    Sentry.captureException(error);
  },
});

const database = new Database({
  adapter,
  modelClasses: [PollingStationsNom, CitizenLocation],
});

export { database };
