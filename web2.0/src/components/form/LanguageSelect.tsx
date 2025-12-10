import { LanguageList } from '@/types/language'
import { mapLanguageNameByCode } from '@/lib/i18n'
import { useFieldContext } from '@/hooks/form-context'
import {
  Field,
  FieldDescription,
  FieldError,
  FieldLabel,
} from '@/components/ui/field'
import { SearchableCombobox } from '../ui/searchable-combobox'

export default function LanguageSelect({
  label,
  description,
  id,
  placeholder = 'Select a language',
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
      <SearchableCombobox
        id={id}
        title={label}
        options={LanguageList.map((language) => ({
          value: language,
          label: mapLanguageNameByCode(language),
        }))}
        value={field.state.value}
        placeholder={placeholder}
        onValueChange={(value) => field.handleChange(value)}
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
