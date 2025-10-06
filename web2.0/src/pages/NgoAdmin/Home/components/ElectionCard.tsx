import { CountryFlag } from "@/components/country-flag";
import ElectionRoundStatusBadge from "@/components/election-round-status-badge";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardAction,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import type { MonitoredElection } from "@/types/monitored-election";
import { Link } from "@tanstack/react-router";
import { CalendarDays, Eye, Shield, Users } from "lucide-react";

interface ElectionCardProps {
  electionRound: MonitoredElection;
}

export function ElectionRoundCard({ electionRound }: ElectionCardProps) {
  const showCoalition =
    !electionRound.coalitionId || electionRound.coalitionId === "";

  return (
    <Card>
      <CardHeader>
        <CardTitle>{electionRound.title}</CardTitle>
        <CardDescription>{electionRound.englishTitle}</CardDescription>
        <CardAction>
          <Button variant="link" asChild>
            <Link
              to="/elections/$electionRoundId"
              params={{ electionRoundId: electionRound.id }}
            >
              <CountryFlag
                code={electionRound.countryIso2}
                className="size-10 rounded-lg"
              />
            </Link>
          </Button>
        </CardAction>
      </CardHeader>
      <CardContent>
        <div className="space-y-2">
          <div className="flex items-center gap-2 text-sm">
            <CalendarDays className="h-4 w-4 text-muted-foreground" />
            <span>{electionRound.startDate}</span>
          </div>

          <div className="flex flex-wrap gap-2">
            <ElectionRoundStatusBadge status={electionRound.status} />
            {showCoalition && (
              <div className="flex items-center gap-2 text-sm">
                <Users className="h-4 w-4 text-muted-foreground" />
                <span className="font-medium">Coalition:</span>
                <span>{electionRound.coalitionName}</span>
              </div>
            )}
            {electionRound.isCoalitionLeader && (
              <Badge variant="secondary" className="text-xs">
                <Shield className="h-3 w-3 mr-1" />
                Coalition Leader
              </Badge>
            )}

            {electionRound.isMonitoringNgoForCitizenReporting && (
              <Badge variant="outline" className="text-xs">
                Citizen Reporting
              </Badge>
            )}
          </div>
        </div>
      </CardContent>
      <CardFooter className="flex justify-end">
        <Button variant="link" asChild>
          <Link
            to="/elections/$electionRoundId"
            params={{ electionRoundId: electionRound.id }}
          >
            <Eye className="h-4 w-4 mr-2" />
            View Details
          </Link>
        </Button>
      </CardFooter>
    </Card>
  );
}
