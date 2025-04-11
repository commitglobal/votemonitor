import { promises as fs } from "fs";
import Papa from "papaparse";

import { existsSync } from "node:fs";
import yargs from "yargs";
import { hideBin } from "yargs/helpers";

interface RawElectionsDataRow {
  Level1: string;
  Level2: string;
  Level3: string;
  Level4: string;
  Level5: string;
  Address: string;
  GID_0: string;
  GID_0_NAME: string;
  GID_1: string;
  GID_1_NAME: string;
  GID_2: string;
  GID_2_NAME: string;
  GID_3: string;
  GID_3_NAME: string;
  GID_4: string;
  GID_4_NAME: string;
  NumberOfPollingStations: number;
  QuickReportsSubmitted: number;
  FormSubmitted: number;
  PSISubmitted: number;
  NumberOfQuestionsAnswered: number;
  NumberOfFlaggedAnswers: number;
  ObserversWithForms: number;
  ObserversWithQuickReports: number;
  ObserversWithPSI: number;
  ActiveObservers: number;
  VisitedPollingStations: number;
}

async function transformData() {
  console.log("⏳ Generating ...");

  const start = Date.now();

  const options = await yargs(hideBin(process.argv))
    .usage("Usage: -i <election-id>")
    .option("i", {
      alias: "electionId",
      describe: "Election id",
      type: "string",
      demandOption: true,
    }).argv;

  const end = Date.now();

  const rawElectionsDataPath = `./raw-data/${options.i}.csv`;
  const outputPath = `./raw-data/${options.i}.sql`;
  if (!existsSync(rawElectionsDataPath)) {
    throw new Error(`Could not find raw CSV file = ${rawElectionsDataPath}`);
  }

  const rawElectionsData = await fs.readFile(rawElectionsDataPath, "utf-8");

  const results = Papa.parse<RawElectionsDataRow>(rawElectionsData, {
    header: true,
    skipEmptyLines: true,
    delimiter: ",",
    dynamicTyping: true,
  });

  if (results.errors.length) {
    results.errors.forEach((error, row) => console.log(`Error #${row}`, error));

    throw new Error("❌ Error parsing CSV file!");
  }

  const updates: string[] = results.data.map((row) => {
    const gidTags = {
      GID_0: row.GID_0,
      GID_0_NAME: row.GID_0_NAME,
      GID_1: row.GID_1,
      GID_1_NAME: row.GID_1_NAME,
      GID_2: row.GID_2,
      GID_2_NAME: row.GID_2_NAME,
      GID_3: row.GID_3,
      GID_3_NAME: row.GID_3_NAME,
      GID_4: row.GID_4,
      GID_4_NAME: row.GID_4_NAME,
    };

    const serializedTags = JSON.stringify(gidTags);

    return `UPDATE public."PollingStations"
            SET "Tags" = '${serializedTags}'
            WHERE "Level1" = '${row.Level1 ? row.Level1 : ""}'
            AND "Level2" = '${row.Level2 ? row.Level2 : ""}' 
            AND "Level3" = '${row.Level3 ? row.Level3 : ""}' 
            AND "Level4" = '${row.Level4 ? row.Level4 : ""}' 
            AND "Level5" = '${row.Level5 ? row.Level5 : ""}' 
            AND "Address" = '${row.Address ? row.Address : ""}'
            ;`;
  });

  await fs.writeFile(outputPath, updates.join("\n"), "utf8");

  console.log(`✅ Transformation completed in ${end - start}ms`);

  process.exit(0);
}

transformData().catch((err) => {
  console.error("❌ SQL generation failed");
  console.error(err);
  process.exit(1);
});
