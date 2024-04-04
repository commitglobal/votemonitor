import API from "./api";
import { ApiFormAnswer } from "./interfaces/answer.type";
import { ApiFormQuestion } from "./interfaces/question.type";

/** ========================================================================
    ====================== GET Election Rounds ===================
    ========================================================================
    @description The election rounds where my user is asigned
    @returns {ElectionRoundsAPIResponse} 
*/
export type ElectionRoundsAPIResponse = {
  electionRounds: {
    id: string;
    countryId: string;
    country: string;
    title: string;
    englishTitle: string;
    startDate: string;
    status: "Archived" | "NotStarted" | "Started";
    createdOn: string;
    lastModifiedOn: string | null;
  }[];
};

export const getElectionRounds = (): Promise<ElectionRoundsAPIResponse> => {
  return API.get("election-rounds:my").then((res) => res.data);
};

/** ========================================================================
    ====================== GET pollingStationNomenclator ===================
    ========================================================================
    @description All the polling stations for a given election round 
    @param {string} electionRoundId 
    @returns {PollingStationNomenclatorAPIResponse} 
*/
export type PollingStationNomenclatorNodeAPIResponse = {
  id: number;
  name: string;
  parentId?: number; // available for the leafs
  number?: number; // available for the leafs
  pollingStationId?: string; // available for the leafs
};
export type PollingStationNomenclatorAPIResponse = {
  electionRoundId: string;
  version: string; // cache bust key
  nodes: PollingStationNomenclatorNodeAPIResponse[];
};

export const getPollingStationNomenclator = (
  electionRoundId: string
): Promise<PollingStationNomenclatorAPIResponse> => {
  return API.get(
    `election-rounds/${electionRoundId}/polling-stations:fetchAll`
  ).then((res) => res.data);
};
/** ========================================================================
    ====================== GET pollingStationNomenclatorVersion ============
    ========================================================================
    @description Version of the nomenclator, to bust the cache if necessary 
    @param {string} electionRoundId 
    @returns {PollingStationNomenclatorVersionAPIResponse} 
*/
export type PollingStationNomenclatorVersionAPIResponse = {
  electionRoundId: string;
  cacheKey: string;
};

export const getPollingStationNomenclatorVersion = (
  electionRoundId: string
): Promise<PollingStationNomenclatorVersionAPIResponse> => {
  return API.get(
    `election-rounds/${electionRoundId}/polling-stations:version`
  ).then((res) => res.data);
};

/** ========================================================================
    ====================== GET pollingStationVisits ========================
    ========================================================================
    @description Polling Station where I completed some data is registered as visited
    @param {string} electionRoundId 
    @returns {PollingStationVisitsAPIResponse} 
*/
export type PollingStationVisitsAPIResponse = {
  visits: {
    pollingStationId: string;
    visitedAt: string; // ISO date
  };
};

export const getPollingStationsVisits = (
  electionRoundId: string
): Promise<PollingStationVisitsAPIResponse> => {
  return API.get(
    `election-rounds/${electionRoundId}/polling-station-visits:my`
  ).then((res) => res.data);
};

/**
    ========================================================================
    ================= POST pollingStationInformation ====================
    ======== The general form to be completed for each polling station =====
    // TODO: Needs to send all data, Ion will create PATCH
    ========================================================================
    @description Updates: Arrival/Departure Time and Polling Station Information Form
    @param {PollingStationInformationAPIPayload} payload 
    @returns {PollingStationInformationAPIResponse} updated data 

*/
export type PollingStationInformationAPIPayload = {
  electionRoundId: string;
  pollingStationId: string;
  arrivalTime: string; // ISO String  "2024-04-01T12:58:06.670Z";
  departureTime: string;
  answers: ApiFormAnswer[];
};

export type PollingStationInformationAPIResponse = {
  id: string;
  pollingStationId: string;
  arrivalTime: string;
  departureTime: string;
  answers: ApiFormAnswer[];
};

export const upsertPollingStationGeneralInformation = ({
  electionRoundId,
  pollingStationId,
  ...rest
}: PollingStationInformationAPIPayload): Promise<PollingStationInformationAPIResponse> => {
  return API.post(
    `election-rounds/${electionRoundId}/polling-stations/${pollingStationId}/information`,
    {
      ...rest,
    }
  ).then((res) => res.data);
};

/** ========================================================================
    ================= GET pollingStationInformationForm ====================
    ======== The general form to be completed for each polling station =====
    ========================================================================
    @description Get the general data for the Polling Station (arrival/departure time and information form)
    @param {string} electionRoundId 
    @returns {PollingStationInformationFormAPIResponse} 
*/
export type PollingStationInformationFormAPIResponse = {
  id: string;
  createdOn: string;
  lastModifiedOn: string;
  languages: string[]; //["RO", "EN"]
  questions: ApiFormQuestion[];
};

export const getPollingStationInformationForm = (
  electionRoundId: string
): Promise<PollingStationInformationFormAPIResponse> => {
  return API.get(
    `election-rounds/${electionRoundId}/polling-station-information-form`
  ).then((res) => res.data);
};

export const getPollingStationInformation = (
  electionRoundId: string,
  pollingStationIds?: string[]
): Promise<PollingStationInformationAPIResponse> => {
  return API.get(`election-rounds/${electionRoundId}/information:my`, {
    params: {
      pollingStationIds,
    },
    paramsSerializer: {
      indexes: null,
    },
  }).then((res) => res.data?.informations);
};
