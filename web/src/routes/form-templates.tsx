
import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/form-templates')({
  component: FormTemplates,
})

function FormTemplates() {
  return <div className="p-2">Hello from FormTemplates!</div>
}