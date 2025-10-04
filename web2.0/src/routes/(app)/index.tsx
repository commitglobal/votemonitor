import Layout from "@/components/Layout";
import { useAuth } from "@/contexts/auth.context";
import NgoAdminHomePage from "@/pages/NgoAdmin/Home/Page";
import PlatformAdminHomepage from "@/pages/PlatformAdmin/Home/Page";
import { electionsSearchSchema } from "@/types/election";
import { createFileRoute, redirect } from "@tanstack/react-router";

export const Route = createFileRoute("/(app)/")({
  beforeLoad: ({ context }) => {
    if (!context.auth.isAuthenticated && !context.auth.isLoading) {
      console.log("redirecting to login");
      throw redirect({ to: "/login" });
    }
  },
  component: RouteComponent,
  validateSearch: electionsSearchSchema,
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
