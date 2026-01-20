import {
  Field,
  FieldContent,
  FieldDescription,
  FieldError,
  FieldLabel,
} from '@/components/ui/field'
import { useFieldContext } from '@/hooks/form-context'
import { SearchableCombobox } from '../ui/searchable-combobox'

export default function Select({
  label,
  description,
  id,
  placeholder = 'Select an option',
  required,
  className,
  options,
  disabled,
}: {
  label: string
  id: string
  description?: string
  placeholder?: string
  required?: boolean
  className?: string
  options: { value: string; label: string }[]
  disabled?: boolean
}) {
  const field = useFieldContext<string>()
  return (
    <Field className={className}>
      <FieldLabel htmlFor={id}>
        {label}
        {required && <span className='text-destructive ml-1'>*</span>}
      </FieldLabel>
      <FieldContent>
        <SearchableCombobox
          id={id}
          title={label}
          options={options}
          value={field.state.value}
          placeholder={placeholder}
          onValueChange={(value) => field.handleChange(value)}
          disabled={disabled}
        />
      </FieldContent>
      {description && <FieldDescription>{description}</FieldDescription>}
      {!field.state.meta.isValid && (
        <FieldError>
          {field.state.meta.errors.map((error) => error?.message).join(', ')}
        </FieldError>
      )}
    </Field>
  )
}
