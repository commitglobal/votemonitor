import { CameraResult } from "../hooks/useCamera";
import API from "./api";
import { ApiFormAnswer } from "./interfaces/answer.type";
import { ApiFormQuestion } from "./interfaces/question.type";

/** ========================================================================
    ====================== GET Election Rounds ===================
    ========================================================================
    @description The election rounds where my user is asigned
    @returns {ElectionRoundsAPIResponse} 
*/
export type ElectionRoundVM = {
  id: string;
  countryId: string;
  country: string;
  title: string;
  englishTitle: string;
  startDate: string;
  status: "Archived" | "NotStarted" | "Started";
  createdOn: string;
  lastModifiedOn: string | null;
};

export type ElectionRoundsAPIResponse = {
  electionRounds: ElectionRoundVM[];
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
  number?: string; // available for the leafs
  pollingStationId?: string; // available for the leafs
};
export type PollingStationNomenclatorAPIResponse = {
  electionRoundId: string;
  version: string; // cache bust key
  nodes: PollingStationNomenclatorNodeAPIResponse[];
};

export const getPollingStationNomenclator = (
  electionRoundId: string,
): Promise<PollingStationNomenclatorAPIResponse> => {
  return API.get(`election-rounds/${electionRoundId}/polling-stations:fetchAll`).then(
    (res) => res.data,
  );
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
  electionRoundId: string,
): Promise<PollingStationNomenclatorVersionAPIResponse> => {
  return API.get(`election-rounds/${electionRoundId}/polling-stations:version`).then(
    (res) => res.data,
  );
};

/** ========================================================================
    ====================== GET pollingStationVisits ========================
    ========================================================================
    @description Polling Station where I completed some data is registered as visited
    @param {string} electionRoundId 
    @returns {PollingStationVisitsAPIResponse} 
*/
export type PollingStationVisitVM = {
  pollingStationId: string;
  visitedAt: string; // ISO date
};

export type PollingStationVisitsAPIResponse = {
  visits: PollingStationVisitVM[];
};

export const getPollingStationsVisits = (
  electionRoundId: string,
): Promise<PollingStationVisitsAPIResponse> => {
  return API.get(`election-rounds/${electionRoundId}/polling-station-visits:my`).then(
    (res) => res.data,
  );
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
    },
  )
    .then((res) => res.data)
    .catch(console.log);
};

/** ========================================================================
    ================= GET pollingStationInformationForm ====================
    ======== The form data to be completed for each polling station =====
    ========================================================================
    @description Get the general data for the Polling Station (arrival/departure time and information form)
    @param {string} electionRoundId 
    @returns {PollingStationInformationFormAPIResponse} 
*/
export type PollingStationInformationFormAPIResponse = {
  id: string;
  createdOn: string;
  lastModifiedOn: string;
  languages: string[]; // ["RO", "EN"]
  questions: ApiFormQuestion[];
};

export const getPollingStationInformationForm = (
  electionRoundId: string,
): Promise<PollingStationInformationFormAPIResponse> => {
  return API.get(`election-rounds/${electionRoundId}/polling-station-information-form`).then(
    (res) => res.data,
  );
};

/** ========================================================================
    ================= GET getPollingStationInformation ====================
    ========================================================================
    @description Get the available completed form data for the Polling Station (arrival/departure time and information form)
    @param {string} electionRoundId 
    @returns {PollingStationInformationAPIResponse} 
*/
export const getPollingStationInformation = (
  electionRoundId: string,
  pollingStationId: string,
): Promise<PollingStationInformationAPIResponse> => {
  return API.get(`election-rounds/${electionRoundId}/information:my`, {
    params: {
      pollingStationIds: [pollingStationId],
    },
    paramsSerializer: {
      indexes: null,
    },
  }).then((res) => res.data?.informations[0]);
};

/** ========================================================================
    ================= POST addAttachment ====================
    ========================================================================
    @description Sends a photo/video to the backend to be saved
    @param {AddAttachmentAPIPayload} payload 
    @returns {AddAttachmentAPIResponse} 
*/
export type AddAttachmentAPIPayload = {
  electionRoundId: string;
  pollingStationId: string;
  cameraResult: CameraResult;
};

export type AddAttachmentAPIResponse = {
  id: string;
  fileName: string;
  mimeType: string;
  presignedUrl: string;
  urlValidityInSeconds: number;
};

export const addAttachment = ({
  electionRoundId,
  pollingStationId,
  cameraResult,
}: AddAttachmentAPIPayload): Promise<AddAttachmentAPIResponse> => {
  const formData = new FormData();
  formData.append("attachment", {
    uri: cameraResult.uri,
    name: cameraResult.name,
    type: cameraResult.type,
  } as unknown as Blob);

  return API.post(
    `election-rounds/${electionRoundId}/polling-stations/${pollingStationId}/attachments`,
    formData,
    {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    },
  ).then((res) => res.data);
};
