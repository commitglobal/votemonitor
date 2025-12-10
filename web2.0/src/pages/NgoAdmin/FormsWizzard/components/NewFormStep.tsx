'use client'

import z from 'zod'
import { FormType } from '@/types/form'
import { Language, LanguageList } from '@/types/language'
import { formOptions, revalidateLogic } from '@tanstack/react-form'
import { ArrowLeft } from 'lucide-react'
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
  onComplete: (data: unknown) => void
  onBack: () => void
}

export const newFormSchema = z.object({
  code: z
    .string()
    .min(1, 'Form code is required')
    .max(256, 'Form code must be less than 256 characters'),
  name: z.string().min(2, 'Form name is required'),
  description: z.string().optional(),
  language: z.enum(LanguageList, { message: 'Language is required' }),
  formType: z.enum(
    [
      FormType.Opening,
      FormType.Voting,
      FormType.ClosingAndCounting,
      FormType.CitizenReporting,
      FormType.IncidentReporting,
      FormType.Other,
    ],
    { message: 'Form type is required' }
  ),
})

export const defaultValues: z.infer<typeof newFormSchema> = {
  code: '',
  name: '',
  description: '',
  language: Language.EN,
  formType: FormType.Opening,
}

export const newFormOpts = formOptions({
  defaultValues,
})

export function NewFormStep({ onComplete, onBack }: NewFormStepProps) {
  const form = useAppForm({
    ...newFormOpts,
    onSubmit: ({ value, meta }) => {
      console.log(value)
      console.log(newFormSchema.parse(value))
      console.log(meta)
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
                  <form.AppField name='language'>
                    {(field) => {
                      return (
                        <field.LanguageSelect
                          label='Language'
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
