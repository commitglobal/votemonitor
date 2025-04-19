import React from "react";
import { SiteHeader } from "./SiteHeader";
export interface LayoutProps {
  children: React.ReactNode;
}

function Layout({ children }: LayoutProps) {
  return (
    <>
      <SiteHeader />

      <div className="container-wrapper">
        <div className="container py-6">
          <section>{children}</section>
        </div>
      </div>
    </>
  );
}

export default Layout;
