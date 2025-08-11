import { useCurrentElectionRound } from "@/contexts/election-round.context";
import { cn } from "@/lib/utils";
import { Link } from "@tanstack/react-router";

export interface PlatformAdminNavProps {
  electionRoundId: string;
}
export default function PlatformAdminNav() {
  const { electionRoundId } = useCurrentElectionRound();

  return (
    <div className="mr-4 hidden md:flex">
      <nav className="flex items-center gap-4 text-sm xl:gap-6">
        <Link
          to="/elections/$electionRoundId"
          params={{ electionRoundId }}
          className={cn("transition-colors hover:text-foreground/80")}
        >
          Dashboard
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
      </nav>
    </div>
  );
}
