#!/usr/bin/env node

import { readFileSync, writeFileSync } from "fs";

import { existsSync, readdirSync } from "node:fs";
import path from "path";

// Path to the directory containing the files
const directoryPath = "./out/answers-fragments";

// The DOCTYPE declaration to remove
const doctypeDeclaration = `<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">`;

// Read all files in the directory
const files = readdirSync(directoryPath);
// Loop through each file in the directory
files.forEach((file) => {
  const filePath = path.join(directoryPath, file);
  console.log(filePath);
  const fragment = readFileSync(filePath, "utf8");

  // Remove the DOCTYPE declaration if it exists
  if (fragment.includes(doctypeDeclaration)) {
    const updatedData = fragment.replace(doctypeDeclaration, "");

    // Write the updated content back to the file
    writeFileSync(filePath, updatedData, "utf8");

    console.log(`Updated file: ${file}`);
  } else {
    console.log(`DOCTYPE not found in file: ${file}`);
  }
});
