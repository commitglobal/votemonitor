import { skipToken, useQuery } from "@tanstack/react-query";
import { useCallback } from "react";
import { arrayToKeyObject } from "../../helpers/misc";
import {
  getIncidentReports,
  IncidentReportAPIResponse,
} from "../api/incident-report/get-incident-reports.api";
import { incidentReportAttachmentsKeys } from "./incident-report-attachments.query";

export const incidentReportKeys = {
  all: ["incident-reports"] as const,
  byElectionRound: (electionRoundId: string | undefined) => [
    ...incidentReportKeys.all,
    "electionRoundId",
    electionRoundId,
  ],
  add: () => [...incidentReportKeys.all, "add"] as const,
  addAttachment: () => [
    ...incidentReportAttachmentsKeys.all,
    ...incidentReportKeys.all,
    "addAttachment",
  ],
  upsertIncidentReport: () => [...incidentReportKeys.all, "upsertIncidentReport"] as const,
  allFormSubmissions: (electionRoundId: string | undefined) =>
    [
      ...incidentReportKeys.all,
      "electionRoundId",
      electionRoundId,
      "form-submissions",
      "all",
    ] as const,
  formSubmissions: (
    electionRoundId: string | undefined,
    incidentReportId: string | undefined,
    formId: string | undefined,
  ) =>
    [
      ...incidentReportKeys.all,
      "electionRoundId",
      electionRoundId,
      "incidentReportId",
      incidentReportId,
      "formID",
      formId,
      "form-submissions",
    ] as const,
};

export const useIncidentReports = <TResult = IncidentReportAPIResponse>(
  electionRoundId?: string,
  select?: (data: IncidentReportAPIResponse) => TResult,
) => {
  return useQuery({
    queryKey: incidentReportKeys.byElectionRound(electionRoundId),
    queryFn: electionRoundId ? () => getIncidentReports(electionRoundId) : skipToken,
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
      (data: IncidentReportAPIResponse) => {
        const selectedIncidentReport = data.incidentReports.find(
          (incidentReport) => incidentReport.id === incidentReportId,
        );
        return selectedIncidentReport;
      },
      [electionRoundId, incidentReportId],
    ),
  );
};

export const useIncidentReportFormAnswers = (
  electionRoundId: string | undefined,
  incidentReportId: string | undefined,
  formId: string,
) => {
  return useIncidentReports(
    electionRoundId,
    useCallback(
      (data: IncidentReportAPIResponse) => {
        const incidentReport = data.incidentReports?.find(
          (report) => report.formId === formId && report.id === incidentReportId,
        );
        return arrayToKeyObject(incidentReport?.answers || [], "questionId");
      },
      [electionRoundId, incidentReportId, formId],
    ),
  );
};
