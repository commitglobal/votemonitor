/* eslint-disable */
/* tslint:disable */
/*
 * ---------------------------------------------------------------
 * ## THIS FILE WAS GENERATED VIA SWAGGER-TYPESCRIPT-API        ##
 * ##                                                           ##
 * ## AUTHOR: acacode                                           ##
 * ## SOURCE: https://github.com/acacode/swagger-typescript-api ##
 * ---------------------------------------------------------------
 */

export type VoteMonitorApiFeatureAuthNgoAdminsOnlyRequest = object;

export type VoteMonitorApiFeatureAuthMonitoringObserversOnlyRequest = object;

export type VoteMonitorApiFeatureAuthMonitoringNgoAdminsOnlyRequest = object;

export interface VoteMonitorApiFeatureAuthLoginResponse {
  token?: string;
}

/**
 * RFC7807 compatible problem details/ error response class. this can be used by configuring startup like so:
 *
 *     app.UseFastEndpoints(x => x.Errors.ResponseBuilder = ProblemDetails.ResponseBuilder);
 */
export interface FastEndpointsProblemDetails {
  /** @default "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.1" */
  type?: string;
  /** @default "One or more validation errors occurred." */
  title?: string;
  /**
   * @format int32
   * @default 400
   */
  status?: number;
  /** @default "/api/route" */
  instance?: string;
  /** @default "0HMPNHL0JHL76:00000001" */
  traceId?: string;
  /** the details of the error */
  detail?: string | null;
  errors?: FastEndpointsProblemDetailsError[];
}

/** the error details object */
export interface FastEndpointsProblemDetailsError {
  /**
   * the name of the error or property of the dto that caused the error
   * @default "Error or field name"
   */
  name?: string;
  /**
   * the reason for the error
   * @default "Error reason"
   */
  reason?: string;
  /** the code of the error */
  code?: string | null;
  /** the severity of the error */
  severity?: string | null;
}

export interface VoteMonitorApiFeatureAuthLoginRequest {
  /** @minLength 1 */
  username: string;
  /** @minLength 1 */
  password: string;
}

export interface VoteMonitorApiFeaturePollingStationUpdateRequest {
  /**
   * @format int32
   * @min 0
   */
  displayOrder?: number;
  /** @minLength 1 */
  address: string;
  /** @minLength 1 */
  tags: Record<string, string>;
}

export interface VoteMonitorCoreModelsPagedResponseOfPollingStationModel {
  /** @format int32 */
  currentPage?: number;
  /** @format int32 */
  pageSize?: number;
  /** @format int32 */
  totalCount?: number;
  items?: VoteMonitorApiFeaturePollingStationPollingStationModel[];
}

export interface VoteMonitorApiFeaturePollingStationPollingStationModel {
  /** @format guid */
  id?: string;
  address?: string;
  /** @format int32 */
  displayOrder?: number;
  tags?: Record<string, string>;
}

export interface VoteMonitorApiFeaturePollingStationListRequest {
  filter?: Record<string, string>;
}

export enum VoteMonitorCoreModelsSortOrder {
  Asc = 'Asc',
  Desc = 'Desc',
}

export interface VoteMonitorApiFeaturePollingStationImportResponse {
  /** @format int32 */
  rowsImported?: number;
}

export interface VoteMonitorApiFeaturePollingStationImportRequest {
  /**
   * @format binary
   * @minLength 1
   */
  file: File;
}

export type VoteMonitorApiFeaturePollingStationGetRequest = object;

export interface VoteMonitorApiFeaturePollingStationTagModel {
  name?: string;
  value?: string;
}

export interface VoteMonitorApiFeaturePollingStationGetTagValuesRequest {
  /** @minLength 1 */
  selectTag: string;
  filter?: Record<string, string>;
}

export type VoteMonitorApiFeaturePollingStationGetTagsRequest = object;

export type VoteMonitorApiFeaturePollingStationDeleteRequest = object;

export interface VoteMonitorApiFeaturePollingStationCreateRequest {
  /**
   * @format int32
   * @min 0
   */
  displayOrder?: number;
  /** @minLength 1 */
  address: string;
  /** @minLength 1 */
  tags: Record<string, string>;
}


export interface VoteMonitorApiFeatureNgoUpdateRequest {
  /**
   * @minLength 3
   * @maxLength 256
   */
  name: string;
}

export interface VoteMonitorCoreModelsPagedResponseOfNgoModel {
  /** @format int32 */
  currentPage?: number;
  /** @format int32 */
  pageSize?: number;
  /** @format int32 */
  totalCount?: number;
  items?: VoteMonitorApiFeatureNgoNgoModel[];
}

export interface VoteMonitorApiFeatureNgoNgoModel {
  /** @format guid */
  id?: string;
  name?: string;
  status?: VoteMonitorDomainEntitiesNgoAggregateNgoStatus;
  /** @format date-time */
  createdOn?: string;
  /** @format date-time */
  lastModifiedOn?: string | null;
}

export enum VoteMonitorDomainEntitiesNgoAggregateNgoStatus {
  Activated = 'Activated',
  Deactivated = 'Deactivated',
}

export type VoteMonitorApiFeatureNgoListRequest = object;

export type VoteMonitorApiFeatureNgoGetRequest = object;

export type VoteMonitorApiFeatureNgoDeleteRequest = object;

export type VoteMonitorApiFeatureNgoDeactivateRequest = object;

export interface VoteMonitorApiFeatureNgoCreateRequest {
  /**
   * @minLength 1
   * @maxLength 256
   */
  name: string;
}

export type VoteMonitorApiFeatureNgoActivateRequest = object;

export interface VoteMonitorApiFeatureNgoAdminUpdateRequest {
  /** @minLength 3 */
  name: string;
}

export interface VoteMonitorCoreModelsPagedResponseOfNgoAdminModel {
  /** @format int32 */
  currentPage?: number;
  /** @format int32 */
  pageSize?: number;
  /** @format int32 */
  totalCount?: number;
  items?: VoteMonitorApiFeatureNgoAdminNgoAdminModel[];
}

export interface VoteMonitorApiFeatureNgoAdminNgoAdminModel {
  /** @format guid */
  id?: string;
  login?: string;
  name?: string;
  status?: VoteMonitorDomainEntitiesApplicationUserAggregateUserStatus;
  /** @format date-time */
  createdOn?: string;
  /** @format date-time */
  lastModifiedOn?: string | null;
}

export enum VoteMonitorDomainEntitiesApplicationUserAggregateUserStatus {
  Active = 'Active',
  Deactivated = 'Deactivated',
}

export type VoteMonitorApiFeatureNgoAdminListRequest = object;

export type VoteMonitorApiFeatureNgoAdminGetRequest = object;

export type VoteMonitorApiFeatureNgoAdminDeleteRequest = object;

export type VoteMonitorApiFeatureNgoAdminDeactivateRequest = object;

export interface VoteMonitorApiFeatureNgoAdminCreateRequest {
  /** @minLength 1 */
  name: string;
  /** @minLength 1 */
  login: string;
  /** @minLength 1 */
  password: string;
}

export type VoteMonitorApiFeatureNgoAdminActivateRequest = object;

export interface VoteMonitorApiFeatureObserverUpdateRequest {
  /** @minLength 3 */
  name: string;
  phoneNumber?: string;
}

export interface VoteMonitorCoreModelsPagedResponseOfObserverModel {
  /** @format int32 */
  currentPage?: number;
  /** @format int32 */
  pageSize?: number;
  /** @format int32 */
  totalCount?: number;
  items?: VoteMonitorApiFeatureObserverObserverModel[];
}

export interface VoteMonitorApiFeatureObserverObserverModel {
  /** @format guid */
  id?: string;
  name?: string;
  login?: string;
  phoneNumber?: string;
  status?: VoteMonitorDomainEntitiesApplicationUserAggregateUserStatus;
  /** @format date-time */
  createdOn?: string;
  /** @format date-time */
  lastModifiedOn?: string | null;
}

export type VoteMonitorApiFeatureObserverListRequest = object;

export interface VoteMonitorApiFeatureObserverImportValidationErrorModel {
  /** @format guid */
  id?: string;
  message?: string | null;
}

export interface VoteMonitorApiFeatureObserverImportRequest {
  /**
   * @format binary
   * @minLength 1
   */
  file: File;
}

export type VoteMonitorApiFeatureObserverGetRequest = object;

export type VoteMonitorApiFeatureObserverDeleteRequest = object;

export interface VoteMonitorApiFeatureObserverDeactivateRequest {
  /**
   * @format guid
   * @minLength 1
   */
  ngoId: string;
}

export interface VoteMonitorApiFeatureObserverCreateRequest {
  /** @minLength 1 */
  name: string;
  /**
   * @format email
   * @minLength 1
   * @pattern ^[^@]+@[^@]+$
   */
  email: string;
  /** @minLength 1 */
  password: string;
  /** @minLength 1 */
  phoneNumber: string;
}

export type VoteMonitorApiFeatureObserverActivateRequest = object;

export interface VoteMonitorApiFeatureElectionRoundUpdateRequest {
  /**
   * @minLength 1
   * @maxLength 256
   */
  title: string;
  /**
   * @minLength 1
   * @maxLength 256
   */
  englishTitle: string;
  /** @format date */
  startDate?: string;
  /**
   * @format guid
   * @minLength 1
   */
  countryId: string;
}

export type VoteMonitorApiFeatureElectionRoundUnstartRequest = object;

export type VoteMonitorApiFeatureElectionRoundUnarchiveRequest = object;

export type VoteMonitorApiFeatureElectionRoundStartRequest = object;

export interface VoteMonitorCoreModelsPagedResponseOfElectionRoundBaseModel {
  /** @format int32 */
  currentPage?: number;
  /** @format int32 */
  pageSize?: number;
  /** @format int32 */
  totalCount?: number;
  items?: VoteMonitorApiFeatureElectionRoundElectionRoundBaseModel[];
}

export interface VoteMonitorApiFeatureElectionRoundElectionRoundBaseModel {
  /** @format guid */
  id?: string;
  /** @format guid */
  countryId?: string;
  country?: string;
  title?: string;
  englishTitle?: string;
  /** @format date */
  startDate?: string;
  status?: VoteMonitorDomainEntitiesElectionRoundAggregateElectionRoundStatus;
  /** @format date-time */
  createdOn?: string;
  /** @format date-time */
  lastModifiedOn?: string | null;
}

export enum VoteMonitorDomainEntitiesElectionRoundAggregateElectionRoundStatus {
  Archived = 'Archived',
  NotStarted = 'NotStarted',
  Started = 'Started',
}

export type VoteMonitorApiFeatureElectionRoundListRequest = object;

export interface VoteMonitorApiFeatureElectionRoundElectionRoundModel {
  /** @format guid */
  id?: string;
  /** @format guid */
  countryId?: string;
  country?: string;
  title?: string;
  englishTitle?: string;
  /** @format date */
  startDate?: string;
  status?: VoteMonitorDomainEntitiesElectionRoundAggregateElectionRoundStatus;
  /** @format date-time */
  createdOn?: string;
  /** @format date-time */
  lastModifiedOn?: string | null;
  monitoringNgos?: VoteMonitorApiFeatureElectionRoundMonitoringNgoModel[];
}

export interface VoteMonitorApiFeatureElectionRoundMonitoringNgoModel {
  /** @format guid */
  ngoId?: string;
  name?: string;
  status?: VoteMonitorDomainEntitiesNgoAggregateNgoStatus;
  monitoringObservers?: VoteMonitorApiFeatureElectionRoundMonitoringObserverModel[];
}

export interface VoteMonitorApiFeatureElectionRoundMonitoringObserverModel {
  /** @format guid */
  observerId?: string;
  name?: string;
  status?: VoteMonitorDomainEntitiesApplicationUserAggregateUserStatus;
}

export type VoteMonitorApiFeatureElectionRoundGetRequest = object;

export type VoteMonitorApiFeatureElectionRoundDeleteRequest = object;

export interface VoteMonitorApiFeatureElectionRoundCreateRequest {
  /**
   * @format guid
   * @minLength 1
   */
  countryId: string;
  /**
   * @minLength 1
   * @maxLength 256
   */
  title: string;
  /**
   * @minLength 1
   * @maxLength 256
   */
  englishTitle: string;
  /** @format date */
  startDate?: string;
}

export type VoteMonitorApiFeatureElectionRoundArchiveRequest = object;

export type VoteMonitorApiFeatureMonitoringRemoveObserverRequest = object;

export type VoteMonitorApiFeatureMonitoringRemoveNgoRequest = object;

export interface VoteMonitorApiFeatureMonitoringAddObserverMonitoringObserverModel {
  /** @format guid */
  id?: string;
  /** @format guid */
  inviterNgoId?: string;
  status?: VoteMonitorDomainEntitiesMonitoringObserverAggregateMonitoringObserverStatus;
}

export enum VoteMonitorDomainEntitiesMonitoringObserverAggregateMonitoringObserverStatus {
  Active = 'Active',
  Suspended = 'Suspended',
}

export interface MicrosoftAspNetCoreHttpHttpValidationProblemDetails {
  type?: string | null;
  title?: string | null;
  /** @format int32 */
  status?: number | null;
  detail?: string | null;
  instance?: string | null;
  errors?: Record<string, string[]>;
  [key: string]: any;
}

export interface VoteMonitorApiFeatureMonitoringAddObserverRequest {
  /**
   * @format guid
   * @minLength 1
   */
  observerId: string;
}

export interface VoteMonitorApiFeatureMonitoringAddNgoRequest {
  /**
   * @format guid
   * @minLength 1
   */
  ngoId: string;
}

export interface VoteMonitorApiFeatureUserPreferencesUpdateRequest {
  /**
   * @format guid
   * @minLength 1
   */
  languageId: string;
}

export interface VoteMonitorApiFeatureUserPreferencesUserPreferencesModel {
  /** @format guid */
  languageId?: string;
}

export type VoteMonitorApiFeatureUserPreferencesGetRequest = object;

export interface VoteMonitorApiFeatureFormTemplateUpdateRequest {
  /**
   * @minLength 0
   * @maxLength 256
   */
  code: string;
  name?: VoteMonitorCoreModelsTranslatedString;
  /** @minLength 1 */
  formTemplateType: VoteMonitorDomainEntitiesFormTemplateAggregateFormTemplateType;
  /** @minLength 1 */
  languages: string[];
  questions?: VoteMonitorFormModuleRequestsBaseQuestionRequest[];
}

export type VoteMonitorCoreModelsTranslatedString = object;

export enum VoteMonitorDomainEntitiesFormTemplateAggregateFormTemplateType {
  ClosingAndCounting = 'ClosingAndCounting',
  Opening = 'Opening',
  Voting = 'Voting',
}

export interface VoteMonitorFormModuleRequestsBaseQuestionRequest {
  $questionType?: string;
  /** @format guid */
  id?: string;
  code?: string;
  text?: VoteMonitorCoreModelsTranslatedString;
  helptext?: VoteMonitorCoreModelsTranslatedString | null;
}

export type VoteMonitorApiFeatureFormTemplatePublishRequest = object;

export interface VoteMonitorCoreModelsPagedResponseOfFormTemplateModel {
  /** @format int32 */
  currentPage?: number;
  /** @format int32 */
  pageSize?: number;
  /** @format int32 */
  totalCount?: number;
  items?: VoteMonitorApiFeatureFormTemplateFormTemplateModel[];
}

export interface VoteMonitorApiFeatureFormTemplateFormTemplateModel {
  /** @format guid */
  id?: string;
  code?: string;
  name?: VoteMonitorCoreModelsTranslatedString;
  status?: VoteMonitorDomainEntitiesFormTemplateAggregateFormTemplateStatus;
  /** @format date-time */
  createdOn?: string;
  /** @format date-time */
  lastModifiedOn?: string | null;
  questions?: VoteMonitorFormModuleModelsBaseQuestionModel[];
  languages?: string[];
}

export enum VoteMonitorDomainEntitiesFormTemplateAggregateFormTemplateStatus {
  Drafted = 'Drafted',
  Published = 'Published',
}

export interface VoteMonitorFormModuleModelsBaseQuestionModel {
  $questionType?: string;
  text?: VoteMonitorCoreModelsTranslatedString;
  helptext?: VoteMonitorCoreModelsTranslatedString | null;
}

export type VoteMonitorApiFeatureFormTemplateListRequest = object;

export type VoteMonitorApiFeatureFormTemplateGetRequest = object;

export type VoteMonitorApiFeatureFormTemplateDraftRequest = object;

export type VoteMonitorApiFeatureFormTemplateDeleteRequest = object;

export interface VoteMonitorApiFeatureFormTemplateCreateRequest {
  /**
   * @minLength 0
   * @maxLength 256
   */
  code: string;
  name?: VoteMonitorCoreModelsTranslatedString;
  /** @minLength 1 */
  formTemplateType: VoteMonitorDomainEntitiesFormTemplateAggregateFormTemplateType;
  /** @minLength 1 */
  languages: string[];
}

export interface VoteMonitorApiFeaturePollingStationAttachmentsAttachmentModel {
  /** @format guid */
  id?: string;
  fileName?: string;
  mimeType?: string;
  presignedUrl?: string;
  /** @format int32 */
  urlValidityInSeconds?: number;
}

export type VoteMonitorApiFeaturePollingStationAttachmentsListRequest = object;

export type VoteMonitorApiFeaturePollingStationAttachmentsGetRequest = object;

export type VoteMonitorApiFeaturePollingStationAttachmentsDeleteRequest = object;

export interface VoteMonitorApiFeaturePollingStationAttachmentsCreateRequest {
  /**
   * @format binary
   * @minLength 1
   */
  attachment: File;
}

export interface VoteMonitorApiFeaturePollingStationNotesNoteModel {
  /** @format guid */
  id?: string;
  text?: string;
  /** @format date-time */
  createdAt?: string;
  /** @format date-time */
  updatedAt?: string | null;
}

export interface VoteMonitorApiFeaturePollingStationNotesUpdateRequest {
  /**
   * @minLength 0
   * @maxLength 1024
   */
  text: string;
}

export type VoteMonitorApiFeaturePollingStationNotesListRequest = object;

export type VoteMonitorApiFeaturePollingStationNotesGetRequest = object;

export type VoteMonitorApiFeaturePollingStationNotesDeleteRequest = object;

export interface VoteMonitorApiFeaturePollingStationNotesCreateRequest {
  /**
   * @minLength 0
   * @maxLength 10000
   */
  text: string;
}

export interface VoteMonitorApiFeatureNotificationsSubscribeRequest {
  /**
   * @minLength 0
   * @maxLength 1024
   */
  token: string;
}

export interface VoteMonitorApiFeatureNotificationsSendResponse {
  /** @format int32 */
  successCount?: number;
  /** @format int32 */
  failedCount?: number;
}

export interface VoteMonitorApiFeatureNotificationsSendRequest {
  /** @minLength 1 */
  observerIds: string[];
  /**
   * @minLength 0
   * @maxLength 256
   */
  title: string;
  /**
   * @minLength 0
   * @maxLength 1024
   */
  body: string;
}

export interface VoteMonitorApiFeatureNotificationsListSentResponse {
  notifications?: VoteMonitorApiFeatureNotificationsListSentSentNotificationModel[];
}

export interface VoteMonitorApiFeatureNotificationsListSentSentNotificationModel {
  title?: string;
  body?: string;
  sender?: string;
  /** @format date-time */
  sentAt?: string;
  receivers?: VoteMonitorApiFeatureNotificationsListSentNotificationReceiver[];
}

export interface VoteMonitorApiFeatureNotificationsListSentNotificationReceiver {
  /** @format guid */
  id?: string;
  name?: string;
}

export type VoteMonitorApiFeatureNotificationsListSentRequest = object;

export interface VoteMonitorApiFeatureNotificationsListReceivedResponse {
  notifications?: VoteMonitorApiFeatureNotificationsListReceivedReceivedNotificationModel[];
}

export interface VoteMonitorApiFeatureNotificationsListReceivedReceivedNotificationModel {
  title?: string;
  body?: string;
  sender?: string;
  /** @format date-time */
  sentAt?: string;
}

export type VoteMonitorApiFeatureNotificationsListReceivedRequest = object;

export interface VoteMonitorApiFeatureAnswersNotesNoteModel {
  /** @format guid */
  id?: string;
  text?: string;
  /** @format date-time */
  createdAt?: string;
  /** @format date-time */
  updatedAt?: string | null;
}

export interface VoteMonitorApiFeatureAnswersNotesUpdateRequest {
  /**
   * @minLength 0
   * @maxLength 1024
   */
  text: string;
}

export interface VoteMonitorApiFeatureAnswersNotesListResponse {
  notes?: VoteMonitorApiFeatureAnswersNotesNoteModel[];
}

export type VoteMonitorApiFeatureAnswersNotesListRequest = object;

export type VoteMonitorApiFeatureAnswersNotesGetRequest = object;

export type VoteMonitorApiFeatureAnswersNotesDeleteRequest = object;

export interface VoteMonitorApiFeatureAnswersNotesCreateRequest {
  /**
   * @minLength 0
   * @maxLength 1024
   */
  text: string;
}

export interface VoteMonitorApiFeatureAnswersAttachmentsListResponse {
  attachments?: VoteMonitorApiFeatureAnswersAttachmentsAttachmentModel[];
}

export interface VoteMonitorApiFeatureAnswersAttachmentsAttachmentModel {
  /** @format guid */
  id?: string;
  fileName?: string;
  mimeType?: string;
  presignedUrl?: string;
  /** @format int32 */
  urlValidityInSeconds?: number;
}

export type VoteMonitorApiFeatureAnswersAttachmentsListRequest = object;

export type VoteMonitorApiFeatureAnswersAttachmentsGetRequest = object;

export type VoteMonitorApiFeatureAnswersAttachmentsDeleteRequest = object;

export interface VoteMonitorApiFeatureAnswersAttachmentsCreateRequest {
  /** @format binary */
  attachment?: File;
}

export interface VoteMonitorApiFeatureEmergenciesListSubmittedResponse {
  attachments?: VoteMonitorApiFeatureEmergenciesEmergencyModel[];
}

export interface VoteMonitorApiFeatureEmergenciesEmergencyModel {
  /** @format guid */
  id?: string;
  /** @format guid */
  observerId?: string;
  observerName?: string;
  title?: string;
  description?: string;
  attachments?: VoteMonitorApiFeatureEmergenciesAttachmentModel[];
}

export interface VoteMonitorApiFeatureEmergenciesAttachmentModel {
  /** @format guid */
  id?: string;
  fileName?: string;
  mimeType?: string;
  presignedUrl?: string;
  /** @format int32 */
  urlValidityInSeconds?: number;
}

export type VoteMonitorApiFeatureEmergenciesListSubmittedRequest = object;

export interface VoteMonitorApiFeatureEmergenciesListReceivedResponse {
  attachments?: VoteMonitorApiFeatureEmergenciesEmergencyModel[];
}

export type VoteMonitorApiFeatureEmergenciesListReceivedRequest = object;

export type VoteMonitorApiFeatureEmergenciesGetRequest = object;

export type VoteMonitorApiFeatureEmergenciesDeleteRequest = object;

export type VoteMonitorApiFeatureEmergenciesCreateRequest = object;

export interface VoteMonitorApiFeatureEmergenciesAttachmentsListResponse {
  attachments?: VoteMonitorApiFeatureEmergenciesAttachmentsAttachmentModel[];
}

export interface VoteMonitorApiFeatureEmergenciesAttachmentsAttachmentModel {
  /** @format guid */
  id?: string;
  fileName?: string;
  mimeType?: string;
  presignedUrl?: string;
  /** @format int32 */
  urlValidityInSeconds?: number;
}

export type VoteMonitorApiFeatureEmergenciesAttachmentsListRequest = object;

export type VoteMonitorApiFeatureEmergenciesAttachmentsGetRequest = object;

export interface VoteMonitorApiFeatureEmergenciesAttachmentsDeleteRequest {
  /**
   * @format guid
   * @minLength 1
   */
  pollingStationId: string;
}

export interface VoteMonitorApiFeatureEmergenciesAttachmentsCreateRequest {
  /**
   * @format binary
   * @minLength 1
   */
  attachment: File;
}

export interface VoteMonitorApiFeaturePollingStationInformationPollingStationInformationModel {
  /** @format guid */
  id?: string;
  /** @format date-time */
  createdAt?: string;
  /** @format date-time */
  updatedAt?: string | null;
  answers?: VoteMonitorAnswerModuleModelsBaseAnswerModel[];
}

export interface VoteMonitorAnswerModuleModelsBaseAnswerModel {
  $answerType?: string;
  /** @format guid */
  questionId?: string;
}

export interface VoteMonitorApiFeaturePollingStationInformationUpsertRequest {
  answers?: VoteMonitorAnswerModuleRequestsBaseAnswerRequest[];
}

export interface VoteMonitorAnswerModuleRequestsBaseAnswerRequest {
  $answerType?: string;
  /** @format guid */
  questionId?: string;
}

export interface VoteMonitorApiFeaturePollingStationInformationListResponse {
  informations?: VoteMonitorApiFeaturePollingStationInformationPollingStationInformationModel[];
}

export type VoteMonitorApiFeaturePollingStationInformationListRequest = object;

export interface VoteMonitorApiFeaturePollingStationInformationListMyResponse {
  informations?: VoteMonitorApiFeaturePollingStationInformationPollingStationInformationModel[];
}

export type VoteMonitorApiFeaturePollingStationInformationListMyRequest = object;

export type VoteMonitorApiFeaturePollingStationInformationGetRequest = object;

export type VoteMonitorApiFeaturePollingStationInformationDeleteRequest = object;

export interface VoteMonitorApiFeatureFormUpdateRequest {
  /**
   * @format guid
   * @minLength 1
   */
  monitoringNgoId: string;
  /**
   * @minLength 0
   * @maxLength 256
   */
  code: string;
  name?: VoteMonitorCoreModelsTranslatedString;
  /** @minLength 1 */
  formType: VoteMonitorDomainEntitiesFormAggregateFormType;
  /** @minLength 1 */
  languages: string[];
  questions?: VoteMonitorFormModuleRequestsBaseQuestionRequest[];
}

export enum VoteMonitorDomainEntitiesFormAggregateFormType {
  ClosingAndCounting = 'ClosingAndCounting',
  Opening = 'Opening',
  Voting = 'Voting',
}

export interface VoteMonitorApiFeatureFormPublishRequest {
  /**
   * @format guid
   * @minLength 1
   */
  monitoringNgoId: string;
}

export interface VoteMonitorCoreModelsPagedResponseOfFormModel {
  /** @format int32 */
  currentPage?: number;
  /** @format int32 */
  pageSize?: number;
  /** @format int32 */
  totalCount?: number;
  items?: VoteMonitorApiFeatureFormFormModel[];
}

export type VoteMonitorApiFeatureFormFormModel = object;

export type VoteMonitorApiFeatureFormListRequest = object;

export type VoteMonitorApiFeatureFormGetRequest = object;

export interface VoteMonitorApiFeatureFormDraftRequest {
  /**
   * @format guid
   * @minLength 1
   */
  monitoringNgoId: string;
}

export interface VoteMonitorApiFeatureFormDeleteRequest {
  /**
   * @format guid
   * @minLength 1
   */
  monitoringNgoId: string;
}

export interface VoteMonitorApiFeatureFormCreateRequest {
  /**
   * @format guid
   * @minLength 1
   */
  monitoringNgoId: string;
  /**
   * @minLength 0
   * @maxLength 256
   */
  code: string;
  name?: VoteMonitorCoreModelsTranslatedString;
  /** @minLength 1 */
  formType: VoteMonitorDomainEntitiesFormAggregateFormType;
  /** @minLength 1 */
  languages: string[];
}

export interface FeaturePollingStationInformationFormPollingStationInformationFormModel {
  /** @format guid */
  id?: string;
  /** @format date-time */
  createdOn?: string;
  /** @format date-time */
  lastModifiedOn?: string | null;
  questions?: VoteMonitorFormModuleModelsBaseQuestionModel[];
  languages?: string[];
}

export interface FeaturePollingStationInformationFormUpsertRequest {
  /** @minLength 1 */
  languages: string[];
  questions?: VoteMonitorFormModuleRequestsBaseQuestionRequest[];
}

export type FeaturePollingStationInformationFormGetRequest = object;

export type FeaturePollingStationInformationFormDeleteRequest = object;
