import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/(auth)/set-password')({
  component: RouteComponent,
})

function RouteComponent() {
  return <div>Hello "/(auth)/set-password"!</div>
}
