import { stringify } from "csv-stringify/sync";
import { promises as fs } from "fs"; // 'fs/promises' not available in node 12

const rawLocations = await fs.readFile("locations.json", "utf-8");

const locations = JSON.parse(rawLocations);

const output = stringify(
  locations.map((feature) => ({
    Level1: "Diaspora",
    Level2: feature.country,
    Level3: feature.city,
    Level4: "",
    Level5: "",
    Number: feature.number,
    Address: feature.address,
  })),
  {
    header: true,
    quoted: true,
  }
);

await fs.writeFile(`diaspora-to-import.csv`, output, "utf8");
