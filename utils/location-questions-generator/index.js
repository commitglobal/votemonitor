#!/usr/bin/env node

import yargs from "yargs";
import { v4 as uuidv4 } from "uuid";

import { parse } from "csv-parse/sync";
import { hideBin } from "yargs/helpers";
import { promises as fs } from "fs";
import os from "os";
import { existsSync } from "node:fs";
import { uniqBy } from "lodash-es";

const newTranslatedString = (availableLanguages, value) => {
  const translatedString = {};
  availableLanguages.forEach((language) => {
    translatedString[language] = value;
  });

  return translatedString;
};

function buildKey(...args) {
  return args.join("-");
}

const options = yargs(hideBin(process.argv))
  .usage("Usage: -f <path-to-csv-file-with-locations>")
  .option("s", {
    alias: "pollingStationsCSVPath",
    describe: "CSV file containing polling stations",
    type: "string",
    demandOption: true,
  })
  .option("l", {
    alias: "localitiesCSVPath",
    describe: "CSV file containing locations",
    type: "string",
    demandOption: true,
  })
  .option("o", {
    alias: "outputDirectory",
    describe: "Output folder where to output generated questions.",
    type: "string",
    demandOption: true,
  })
  .option("t", {
    alias: "languages",
    describe: "Language codes for your questions.",
    type: "array",
    demandOption: true,
  }).argv;

if (!existsSync(options.outputDirectory)) {
  fs.mkdirSync(options.outputDirectory);
}

if (!existsSync(options.pollingStationsCSVPath)) {
  throw new Error(
    `Could not find polling stations CSV file = ${options.pollingStationsCSVPath}`
  );
}

if (!existsSync(options.localitiesCSVPath)) {
  throw new Error(
    `Could not find localities CSV file = ${options.localitiesCSVPath}`
  );
}

const pollingStationsContent = await fs.readFile(
  options.pollingStationsCSVPath
);
const localitiesContent = await fs.readFile(options.localitiesCSVPath);

const pollingStations = parse(pollingStationsContent)
  .slice(1)
  .map((row) => ({
    level1: row[0],
    level2: row[1],
    level3: row[2],
    level4: row[3],
    level5: row[4],
  }));

const localities = parse(localitiesContent)
  .slice(1)
  .map((row) => ({
    level1: row[0],
    level2: row[1],
    level3: row[2],
    level4: row[3],
    level5: row[4],
  }));

const keyStore = {};

const localitiesLevel1 = uniqBy(
  localities.filter((l) => !!l.level1),
  (l) => buildKey(l.level1)
);
const localitiesLevel2 = uniqBy(
  localities.filter((l) => !!l.level2),
  (l) => buildKey(l.level1, l.level2)
);
const localitiesLevel3 = uniqBy(
  localities.filter((l) => !!l.level3),
  (l) => buildKey(l.level1, l.level2, l.level3)
);
const localitiesLevel4 = uniqBy(
  localities.filter((l) => !!l.level4),
  (l) => buildKey(l.level1, l.level2, l.level3, l.level4)
);
const localitiesLevel5 = uniqBy(
  localities.filter((l) => !!l.level5),
  (l) => buildKey(l.level1, l.level2, l.level3, l.level4, l.level5)
);

localitiesLevel1.forEach((l) => {
  if (!keyStore[buildKey(l.level1)]) {
    keyStore[buildKey(l.level1)] = uuidv4();
  }
});

localitiesLevel2.forEach((l) => {
  if (!keyStore[buildKey(l.level1, l.level2)]) {
    keyStore[buildKey(l.level1, l.level2)] = uuidv4();
  }
});

localitiesLevel3.forEach((l) => {
  if (!keyStore[buildKey(l.level1, l.level2, l.level3)]) {
    keyStore[buildKey(l.level1, l.level2, l.level3)] = uuidv4();
  }
});

localitiesLevel4.forEach((l) => {
  if (!keyStore[buildKey(l.level1, l.level2, l.level3, l.level4)]) {
    keyStore[buildKey(l.level1, l.level2, l.level3, l.level4)] = uuidv4();
  }
});

localitiesLevel5.forEach((l) => {
  if (!keyStore[buildKey(l.level1, l.level2, l.level3, l.level4, l.level5)]) {
    keyStore[buildKey(l.level1, l.level2, l.level3, l.level4, l.level5)] =
      uuidv4();
  }
});

const rootQuestion = {
  Id: uuidv4(),
  Code: "root",
  Text: newTranslatedString(options.languages, "Select level1"),
  Options: localitiesLevel1.map((o) => ({
    Id: keyStore[buildKey(o.level1)],
    Text: newTranslatedString(options.languages, o.level1),
    IsFlagged: false,
    IsFreeText: false,
  })),
  $questionType: "singleSelectQuestion",
};

const level1Questions = localitiesLevel2.length
  ? localitiesLevel1.map((l) => ({
      Id: keyStore[buildKey(l.level1)],
      Code: buildKey(l.level1),
      Text: newTranslatedString(options.languages, "Select level 2"),
      Options: localitiesLevel2
        .filter((o) => buildKey(o.level1) === buildKey(l.level1))
        .map((o) => ({
          Id: keyStore[buildKey(o.level1, o.level2)],
          Text: newTranslatedString(options.languages, o.level2),
          IsFlagged: false,
          IsFreeText: false,
        })),
      DisplayCondition: {
        ParentQuestionId: rootQuestion.Id,
        Condition: "Includes",
        Value: keyStore[buildKey(l.level1)],
      },
      $questionType: "singleSelectQuestion",
    }))
  : [];

const level2Questions = localitiesLevel3.length
  ? localitiesLevel2.map((l) => ({
      Id: keyStore[buildKey(l.level1, l.level2)],
      Code: buildKey(l.level1, l.level2),
      Text: newTranslatedString(options.languages, "Select level 3"),
      Options: localitiesLevel3
        .filter(
          (o) => buildKey(o.level1, o.level2) === buildKey(l.level1, l.level2)
        )
        .map((o) => ({
          Id: keyStore[buildKey(o.level1, o.level2, o.level3)],
          Text: newTranslatedString(options.languages, o.level3),
          IsFlagged: false,
          IsFreeText: false,
        })),
      DisplayCondition: {
        ParentQuestionId: buildKey(l.level1),
        Condition: "Includes",
        Value: keyStore[buildKey(l.level1, l.level2)],
      },
      $questionType: "singleSelectQuestion",
    }))
  : [];

const level3Questions = localitiesLevel4.length
  ? localitiesLevel3.map((l) => ({
      Id: keyStore[buildKey(l.level1, l.level2, l.level3)],
      Code: buildKey(l.level1, l.level2, l.level3),
      Text: newTranslatedString(options.languages, "Select level 4"),
      Options: localitiesLevel4
        .filter(
          (o) =>
            buildKey(o.level1, o.level2, o.level3) ===
            buildKey(l.level1, l.level2, l.level3)
        )
        .map((o) => ({
          Id: keyStore[buildKey(o.level1, o.level2, o.level3, o.level4)],
          Text: newTranslatedString(options.languages, o.level4),
          IsFlagged: false,
          IsFreeText: false,
        })),
      DisplayCondition: {
        ParentQuestionId: buildKey(l.level1, l.level2),
        Condition: "Includes",
        Value: keyStore[buildKey(l.level1, l.level2, o.level3)],
      },
      $questionType: "singleSelectQuestion",
    }))
  : [];

const level4Questions = localitiesLevel5.length
  ? localitiesLevel4.map((l) => ({
      Id: keyStore[buildKey(l.level1, l.level2, l.level3, l.level4)],
      Code: buildKey(l.level1, l.level2, l.level3, l.level4),
      Text: newTranslatedString(options.languages, "Select level 5"),
      Options: localitiesLevel4
        .filter(
          (o) =>
            buildKey(o.level1, o.level2, o.level3, l.level4) ===
            buildKey(l.level1, l.level2, l.level3, l.level4)
        )
        .map((o) => ({
          Id: keyStore[
            buildKey(o.level1, o.level2, o.level3, o.level4, o.level5)
          ],
          Text: newTranslatedString(options.languages, o.level5),
          IsFlagged: false,
          IsFreeText: false,
        })),
      DisplayCondition: {
        ParentQuestionId: buildKey(l.level1, l.level2, l.level3),
        Condition: "Includes",
        Value: keyStore[buildKey(l.level1, l.level2, l.level3, l.level4)],
      },
      $questionType: "singleSelectQuestion",
    }))
  : [];

const questions = [
  rootQuestion,
  ...level1Questions,
  ...level2Questions,
  ...level3Questions,
  ...level4Questions,
];

await fs.writeFile(
  `localities-flow.json`,
  JSON.stringify(questions, null, 2),
  "utf8"
);
