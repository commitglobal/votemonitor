import { FormType } from '@/types/form'
import { mapFormType } from '@/lib/i18n'
import { useFieldContext } from '@/hooks/form-context'
import {
  Field,
  FieldDescription,
  FieldError,
  FieldLabel,
} from '@/components/ui/field'
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '../ui/select'

export default function FormTypeSelect({
  label,
  description,
  id,
  placeholder = 'Select a form type',
  required,
  className,
}: {
  label: string
  id: string
  description?: string
  placeholder?: string
  required?: boolean
  className?: string
}) {
  const field = useFieldContext<string>()
  return (
    <Field className={className}>
      <FieldLabel htmlFor={id}>
        {label}
        {required && <span className='text-destructive ml-1'>*</span>}
      </FieldLabel>
      <Select
        value={field.state.value}
        onValueChange={(value) => field.handleChange(value as any)}
      >
        <SelectTrigger className='w-full' id={id} onBlur={field.handleBlur}>
          <SelectValue placeholder={placeholder} />
        </SelectTrigger>
        <SelectContent>
          <SelectGroup>
            {[
              FormType.Opening,
              FormType.Voting,
              FormType.ClosingAndCounting,
              FormType.CitizenReporting,
              FormType.IncidentReporting,
              FormType.Other,
            ].map((formType) => (
              <SelectItem key={formType} value={formType}>
                {mapFormType(formType)}
              </SelectItem>
            ))}
          </SelectGroup>
        </SelectContent>
      </Select>
      {description && <FieldDescription>{description}</FieldDescription>}
      {!field.state.meta.isValid && (
        <FieldError>
          {field.state.meta.errors.map((error) => error?.message).join(', ')}
        </FieldError>
      )}
    </Field>
  )
}
