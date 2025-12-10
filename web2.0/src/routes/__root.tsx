import type { QueryClient } from '@tanstack/react-query'
import { Outlet, createRootRouteWithContext } from '@tanstack/react-router'
import type { AuthContext } from '@/contexts/auth.context'
import { NavigationProgress } from '@/components/NavigationProgress'

interface RouterContext {
  auth: AuthContext
  queryClient: QueryClient
}

export const Route = createRootRouteWithContext<RouterContext>()({
  component: () => (
    <>
      <NavigationProgress />

      <Outlet />
    </>
  ),
})
