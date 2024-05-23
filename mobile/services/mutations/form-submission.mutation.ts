import { useMutation, useQueryClient } from "@tanstack/react-query";
import {
  FormSubmissionAPIPayload,
  FormSubmissionsApiResponse,
  upsertFormSubmission,
} from "../definitions.api";
import { useMemo } from "react";
import { notesKeys, pollingStationsKeys } from "../queries.service";
import * as Crypto from "expo-crypto";
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
    () => pollingStationsKeys.formSubmissions(electionRoundId, pollingStationId),
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
      // Cancel any outgoing refetches (so they don't overwrite our optimistic update)
      await queryClient.cancelQueries({ queryKey: formSubmissionsQK });

      // Snapshot the previous value
      const previousData = queryClient.getQueryData<FormSubmissionsApiResponse>(formSubmissionsQK);

      // 1. SUBMISSIONS: Optimistically update the submissions
      const updatedSubmission = previousData?.submissions?.find((s) => s.formId === payload.formId);

      queryClient.setQueryData<FormSubmissionsApiResponse>(formSubmissionsQK, {
        submissions: [
          ...(previousData?.submissions?.filter((s) => s.formId !== payload.formId) || []),
          {
            ...payload,
            id: updatedSubmission?.id || Crypto.randomUUID(),
          },
        ],
      });

      const questionIds = payload?.answers?.map((a) => a.questionId) || [];

      // 2. ATTACHMENTS: Optimistically delete all orphaned Attachments
      const attachmentsQK = AttachmentsKeys.attachments(
        electionRoundId,
        pollingStationId,
        payload.formId,
      );
      const prevAttachments = queryClient.getQueryData<AttachmentApiResponse[]>(attachmentsQK);

      // Keep in attachments only those who exist in the current payload.answers
      const newAttachments =
        prevAttachments?.filter((att) => questionIds.includes(att.questionId)) || [];
      queryClient.setQueryData(attachmentsQK, newAttachments);

      // 3. NOTES: Optimistically delete all orphaned Notes
      const notesQK = notesKeys.notes(electionRoundId, pollingStationId, payload.formId);
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
        notesKeys.notes(electionRoundId, pollingStationId, variables.formId),
        context?.previousNotes,
      );
      queryClient.setQueryData(
        AttachmentsKeys.attachments(electionRoundId, pollingStationId, variables.formId),
        context?.prevAttachments,
      );
    },
    onSettled: (_data, _err, variables) => {
      queryClient.invalidateQueries({
        queryKey: AttachmentsKeys.attachments(electionRoundId, pollingStationId, variables.formId),
      });
      queryClient.invalidateQueries({
        queryKey: notesKeys.notes(electionRoundId, pollingStationId, variables.formId),
      });
      queryClient.invalidateQueries({ queryKey: formSubmissionsQK });
    },
  });
};
