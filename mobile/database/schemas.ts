import { appSchema, tableSchema } from "@nozbe/watermelondb";

export enum DB_TABLE_NAMES {
  ELECTION_ROUNDS = "election_rounds",
  POLLING_STATIONS_NOMENCLATOR = "polling_stations_nom",
  POLLING_STATION_INFORMATION_FORM = "POLLING_STATION_INFORMATION_FORM",
}

export const schema = appSchema({
  version: 16,
  tables: [
    tableSchema({
      name: DB_TABLE_NAMES.POLLING_STATIONS_NOMENCLATOR,
      columns: [
        { name: "_id", type: "number", isIndexed: true }, // indexed means that we can search the column based on the title
        { name: "name", type: "string" },
        { name: "election_round_id", type: "string" },
        { name: "polling_station_id", type: "string", isOptional: true },
        { name: "polling_station_number", type: "number", isOptional: true },
        { name: "parent_id", type: "number", isOptional: true },
      ],
    }),
    tableSchema({
      name: DB_TABLE_NAMES.ELECTION_ROUNDS,
      columns: [
        { name: "_id", type: "string" },
        { name: "country_id", type: "string" },
        { name: "country", type: "string" },
        { name: "title", type: "string" },
        { name: "english_title", type: "string" },
        { name: "start_date", type: "string" },
        { name: "end_date", type: "string" },
        { name: "status", type: "string" },
        { name: "created_on", type: "string" },
        { name: "last_modified_on", type: "string" },
      ],
    }),
    tableSchema({
      name: DB_TABLE_NAMES.POLLING_STATION_INFORMATION_FORM,
      columns: [
        { name: "_id", type: "string" },

        { name: "languages", type: "string" }, // JSON
        { name: "questions", type: "string" }, // JSON

        { name: "created_on", type: "string" },
        { name: "last_modified_on", type: "string" },

        { name: "election_round_id", type: "string" },
      ],
    }),
  ],
});
