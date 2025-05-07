import { promises as fs } from "fs"; // 'fs/promises' not available in node 12
import { stringify } from "csv-stringify/sync";
import { sortBy, uniqBy } from "lodash-es";
import Papa from "papaparse";
interface LocationRow {
  Level1: string;
  Level2: string;
  Level3: string;
  Level4: string;
  Level5: string;
  Nationals: number;
  ForeignResidents: number;
  Address: string;
}

const rawLocations = await fs.readFile("buenos-aires.csv", "utf-8");

const parseResults = Papa.parse<LocationRow>(rawLocations, {
  header: true,
  skipEmptyLines: true,
  delimiter: ",",
  dynamicTyping: true,
});

if (parseResults.errors.length) {
  parseResults.errors.forEach((error, row) =>
    console.log(`Error #${row}`, error)
  );

  throw new Error("❌ Error parsing CSV file!");
}

const result = parseResults.data.flatMap((row) => {
  const nationalsPollingStations = row.Nationals
    ? Array.from({ length: row.Nationals }, (_, i) => ({
        Level1: row.Level1.trim(),
        Level2: "Nationals Table",
        Level3: "",
        Level4: "",
        Level5: "",
        Number: i + 1,
        Address: row.Address.trim(),
      }))
    : [];

  const foreignResidentsPollingStations = row.ForeignResidents
    ? Array.from({ length: row.ForeignResidents }, (_, i) => ({
        Level1: row.Level1,
        Level2: "Foreign Residents Table",
        Level3: "",
        Level4: "",
        Level5: "",
        Number: i + 1,
        Address: row.Address,
      }))
    : [];
  return [...nationalsPollingStations, ...foreignResidentsPollingStations];
});

const output = stringify(
  result.map((location, index) => ({ ...location, DisplayOrder: index + 1 })),
  {
    header: true,
    quoted: true,
    columns: {
      Level1: "Level1",
      Level2: "Level2",
      Level3: "Level3",
      Level4: "Level4",
      Level5: "Level5",
      Number: "Number",
      Address: "Address",
      DisplayOrder: "DisplayOrder",
    },
  }
);

await fs.writeFile(`polling-stations.csv`, output, "utf8");
