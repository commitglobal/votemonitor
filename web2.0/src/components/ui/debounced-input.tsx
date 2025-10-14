import { useEffect, useState } from 'react'
import { Input } from './input'

export function DebouncedInput({
  value: initialValue,
  onChange,
  debounce = 200,
  ...props
}: {
  value: string
  onChange: (value: string) => void
  debounce?: number
} & Omit<React.ComponentProps<'input'>, 'onChange'>) {
  const [value, setValue] = useState<string>(initialValue)

  useEffect(() => {
    setValue(initialValue)
  }, [initialValue])

  useEffect(() => {
    const timeout = setTimeout(() => {
      onChange(value)
    }, debounce)

    return () => clearTimeout(timeout)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [value])

  return (
    <Input
      {...props}
      value={value ?? ''}
      onChange={(e) => {
        if (e.target.value === '') return setValue('')
        setValue(e.target.value)
      }}
    />
  )
}
