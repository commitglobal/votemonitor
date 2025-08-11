import { ElectionSiteHeader } from "@/components/ElectionSiteHeader";
import {
  CurrentElectionRoundProvider,
  useCurrentElectionRound,
} from "@/contexts/election-round.context";
import { DataSource } from "@/types/common";
import {
  createFileRoute,
  Outlet,
  stripSearchParams,
} from "@tanstack/react-router";
import { useEffect } from "react";
import z from "zod";

export const dataSourceSearchSchema = z.object({
  dataSource: z.enum(DataSource).default(DataSource.Ngo).catch(DataSource.Ngo),
});

export const Route = createFileRoute("/(app)/elections/$electionRoundId")({
  component: RouteComponentWrapper,
  validateSearch: dataSourceSearchSchema,

  search: {
    middlewares: [
      stripSearchParams({
        dataSource: DataSource.Ngo,
      }),
    ],
  },
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
