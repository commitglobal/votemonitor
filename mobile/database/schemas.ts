import { appSchema, tableSchema } from "@nozbe/watermelondb";

export enum DB_TABLE_NAMES {
  POLLING_STATIONS_NOMENCLATOR = "polling_stations_nom",
}

export const schema = appSchema({
  version: 17,
  tables: [
    tableSchema({
      name: DB_TABLE_NAMES.POLLING_STATIONS_NOMENCLATOR,
      columns: [
        { name: "_id", type: "number", isIndexed: true }, // indexed means that we can search the column based on the title
        { name: "name", type: "string" },
        { name: "election_round_id", type: "string" },
        { name: "polling_station_id", type: "string", isOptional: true },
        { name: "polling_station_number", type: "string", isOptional: true },
        { name: "parent_id", type: "number", isOptional: true },
      ],
    }),
  ],
});
