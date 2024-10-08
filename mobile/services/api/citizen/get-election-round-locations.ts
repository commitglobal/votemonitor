import API from "../../api";

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
