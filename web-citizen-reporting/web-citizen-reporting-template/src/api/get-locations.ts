import type {
  ElectionRoundLocationsModel,
  ElectionsRoundLocationsCacheModel,
  LocationNodeModel,
} from "@/common/types";
import { electionRoundId } from "@/lib/utils";
import { API } from "./api";

export const getLocations = (): Promise<
  Record<string, LocationNodeModel[]>
> => {
  return API.get<ElectionRoundLocationsModel>(
    `/api/election-rounds/${electionRoundId}/locations:fetchAll`
  ).then((response) =>
    response.data.nodes.reduce<Record<string, LocationNodeModel[]>>(
      (group, node) => ({
        ...group,
        [node.depth]: [...(group[node.depth] ?? []), node],
      }),
      {}
    )
  );
};

export const getLocationsVersion = (
  electionRoundId: string
): Promise<ElectionsRoundLocationsCacheModel> => {
  return API.get(
    `/api/election-rounds/${electionRoundId}/locations:version`
  ).then((res) => res.data);
};
