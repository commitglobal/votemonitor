
import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/form-templates/$formTemplateId')({
  component: FormTemplateDetails,
})

function FormTemplateDetails() {
  return <div className="p-2">Hello form details!</div>
}