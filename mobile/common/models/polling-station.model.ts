export type PollingStationNomenclatorNodeVM = {
  id: number;
  name: string;
  parentId?: number; // available for the leafs
  number?: string; // available for the leafs
  pollingStationId?: string; // available for the leafs
};

export type PollingStationVisitVM = {
  pollingStationId: string;
  visitedAt: string; // ISO date
  address: string;
  number: string;
  level1?: string;
  level2?: string;
  level3?: string;
};
