#!/usr/bin/env node

import yargs from 'yargs';
import { v4 as uuidv4 } from 'uuid';

import { parse } from 'csv-parse/sync';
import { hideBin } from 'yargs/helpers';
import {promises as fs} from 'fs';
import os from 'os';
import { existsSync } from 'node:fs';

const options = yargs(hideBin(process.argv))
  .usage("Usage: -f <path-to-csv-file-with-locations>")
  .option("s", { alias: "pollingStationsCSVPath", describe: "CSV file containing polling stations", type: "string", demandOption: true })
  .option("l", { alias: "localitiesCSVPath", describe: "CSV file containing locations", type: "string", demandOption: true })
  .option('o', { alias: 'outputDirectory', describe: 'Output folder where to output generated questions.', type: 'string', demandOption: true })
  .option('t', { alias: 'languages', describe: 'Language codes for your questions.', type: 'array', demandOption: true })
  .argv;


if (!existsSync(options.outputDirectory)) {
  fs.mkdirSync(options.outputDirectory)
}

if (!existsSync(options.pollingStationsCSVPath)) {
  throw new Error(`Could not find polling stations CSV file = ${options.pollingStationsCSVPath}`)
}

if (!existsSync(options.localitiesCSVPath)) {
  throw new Error(`Could not find localities CSV file = ${options.localitiesCSVPath}`)
}


const pollingStationsContent = await fs.readFile(options.pollingStationsCSVPath);
const localitiesContent = await fs.readFile(options.localitiesCSVPath);

const pollingStations = parse(pollingStationsContent);
const localities = parse(localitiesContent);
