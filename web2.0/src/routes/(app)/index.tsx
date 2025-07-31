import { createFileRoute, redirect } from "@tanstack/react-router";
import NgoAdminHomePage from "@/pages/NgoAdmin/Home/Page";
import PlatformAdminHomepage from "@/pages/PlatformAdmin/Home/Page";
import { useAuth } from "@/contexts/auth.context";
import Layout from "@/components/Layout";
import { electionsSearchSchema } from "@/types/election";
import { zodValidator } from "@tanstack/zod-adapter";

export const Route = createFileRoute("/(app)/")({
  beforeLoad: ({ context }) => {
    if (!context.auth.isAuthenticated) {
      throw redirect({ to: "/login" });
    }
  },
  component: RouteComponent,
  validateSearch: zodValidator(electionsSearchSchema),
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
