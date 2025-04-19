import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/(auth)/success')({
  component: RouteComponent,
})

function RouteComponent() {
  return <div>Hello "/(auth)/success"!</div>
}
