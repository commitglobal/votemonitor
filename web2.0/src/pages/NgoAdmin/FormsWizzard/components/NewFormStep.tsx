import z from 'zod'
import { useNavigate } from '@tanstack/react-router'
import { useCurrentElectionRound } from '@/contexts/election-round.context'
import { useCreateFormMutation } from '@/mutations/form-mutations'
import { FormType } from '@/types/form'
import { Language, LanguageList } from '@/types/language'
import { formOptions, revalidateLogic } from '@tanstack/react-form'
import { ArrowLeft } from 'lucide-react'
import { newTranslatedString } from '@/lib/translated-string'
import { useAppForm } from '@/hooks/form'
import { Button } from '@/components/ui/button'
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card'
import { FieldGroup } from '@/components/ui/field'

interface NewFormStepProps {
  onBack: () => void
}

export const newFormSchema = z.object({
  code: z
    .string()
    .min(1, 'Form code is required')
    .max(256, 'Form code must be less than 256 characters'),
  name: z.string().min(2, 'Form name is required'),
  description: z.string().optional(),
  defaultLanguage: z.enum(LanguageList, { message: 'Language is required' }),
  formType: z.enum(
    [
      FormType.Opening,
      FormType.Voting,
      FormType.ClosingAndCounting,
      // FormType.CitizenReporting,
      // FormType.IncidentReporting,
      FormType.Other,
    ],
    { message: 'Form type is required' }
  ),
})

export const defaultValues: z.infer<typeof newFormSchema> = {
  code: '',
  name: '',
  description: '',
  defaultLanguage: Language.EN,
  formType: FormType.Opening,
}

export const newFormOpts = formOptions({
  defaultValues,
})

export function NewFormStep({ onBack }: NewFormStepProps) {
  const { electionRound } = useCurrentElectionRound()
  const createFormMutation = useCreateFormMutation(electionRound.id)
  const navigate = useNavigate()

  const form = useAppForm({
    ...newFormOpts,
    onSubmit: async ({ formApi, value }) => {
      await createFormMutation.mutateAsync(
        {
          code: value.code,
          name: newTranslatedString(
            [value.defaultLanguage],
            value.defaultLanguage,
            value.name
          ),
          defaultLanguage: value.defaultLanguage,
          languages: [value.defaultLanguage],
          formType: value.formType,
          description: newTranslatedString(
            [value.defaultLanguage],
            value.defaultLanguage,
            value.description ?? ''
          ),
        },
        {
          onSuccess: (form) => {
            formApi.reset()
            navigate({
              to: '/elections/$electionRoundId/forms/$formId/edit/$languageCode',
              params: { electionRoundId: electionRound.id, formId: form.id, languageCode: form.defaultLanguage },
            })
          },
        }
      )
    },
    validators: {
      onDynamic: newFormSchema,
    },
    validationLogic: revalidateLogic({
      mode: 'submit',
      modeAfterSubmission: 'blur',
    }),
  })

  return (
    <Card>
      <CardHeader>
        <CardTitle>Create New Form</CardTitle>
        <CardDescription>
          Fill in the details below to create your new form
        </CardDescription>
      </CardHeader>
      <CardContent>
        <form.AppForm>
          <form
            onSubmit={(e) => {
              e.preventDefault()
              form.handleSubmit({ key: 'value' })
            }}
          >
            <FieldGroup>
              <div className='grid grid-cols-1 gap-4 md:grid-cols-3'>
                <div className='md:col-span-1'>
                  <form.AppField name='code'>
                    {(field) => {
                      return (
                        <field.TextInput id='code' label='Form Code' required />
                      )
                    }}
                  </form.AppField>
                </div>
                <div className='md:col-span-2'>
                  <form.AppField name='name'>
                    {(field) => (
                      <field.TextInput id='name' label='Form Name' required />
                    )}
                  </form.AppField>
                </div>
              </div>

              <div className='grid grid-cols-1 gap-4 md:grid-cols-2'>
                <div className='md:col-span-1'>
                  <form.AppField name='defaultLanguage'>
                    {(field) => {
                      return (
                        <field.LanguageSelect
                          label='Base language'
                          id='language'
                          description='The base language of the form'
                          required
                        />
                      )
                    }}
                  </form.AppField>
                </div>
                <div className='md:col-span-1'>
                  <form.AppField name='formType'>
                    {(field) => {
                      return (
                        <field.FormTypeSelect
                          label='Form Type'
                          id='formType'
                          required
                        />
                      )
                    }}
                  </form.AppField>
                </div>
              </div>

              <form.AppField name='description'>
                {(field) => {
                  return (
                    <field.Textarea label='Form Description' id='description' />
                  )
                }}
              </form.AppField>
            </FieldGroup>

            <div className='mt-4 flex items-center justify-between gap-4'>
              <Button
                type='button'
                variant='ghost'
                onClick={onBack}
                className='gap-2'
              >
                <ArrowLeft className='h-4 w-4' />
                Back
              </Button>
              <form.SubmitButton />
            </div>
          </form>
        </form.AppForm>
      </CardContent>
    </Card>
  )
}
