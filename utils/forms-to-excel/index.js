#!/usr/bin/env node

import { readFileSync } from "fs";

import ExcelJS from "exceljs";
import { existsSync, readdirSync } from "node:fs";
import path from "path";

if (!existsSync("./forms")) {
  throw new Error(`Missing forms folder`);
}

const FORM_LANGUAGES = ["EN", "HY", "RU"];
const EXCEL_LANGUAGES = ["HY", "EN", "RU"];
const DEFAULT_LANGUAGE = "HY";

function isLocalizedMap(obj) {
  if (typeof obj !== "object" || obj === null || Array.isArray(obj)) {
    return false;
  }
  const keys = Object.keys(obj);
  return keys.length > 0 && keys.every((k) => /^[A-Z]{2}$/.test(k));
}

function flattenObject(obj, parentKey = "", result = {}) {
  for (let key in obj) {
    if (!obj.hasOwnProperty(key)) {
      continue;
    }

    const newKey = parentKey ? `${parentKey}.${key}` : key;
    const value = obj[key];

    if (key === "DisplayLogic") {
      result[newKey] =
        value === null || value === undefined ? "" : JSON.stringify(value);
      continue;
    }

    if (value === null || value === undefined) {
      result[newKey] = value;
      continue;
    }

    if (Array.isArray(value)) {
      value.forEach((item, index) => {
        const itemKey = `${newKey}[${index}]`;
        if (typeof item === "object" && item !== null) {
          flattenObject(item, itemKey, result);
        } else {
          result[itemKey] = item;
        }
      });
    } else if (isLocalizedMap(value)) {
      for (const lang of Object.keys(value)) {
        result[`${newKey}.${lang}`] = value[lang];
      }
    } else if (typeof value === "object") {
      flattenObject(value, newKey, result);
    } else {
      result[newKey] = value;
    }
  }
  return result;
}

function normalizeLocalizedMaps(value, languages) {
  if (Array.isArray(value)) {
    return value.map((item) => normalizeLocalizedMaps(item, languages));
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
      result[key] = normalizeLocalizedMaps(value[key], languages);
    }
    return result;
  }
  return value;
}

function normalizeForm(form) {
  form.defaultLanguage = DEFAULT_LANGUAGE;
  form.languages = [...FORM_LANGUAGES];
  return normalizeLocalizedMaps(form, FORM_LANGUAGES);
}

function getFieldName(key) {
  const dot = key.lastIndexOf(".");
  return dot === -1 ? key : key.slice(dot + 1);
}

function getPrefix(key) {
  const dot = key.lastIndexOf(".");
  return dot === -1 ? "" : key.slice(0, dot);
}

function fieldPriority(field) {
  switch (field.toLowerCase()) {
    case "id":
      return 0;
    case "code":
      return 1;
    case "text":
      return 2;
    case "helptext":
      return 3;
    default:
      return 100;
  }
}

function rootKeyPriority(key) {
  if (key === "id") return 0;
  if (key === "name") return 1;
  if (key === "description") return 2;
  if (key === "defaultLanguage") return 3;
  if (key.startsWith("languages[")) return 4;
  if (key.startsWith("questions[")) return 5;
  return 50;
}

function tokenizePath(path) {
  if (!path) return [];
  return path.split(/\.|\[|\]/).filter((t) => t !== "");
}

function comparePathTokens(a, b) {
  const ta = tokenizePath(a);
  const tb = tokenizePath(b);
  for (let i = 0; i < Math.max(ta.length, tb.length); i++) {
    const va = ta[i];
    const vb = tb[i];
    if (va === undefined) return -1;
    if (vb === undefined) return 1;
    const na = Number(va);
    const nb = Number(vb);
    if (!Number.isNaN(na) && !Number.isNaN(nb)) {
      if (na !== nb) return na - nb;
    } else {
      const c = va.localeCompare(vb, undefined, { sensitivity: "base" });
      if (c !== 0) return c;
    }
  }
  return 0;
}

function compareRowKeys(a, b) {
  const rootCmp = rootKeyPriority(a) - rootKeyPriority(b);
  if (rootCmp !== 0) return rootCmp;

  const prefixCmp = comparePathTokens(getPrefix(a), getPrefix(b));
  if (prefixCmp !== 0) return prefixCmp;

  const fieldCmp = fieldPriority(getFieldName(a)) - fieldPriority(getFieldName(b));
  if (fieldCmp !== 0) return fieldCmp;

  return a.localeCompare(b);
}

function formRows(flat, languages) {
  const localizedBases = new Set();
  const consumed = new Set();

  for (const key of Object.keys(flat)) {
    for (const lang of languages) {
      if (key.endsWith(`.${lang}`)) {
        localizedBases.add(key.slice(0, -(lang.length + 1)));
      }
    }
  }

  const rows = [];

  for (const base of localizedBases) {
    const row = { key: base, value: "" };
    for (const lang of languages) {
      consumed.add(`${base}.${lang}`);
      row[lang] = flat[`${base}.${lang}`] ?? "";
    }
    rows.push(row);
  }

  for (const key of Object.keys(flat)) {
    if (consumed.has(key)) {
      continue;
    }
    const row = { key, value: flat[key] ?? "" };
    for (const lang of languages) {
      row[lang] = "";
    }
    rows.push(row);
  }

  return rows.sort((a, b) => compareRowKeys(a.key, b.key));
}

function sanitizeSheetName(name) {
  return name.replace(/[\\/?*[\]:]/g, "").trim() || "Sheet";
}

function formDisplayName(form) {
  return form.name?.[DEFAULT_LANGUAGE] ?? form.id;
}

function worksheetName(form, usedNames) {
  const raw = formDisplayName(form);
  let name = sanitizeSheetName(String(raw));
  if (name.length > 31) {
    name = name.slice(0, 31);
  }

  let candidate = name;
  let n = 2;
  while (usedNames.has(candidate)) {
    const suffix = ` (${n})`;
    candidate = name.slice(0, 31 - suffix.length) + suffix;
    n++;
  }
  usedNames.add(candidate);
  return candidate;
}

const forms = [];

readdirSync("./forms", { withFileTypes: true })
  .filter((file) => file.isFile() && file.name.endsWith(".json"))
  .forEach((file) => {
    const form = readFileSync(path.join(file.parentPath, file.name), "utf8");
    const json = normalizeForm(JSON.parse(form));
    forms.push({ json, flat: flattenObject(json) });
  });

forms.sort((a, b) =>
  formDisplayName(a.json).localeCompare(formDisplayName(b.json), undefined, {
    sensitivity: "base",
  })
);

const workbook = new ExcelJS.Workbook();
const usedSheetNames = new Set();

for (const { json, flat } of forms) {
  const worksheet = workbook.addWorksheet(worksheetName(json, usedSheetNames));
  worksheet.columns = [
    { header: "Key", key: "key", width: 90 },
    ...EXCEL_LANGUAGES.map((lang) => ({ header: lang, key: lang, width: 50 })),
    { header: "Value", key: "value", width: 80 },
  ];

  formRows(flat, EXCEL_LANGUAGES).forEach((row) => {
    worksheet.addRow(row);
  });
}

await workbook.xlsx.writeFile("locales.xlsx");
console.log("Excel file created!");
