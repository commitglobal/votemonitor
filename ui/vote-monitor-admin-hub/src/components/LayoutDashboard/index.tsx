import { Outlet } from "react-router-dom";
import Section from "@/components/Section";

const LayoutDashboard = () => (
  <div className="flex flex-col h-screen">
    <div className="mb-auto">
      <Outlet />
    </div>
    <Section className="bg-gray-100">
    </Section>
    <Section className="bg-gray-50">
      <div />
    </Section>
  </div>
);

export default LayoutDashboard;
