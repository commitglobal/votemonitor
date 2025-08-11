import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { siteConfig } from "@/config/site";
import { useAuth } from "@/contexts/auth.context";
import { useCurrentElectionRound } from "@/contexts/election-round.context";
import { useListMonitoringElections } from "@/queries/monitoring-elections";
import type { MonitoredElection } from "@/types/monitored-election";
import { Link } from "@tanstack/react-router";
import { Check, ChevronsUpDown } from "lucide-react";
import { useEffect, useState } from "react";
import { Icons } from "./Icons";
import MainNav from "./MainNav";
import { ModeSwitcher } from "./ModeSwitcher";
import NgoAdminNav from "./NgoAdminNav";
import PlatformAdminNav from "./PlatformAdminNav";
import { ProfileDropdown } from "./ProfileDropdown";
import { Button } from "./ui/button";

export function ElectionSiteHeader() {
  const auth = useAuth();
  const { data: elections } = useListMonitoringElections();
  const [currentElectionRound, setCurrentElectionRound] = useState<
    MonitoredElection | undefined
  >();
  const { electionRoundId } = useCurrentElectionRound();

  console.log(electionRoundId);

  useEffect(() => {
    setCurrentElectionRound(elections?.find((x) => x.id === electionRoundId));
  }, [elections, electionRoundId]);

  return (
    <header className="border-grid sticky top-0 z-50 w-full border-b bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
      <div className="container-wrapper">
        <div className="container flex h-14 items-center gap-2 md:gap-4">
          <div className="flex items-center">
            <MainNav />
            <span className="ml-2">/</span>

            <DropdownMenu>
              <DropdownMenuTrigger className="flex items-center gap-2 py-2.5 px-3 rounded-lg">
                <div className="text-start flex flex-col gap-1 leading-none">
                  <span className="text-sm leading-none font-semibold truncate ">
                    {currentElectionRound?.title}
                  </span>
                </div>
                <ChevronsUpDown className="ml-6 h-4 w-4 text-muted-foreground" />
              </DropdownMenuTrigger>
              <DropdownMenuContent className="w-xl" align="start">
                <DropdownMenuLabel>Monitored elections</DropdownMenuLabel>
                {elections?.map((election) => (
                  <DropdownMenuItem key={election.id}>
                    <Link
                      to="/elections/$electionRoundId"
                      params={{ electionRoundId: election.id }}
                      className="flex items-center gap-2"
                    >
                      {election.title}
                      {currentElectionRound?.id === election.id && (
                        <Check className="ml-auto" />
                      )}
                    </Link>
                  </DropdownMenuItem>
                ))}
              </DropdownMenuContent>
            </DropdownMenu>
          </div>
          <div className="ml-auto flex items-center gap-2 md:flex-1 md:justify-end">
            <nav className="flex items-center gap-0.5">
              <Button
                asChild
                variant="ghost"
                size="icon"
                className="h-8 w-8 px-0"
              >
                <Link
                  to={siteConfig.links.github}
                  target="_blank"
                  rel="noreferrer"
                >
                  <Icons.gitHub className="h-4 w-4" />
                  <span className="sr-only">GitHub</span>
                </Link>
              </Button>
              <ModeSwitcher />
              <ProfileDropdown />
            </nav>
          </div>
        </div>
        <div className="container flex items-center gap-2 md:gap-4">
          {auth.userRole === "NgoAdmin" ? (
            <NgoAdminNav />
          ) : (
            <PlatformAdminNav />
          )}
        </div>
      </div>
    </header>
  );
}
