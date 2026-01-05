import FormStatusBadge from '@/components/badges/from-status-badge'
import SubmitButton from '@/components/form/SubmitButton'
import { H1 } from '@/components/ui/typography'
import { useFormContext } from '@/hooks/form-context'
import { useSuspenseGetFormDetails } from '@/queries/forms'
import { Route } from '@/routes/(app)/elections/$electionRoundId/forms/$formId/edit.$languageCode'

export function FormHeader() {
  const { electionRoundId, formId } = Route.useParams()
  const { data: formData } = useSuspenseGetFormDetails(electionRoundId, formId)
  const form = useFormContext()

  return (
    <div className='mb-6'>
      <div className='flex items-center justify-between'>
        <div className='flex items-center gap-3'>
          <form.Subscribe selector={(state) => state.values.name}>
            {(name) => (
              <H1>
                {name}
              </H1>
            )}
          </form.Subscribe>
          <FormStatusBadge formStatus={formData.status} />

        </div>
        <div className='flex items-center gap-2'>
          <SubmitButton />
        </div>
      </div>
    </div>
  )
}

