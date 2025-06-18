import { FormBuilderScreenReuse } from '@/features/forms/components/FormBuilder/components/FormBuilderScreenReuse'
import { redirectIfNotAuth } from '@/lib/utils'
import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/(app)/forms/new_/reuse')({
  beforeLoad: () => {
    redirectIfNotAuth()
  },
  component: CreateNewFormFromOldForm,
})

function CreateNewFormFromOldForm() {
  return <FormBuilderScreenReuse />
}
