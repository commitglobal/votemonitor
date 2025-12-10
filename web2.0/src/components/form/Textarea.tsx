import { useFieldContext } from '@/hooks/form-context'
import {
  Field,
  FieldDescription,
  FieldError,
  FieldLabel,
} from '@/components/ui/field'
import { Textarea as TextareaComponent } from '../ui/textarea'

export default function Textarea({
  label,
  description,
  id,
  rows = 3,
}: {
  label: string
  id: string
  description?: string
  rows?: number
}) {
  const field = useFieldContext<string>()
  return (
    <Field>
      <FieldLabel htmlFor={id}>{label}</FieldLabel>
      <TextareaComponent
        id={id}
        rows={rows}
        placeholder={label}
        value={field.state.value}
        onBlur={field.handleBlur}
        onChange={(e) => field.handleChange(e.target.value)}
      />
      {description && <FieldDescription>{description}</FieldDescription>}
      {!field.state.meta.isValid && (
        <FieldError>
          {field.state.meta.errors.map((error) => error?.message).join(', ')}
        </FieldError>
      )}
    </Field>
  )
}
