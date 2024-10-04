import { skipToken, useQuery } from "@tanstack/react-query";
import { getNotesForIncidentReport } from "../definitions.api";
import { incidentReportNotesKeys } from "../queries.service";

import { useCallback } from "react";
import { IncidentReportNote } from "../../common/models/incident-report-note";

export const useIncidentReportNotes = <TResult = IncidentReportNote[]>(
  electionRoundId: string | undefined,
  incidentReportId: string | undefined,
  select?: (data: IncidentReportNote[]) => TResult,
) => {
  return useQuery({
    queryKey: incidentReportNotesKeys.notes(electionRoundId, incidentReportId),
    queryFn:
      electionRoundId && incidentReportId
        ? () => getNotesForIncidentReport(electionRoundId, incidentReportId)
        : skipToken,
    select,
  });
};

export const useIncidentReportNotesForQuestionId = (
  electionRoundId: string | undefined,
  incidentReportId: string | undefined,
  questionId: string,
) => {
  return useIncidentReportNotes(
    electionRoundId,
    incidentReportId,
    useCallback(
      (data: IncidentReportNote[]) => {
        return data
          .filter((note) => note.questionId === questionId)
          .sort((a, b) => +new Date(b.createdAt) - +new Date(a.createdAt)); // added unary '+' operator to avoid ts error that the right-hand side of an arithmetic operation must be of type 'any', 'number', 'bigint' or an enum type
      },
      [electionRoundId, incidentReportId, questionId],
    ),
  );
};

export const useIncidentReportNotesForFormId = (
  electionRoundId: string | undefined,
  incidentReportId: string | undefined,
) => {
  return useIncidentReportNotes(
    electionRoundId,
    incidentReportId,
    useCallback(
      (data: IncidentReportNote[]) => {
        return data?.reduce(
          (acc: Record<string, IncidentReportNote[]>, curr: IncidentReportNote) => {
            if (!acc[curr.questionId]) {
              acc[curr.questionId] = [];
            }
            acc[curr.questionId].push(curr);
            return acc;
          },
          {},
        );
      },
      [electionRoundId, incidentReportId],
    ),
  );
};
