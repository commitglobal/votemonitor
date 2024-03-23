import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/form-templates/$formTemplateId/edit')({
  component: EditFormTemplate
})

function EditFormTemplate(){
  return <div>Hello form template edit</div>
}