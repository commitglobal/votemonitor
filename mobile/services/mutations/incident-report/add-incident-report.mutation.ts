import { useMutation, useQueryClient } from "@tanstack/react-query";
import {
  IncidentReportAPIResponse,
  IncidentReportAttachmentAPIResponse,
  IncidentReportNoteAPIResponse,
  UpsertIncidentReportAPIPayload,
  UpsertIncidentReportNoteAPIPayload,
  DeleteIncidentReportNoteAPIPayload,
  deleteIncidentReportAttachment,
} from "../../api/incident-report/incident-report.api";
import { IncidentReportKeys } from "../../queries/incident-reports.query";
import { isNetworkError } from "../../api";

/*
  Incident report saves (the report and its notes) must not be lost when the connection drops: a request
  that got no response is retried (with a growing delay, paused while offline) until it goes through.
  Errors the server answered (e.g. a locked report) are not retried.
*/
const retryWhileNoConnection = (_failureCount: number, error: unknown) => isNetworkError(error);
const retryDelay = (attempt: number) => Math.min(1000 * 2 ** attempt, 30 * 1000);

// attachments and notes are only used for the optimistic update, they are sent separately
export type AddIncidentReportMutationPayload = UpsertIncidentReportAPIPayload & {
  attachments: IncidentReportAttachmentAPIResponse[];
  notes: IncidentReportNoteAPIResponse[];
};

// the mutationFns are set through setMutationDefaults (PersistQueryContext) so they can resume offline;
// the report and its notes share a scope, so the notes are only sent after the report was created
export const useAddIncidentReport = (incidentReportId: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationKey: IncidentReportKeys.add(),
    scope: { id: IncidentReportKeys.mutationScope(incidentReportId) },
    retry: retryWhileNoConnection,
    retryDelay,
    onMutate: async ({
      attachments,
      notes,
      electionRoundId,
      ...payload
    }: AddIncidentReportMutationPayload) => {
      const queryKey = IncidentReportKeys.byElectionRound(electionRoundId);
      // so an outgoing refetch does not overwrite the optimistic update
      await queryClient.cancelQueries({ queryKey });

      const previousReports = queryClient.getQueryData(queryKey);

      queryClient.setQueryData<IncidentReportAPIResponse[]>(queryKey, (old = []) => [
        ...old.filter((report) => report.id !== payload.id),
        {
          id: payload.id,
          formId: payload.formId,
          locationType: payload.locationType,
          pollingStationId: payload.pollingStationId,
          locationDescription: payload.locationDescription,
          answers: payload.answers,
          isCompleted: payload.isCompleted ?? false,
          timestamp: payload.lastUpdatedAt,
          notes,
          attachments,
          isNotSynched: true,
        },
      ]);

      return { previousReports };
    },
    onError: (_err, variables, context) => {
      const queryKey = IncidentReportKeys.byElectionRound(variables.electionRoundId);
      queryClient.setQueryData(queryKey, context?.previousReports);
    },
    onSettled: (_data, _err, variables) => {
      return queryClient.invalidateQueries({
        queryKey: IncidentReportKeys.byElectionRound(variables.electionRoundId),
      });
    },
  });
};

export const useAddIncidentReportNote = (incidentReportId: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationKey: IncidentReportKeys.addNote(),
    scope: { id: IncidentReportKeys.mutationScope(incidentReportId) },
    retry: retryWhileNoConnection,
    retryDelay,
    onSettled: (_data, _err, variables: UpsertIncidentReportNoteAPIPayload) => {
      return queryClient.invalidateQueries({
        queryKey: IncidentReportKeys.byElectionRound(variables.electionRoundId),
      });
    },
  });
};

export const useDeleteIncidentReportNote = (incidentReportId: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationKey: IncidentReportKeys.deleteNote(),
    scope: { id: IncidentReportKeys.mutationScope(incidentReportId) },
    retry: retryWhileNoConnection,
    retryDelay,
    onSettled: (_data, _err, variables: DeleteIncidentReportNoteAPIPayload) => {
      return queryClient.invalidateQueries({
        queryKey: IncidentReportKeys.byElectionRound(variables.electionRoundId),
      });
    },
  });
};

// online only: deleting a file needs the server (adding one does too)
export const useDeleteIncidentReportAttachment = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: deleteIncidentReportAttachment,
    onSettled: (_data, _err, variables) => {
      queryClient.invalidateQueries({
        queryKey: IncidentReportKeys.byElectionRound(variables.electionRoundId),
      });
      queryClient.invalidateQueries({
        queryKey: IncidentReportKeys.attachments(
          variables.electionRoundId,
          variables.incidentReportId,
        ),
      });
    },
  });
};
