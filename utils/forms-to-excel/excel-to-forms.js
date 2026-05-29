#!/usr/bin/env node

import ExcelJS from "exceljs";
import { existsSync, mkdirSync, writeFileSync } from "node:fs";

const inputPath = process.argv[2] ?? "./locales.xlsx";
const outputDir = process.argv[3] ?? "./forms";

const FORM_LANGUAGES = ["EN", "HY", "RU"];
const DEFAULT_LANGUAGE = "HY";

function isLocalizedMap(obj) {
  if (!obj || typeof obj !== "object" || Array.isArray(obj)) {
    return false;
  }
  const keys = Object.keys(obj);
  return keys.length > 0 && keys.every((k) => /^[A-Z]{2}$/.test(k));
}

function pruneByLanguages(value, languages) {
  if (Array.isArray(value)) {
    return value.map((item) => pruneByLanguages(item, languages));
  }
  if (isLocalizedMap(value)) {
    const result = {};
    for (const lang of languages) {
      result[lang] = value[lang] ?? "";
    }
    return result;
  }
  if (value && typeof value === "object") {
    const result = {};
    for (const key of Object.keys(value)) {
      result[key] = pruneByLanguages(value[key], languages);
    }
    return result;
  }
  return value;
}

function unflattenObject(flattened) {
  const result = {};

  for (const key in flattened) {
    if (!Object.hasOwn(flattened, key)) {
      continue;
    }

    const keys = key.split(/\.|\[|\]/).filter((k) => k !== "");
    keys.reduce((acc, part, index) => {
      if (index === keys.length - 1) {
        acc[part] = flattened[key];
      } else if (!acc[part]) {
        acc[part] = Number.isNaN(Number(keys[index + 1])) ? {} : [];
      }
      return acc[part];
    }, result);
  }

  return result;
}

function cellText(cell) {
  const value = cell?.value;
  if (value === null || value === undefined) {
    return "";
  }
  if (typeof value === "object" && Array.isArray(value.richText)) {
    return value.richText.map((part) => part.text).join("");
  }
  if (typeof value === "object" && "text" in value) {
    return value.text;
  }
  return value;
}

function isEmpty(value) {
  return value === "" || value === null || value === undefined;
}

function parseScalar(key, value) {
  if (typeof value === "boolean" || typeof value === "number") {
    return value;
  }

  const text = String(value).trim();
  if (text === "") {
    return "";
  }
  if (text === "true") {
    return true;
  }
  if (text === "false") {
    return false;
  }
  if (text === "null") {
    return null;
  }
  if (key.endsWith("DisplayLogic") || (text.startsWith("{") && text.endsWith("}"))) {
    try {
      return JSON.parse(text);
    } catch {
      return text;
    }
  }
  return text;
}

function isLocalizedRow(key, value) {
  if (!isEmpty(value)) {
    return false;
  }
  return !key.endsWith("DisplayLogic");
}

function findColumn(header, name) {
  const index = header.findIndex((cell) => cell === name);
  if (index < 1) {
    return -1;
  }
  return index;
}

function languageColumns(header) {
  const valueCol = findColumn(header, "value");
  const columns = {};

  for (let i = 1; i < header.length; i++) {
    if (i === valueCol) {
      continue;
    }
    const name = header[i];
    if (name && name !== "key") {
      columns[name.toUpperCase()] = i;
    }
  }

  return columns;
}

function worksheetToFlat(worksheet) {
  const flat = {};
  const header = worksheet.getRow(1).values.map((v) =>
    String(v ?? "")
      .trim()
      .toLowerCase()
  );
  const keyCol = findColumn(header, "key");
  const valueCol = findColumn(header, "value");
  const langCols = languageColumns(header);

  if (keyCol < 0 || valueCol < 0 || Object.keys(langCols).length === 0) {
    throw new Error(
      `Worksheet "${worksheet.name}" is missing Key, language, or Value columns`
    );
  }

  worksheet.eachRow((row, rowNumber) => {
    if (rowNumber === 1) {
      return;
    }

    const key = String(cellText(row.getCell(keyCol)) ?? "").trim();
    if (!key) {
      return;
    }

    const value = cellText(row.getCell(valueCol));

    if (isLocalizedRow(key, value)) {
      for (const [lang, col] of Object.entries(langCols)) {
        flat[`${key}.${lang}`] = cellText(row.getCell(col)) ?? "";
      }
      return;
    }

    if (key.endsWith("DisplayLogic") && isEmpty(value)) {
      flat[key] = null;
      return;
    }

    flat[key] = parseScalar(key, value);
  });

  return flat;
}

if (!existsSync(inputPath)) {
  throw new Error(`Missing Excel file: ${inputPath}`);
}

mkdirSync(outputDir, { recursive: true });

const workbook = new ExcelJS.Workbook();
await workbook.xlsx.readFile(inputPath);

for (const worksheet of workbook.worksheets) {
  const flat = worksheetToFlat(worksheet);
  let form = unflattenObject(flat);
  form = pruneByLanguages(form, FORM_LANGUAGES);
  form.defaultLanguage = DEFAULT_LANGUAGE;
  form.languages = [...FORM_LANGUAGES];
  const formId = form.id;

  if (!formId) {
    throw new Error(`Worksheet "${worksheet.name}" has no form id`);
  }

  const outPath = `${outputDir}/${formId}.json`;
  writeFileSync(outPath, `${JSON.stringify(form, null, 4)}\n`, "utf8");
  console.log(`Wrote ${outPath}`);
}

console.log("Done");
