import { Outlet, createRootRoute } from "@tanstack/react-router";

import Footer from "@/components/Footer";
import { SiteHeader } from "@/components/SiteHeader";

export const Route = createRootRoute({
  component: () => (
    <>
      <SiteHeader />
      <div className="container-wrapper">
        <div className="container py-6">
          <section className="scroll-mt-20">
            <Outlet />
          </section>
          <Footer />
        </div>
      </div>
    </>
  ),
});
