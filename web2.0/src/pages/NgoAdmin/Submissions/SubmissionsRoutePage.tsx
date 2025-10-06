import { H1 } from "@/components/ui/typography";
import { Route } from "@/routes/(app)/elections/$electionRoundId/submissions/route";
import { Outlet } from "@tanstack/react-router";
import { UserCog, Wrench } from "lucide-react";
import { useMemo } from "react";
import { PageNav } from "./components/page-nav";

export function SubmissionsRoutePage() {
  const { electionRoundId } = Route.useParams();
  const sidebarNavItems = useMemo(
    () => [
      {
        title: "All submissions",
        href: `/elections/${electionRoundId}/submissions`,
        icon: <UserCog size={18} />,
      },
      {
        title: "Submission aggregated by form",
        href: `/elections/${electionRoundId}/submissions/by-form`,
        icon: <Wrench size={18} />,
      },
    ],
    [electionRoundId]
  );

  return (
    <>
      <div className="space-y-0.5">
        <H1>Submissions</H1>
      </div>
      <div className="flex flex-1 flex-col space-y-2 overflow-hidden md:space-y-2 lg:space-y-0 lg:space-x-12">
        <aside className="top-0 lg:sticky">
          <PageNav items={sidebarNavItems} />
        </aside>
        <div className="flex w-full overflow-y-hidden p-1">
          <Outlet />
        </div>
      </div>
    </>
  );
}
