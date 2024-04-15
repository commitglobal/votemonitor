import { useMutation, useQuery } from "@tanstack/react-query";
import {
  ElectionRoundsAPIResponse,
  PollingStationInformationAPIPayload,
  getElectionRounds,
  getPollingStationInformation,
  getPollingStationInformationForm,
  getPollingStationNomenclator,
  getPollingStationNomenclatorVersion,
  getPollingStationsVisits,
  upsertPollingStationGeneralInformation,
} from "./definitions.api";
import * as DB from "../database/DAO/PollingStationsNomenclatorDAO";
import * as API from "./definitions.api";

import { PollingStationNomenclatorNodeVM } from "../common/models/polling-station.model";
import AsyncStorage from "@react-native-async-storage/async-storage";
import { performanceLog } from "../helpers/misc";

const electionRoundsKeys = {
  all: ["election-rounds"] as const,
  one: (id: string) => [...electionRoundsKeys.all, id] as const,
};

export const pollingStationsKeys = {
  all: ["polling-stations"] as const,
  visits: (electionRoundId: string) =>
    [...pollingStationsKeys.all, "visits", electionRoundId] as const,
  nomenclatorList: (parentId: number | null = -1) =>
    [...pollingStationsKeys.all, "node", parentId] as const,
  one: (id: number) => [...pollingStationsKeys.all, id] as const,
  nomenclator: (electionRoundId: string) => [
    ...pollingStationsKeys.all,
    "nomenclator",
    electionRoundId,
  ],
  nomenclatorCacheKey: (electionRoundId: string) => [
    ...pollingStationsKeys.nomenclator(electionRoundId),
    "cacheKey",
  ],
  addAttachmentMutation: () => [...pollingStationsKeys.all, "addAttachment"],
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
    queryKey: pollingStationsKeys.nomenclator(electionRoundId),
    queryFn: async () => {
      try {
        const { cacheKey: serverVersionKey } =
          await getPollingStationNomenclatorVersion(electionRoundId);
        const localVersionKey = await AsyncStorage.getItem(
          pollingStationsKeys.nomenclatorCacheKey(electionRoundId).join(),
        );
        const exists = await DB.getOne(electionRoundId);

        if (!localVersionKey) console.log("🆕🆕🆕🆕 Nomenclator: No Local Version Key");
        if (!exists) console.log("🆕🆕🆕🆕 Nomenclator: No data for the election round");
        if (localVersionKey !== serverVersionKey)
          console.log("❌❌❌❌ Nomenclator: Busting cache, new data coming");

        if (!localVersionKey || !exists || serverVersionKey !== localVersionKey) {
          await DB.deleteAll(electionRoundId);
          const data = await getPollingStationNomenclator(electionRoundId);
          await DB.addPollingStationsNomenclatureBulk(electionRoundId, data.nodes);
          await AsyncStorage.setItem(
            pollingStationsKeys.nomenclatorCacheKey(electionRoundId).join(),
            serverVersionKey,
          );
          return "ADDED TO DB";
        } else {
          return "RETRIEVED FROM DB";
        }
      } catch (err) {
        // TODO: Add Sentry
        console.warn("usePollingStationsNomenclatorQuery", err);
        throw err;
      }
    },
    enabled: !!electionRoundId,
    staleTime: 5 * 60 * 1000,
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
    initialData: [],
    staleTime: 0,
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

export const addAttachmentMutation = () => {
  return useMutation({
    mutationKey: pollingStationsKeys.addAttachmentMutation(),
    mutationFn: async (
      payload: API.AddAttachmentAPIPayload,
    ): Promise<API.AddAttachmentAPIResponse> => {
      return performanceLog(() => API.addAttachment(payload));
    },
  });
};
