import type {
  ElectionRoundLocationsModel,
  ElectionsRoundLocationsCacheModel,
} from "@/common/types";
import { electionRoundId } from "@/lib/utils";
import { API } from "./api";

export const getLocations = (): Promise<ElectionRoundLocationsModel> => {
  return API.get(
    `/api//election-rounds/${electionRoundId}/locations:fetchAll`
  ).then((res) => res.data);
};

export const getLocationsVersion = (
  electionRoundId: string
): Promise<ElectionsRoundLocationsCacheModel> => {
  return API.get(`/election-rounds/${electionRoundId}/locations:version`).then(
    (res) => res.data
  );
};
