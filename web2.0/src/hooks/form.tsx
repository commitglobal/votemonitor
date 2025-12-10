import { lazy } from 'react'
import { createFormHook } from '@tanstack/react-form'
import { fieldContext, formContext } from './form-context'

const TextInput = lazy(() => import('@/components/form/TextInput.tsx'))
const Checkbox = lazy(() => import('@/components/form/Checkbox.tsx'))
const SubmitButton = lazy(() => import('@/components/form/SubmitButton.tsx'))
const Textarea = lazy(() => import('@/components/form/Textarea.tsx'))
const FormTypeSelect = lazy(
  () => import('@/components/form/FormTypeSelect.tsx')
)
const LanguageSelect = lazy(
  () => import('@/components/form/LanguageSelect.tsx')
)

export const { useAppForm, withForm, withFieldGroup } = createFormHook({
  fieldContext: fieldContext,
  formContext: formContext,
  fieldComponents: {
    TextInput: TextInput,
    Checkbox: Checkbox,
    Textarea: Textarea,
    FormTypeSelect: FormTypeSelect,
    LanguageSelect: LanguageSelect,
  },
  formComponents: {
    SubmitButton: SubmitButton,
  },
})
