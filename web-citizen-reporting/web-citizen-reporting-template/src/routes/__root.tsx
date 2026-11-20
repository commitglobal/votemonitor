import {
  Outlet,
  createRootRouteWithContext,
  useLocation,
  useRouterState,
} from "@tanstack/react-router";

import Footer from "@/components/Footer";
import { SiteHeader } from "@/components/SiteHeader";
import { Spinner } from "@/components/Spinner";
import NotFound from "@/pages/NotFound";
import type { QueryClient } from "@tanstack/react-query";

function RouterSpinner() {
  const isLoading = useRouterState({ select: (s) => s.status === "pending" });
  return <Spinner show={isLoading} />;
}

function RootComponent() {
  const pathname = useLocation({
    select: (location) => location.pathname,
  });
  const isFooterHidden = pathname === "/thank-you";

  return (
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
          {!isFooterHidden && <Footer />}
        </div>
      </div>
    </>
  );
}

export const Route = createRootRouteWithContext<{
  queryClient: QueryClient;
}>()({
  component: () => <RootComponent />,
  notFoundComponent: () => <NotFound />,
});
