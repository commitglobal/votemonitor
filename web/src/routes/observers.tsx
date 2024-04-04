
import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/observers')({
  component: Observers,
})

function Observers() {
  return <div className="p-2">Hello from Observers!</div>
}