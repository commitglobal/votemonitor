import { QueryClient, skipToken, useQuery, useQueryClient } from "@tanstack/react-query";
import { useCallback } from "react";
import {
  getIncidentReportAttachments,
  getMyIncidentReports,
  IncidentReportAPIResponse,
  UpsertIncidentReportAPIPayload,
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

/*
  A sent report is locked as soon as "Send report" is pressed, even before the server has it: while
  that save is still pending (queued offline, or behind the report's earlier saves) the server still
  returns the report as a draft, so a refetch would make it editable again. Those reports keep what
  was sent, as completed.
*/
const withPendingSends = (
  queryClient: QueryClient,
  reports: IncidentReportAPIResponse[],
): IncidentReportAPIResponse[] => {
  const pendingSends = new Map(
    queryClient
      .getMutationCache()
      .findAll({ mutationKey: IncidentReportKeys.add(), status: "pending" })
      .map((mutation) => mutation.state.variables as UpsertIncidentReportAPIPayload | undefined)
      .filter((payload) => !!payload?.isCompleted)
      .map((payload) => [payload!.id, payload!]),
  );
  if (!pendingSends.size) return reports;

  return reports.map((report) => {
    const sent = pendingSends.get(report.id);
    return sent && !report.isCompleted
      ? { ...report, answers: sent.answers, isCompleted: true, isNotSynched: true }
      : report;
  });
};

export const useIncidentReports = <TResult = IncidentReportAPIResponse[]>(
  electionRoundId?: string,
  select?: (data: IncidentReportAPIResponse[]) => TResult,
) => {
  const queryClient = useQueryClient();

  return useQuery({
    queryKey: IncidentReportKeys.byElectionRound(electionRoundId),
    queryFn: electionRoundId
      ? async () => withPendingSends(queryClient, await getMyIncidentReports(electionRoundId))
      : skipToken,
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
