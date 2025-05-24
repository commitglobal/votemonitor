import { cn } from "@/lib/utils";
import { Link } from "@tanstack/react-router";

export interface NgoAdminNavProps {
  electionRoundId: string;
}
export default function NgoAdminNav({ electionRoundId }: NgoAdminNavProps) {
  return (
    <div className="mr-4 hidden md:flex">
      <nav className="flex items-center gap-4 text-sm xl:gap-6">
        <Link
          to="/elections/$electionRoundId/dashboard"
          params={{ electionRoundId }}
          className={cn("transition-colors hover:text-foreground/80")}
        >
          Dashboard
        </Link>
        <Link
          to="/elections/$electionRoundId/forms"
          params={{ electionRoundId }}
          className={cn(
            "transition-colors hover:text-foreground/80 text-foreground/80"
          )}
          activeProps={{
            className: "text-foreground",
          }}
        >
          Forms
        </Link>
        <Link
          to="/elections/$electionRoundId/observers"
          params={{ electionRoundId }}
          className={cn(
            "transition-colors hover:text-foreground/80 text-foreground/80"
          )}
          activeProps={{
            className: "text-foreground",
          }}
        >
          Observers
        </Link>
        <Link
          to="/elections/$electionRoundId/guides"
          params={{ electionRoundId }}
          className={cn(
            "transition-colors hover:text-foreground/80 text-foreground/80"
          )}
          activeProps={{
            className: "text-foreground",
          }}
        >
          Guides
        </Link>
        <Link
          to="/elections/$electionRoundId/submissions"
          params={{ electionRoundId }}
          className={cn(
            "transition-colors hover:text-foreground/80 text-foreground/80"
          )}
          activeProps={{
            className: "text-foreground",
          }}
        >
          Submissions
        </Link>

        <Link
          to="/elections/$electionRoundId/quick-reports"
          params={{ electionRoundId }}
          className={cn(
            "transition-colors hover:text-foreground/80 text-foreground/80"
          )}
          activeProps={{
            className: "text-foreground",
          }}
        >
          Quick Reports
        </Link>
      </nav>
    </div>
  );
}
