import { QueryClient, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  AddAttachmentAbortAPIPayload,
  AddAttachmentCompleteAPIPayload,
  AddAttachmentMultipartStartAPIResponse,
  AddAttachmentStartAPIPayload,
  addAttachmentMultipartAbort,
  addAttachmentMultipartComplete,
  addAttachmentMultipartStart,
  uploadS3Chunk,
} from "../../api/add-attachment.api";
import { AttachmentsKeys } from "../../queries/attachments.query";
import * as Sentry from "@sentry/react-native";
import { MUTATION_SCOPE_DO_NOT_HYDRATE } from "../../../common/constants";

// Multipart Upload - Start
export const useUploadAttachmentMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    scope: {
      id: MUTATION_SCOPE_DO_NOT_HYDRATE,
    },
    mutationKey: AttachmentsKeys.addAttachmentMutation(),
    mutationFn: (
      payload: AddAttachmentStartAPIPayload,
    ): Promise<AddAttachmentMultipartStartAPIResponse> => addAttachmentMultipartStart(payload),
    onError: (err, payload, context) => {
      Sentry.captureException(err, { data: { payload, context } });
    },
    onSettled: (_data, _err, variables) => {
      return queryClient.invalidateQueries({
        queryKey: AttachmentsKeys.attachments(variables.electionRoundId, variables.submissionId),
      });
    },
    retry: 3,
  });
};

export const useUploadAttachmentCompleteMutation = () => {
  return useMutation({
    scope: {
      id: MUTATION_SCOPE_DO_NOT_HYDRATE,
    },
    mutationKey: AttachmentsKeys.addAttachmentCompleteMutation(),
    mutationFn: (payload: AddAttachmentCompleteAPIPayload) =>
      addAttachmentMultipartComplete(payload),
    retry: 3,
  });
};

export const useUploadAttachmentAbortMutation = () => {
  return useMutation({
    scope: {
      id: MUTATION_SCOPE_DO_NOT_HYDRATE,
    },
    mutationKey: AttachmentsKeys.addAttachmentAbortMutation(),
    mutationFn: (payload: AddAttachmentAbortAPIPayload) => addAttachmentMultipartAbort(payload),
    retry: 3,
  });
};

export const useUploadS3ChunkMutation = () => {
  return useMutation({
    scope: {
      id: MUTATION_SCOPE_DO_NOT_HYDRATE,
    },
    mutationKey: AttachmentsKeys.addS3ChunkMutation(),
    mutationFn: (payload: { url: string; chunk: any }): Promise<{ ETag: string }> =>
      uploadS3Chunk(payload.url, payload.chunk),
    retry: 3,
  });
};

export const removeMutationByScopeId = (queryClient: QueryClient, scopeId: string) => {
  queryClient
    .getMutationCache()
    .getAll()
    .filter((mutation) => mutation.options.scope?.id === scopeId)
    .forEach((mutation) => {
      queryClient.getMutationCache().remove(mutation);
    });
};
