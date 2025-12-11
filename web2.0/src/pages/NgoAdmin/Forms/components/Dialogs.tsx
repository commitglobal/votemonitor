import { ConfirmDialog } from '@/components/ConfirmDialog'
import { useForms } from './FormsProvider'

export function FormsDialogs() {
  const { open, setOpen, currentRow, setCurrentRow } = useForms()
  // const { electionRound } = useCurrentElectionRound()

  return (
    <>
      {currentRow && (
        <>
          <ConfirmDialog
            key='task-delete'
            destructive
            open={open === 'delete'}
            onOpenChange={() => {
              setOpen('delete')
              setTimeout(() => {
                setCurrentRow(null)
              }, 500)
            }}
            handleConfirm={() => {
              setOpen(null)
              setTimeout(() => {
                setCurrentRow(null)
              }, 500)
              console.log(currentRow)
            }}
            className='max-w-md'
            title={`Delete this form: ${currentRow.id} ?`}
            desc={`You are about to delete a form with the ID ${currentRow.id}. This action cannot be undone.`}
            confirmText='Delete'
          />
        </>
      )}
    </>
  )
}
