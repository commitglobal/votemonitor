import { cn } from "@/lib/utils";
import { ElectionRoundStatus } from "@/types/election";
import { Badge } from "./ui/badge";
import { mapElectionRoundStatus } from "@/lib/i18n";

export default function ElectionRoundStatusBadge({
  status,
}: {
  status: ElectionRoundStatus;
}) {
  return (
    <Badge
      className={cn("w-fit", {
        "text-green-600 bg-green-200": status === ElectionRoundStatus.Started,
        "text-yellow-600 bg-yellow-200":
          status === ElectionRoundStatus.Archived,
        "text-slate-700 bg-slate-200":
          status === ElectionRoundStatus.NotStarted,
      })}
    >
      {mapElectionRoundStatus(status)}
    </Badge>
  );
}
