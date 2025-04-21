import { createFileRoute, redirect } from "@tanstack/react-router";
import NgoAdminHomePage from "@/pages/NgoAdmin/Home/Page";
import PlatformAdminHomepage from "@/pages/PlatformAdmin/HomePage";
import { useAuth } from "@/contexts/auth.context";
import Layout from "@/components/Layout";

export const Route = createFileRoute("/(app)/")({
  beforeLoad: ({ context }) => {
    if (!context.auth.isAuthenticated) {
      throw redirect({ to: "/login" });
    }
  },
  component: RouteComponent,
});

function RouteComponent() {
  const auth = useAuth();

  return (
    <Layout>
      {auth.userRole === "NgoAdmin" ? (
        <NgoAdminHomePage />
      ) : (
        <PlatformAdminHomepage />
      )}
    </Layout>
  );
}
