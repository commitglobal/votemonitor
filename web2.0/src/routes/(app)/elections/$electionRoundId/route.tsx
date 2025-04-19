import { ElectionSiteHeader } from "@/components/ElectionSiteHeader";
import { createFileRoute, Outlet, redirect } from "@tanstack/react-router";

export const Route = createFileRoute("/(app)/elections/$electionRoundId")({
  beforeLoad: ({ context }) => {
    if (!context.auth.isAuthenticated) {
      throw redirect({ to: "/login" });
    }
  },
  component: RouteComponent,
});

function RouteComponent() {
  const { electionRoundId } = Route.useParams();
  return (
    <>
      <ElectionSiteHeader electionId={electionRoundId} />
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
