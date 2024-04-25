import { ElectionRoundVM } from "../common/models/election-round.model";
import { Note } from "../common/models/note";
import { PollingStationVisitVM } from "../common/models/polling-station.model";
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

export type ElectionRoundsAPIResponse = {
  electionRounds: ElectionRoundVM[];
};

export const getElectionRounds = (): Promise<ElectionRoundVM[]> => {
  return API.get<ElectionRoundsAPIResponse>("election-rounds:my").then(
    (res) => res.data.electionRounds,
  );
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
export type PollingStationVisitsAPIResponse = {
  visits: {
    pollingStationId: string;
    visitedAt: string; // ISO date
    address: string;
    number: string;
  }[];
};

export const getPollingStationsVisits = (
  electionRoundId: string,
): Promise<PollingStationVisitVM[]> => {
  return API.get<PollingStationVisitsAPIResponse>(
    `election-rounds/${electionRoundId}/polling-station-visits:my`,
  ).then((res) => res.data.visits);
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
  arrivalTime?: string | null; // ISO String  "2024-04-01T12:58:06.670Z";
  departureTime?: string | null;
  answers?: ApiFormAnswer[];
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
  }).then((res) => res.data?.informations[0] || null);
};

/** ========================================================================
    ================= GET ElectionRoundAllForms ====================
    ========================================================================
    @description Get all the possible forms for a given election round
    @param {string} electionRoundId 
    @returns {PollingStationInformationAPIResponse} 
*/

export type ElectionRoundsAllFormsAPIResponse = {
  electionRoundId: string;
  version: string;
  forms: {
    id: string;
    formType: string; // "ClosingAndCounting",
    code: string; // "A1",
    name: Record<string, string>; // { "EN": "test form", "RO": "formular de test" },
    status: string; // "Published",
    defaultLanguage: string; // "RO",
    languages: string[]; // [ "RO", "EN" ],
    createdOn: string;
    lastModifiedOn: string; // "2024-04-12T11:45:38.589445Z"
    questions: ApiFormQuestion[];
  }[];
};

export const getElectionRoundAllForms = (
  electionRoundId: string,
): Promise<ElectionRoundsAllFormsAPIResponse> => {
  return API.get(`election-rounds/${electionRoundId}/forms:fetchAll`, {}).then((res) => res.data);
};

/** ========================================================================
    ================= GET NotesForPollingStation ====================
    ========================================================================
    @description Get all the possible notes for a given polling station
    @param {string} electionRoundId 
    @param {string} pollingStationId
    @param {string} formId 
    @returns {Note[]} 
*/

export const getNotesForPollingStation = (
  electionRoundId: string,
  pollingStationId: string,
  formId: string,
): Promise<Note[]> => {
  return API.get(`election-rounds/${electionRoundId}/notes`, {
    params: {
      electionRoundId: [electionRoundId],
      pollingStationId: [pollingStationId],
      formId: [formId],
    },
    paramsSerializer: {
      indexes: null,
    },
  }).then((res) => res.data);
};

/** ========================================================================
    ================= GET FORM SUBMISSIONS ====================
    ========================================================================
    @description Get form submissions for a given >polling station< in an >election round<
    @param {string} electionRoundId 
    @param {string} pollingStationId 
    @returns {unknown} 
*/

export type FormSubmission = {
  id: string;
  formId: string;
  pollingStationId: string;
  answers: ApiFormAnswer[];
};

export type FormSubmissionsApiResponse = {
  submissions: FormSubmission[];
};

export const getFormSubmissions = (
  electionRoundId: string,
  pollingStationId: string,
): Promise<FormSubmissionsApiResponse> => {
  return API.get(`election-rounds/${electionRoundId}/form-submissions:my`, {
    params: {
      pollingStationIds: [pollingStationId],
    },
    paramsSerializer: {
      indexes: null,
    },
  }).then((res) => res.data);
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

/**
    ========================================================================
    ================= POST form submission ====================
    ======== Upsert answer for a specific form from a polling station =====
    ========================================================================
    @description Updates: Arrival/Departure Time and Polling Station Information Form
    @param {FormSubmissionAPIPayload} payload 
    @returns {FormSubmission} updated data 

*/
export type FormSubmissionAPIPayload = Omit<FormSubmission, "id"> & { electionRoundId: string };

export const upsertFormSubmission = ({
  electionRoundId,
  ...payload
}: FormSubmissionAPIPayload): Promise<FormSubmission> => {
  return API.post(`election-rounds/${electionRoundId}/form-submissions`, payload)
    .then((res) => res.data)
    .catch(console.log);
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

/** ========================================================================
    ================= POST addNote ====================
    ========================================================================
    @description Get all the possible notes for a given polling station
    @param {string} electionRoundId 
    @returns {Note} 
*/

export type NotePayload = {
  electionRoundId: string | undefined;
  pollingStationId: string;
  text: string;
  formId: string;
  questionId: string;
};

export const addNote = ({ electionRoundId, ...notePayload }: NotePayload): Promise<Note> => {
  return API.post(`election-rounds/${electionRoundId}/notes`, notePayload).then((res) => res.data);
};
