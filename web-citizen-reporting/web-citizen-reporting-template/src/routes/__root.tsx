import { Outlet, createRootRoute, useLocation } from "@tanstack/react-router";

import Footer from "@/components/Footer";
import { SiteHeader } from "@/components/SiteHeader";

export const Route = createRootRoute({
  component: () => <RootComponent />,
});

const RootComponent = () => {
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
            <Outlet />
          </section>
          {!isFooterHidden && <Footer />}
        </div>
      </div>
    </>
  );
};
