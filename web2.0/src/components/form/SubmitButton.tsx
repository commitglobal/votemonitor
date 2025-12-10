import { useFormContext } from '@/hooks/form-context'
import { Button } from '../ui/button'

export default function SubmitButton() {
  const form = useFormContext()
  return (
    <form.Subscribe selector={(state) => state.canSubmit}>
      {(canSubmit) => (
        <div className='flex items-center gap-2'>
          <Button
            disabled={!canSubmit}
            type='submit'
            onClick={() => form.handleSubmit({ submitAction: 'save' })}
          >
            Save
          </Button>
        </div>
      )}
    </form.Subscribe>
  )
}
