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
}

const rawLocations = await fs.readFile("ecuador-locations.csv", "utf-8");

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
// console.log(parseResults.data[0]);
const results = sortBy(
  uniqBy(parseResults.data, (l) =>
    [l.Level1, l.Level2, l.Level3, l.Level4, l.Level5].join(",")
  ),
  [
    (l) => l.Level1,
    (l) => l.Level2,
    (l) => l.Level3,
    (l) => l.Level4,
    (l) => l.Level5,
  ]
);

const output = stringify(
  results.map((location, index) => ({ ...location, DisplayOrder: index + 1 })),
  {
    header: true,
    quoted: true,
    columns: {
      Level1: "Level1",
      Level2: "Level2",
      Level3: "Level3",
      Level4: "Level4",
      Level5: "Level5",
      DisplayOrder: "DisplayOrder",
    },
  }
);

await fs.writeFile(`localities.csv`, output, "utf8");
