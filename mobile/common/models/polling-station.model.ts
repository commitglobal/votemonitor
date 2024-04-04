export type PollingStationNomenclatorNodeVM = {
  id: number;
  name: string;
  parentId?: number; // available for the leafs
  number?: number; // available for the leafs
  pollingStationId?: string; // available for the leafs
};

export type PollingStationVM = {
  id: string;
  name: string;
  number: number;
};
