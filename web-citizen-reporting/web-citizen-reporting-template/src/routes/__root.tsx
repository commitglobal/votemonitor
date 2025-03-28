import {
  Outlet,
  createRootRouteWithContext,
  useRouterState,
} from "@tanstack/react-router";

import Footer from "@/components/Footer";
import { SiteHeader } from "@/components/SiteHeader";
import type { QueryClient } from "@tanstack/react-query";
import NotFound from "@/pages/NotFound";
import { Spinner } from "@/components/Spinner";

function RouterSpinner() {
  const isLoading = useRouterState({ select: (s) => s.status === "pending" });
  return <Spinner show={isLoading} />;
}

export const Route = createRootRouteWithContext<{
  queryClient: QueryClient;
}>()({
  component: () => (
    <>
      <SiteHeader />
      <div className="container-wrapper">
        <div className="container py-6">
          <section className="scroll-mt-20">
            <div className={`text-3xl`}>
              <RouterSpinner />
            </div>
            <Outlet />
          </section>
          <Footer />
        </div>
      </div>
    </>
  ),
  notFoundComponent: () => <NotFound />,
});
