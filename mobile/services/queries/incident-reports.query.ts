import { skipToken, useQuery } from "@tanstack/react-query";
import { useCallback } from "react";
import {
  getIncidentReportAttachments,
  getMyIncidentReports,
  IncidentReportAPIResponse,
} from "../api/incident-report/incident-report.api";
import {
  ElectionRoundsAllFormsAPIResponse,
  FormAPIModel,
  isIncidentReportingForm,
} from "../definitions.api";
import { AttachmentsKeys } from "./attachments.query";
import { useElectionRoundAllForms } from "./forms.query";

export const IncidentReportKeys = {
  all: ["incident-reports"] as const,
  byElectionRound: (electionRoundId: string | undefined) => [
    ...IncidentReportKeys.all,
    "electionRoundId",
    electionRoundId,
  ],
  add: () => [...IncidentReportKeys.all, "add"] as const,
  addNote: () => [...IncidentReportKeys.all, "addNote"] as const,
  deleteNote: () => [...IncidentReportKeys.all, "deleteNote"] as const,
  attachments: (electionRoundId: string | undefined, incidentReportId: string | undefined) => [
    ...IncidentReportKeys.all,
    "attachments",
    electionRoundId,
    incidentReportId,
  ],
  // the report and its notes run one after the other (also when resumed after being offline):
  // a note can only be added to a report that exists on the server
  mutationScope: (incidentReportId: string) => `incident-report:${incidentReportId}`,
  addAttachment: () => [...AttachmentsKeys.all, ...IncidentReportKeys.all, "addAttachment"],
  addAttachmentComplete: () => [...IncidentReportKeys.all, "addAttachmentComplete"],
  addAttachmentAbort: () => [...IncidentReportKeys.all, "addAttachmentAbort"],
};

export const useIncidentReports = <TResult = IncidentReportAPIResponse[]>(
  electionRoundId?: string,
  select?: (data: IncidentReportAPIResponse[]) => TResult,
) => {
  return useQuery({
    queryKey: IncidentReportKeys.byElectionRound(electionRoundId),
    queryFn: electionRoundId ? () => getMyIncidentReports(electionRoundId) : skipToken,
    select,
  });
};

export const useIncidentReportById = (
  electionRoundId: string | undefined,
  incidentReportId: string,
) => {
  return useIncidentReports(
    electionRoundId,
    useCallback(
      (data: IncidentReportAPIResponse[]) =>
        data.find((incidentReport) => incidentReport.id === incidentReportId),
      [incidentReportId],
    ),
  );
};

const selectIncidentReportingForms = (data: ElectionRoundsAllFormsAPIResponse): FormAPIModel[] =>
  data.forms.filter(isIncidentReportingForm);

export const useIncidentReportingForms = (electionRoundId: string | undefined) =>
  useElectionRoundAllForms(electionRoundId, selectIncidentReportingForms);

// the attachments of a report with their ids (incident-reports:my does not have them)
export const useIncidentReportAttachments = (
  electionRoundId: string | undefined,
  incidentReportId: string | undefined,
  formId: string | undefined,
  enabled: boolean,
) =>
  useQuery({
    queryKey: IncidentReportKeys.attachments(electionRoundId, incidentReportId),
    queryFn:
      enabled && electionRoundId && incidentReportId && formId
        ? () => getIncidentReportAttachments(electionRoundId, incidentReportId, formId)
        : skipToken,
    // continuing a draft needs the current files
    refetchOnMount: "always",
  });
