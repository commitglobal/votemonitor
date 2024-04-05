import { useMutation, useQuery } from "@tanstack/react-query";
import {
  ElectionRoundsAPIResponse,
  PollingStationInformationAPIPayload,
  getElectionRounds,
  getPollingStationInformation,
  getPollingStationInformationForm,
  getPollingStationNomenclator,
  getPollingStationsVisits,
  upsertPollingStationGeneralInformation,
} from "./definitions.api";
import * as DB from "../database/DAO/PollingStationsNomenclatorDAO";
import { performanceLog } from "../helpers/misc";

import { PollingStationNomenclatorNodeVM } from "../common/models/polling-station.model";

const electionRoundsKeys = {
  all: ["election-rounds"] as const,
  one: (id: string) => [...electionRoundsKeys.all, id] as const,
};

const pollingStationsKeys = {
  all: ["polling-stations"] as const,
  visits: (electionRoundId: string) =>
    [...pollingStationsKeys.all, "visits", electionRoundId] as const,
  nomenclatorList: (parentId: number | null = -1) =>
    [...pollingStationsKeys.all, "node", parentId] as const,
  one: (id: number) => [...pollingStationsKeys.all, id] as const,
};

export const useElectionRoundsQuery = () => {
  return useQuery<ElectionRoundsAPIResponse>({
    queryKey: electionRoundsKeys.all,
    queryFn: async () => {
      const apiData = await getElectionRounds();
      return apiData;
    },
  });
};

export const usePollingStationsNomenclatorQuery = (electionRoundId: string) => {
  return useQuery({
    queryKey: ["polling-stations-nomenclator", electionRoundId],
    queryFn: async () => {
      // TODO: Need to save and check if the CacheKey is the same (bust cache)

      const count = await performanceLog(
        () => DB.getPollingStationNomenclatorNodesCount(electionRoundId),
        "DB.getPollingStationNomenclatorNodesCount",
      );

      if (count > 0) {
        return "RETRIEVED FROM DB";
      } else {
        const data = await getPollingStationNomenclator(electionRoundId);
        await DB.addPollingStationsNomenclatureBulk(electionRoundId, data.nodes);
        return "ADDED TO DB";
      }
    },
    enabled: !!electionRoundId,
  });
};

export const usePollingStationsVisits = (electionRoundId: string) => {
  return useQuery({
    queryKey: pollingStationsKeys.visits(electionRoundId),
    queryFn: async () => {
      return getPollingStationsVisits(electionRoundId);
    },
    enabled: !!electionRoundId,
  });
};

export const usePollingStationByParentID = (parentId: number | null) => {
  return useQuery<PollingStationNomenclatorNodeVM[]>({
    queryKey: pollingStationsKeys.nomenclatorList(parentId),
    queryFn: async () => {
      const data = await DB.getPollingStationsByParentId(parentId);
      const mapped: PollingStationNomenclatorNodeVM[] = data?.map((item) => ({
        id: item._id,
        name: item.name,
        number: item.pollingStationNumber,
        parentId: item.parentId,
        pollingStationId: item.pollingStationId,
      }));
      return mapped;
    },
    enabled: !!parentId,
  });
};

export const usePollingStationById = (pollingStationId: number) => {
  return useQuery({
    queryKey: pollingStationsKeys.one(pollingStationId),
    queryFn: async () => {
      const data = await DB.getPollingStationById(pollingStationId);

      if (!data) return null;

      const mapped: PollingStationNomenclatorNodeVM = {
        id: data._id,
        name: data.name,
        number: data.pollingStationNumber,
        parentId: data.parentId,
        pollingStationId: data.pollingStationId,
      };
      return mapped;
    },
    enabled: !!pollingStationId,
  });
};

export const usePollingStationInformationForm = (electionRoundId: string) => {
  return useQuery({
    queryKey: ["polling-station-information-form", electionRoundId],
    queryFn: async () => {
      const data = await getPollingStationInformationForm(electionRoundId);
      return data;
    },
    enabled: !!electionRoundId,
  });
};

export const usePollingStationInformation = (
  electionRoundId: string,
  pollingStationIds?: string[],
) => {
  return useQuery({
    queryKey: ["polling-station-information", electionRoundId, pollingStationIds],
    queryFn: async () => {
      const data = await getPollingStationInformation(electionRoundId, pollingStationIds);
      console.log("usePollingStationInformation", data);
      return data;
    },
    enabled: !!electionRoundId,
  });
};

// ================== Mutations =====================

export const upsertPollingStationGeneralInformationMutation = () => {
  return useMutation({
    mutationKey: ["upsertPollingStationGeneralInformation"],
    mutationFn: async (payload: PollingStationInformationAPIPayload) => {
      return upsertPollingStationGeneralInformation(payload);
    },
  });
};
