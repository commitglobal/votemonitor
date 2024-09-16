#!/usr/bin/env node


import { readFileSync } from "fs";

import ExcelJS from "exceljs";
import { maxBy } from "lodash-es";
import { existsSync, readdirSync } from "node:fs";
import path from "path";

if (!existsSync("./locales")) {
  throw new Error(`Missing locales folder`);
}

function flattenObject(obj, parentKey = "", result = {}) {
  for (let key in obj) {
    if (obj.hasOwnProperty(key)) {
      let newKey = parentKey ? `${parentKey}.${key}` : key;

      if (typeof obj[key] === "object" && obj[key] !== null) {
        if (Array.isArray(obj[key])) {
          // Handle arrays by including the index in the key
          obj[key].forEach((item, index) => {
            flattenObject(item, `${newKey}[${index}]`, result);
          });
        } else {
          flattenObject(obj[key], newKey, result);
        }
      } else {
        result[newKey] = obj[key];
      }
    }
  }
  return result;
}

const dictionary = {};
const languages = [];

readdirSync("./locales", { withFileTypes: true, recursive: true })
  .filter((file) => file.isFile() && file.name.endsWith(".json"))
  .forEach((file) => {
    const languageCode = file.parentPath.replace("locales/", "");

    const locales = readFileSync(path.join(file.parentPath, file.name), "utf8");

    const json = JSON.parse(locales);

    dictionary[languageCode] = flattenObject(json);
    languages.push(languageCode);
  });

const languageWithLongestNumberOfKeys = maxBy(
  languages,
  (l) => Object.keys(dictionary[l]).length
);

const keys = Object.keys(dictionary[languageWithLongestNumberOfKeys]);

const data = keys.map((key) => {
  const keyData = {
    keyName: key,
  };

  languages.forEach((l) => {
    keyData[l] = dictionary[l][key] ?? "";
  });

  return keyData;
});

// Create a new workbook
const workbook = new ExcelJS.Workbook();
const worksheet = workbook.addWorksheet("locales");

// Define columns
worksheet.columns = [
  { header: "Key", key: "keyName", width: 90 },
  ...languages.map((l) => ({ header: l, key: l, width: 50, })),
];

// Add rows
data.forEach((item) => {
  worksheet.addRow(item);
});

// Write the workbook to a file
await workbook.xlsx.writeFile("locales.xlsx");
console.log("Excel file created!");
