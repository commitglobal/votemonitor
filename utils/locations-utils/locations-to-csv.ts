import { stringify } from "csv-stringify/sync";
import { promises as fs } from "fs"; // 'fs/promises' not available in node 12

const rawLocations = await fs.readFile("locations.json", "utf-8");

const featuresCollection = JSON.parse(rawLocations);

const output = stringify(
  featuresCollection.features
    .map((feature, index) => ({
      number: +feature.properties.NumarSectie,
      country: "",
      city: feature.properties.Localitate.trim(),
      location: feature.properties.Institutie.trim(),
      address: feature.properties.Adresa.trim(),
      latitude: feature.geometry.coordinates[1],
      longitude: feature.geometry.coordinates[0],
    }))
    .sort((a, b) => a.number - b.number),
  {
    header: true,
    quoted: true,
  }
);

await fs.writeFile(`diaspora-import.csv`, output, "utf8");
