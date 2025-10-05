import { mapQuickReportFollowUpStatus } from "@/lib/i18n";
import { QuickReportFollowUpStatus } from "@/types/quick-reports";
import { Badge } from "./ui/badge";
import { cn } from "@/lib/utils";

export default function QuickReportFollowUpStatusBadge({
  status,
}: {
  status: QuickReportFollowUpStatus;
}) {
  return (
    <Badge
      className={cn({
        "text-slate-700 bg-slate-200":
          status === QuickReportFollowUpStatus.NotApplicable,
        "text-red-600 bg-red-200":
          status === QuickReportFollowUpStatus.NeedsFollowUp,
        "text-green-600 bg-green-200":
          status === QuickReportFollowUpStatus.Resolved,
      })}
    >
      {mapQuickReportFollowUpStatus(status)}
    </Badge>
  );
}
