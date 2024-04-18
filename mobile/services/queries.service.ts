import { useMutation, useQuery } from "@tanstack/react-query";
import {
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
  forms: () => [...electionRoundsKeys.all, "forms"] as const,
};

export const pollingStationsKeys = {
  all: ["polling-stations"] as const,
  visits: (electionRoundId: string) =>
    [...pollingStationsKeys.all, "visits", electionRoundId] as const,
  formSubmissions: (electionRoundId: string | undefined, pollingStationId: string | undefined) => [
    ...pollingStationsKeys.all,
    "electionRoundId",
    electionRoundId,
    "pollingStationId",
    pollingStationId,
    "form-submissions",
  ],
  nomenclatorList: (parentId: number | null = -1) =>
    [...pollingStationsKeys.all, "node", parentId] as const,
  one: (id: string) => [...pollingStationsKeys.all, "DB.getOneById", id] as const,
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
  pollingStationInformation: (
    electionRoundId: string | undefined,
    pollingStationId: string | undefined,
  ) =>
    [
      ...pollingStationsKeys.all,
      "electionRound",
      electionRoundId,
      "pollingStation",
      pollingStationId,
      "information",
    ] as const,
  informationForm: (electionRoundId?: string) => [
    ...pollingStationsKeys.all,
    "electionRoundId",
    electionRoundId,
    "polling-station-information-form",
  ],
  mutatePollingStationGeneralData: () => [...pollingStationsKeys.all, "mutate-general-data"],
};

export const useElectionRoundsQuery = () => {
  return useQuery({
    queryKey: electionRoundsKeys.all,
    queryFn: getElectionRounds,
  });
};

export const usePollingStationsNomenclatorQuery = (electionRoundId: string | undefined) => {
  return useQuery({
    queryKey: pollingStationsKeys.nomenclator(electionRoundId!),
    queryFn: async () => {
      console.log("usePollingStationsNomenclatorQuery");

      const localVersionKey = await AsyncStorage.getItem(
        pollingStationsKeys.nomenclatorCacheKey(electionRoundId!).join(),
      );

      let serverVersionKey;
      try {
        serverVersionKey = (await getPollingStationNomenclatorVersion(electionRoundId!))?.cacheKey;
      } catch (err) {
        // Possible offline or backend has issues, let it pass
        // Sentry log
        serverVersionKey = localVersionKey ?? "";
        console.log("usePollingStationsNomenclatorQuery", err);
      }

      try {
        const exists = await DB.getOne(electionRoundId!);

        if (!localVersionKey) console.log("🆕🆕🆕🆕 Nomenclator: No Local Version Key");
        if (!exists) console.log("🆕🆕🆕🆕 Nomenclator: No data for the election round");
        if (localVersionKey !== serverVersionKey)
          console.log("❌❌❌❌ Nomenclator: Busting cache, new data coming");

        if (!localVersionKey || !exists || serverVersionKey !== localVersionKey) {
          const data = await getPollingStationNomenclator(electionRoundId!);
          await DB.deleteAll(electionRoundId!);
          await DB.addPollingStationsNomenclatureBulk(electionRoundId!, data.nodes);
          await AsyncStorage.setItem(
            pollingStationsKeys.nomenclatorCacheKey(electionRoundId!).join(),
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
    // staleTime: 5 * 60 * 1000,
    staleTime: 0,
    networkMode: "always",
  });
};

export const usePollingStationsVisits = (electionRoundId: string | undefined) => {
  return useQuery({
    queryKey: pollingStationsKeys.visits(electionRoundId!),
    queryFn: () => {
      return getPollingStationsVisits(electionRoundId!);
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
    networkMode: "always",
  });
};

export const pollingStationByIdQueryFn = async (pollingStationId: string | undefined) => {
  console.log("usePollingStationById", pollingStationId);
  const data = await DB.getPollingStationById(pollingStationId!);

  if (!data)
    throw Error(`Could not find data for ${pollingStationId}, maybe nomenclator not there yet.`);

  const mapped: PollingStationNomenclatorNodeVM = {
    id: data._id,
    name: data.name,
    number: data.pollingStationNumber,
    parentId: data.parentId,
    pollingStationId: data.pollingStationId,
  };
  return mapped;
};
export const usePollingStationById = (pollingStationId: string | undefined) => {
  return useQuery({
    queryKey: pollingStationsKeys.one(pollingStationId!),
    queryFn: () => pollingStationByIdQueryFn(pollingStationId),
    enabled: !!pollingStationId,
    staleTime: 60 * 1000,
    networkMode: "always", // https://tanstack.com/query/v4/docs/framework/react/guides/network-mode#network-mode-always
  });
};

export const usePollingStationInformationForm = (electionRoundId: string | undefined) => {
  return useQuery({
    queryKey: pollingStationsKeys.informationForm(electionRoundId),
    queryFn: () => getPollingStationInformationForm(electionRoundId!),
    enabled: !!electionRoundId,
  });
};

export const useElectionRoundAllForms = (electionRoundId: string | undefined) => {
  return useQuery({
    queryKey: electionRoundsKeys.forms(),
    queryFn: () => API.getElectionRoundAllForms(electionRoundId!),
    enabled: !!electionRoundId,
  });
};

export const useFormSubmissions = (
  electionRoundId: string | undefined,
  pollingStationId: string | undefined,
) => {
  return useQuery({
    queryKey: pollingStationsKeys.formSubmissions(electionRoundId, pollingStationId),
    queryFn: () => API.getFormSubmissions(electionRoundId!, pollingStationId!),
    enabled: !!electionRoundId && !!pollingStationId,
  });
};

export const pollingStationInformationQueryFn = (
  electionRoundId: string | undefined,
  pollingStationId: string | undefined,
) => {
  return getPollingStationInformation(electionRoundId!, pollingStationId!);
};
export const usePollingStationInformation = (
  electionRoundId: string | undefined,
  pollingStationId: string | undefined,
) => {
  return useQuery({
    queryKey: pollingStationsKeys.pollingStationInformation(electionRoundId!, pollingStationId!),
    queryFn: () => pollingStationInformationQueryFn(electionRoundId, pollingStationId),
    enabled: !!electionRoundId && !!pollingStationId,
  });
};

// ================== Mutations =====================

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
