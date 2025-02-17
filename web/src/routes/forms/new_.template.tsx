import { FormBuilderScreenTemplate } from '@/features/forms/components/FormBuilder/components/FormBuilderScreenTemplate'
import { redirectIfNotAuth } from '@/lib/utils'
import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/forms/new_/template')({
  beforeLoad: () => {
    redirectIfNotAuth()
  },
  component: CreateNewFormFromTemplate,
})

function CreateNewFormFromTemplate() {
  return <FormBuilderScreenTemplate />
}
