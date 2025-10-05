import { useCurrentElectionRound } from "@/contexts/election-round.context";
import { cn } from "@/lib/utils";
import { Link } from "@tanstack/react-router";

export default function NgoAdminNav() {
  const { electionRoundId } = useCurrentElectionRound();

  return (
    <div className="mr-4 hidden md:flex">
      <nav className="flex items-center gap-4 text-sm xl:gap-6">
        <Link
          to="/elections/$electionRoundId"
          params={{ electionRoundId: electionRoundId! }}
          className={cn("transition-colors hover:text-foreground/80")}
        >
          Dashboard
        </Link>
        <Link
          to="/elections/$electionRoundId/forms"
          params={{ electionRoundId: electionRoundId! }}
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
          params={{ electionRoundId: electionRoundId! }}
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
          params={{ electionRoundId: electionRoundId! }}
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
          params={{ electionRoundId: electionRoundId! }}
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
          params={{ electionRoundId: electionRoundId! }}
          className={cn(
            "transition-colors hover:text-foreground/80 text-foreground/80"
          )}
          activeProps={{
            className: "text-foreground",
          }}
        >
          Quick reports
        </Link>
        <Link
          to="/elections/$electionRoundId/incidents"
          params={{ electionRoundId: electionRoundId! }}
          className={cn(
            "transition-colors hover:text-foreground/80 text-foreground/80"
          )}
          activeProps={{
            className: "text-foreground",
          }}
        >
          Incident reports
        </Link>
      </nav>
    </div>
  );
}
