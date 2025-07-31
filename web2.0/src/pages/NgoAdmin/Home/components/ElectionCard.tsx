import { Badge } from "@/components/ui/badge";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { ElectionStatus } from "@/types/election";
import type { MonitoredElection } from "@/types/monitored-election";
import { useRouter } from "@tanstack/react-router";
import { CalendarDays, Globe, Shield, Users } from "lucide-react";

function getStatusColor(status: ElectionStatus) {
  switch (status) {
    case ElectionStatus.NotStarted:
      return "bg-blue-100 text-blue-800 hover:bg-blue-100";
    case ElectionStatus.Started:
      return "bg-green-100 text-green-800 hover:bg-green-100";
    case ElectionStatus.Archived:
      return "bg-gray-100 text-gray-800 hover:bg-gray-100";

    default:
      return "bg-gray-100 text-gray-800 hover:bg-gray-100";
  }
}

interface ElectionCardProps {
  election: MonitoredElection;
}

export default function ElectionCard({ election }: ElectionCardProps) {
  const router = useRouter();
  const handleCardClick = () => {
    router.navigate({
      to: "/elections/$electionRoundId",
      params: { electionRoundId: election.id },
    });
  };

  const showCoalition = !election.coalitionId || election.coalitionId === "";

  return (
    <Card
      className="w-full hover:shadow-md transition-shadow cursor-pointer focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
      role="button"
      onClick={handleCardClick}
    >
      <CardHeader className="pb-3">
        <div className="flex items-start justify-between">
          <div className="space-y-1">
            <CardTitle className="text-lg leading-tight">
              {election.title}
            </CardTitle>
            <div className="flex items-center gap-2 text-sm text-muted-foreground">
              <Globe className="h-4 w-4" />
              <span>{election.countryName}</span>
              <Badge variant="outline" className="text-xs">
                {election.countryIso2}
              </Badge>
            </div>
          </div>
          <Badge className={getStatusColor(election.status)}>
            {election.status}
          </Badge>
        </div>
      </CardHeader>
      <CardContent className="space-y-4">
        <div className="flex items-center gap-2 text-sm">
          <CalendarDays className="h-4 w-4 text-muted-foreground" />
          <span>Start Date: {election.startDate}</span>
        </div>

        <div className="space-y-2">
          {showCoalition && (
            <div className="flex items-center gap-2 text-sm">
              <Users className="h-4 w-4 text-muted-foreground" />
              <span className="font-medium">Coalition:</span>
              <span>{election.coalitionName}</span>
            </div>
          )}

          <div className="flex flex-wrap gap-2">
            {election.isCoalitionLeader && (
              <Badge variant="secondary" className="text-xs">
                <Shield className="h-3 w-3 mr-1" />
                Coalition Leader
              </Badge>
            )}
            {election.isMonitoringNgoForCitizenReporting && (
              <Badge variant="outline" className="text-xs">
                Citizen Reporting
              </Badge>
            )}
          </div>
        </div>
      </CardContent>
    </Card>
  );
}
