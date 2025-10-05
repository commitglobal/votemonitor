import { NavigationProgress } from "@/components/navigation-progress";
import type { AuthContext } from "@/contexts/auth.context";
import type { QueryClient } from "@tanstack/react-query";
import { Outlet, createRootRouteWithContext } from "@tanstack/react-router";

interface RouterContext {
  auth: AuthContext;
  queryClient: QueryClient;
}

export const Route = createRootRouteWithContext<RouterContext>()({
  component: () => (
    <>
      <NavigationProgress />

      <Outlet />
    </>
  ),
});
