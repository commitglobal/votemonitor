import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/(auth)/forgot-password')({
  component: RouteComponent,
})

function RouteComponent() {
  return <div>Hello "/(auth)/forgot-password"!</div>
}
