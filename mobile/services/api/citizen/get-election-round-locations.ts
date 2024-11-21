import API from "../../api";
import { PollingStationNomenclatorVersionAPIResponse } from "../../definitions.api";

export interface ICitizenElectionRoundLocation {
  id: number;
  name: string;
  depth: number;
  parentId?: number; // available for the leafs
  displayOrder?: number;
  locationId?: string;
}

export type ICitizenElectionRoundLocationsAPIResponse = {
  electionRoundId: string;
  version: string; // cache bust key
  nodes: ICitizenElectionRoundLocation[];
};

export const getCitizenElectionRoundLocations = (
  electionRoundId: string,
): Promise<ICitizenElectionRoundLocationsAPIResponse> => {
  return API.get(`/election-rounds/${electionRoundId}/locations:fetchAll`).then((res) => res.data);
};

export const getCitizenElectionRoundLocationsVersion = (
  electionRoundId: string,
): Promise<PollingStationNomenclatorVersionAPIResponse> => {
  return API.get(`/election-rounds/${electionRoundId}/locations:version`).then((res) => res.data);
};
