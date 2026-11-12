import { useMutation } from "@tanstack/react-query";
import * as Sentry from "@sentry/react-native";
import { MUTATION_SCOPE_DO_NOT_HYDRATE } from "../../../common/constants";
import {
  AddAttachmentIncidentReportAbortAPIPayload,
  AddAttachmentIncidentReportCompleteAPIPayload,
  AddAttachmentIncidentReportStartAPIPayload,
  addAttachmentIncidentReportMultipartAbort,
  addAttachmentIncidentReportMultipartComplete,
  addAttachmentIncidentReportMultipartStart,
} from "../../api/incident-report/incident-report.api";
import { IncidentReportKeys } from "../../queries/incident-reports.query";

// Multipart Upload - Start
export const useUploadAttachmentIncidentReportMutation = () => {
  return useMutation({
    mutationKey: IncidentReportKeys.addAttachment(),
    scope: {
      id: MUTATION_SCOPE_DO_NOT_HYDRATE,
    },
    mutationFn: (payload: AddAttachmentIncidentReportStartAPIPayload) =>
      addAttachmentIncidentReportMultipartStart(payload),
    onError: (err, variables) => {
      Sentry.captureException(err, { data: { variables } });
    },
    retry: 3,
  });
};

export const useUploadAttachmentIncidentReportCompleteMutation = () => {
  return useMutation({
    mutationKey: IncidentReportKeys.addAttachmentComplete(),
    scope: {
      id: MUTATION_SCOPE_DO_NOT_HYDRATE,
    },
    mutationFn: (payload: AddAttachmentIncidentReportCompleteAPIPayload) =>
      addAttachmentIncidentReportMultipartComplete(payload),
    retry: 3,
  });
};

export const useUploadAttachmentIncidentReportAbortMutation = () => {
  return useMutation({
    mutationKey: IncidentReportKeys.addAttachmentAbort(),
    scope: {
      id: MUTATION_SCOPE_DO_NOT_HYDRATE,
    },
    mutationFn: (payload: AddAttachmentIncidentReportAbortAPIPayload) =>
      addAttachmentIncidentReportMultipartAbort(payload),
    retry: 3,
  });
};
