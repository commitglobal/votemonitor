import type {
  ElectionDetailsModel,
  ElectionModel,
  GIDData,
} from "@/common/types";
import { promises as fs } from "fs";
import Papa from "papaparse";

import { existsSync } from "node:fs";
import elections from "public/elections-data/all.json";
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

function groupAndSumByGid(
  data: RawElectionsDataRow[],
  gidSelector: (row: RawElectionsDataRow) => string,
  gidNameSelector: (row: RawElectionsDataRow) => string
): GIDData[] {
  return Object.values(
    data.reduce((acc, row) => {
      const gid = gidSelector(row)?.trim();
      const gidName = gidNameSelector(row)?.trim();

      if (!gid) {
        return acc;
      }

      if (!acc[gid]) {
        const emptyGidData: GIDData = {
          gid: gid,
          gidName: gidName,
          numberOfPollingStations: 0,
          quickReportsSubmitted: 0,
          formSubmitted: 0,
          psiSubmitted: 0,
          numberOfQuestionsAnswered: 0,
          numberOfFlaggedAnswers: 0,
          observersWithForms: 0,
          observersWithQuickReports: 0,
          observersWithPSI: 0,
          activeObservers: 0,
          visitedPollingStations: 0,
        };

        acc[gid] = emptyGidData;
      }

      acc[gid].numberOfPollingStations += row.NumberOfPollingStations;
      acc[gid].quickReportsSubmitted += row.QuickReportsSubmitted;
      acc[gid].formSubmitted += row.FormSubmitted;
      acc[gid].psiSubmitted += row.PSISubmitted;
      acc[gid].numberOfQuestionsAnswered += row.NumberOfQuestionsAnswered;
      acc[gid].numberOfFlaggedAnswers += row.NumberOfFlaggedAnswers;
      acc[gid].observersWithForms += row.ObserversWithForms;
      acc[gid].observersWithQuickReports += row.ObserversWithQuickReports;
      acc[gid].observersWithPSI += row.ObserversWithPSI;
      acc[gid].activeObservers += row.ActiveObservers;
      acc[gid].visitedPollingStations += row.VisitedPollingStations;

      return acc;
    }, {} as Record<string, GIDData>)
  );
}

async function transformData() {
  console.log("⏳ Transforming ...");

  const start = Date.now();

  const options = await yargs(hideBin(process.argv))
    .usage("Usage: -i <election-id>")
    .option("i", {
      alias: "electionId",
      describe: "Election id",
      type: "string",
      demandOption: true,
    })
    .option("c", {
      alias: "gid0Code",
      describe: "Gid 0 code",
      type: "string",
      demandOption: true,
    })
    .option("m", {
      alias: "map",
      describe: "Map id",
      type: "string",
      demandOption: true,
    }).argv;

  const end = Date.now();

  const allAvailableElections = elections as ElectionModel[];
  const election = allAvailableElections.find((e) => e.id === options.i);
  if (election === undefined) {
    throw new Error("Election not found in public/elections-data/all.json");
  }

  const rawElectionsDataPath = `./raw-data/${options.i}.csv`;
  const outputPath = `./public/elections-data/${options.i}.json`;
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

  const gid0Data = groupAndSumByGid(
    results.data,
    (row) => row.GID_0,
    (row) => row.GID_0_NAME
  );
  const gid1Data = groupAndSumByGid(
    results.data,
    (row) => row.GID_1,
    (row) => row.GID_1_NAME
  );
  const gid2Data = groupAndSumByGid(
    results.data,
    (row) => row.GID_2,
    (row) => row.GID_2_NAME
  );
  const gid3Data = groupAndSumByGid(
    results.data,
    (row) => row.GID_3,
    (row) => row.GID_3_NAME
  );
  const gid4Data = groupAndSumByGid(
    results.data,
    (row) => row.GID_4,
    (row) => row.GID_4_NAME
  );

  if (!results.data.some((row) => row.GID_0 === options.c)) {
    throw new Error(`GID0 ${options.c} not found in data set!`);
  }

  const electionData: ElectionDetailsModel = {
    countryCode: election.countryCode,
    countryShortName: election.countryShortName,
    englishTitle: election.englishTitle,
    startDate: election.startDate,
    title: election.title,
    mapCode: options.m,
    gid0Code: options.c,
    gid0Data,
    gid1Data,
    gid2Data,
    gid3Data,
    gid4Data,
  };

  await fs.writeFile(outputPath, JSON.stringify(electionData, null, 4), "utf8");

  console.log(`✅ Transformation completed in ${end - start}ms`);

  process.exit(0);
}

transformData().catch((err) => {
  console.error("❌ Transformation failed");
  console.error(err);
  process.exit(1);
});
