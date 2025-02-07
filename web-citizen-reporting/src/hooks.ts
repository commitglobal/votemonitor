import { useMutation, useQuery, UseQueryResult } from "@tanstack/react-query";
import { useNavigate } from "@tanstack/react-router";
import { useReducer } from "react";
import { noAuthApi } from "./common/no-auth-api";
import { CitizenReportPageResponse, LevelNode } from "./common/types";
import { locationReducer, LocationState } from "./location-reducer";

type LocationsLevelsResponse = { nodes: LevelNode[] };

type UseLocationsLevelsResult = UseQueryResult<
  Record<string, LevelNode[]>,
  Error
>;

export const useCitizenForms = (electionRoundId: string) => {
  return useQuery({
    queryKey: ["citizen-reporting-forms"],
    queryFn: async () => {
      const response = await noAuthApi.get<CitizenReportPageResponse>(
        `/election-rounds/${electionRoundId}/citizen-reporting-forms`,
        {}
      );

      if (response.status !== 200) {
        throw new Error("Failed to fetch forms");
      }

      return response.data;
    },
    enabled: !!electionRoundId,
  });
};

export function useLocationsNodes(electionRoundId: string) {
  return useQuery({
    queryKey: ["locations", "nodes"],
    queryFn: async () => {
      const response = await noAuthApi.get<LocationsLevelsResponse>(
        `/election-rounds/${electionRoundId}/locations:fetchAll`
      );

      return response.data.nodes;
    },
    enabled: !!electionRoundId,
  });
}

export function useLocationsLevels(
  electionRoundId: string
): UseLocationsLevelsResult {
  return useQuery({
    queryKey: ["locations", "levels"],
    queryFn: async () => {
      const response = await noAuthApi.get<LocationsLevelsResponse>(
        `/election-rounds/${electionRoundId}/locations:fetchAll`
      );

      return response.data.nodes.reduce<Record<string, LevelNode[]>>(
        (group, node) => ({
          ...group,
          [node.depth]: [...(group[node.depth] ?? []), node],
        }),
        {}
      );
    },
    enabled: !!electionRoundId,
  });
}

export type HandleReducerSearchParams = {
  level: keyof LocationState;
  value: string;
};

export const useLocationFilters = (electionRoundId: string) => {
  const [search, dispatch] = useReducer(locationReducer, {});
  const { data: nodes } = useLocationsNodes(electionRoundId);
  const nodesWithLocationId = new Map(
    nodes
      ?.filter((node) => "locationId" in node)
      .map((node) => [node.id, node.locationId])
  );

  const lastNodeIdSelected = Object.values(search)
    .filter((nodeId) => nodeId)
    .at(-1);
  const locationId =
    lastNodeIdSelected && nodesWithLocationId.get(lastNodeIdSelected);

  const handleLocationChange = ({
    level,
    value,
  }: HandleReducerSearchParams) => {
    return dispatch({ type: "SET_FILTER", level, value: Number(value) });
  };

  return { search, locationId, handleLocationChange };
};

export const usePostFormMutation = (electionRoundId: string) => {
  const navigate = useNavigate();

  const postFormMutation = useMutation({
    mutationFn: (obj: any) => {
      return noAuthApi.post<any>(
        `/election-rounds/${electionRoundId}/citizen-reports`,
        obj
      );
    },

    onSuccess: () => {
      navigate({ to: "/thank-you" });
    },

    onError: (err) => {
      alert("An error occurred");
      console.error(err);
    },
  });
  return { postFormMutation };
};
