import { promises as fs } from "fs"; // 'fs/promises' not available in node 12
import * as cheerio from "cheerio";
import { stringify } from "csv-stringify/sync";

async function fetchAndParse(url) {
  const res = await fetch(url, {
    headers: { "user-agent": "Mozilla/5.0" }, // avoid some basic blocks
  });
  if (!res.ok) throw new Error(`HTTP ${res.status}`);
  const html = await res.text();

  const $ = cheerio.load(html);

  return $;
}

fetchAndParse(
  "https://www.valg.no/en/polling-stations/polling-stations/polling-places/"
)
  .then(async ($) => {
    const psList: any = [];
    const counties = $(".vd-list__item a")
      .map((_, el) => {
        return {
          name: $(el).find(".vd-list__name").text().trim(),
          url: $(el).attr("href"),
        };
      })
      .get();

    for (const county of counties) {
      const $c = await fetchAndParse(
        `https://www.valg.no/en/polling-stations/polling-stations/polling-places/${county.url}`
      );
      const municipalities = $c(".vd-list__item a")
        .map((_, el) => {
          return {
            name: $c(el).find(".vd-list__name").text().trim(),
            url: $c(el).attr("href"),
          };
        })
        .get();

      for (const municipality of municipalities) {
        const $m = await fetchAndParse(
          `https://www.valg.no/en/polling-stations/polling-stations/polling-places/${municipality.url}`
        );
        const pollingStations = $m(".vd-list__item a")
          .map((_, el) => {
            return {
              name: $m(el).find(".vd-list__name").text().trim(),
              url: $m(el).attr("href"),
            };
          })
          .get();

        for (const ps of pollingStations) {
          const $ps = await fetchAndParse(`https://www.valg.no${ps.url}`);
          const name = $ps(".vd-section__header").text().trim();

          // Address (join all li inside .vd-address with space)
          const address = $ps(".vd-address .vd-list__item")
            .map((_, el) => $ps(el).text().trim())
            .get()
            .join("\n");

          psList.push({
            county: county.name,
            municipality: municipality.name,
            number: name,
            address,
            url: ps.url,
          });
        }
      }
    }

    const output = stringify(psList);
    await fs.writeFile(`norway-polling-stations.csv`, output, "utf8");
  })
  .catch(console.error);
