import { ElectionRoundVM } from "../common/models/election-round.model";
import { Note } from "../common/models/note";
import { PollingStationVisitVM } from "../common/models/polling-station.model";
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
  isCompleted?: boolean;
};

export type PollingStationInformationAPIResponse = {
  id: string;
  pollingStationId: string;
  arrivalTime: string;
  departureTime: string;
  answers: ApiFormAnswer[];
  isCompleted: boolean;
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
  ).then((res) => res.data);
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
  defaultLanguage: string;
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

export type FormAPIModel = {
  id: string;
  formType: string; // "ClosingAndCounting",
  code: string; // "A1",
  name: Record<string, string>; // { "EN": "test form", "RO": "formular de test" },
  description: Record<string, string>; // { "EN": "test form", "RO": "formular de test" },
  status: string; // "Published",
  defaultLanguage: string; // "RO",
  languages: string[]; // [ "RO", "EN" ],
  numberOfQuestions: number;
  createdOn: string;
  lastModifiedOn: string; // "2024-04-12T11:45:38.589445Z"
  questions: ApiFormQuestion[];
};

export type ElectionRoundsAllFormsAPIResponse = {
  electionRoundId: string;
  version: string;
  forms: FormAPIModel[];
};

export const getElectionRoundAllForms = (
  electionRoundId: string,
): Promise<ElectionRoundsAllFormsAPIResponse> => {
  return API.get(`election-rounds/${electionRoundId}/forms:fetchAll`, {}).then((res) => res.data);
};

/** ========================================================================
    ================= GET ElectionRoundForm ====================
    ========================================================================
    @description Get a form by id for an election round
    @param {string} electionRoundId 
    @param {string} formId 
    @returns {FormAPIModel}
*/

export const getElectionRoundFormById = (
  electionRoundId: string,
  formId: string,
): Promise<FormAPIModel> => {
  return API.get(`election-rounds/${electionRoundId}/forms/${formId}`, {}).then((res) => res.data);
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
  isCompleted: boolean;
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

/**
    ========================================================================
    ================= POST form submission ====================
    ======== Upsert answer for a specific form from a polling station =====
    ========================================================================
    @description Updates: Arrival/Departure Time and Polling Station Information Form
    @param {FormSubmissionAPIPayload} payload 
    @returns {FormSubmission} updated data 

*/
export type FormSubmissionAPIPayload = Omit<FormSubmission, "id" | "isCompleted"> & {
  electionRoundId: string;
};

export const upsertFormSubmission = ({
  electionRoundId,
  ...payload
}: FormSubmissionAPIPayload): Promise<FormSubmission> => {
  return API.post(`election-rounds/${electionRoundId}/form-submissions`, payload).then(
    (res) => res.data,
  );
};

/** ========================================================================
    ================= POST addNote ====================
    ========================================================================
    @description Add new note into the formId 
    @param {string} electionRoundId 
    @returns {Note} 
*/

export type UpsertNotePayload = {
  id: string;
  electionRoundId: string;
  pollingStationId: string;
  text: string;
  formId: string;
  questionId: string;
};

export const upsertNote = ({
  electionRoundId,
  ...notePayload
}: UpsertNotePayload): Promise<Note> => {
  return API.post(`election-rounds/${electionRoundId}/notes`, notePayload).then((res) => res.data);
};

/** ========================================================================
    ================= POST changePassword ====================
    ========================================================================
    @description Change the password for the current user
    @param {ChangePasswordPayload} data includes current, new and confirmed passwords
*/
export type ChangePasswordPayload = {
  password: string;
  newPassword: string;
  confirmNewPassword: string;
};

export const changePassword = (data: ChangePasswordPayload) => {
  return API.post("auth/change-password", data).then((res) => res.data);
};

/**  ================= DELETE deleteNote ====================
    ========================================================================
    @description delete a note 
    @param {string} electionRoundId 
    @param {string} id 
*/

export const deleteNote = ({ electionRoundId, id }: Note) => {
  return API.delete(`election-rounds/${electionRoundId}/notes/${id}`).then((res) => res.data);
};

/** ========================================================================
    ================= POST forgotPassword ====================
    ========================================================================
    @description Change the forgotten password for the current user, sends an email with a reset link
    @param {ForgotPasswwordPayload} data includes the user's email to send the reset link
*/
export type ForgotPasswwordPayload = {
  email: string;
};

export const forgotPassword = async (data: ForgotPasswwordPayload) => {
  return API.post("auth/forgot-password", data).then((res) => res.data);
};

/** ===================================================================================
 * ================= GET does a polling station have form submissions? ================
 * ====================================================================================
 *  @param {string} electionRoundId
 *  @param {string} pollingStationId
 */

export type GetPSHasFormSubmissionsPayload = {
  electionRoundId: string;
  pollingStationId: string;
};

export const getPSHasFormSubmissions = (electionRoundId: string, pollingStationId: string) => {
  return API.get(`/election-rounds/${electionRoundId}/form-submissions:any`, {
    params: {
      pollingStationId,
    },
  }).then((res) => res.data);
};

/** ================= DELETE pollingStation ====================
 * ========================================================================
 * @description delete a polling station
 * @param {DeletePollingStationVisitPayload} data includes electionRoundId and pollingStationId
 */

export type DeletePollingStationVisitPayload = {
  electionRoundId: string;
  pollingStationId: string;
};

export const deletePollingStationVisit = (data: DeletePollingStationVisitPayload) => {
  return API.delete(
    `election-rounds/${data.electionRoundId}/polling-station-visits/${data.pollingStationId}`,
  ).then((res) => res.data);
};

/** ========================================================================
    ================= POST feedback ====================
    ========================================================================
    @param {string} electionRoundId 
*/

type feedbackMetadata = {
  appVersion: string | undefined;
  sentAt: string;
  platform: "ios" | "android" | "windows" | "macos" | "web";
  modelName: string | null;
  electionRoundId: string | undefined;
  systemVersion: string | null;
};

export type AddFeedbackPayload = {
  electionRoundId: string | undefined;
  userFeedback: string;
  metadata: feedbackMetadata;
};

export const addFeedback = ({ electionRoundId, ...feedbackPayload }: AddFeedbackPayload) => {
  return API.post(`election-rounds/${electionRoundId}/feedback/`, feedbackPayload).then(
    (res) => res.data,
  );
};
