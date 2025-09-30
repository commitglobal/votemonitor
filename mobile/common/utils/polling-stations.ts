export const getPollingStationDisplay = (pollingStation: {
  number: string;
  level1?: string;
  level2?: string;
  level3?: string;
  level4?: string;
  level5?: string;
}) =>
  [
    pollingStation.level1,
    pollingStation.level2,
    pollingStation.level3,
    pollingStation.level4,
    pollingStation.level5,
    pollingStation.number,
  ]
    .filter(Boolean)
    .join(" / ");
