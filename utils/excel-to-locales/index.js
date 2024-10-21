#!/usr/bin/env node

import yargs from "yargs";

import { parse } from "csv-parse/sync";
import { promises as fs } from "fs";
import { existsSync, mkdirSync } from "node:fs";
import { hideBin } from "yargs/helpers";

export const isNotNilOrWhitespace = (input) => (input?.trim()?.length || 0) > 0;

function unflattenObject(flattened) {
  const result = {};

  for (let key in flattened) {
    if (flattened.hasOwnProperty(key)) {
      let keys = key.split(/[\.\[\]]+/).filter((k) => k !== ""); // Split by dots or brackets, filtering out empty parts
      keys.reduce((acc, part, index) => {
        // If it's the last part, set the value
        if (index === keys.length - 1) {
          acc[part] = flattened[key];
        } else {
          // If not, ensure the next part is an object or array
          if (!acc[part]) {
            acc[part] = isNaN(keys[index + 1]) ? {} : [];
          }
        }
        return acc[part]; // Move deeper in the object
      }, result);
    }
  }

  return result;
}

function ensureDirectoryExists(directory) {
  if (!existsSync(directory)) {
    mkdirSync(directory, { recursive: true });
  }
}

const options = yargs(hideBin(process.argv))
  .usage("Usage: -l <path-to-csv-file-with-translations>")
  .option("l", {
    alias: "translationsSheetPath",
    describe: "CSV file containing translations",
    type: "string",
    demandOption: true,
  }).argv;

if (!existsSync(options.translationsSheetPath)) {
  throw new Error(
    `Could not find translations CSV file = ${options.translationsSheetPath}`
  );
}

const translationContent = await fs.readFile(options.translationsSheetPath);

const parsedTranslations = parse(translationContent);

const languages = parsedTranslations[0]
  .map((languageCode, columnIndex) => ({
    index: columnIndex,
    code: languageCode,
  }))
  .slice(1)
  .filter((language) => isNotNilOrWhitespace(language.code));

const locales = parsedTranslations.slice(1).map((row) => {
  const translations = {};

  languages.map((language) => {
    translations[language.code] = row[language.index];
  });

  return {
    key: row[0],
    translations,
  };
});

for (const language of languages) {
  const flattenedObject = {};

  for (const locale of locales) {
    flattenedObject[locale.key] = locale.translations[language.code];
  }

  ensureDirectoryExists(`locales/${language.code}`);

  const fileName =
    (language.code.toLowerCase() === "en" ||  language.code.toLowerCase() === "ro" )
      ? `locales/${language.code}/translations.json`
      : `locales/${
          language.code
        }/translations_${language.code.toUpperCase()}.json`;

  await fs.writeFile(
    fileName,
    JSON.stringify(unflattenObject(flattenedObject), null, 4),
    "utf8"
  );
}

console.log("done")