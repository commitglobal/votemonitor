import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/form-templates/')({
  component: () => <div>Hello /form-templates/!</div>
})