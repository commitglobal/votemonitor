import { skipToken, useQuery } from "@tanstack/react-query";
import {
  getElectionRounds,
  getPollingStationInformation,
  getPollingStationInformationForm,
  getPollingStationNomenclator,
  getPollingStationNomenclatorVersion,
  getPollingStationsVisits,
  getNotesForPollingStation,
} from "./definitions.api";
import * as DB from "../database/DAO/PollingStationsNomenclatorDAO";
import * as API from "./definitions.api";

import { PollingStationNomenclatorNodeVM } from "../common/models/polling-station.model";
import AsyncStorage from "@react-native-async-storage/async-storage";

const electionRoundsKeys = {
  all: ["election-rounds"] as const,
  one: (id: string) => [...electionRoundsKeys.all, id] as const,
  forms: () => [...electionRoundsKeys.all, "forms"] as const,
};

export const pollingStationsKeys = {
  all: ["polling-stations"] as const,
  visits: (electionRoundId: string | undefined) =>
    [...pollingStationsKeys.all, "visits", electionRoundId] as const,
  formSubmissions: (electionRoundId: string | undefined, pollingStationId: string | undefined) => [
    ...pollingStationsKeys.all,
    "electionRoundId",
    electionRoundId,
    "pollingStationId",
    pollingStationId,
    "form-submissions",
  ],
  upsertFormSubmission: () => [...pollingStationsKeys.all, "upsertFormSubmission"] as const,
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
  informationForm: (electionRoundId?: string) =>
    [
      ...pollingStationsKeys.all,
      "electionRoundId",
      electionRoundId,
      "polling-station-information-form",
    ] as const,
  notes: (
    electionRoundId: string | undefined,
    pollingStationId: string | undefined,
    formId: string | undefined,
  ) =>
    [
      ...pollingStationsKeys.all,
      "electionRoundId",
      electionRoundId,
      "pollingStationId",
      pollingStationId,
      "formId",
      formId,
      "notes",
    ] as const,
  addNote: () => [...pollingStationsKeys.all, "addNote"] as const,
  updateNote: () => [...pollingStationsKeys.all, "updateNote"] as const,
  deleteNote: () => [...pollingStationsKeys.all, "deleteNote"] as const,
  attachments: (
    electionRoundId: string | undefined,
    pollingStationId: string | undefined,
    formId: string | undefined,
  ) =>
    [
      ...pollingStationsKeys.all,
      "electionRoundId",
      electionRoundId,
      "pollingStationId",
      pollingStationId,
      "formId",
      formId,
      "attachments",
    ] as const,
  deleteAttachment: () => [...pollingStationsKeys.all, "deleteAttachment"] as const,
  mutatePollingStationGeneralData: () =>
    [...pollingStationsKeys.all, "mutate-general-data"] as const,
  changePassword: () => [...pollingStationsKeys.all, "changePassword"] as const,
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
    queryFn: electionRoundId
      ? async () => {
          console.log("usePollingStationsNomenclatorQuery");

          const localVersionKey = await AsyncStorage.getItem(
            pollingStationsKeys.nomenclatorCacheKey(electionRoundId).join(),
          );

          let serverVersionKey;
          try {
            serverVersionKey = (await getPollingStationNomenclatorVersion(electionRoundId))
              ?.cacheKey;
          } catch (err) {
            // Possible offline or backend has issues, let it pass
            // Sentry log
            serverVersionKey = localVersionKey ?? "";
            console.log("usePollingStationsNomenclatorQuery", err);
          }

          try {
            const exists = await DB.getOne(electionRoundId);

            if (!localVersionKey) console.log("🆕🆕🆕🆕 Nomenclator: No Local Version Key");
            if (!exists) console.log("🆕🆕🆕🆕 Nomenclator: No data for the election round");
            if (localVersionKey !== serverVersionKey)
              console.log("❌❌❌❌ Nomenclator: Busting cache, new data coming");

            if (!localVersionKey || !exists || serverVersionKey !== localVersionKey) {
              const data = await getPollingStationNomenclator(electionRoundId);
              await DB.deleteAll(electionRoundId);
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
        }
      : skipToken,
    retry: 0, // to avoid waiting 25s to fail the promise
    staleTime: 0,
    networkMode: "always",
  });
};

export const usePollingStationsVisits = (electionRoundId: string | undefined) => {
  return useQuery({
    queryKey: pollingStationsKeys.visits(electionRoundId),
    queryFn: electionRoundId ? () => getPollingStationsVisits(electionRoundId) : skipToken,
  });
};

export const usePollingStationByParentID = (
  parentId: number | null,
  electionRoundId: string | undefined,
) => {
  return useQuery<PollingStationNomenclatorNodeVM[]>({
    queryKey: pollingStationsKeys.nomenclatorList(parentId),
    queryFn:
      parentId && electionRoundId
        ? async () => {
            const data = await DB.getPollingStationsByParentId(parentId, electionRoundId!);
            const mapped: PollingStationNomenclatorNodeVM[] = data?.map((item) => ({
              id: item._id,
              name: item.name,
              number: item.pollingStationNumber,
              parentId: item.parentId,
              pollingStationId: item.pollingStationId,
            }));
            return mapped;
          }
        : skipToken,
    initialData: [],
    staleTime: 0,
    networkMode: "always",
  });
};

export const pollingStationByIdQueryFn = async (pollingStationId: string) => {
  console.log("usePollingStationById", pollingStationId);
  const data = await DB.getPollingStationById(pollingStationId);

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
    queryFn: pollingStationId ? () => pollingStationByIdQueryFn(pollingStationId) : skipToken,
    staleTime: 5 * 60 * 1000,
    networkMode: "always", // https://tanstack.com/query/v4/docs/framework/react/guides/network-mode#network-mode-always
  });
};

export const usePollingStationInformationForm = (electionRoundId: string | undefined) => {
  return useQuery({
    queryKey: pollingStationsKeys.informationForm(electionRoundId),
    queryFn: electionRoundId ? () => getPollingStationInformationForm(electionRoundId) : skipToken,
  });
};

export const useElectionRoundAllForms = (electionRoundId: string | undefined) => {
  return useQuery({
    queryKey: electionRoundsKeys.forms(),
    queryFn: electionRoundId ? () => API.getElectionRoundAllForms(electionRoundId) : skipToken,
  });
};

export const useFormSubmissions = (
  electionRoundId: string | undefined,
  pollingStationId: string | undefined,
) => {
  return useQuery({
    queryKey: pollingStationsKeys.formSubmissions(electionRoundId, pollingStationId),
    queryFn:
      electionRoundId && pollingStationId
        ? () => API.getFormSubmissions(electionRoundId, pollingStationId)
        : skipToken,
  });
};

export const pollingStationInformationQueryFn = (
  electionRoundId: string | undefined,
  pollingStationId: string,
) => {
  return getPollingStationInformation(electionRoundId!, pollingStationId);
};
export const usePollingStationInformation = (
  electionRoundId: string | undefined,
  pollingStationId: string | undefined,
) => {
  return useQuery({
    queryKey: pollingStationsKeys.pollingStationInformation(electionRoundId!, pollingStationId!),
    queryFn:
      electionRoundId && pollingStationId
        ? () => pollingStationInformationQueryFn(electionRoundId, pollingStationId)
        : skipToken,
  });
};

export const useNotesForPollingStation = (
  electionRoundId: string | undefined,
  pollingStationId: string | undefined,
  formId: string | undefined,
) => {
  return useQuery({
    queryKey: pollingStationsKeys.notes(electionRoundId, pollingStationId, formId),
    queryFn:
      electionRoundId && pollingStationId && formId
        ? () => getNotesForPollingStation(electionRoundId, pollingStationId, formId)
        : skipToken,
  });
};
