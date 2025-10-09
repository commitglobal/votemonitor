import { ElectionSiteHeader } from "@/components/ElectionSiteHeader";
import {
  CurrentElectionRoundProvider,
  useCurrentElectionRound,
} from "@/contexts/election-round.context";
import { createFileRoute, Outlet } from "@tanstack/react-router";
import { useEffect } from "react";

export const Route = createFileRoute("/(app)/elections/$electionRoundId")({
  component: RouteComponentWrapper,
});

function RouteComponentWrapper() {
  return (
    <CurrentElectionRoundProvider>
      <RouteComponent />
    </CurrentElectionRoundProvider>
  );
}

function RouteComponent() {
  const { electionRoundId } = Route.useParams();
  const { setElectionRoundId } = useCurrentElectionRound();

  useEffect(() => {
    setElectionRoundId(electionRoundId);
  }, [electionRoundId, setElectionRoundId]);

  return (
    <>
      <ElectionSiteHeader />
      <div className="container-wrapper">
        <div className="container py-6">
          <section>
            <Outlet />
          </section>
        </div>
      </div>
    </>
  );
}
