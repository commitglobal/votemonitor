import { useMutation, useQueryClient } from "@tanstack/react-query";
import {
  deleteFormSubmission,
  DeleteFormSubmissionAPIPayload,
  FormSubmissionAPIPayload,
  FormSubmissionsApiResponse,
  markFormSubmissionCompletionStatus,
  MarkFormSubmissionCompletionStatusAPIPayload,
  upsertFormSubmission,
} from "../definitions.api";
import { useMemo } from "react";
import { notesKeys, pollingStationsKeys } from "../queries.service";
// import * as Crypto from "expo-crypto";
import { AttachmentsKeys } from "../queries/attachments.query";
import { AttachmentApiResponse } from "../api/get-attachments.api";
import { Note } from "../../common/models/note";

export const useFormSubmissionMutation = ({
  electionRoundId,
  pollingStationId,
  scopeId,
}: {
  electionRoundId: string | undefined;
  pollingStationId: string | undefined;
  scopeId: string;
}) => {
  const queryClient = useQueryClient();

  const formSubmissionsQK = useMemo(
    () => pollingStationsKeys.allFormSubmissions(electionRoundId, pollingStationId),
    [electionRoundId, pollingStationId],
  );

  return useMutation({
    mutationKey: pollingStationsKeys.upsertFormSubmission(),
    mutationFn: async (payload: FormSubmissionAPIPayload) => {
      return upsertFormSubmission(payload);
    },
    scope: {
      id: scopeId,
    },
    onMutate: async (payload: FormSubmissionAPIPayload) => {
      // Remove paused mutations for the same resource. Send only the last one!
      queryClient
        .getMutationCache()
        .getAll()
        .filter((mutation) => mutation.state.isPaused && mutation.options.scope?.id === scopeId)
        .sort((a, b) => b.state.submittedAt - a.state.submittedAt)
        .slice(1)
        .forEach((mutation) => {
          queryClient.getMutationCache().remove(mutation);
        });

      // Cancel any outgoing refetches (so they don't overwrite our optimistic update)
      await queryClient.cancelQueries({ queryKey: formSubmissionsQK });

      // Snapshot the previous value
      const previousData = queryClient.getQueryData<FormSubmissionsApiResponse>(formSubmissionsQK);

      // 1. SUBMISSIONS: Optimistically update the submissions
      const updatedSubmission = previousData?.submissions?.find((s) => s.id === payload.id);

      queryClient.setQueryData<FormSubmissionsApiResponse>(formSubmissionsQK, {
        submissions: [
          ...(previousData?.submissions?.filter((s) => s.id !== payload.id) || []),
          {
            ...payload,
            isCompleted: updatedSubmission?.isCompleted || false,
          },
        ],
      });

      const questionIds = payload?.answers?.map((a) => a.questionId) || [];

      // 2. ATTACHMENTS: Optimistically delete all orphaned Attachments
      const attachmentsQK = AttachmentsKeys.attachments(electionRoundId, payload.id);
      const prevAttachments = queryClient.getQueryData<AttachmentApiResponse[]>(attachmentsQK);

      // Keep in attachments only those who exist in the current payload.answers
      const newAttachments =
        prevAttachments?.filter((att) => questionIds.includes(att.questionId)) || [];
      queryClient.setQueryData(attachmentsQK, newAttachments);

      // 3. NOTES: Optimistically delete all orphaned Notes
      const notesQK = notesKeys.notes(electionRoundId, payload.id);
      const previousNotes = queryClient.getQueryData<Note[]>(notesQK);
      const newNotes = previousNotes?.filter((note) => questionIds.includes(note.questionId)) || [];
      queryClient.setQueryData(notesQK, newNotes);

      // Return a context object with the snapshotted value
      return { previousData, prevAttachments, previousNotes };
    },
    onError: (err, variables, context) => {
      console.log(err);
      queryClient.setQueryData(formSubmissionsQK, context?.previousData);
      queryClient.setQueryData(
        notesKeys.notes(electionRoundId, variables.id),
        context?.previousNotes,
      );
      queryClient.setQueryData(
        AttachmentsKeys.attachments(electionRoundId, variables.id),
        context?.prevAttachments,
      );
    },
    onSettled: (_data, _err, variables) => {
      queryClient.invalidateQueries({
        queryKey: AttachmentsKeys.attachments(electionRoundId, variables.id),
      });
      queryClient.invalidateQueries({
        queryKey: notesKeys.notes(electionRoundId, variables.id),
      });
      // FYI: these 2 queries actually make the same call, but use different query keys, therefore both need invalidation (used different keys for Pull To Refresh feature)
      // todo: maybe use the same call for the future if possible
      queryClient.invalidateQueries({ queryKey: formSubmissionsQK });
    },
  });
};

export const useMarkFormSubmissionCompletionStatusMutation = ({
  electionRoundId,
  pollingStationId,
  scopeId,
}: {
  electionRoundId: string | undefined;
  pollingStationId: string | undefined;
  scopeId: string;
}) => {
  const queryClient = useQueryClient();

  const formSubmissionsQK = useMemo(
    () => pollingStationsKeys.allFormSubmissions(electionRoundId, pollingStationId),
    [electionRoundId, pollingStationId],
  );

  return useMutation({
    mutationKey: pollingStationsKeys.markFormSubmissionCompletionStatus(),
    scope: {
      id: scopeId,
    },
    mutationFn: async (payload: MarkFormSubmissionCompletionStatusAPIPayload) => {
      return markFormSubmissionCompletionStatus(payload);
    },
    onMutate: async (payload: MarkFormSubmissionCompletionStatusAPIPayload) => {
      // Cancel any outgoing refetches (so they don't overwrite our optimistic update)
      await queryClient.cancelQueries({ queryKey: formSubmissionsQK });

      // Snapshot the previous value
      const previousData = queryClient.getQueryData<FormSubmissionsApiResponse>(formSubmissionsQK);

      // 1. SUBMISSIONS: Optimistically update the submissions with the new completion status
      const updatedSubmission = previousData?.submissions?.find(
        (s) => s.id === payload.submissionId,
      );

      queryClient.setQueryData<FormSubmissionsApiResponse>(
        formSubmissionsQK,
        updatedSubmission
          ? {
              submissions: [
                ...(previousData?.submissions?.filter((s) => s.id !== payload.submissionId) || []),
                {
                  ...updatedSubmission,
                  isCompleted: payload.isCompleted, // new value
                  lastUpdatedAt: updatedSubmission?.lastUpdatedAt || new Date().toISOString(),
                },
              ],
            }
          : previousData,
      );

      return { previousData };
    },
    onError: (err, variables, context) => {
      console.log(err);
      queryClient.setQueryData(formSubmissionsQK, context?.previousData);
    },
    onSettled: (_, _err, _variables) => {
      queryClient.invalidateQueries({ queryKey: formSubmissionsQK });
    },
  });
};

export const useDeleteFormSubmissionMutation = ({
  electionRoundId,
  pollingStationId,
  scopeId,
}: {
  electionRoundId: string | undefined;
  pollingStationId: string | undefined;
  scopeId: string;
}) => {
  const queryClient = useQueryClient();

  const formSubmissionsQK = useMemo(
    () => pollingStationsKeys.allFormSubmissions(electionRoundId, pollingStationId),
    [electionRoundId, pollingStationId],
  );

  return useMutation({
    mutationKey: pollingStationsKeys.deleteFormSubmission(),
    mutationFn: async (payload: DeleteFormSubmissionAPIPayload) => {
      return deleteFormSubmission(payload);
    },
    scope: {
      id: scopeId,
    },
    onMutate: async (payload: DeleteFormSubmissionAPIPayload) => {
      // remove updates for submission
      queryClient
        .getMutationCache()
        .getAll()
        .filter(
          (mutation) =>
            mutation.state.isPaused &&
            mutation.options.scope?.id ===
              `Submit_Answers_${payload.electionRoundId}_${payload.id}`,
        )
        .sort((a, b) => b.state.submittedAt - a.state.submittedAt)
        .slice(1)
        .forEach((mutation) => {
          queryClient.getMutationCache().remove(mutation);
        });

      // Remove paused mutations for the same resource. Send only the last one!
      queryClient
        .getMutationCache()
        .getAll()
        .filter((mutation) => mutation.state.isPaused && mutation.options.scope?.id === scopeId)
        .sort((a, b) => b.state.submittedAt - a.state.submittedAt)
        .slice(1)
        .forEach((mutation) => {
          queryClient.getMutationCache().remove(mutation);
        });

      // Cancel any outgoing refetches (so they don't overwrite our optimistic update)
      await queryClient.cancelQueries({ queryKey: formSubmissionsQK });

      // Snapshot the previous value
      const previousData = queryClient.getQueryData<FormSubmissionsApiResponse>(formSubmissionsQK);

      // 1. SUBMISSIONS: Optimistically update the submissions list
      queryClient.setQueryData<FormSubmissionsApiResponse>(formSubmissionsQK, {
        submissions: [...(previousData?.submissions?.filter((s) => s.id !== payload.id) || [])],
      });

      // 2. ATTACHMENTS: Optimistically delete all orphaned Attachments
      const attachmentsQK = AttachmentsKeys.attachments(electionRoundId, payload.id);
      const prevAttachments = queryClient.getQueryData<AttachmentApiResponse[]>(attachmentsQK);
      await queryClient.cancelQueries({ queryKey: attachmentsQK });

      // Keep in attachments only those who are not for current submissionId
      const newAttachments =
        prevAttachments?.filter((att) => att.submissionId !== payload.id) || [];
      queryClient.setQueryData(attachmentsQK, newAttachments);

      // 3. NOTES: Optimistically delete all orphaned Notes
      const notesQK = notesKeys.notes(electionRoundId, payload.id);
      const previousNotes = queryClient.getQueryData<Note[]>(notesQK);
      await queryClient.cancelQueries({ queryKey: notesQK });
      const newNotes = previousNotes?.filter((note) => note.submissionId !== payload.id) || [];
      queryClient.setQueryData(notesQK, newNotes);

      // Return a context object with the snapshotted value
      return { previousData, prevAttachments, previousNotes };
    },
    onError: (err, variables, context) => {
      console.log(err);
      queryClient.setQueryData(formSubmissionsQK, context?.previousData);
      queryClient.setQueryData(
        notesKeys.notes(electionRoundId, variables.id),
        context?.previousNotes,
      );
      queryClient.setQueryData(
        AttachmentsKeys.attachments(electionRoundId, variables.id),
        context?.prevAttachments,
      );
    },
    onSettled: (_data, _err, variables) => {
      queryClient.invalidateQueries({
        queryKey: AttachmentsKeys.attachments(electionRoundId, variables.id),
      });
      queryClient.invalidateQueries({
        queryKey: notesKeys.notes(electionRoundId, variables.id),
      });
      // FYI: these 2 queries actually make the same call, but use different query keys, therefore both need invalidation (used different keys for Pull To Refresh feature)
      // todo: maybe use the same call for the future if possible
      queryClient.invalidateQueries({ queryKey: formSubmissionsQK });
    },
  });
};
