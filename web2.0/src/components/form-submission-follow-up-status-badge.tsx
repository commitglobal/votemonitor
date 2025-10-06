import { mapFormSubmissionFollowUpStatus } from "@/lib/i18n";
import { cn } from "@/lib/utils";
import { FormSubmissionFollowUpStatus } from "@/types/forms-submission";
import { Badge } from "./ui/badge";

export default function FormSubmissionFollowUpStatusBadge({
  followUpStatus,
}: {
  followUpStatus: FormSubmissionFollowUpStatus;
}) {
  return (
    <Badge
      className={cn({
        "text-slate-700 bg-slate-200":
          followUpStatus === FormSubmissionFollowUpStatus.NotApplicable,
        "text-red-600 bg-red-200":
          followUpStatus === FormSubmissionFollowUpStatus.NeedsFollowUp,
        "text-green-600 bg-green-200":
          followUpStatus === FormSubmissionFollowUpStatus.Resolved,
      })}
    >
      {mapFormSubmissionFollowUpStatus(followUpStatus)}
    </Badge>
  );
}
