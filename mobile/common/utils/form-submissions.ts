import { SubmissionStatus } from "../../services/form.parser";

export const SubmissionStateToTextMapper: Record<SubmissionStatus, string> = {
  [SubmissionStatus.NOT_STARTED]: "status.not_started",
  [SubmissionStatus.IN_PROGRESS]: "status.in_progress",
  [SubmissionStatus.COMPLETED]: "status.completed",
  [SubmissionStatus.MARKED_AS_COMPLETED]: "status.marked_as_completed",
};
