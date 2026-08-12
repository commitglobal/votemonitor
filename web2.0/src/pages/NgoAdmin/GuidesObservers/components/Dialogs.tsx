import { useDeleteGuideMutation } from '@/mutations/guides-observers-mutations'
import { Route } from '@/routes/(app)/elections/$electionRoundId/guides'
import { toast } from 'sonner'
import { ConfirmDialog } from '@/components/ConfirmDialog'
import { CreateGuideDialog } from './CreateGuideDialog'
import { useGuides } from './GuidesProvider'
import { UpdateGuideDialog } from './UpdateGuideDialog'

/** Every dialog of the guides page, mounted once next to the table. */
export function GuidesDialogs() {
  const { open, setOpen, currentRow } = useGuides()
  const { electionRoundId } = Route.useParams()
  const deleteGuideMutation = useDeleteGuideMutation(electionRoundId)

  const handleDelete = () => {
    if (!currentRow) {
      return
    }

    deleteGuideMutation.mutate(currentRow.id, {
      onSuccess: () => {
        setOpen(null)
        toast.success('Delete was successful')
      },
      onError: () => {
        toast.error('Error deleting guide', {
          description:
            'Please try again or contact support if the problem persists.',
        })
      },
    })
  }

  return (
    <>
      <CreateGuideDialog />
      <UpdateGuideDialog />

      {currentRow && (
        <ConfirmDialog
          destructive
          open={open === 'delete'}
          onOpenChange={(isOpen) => {
            if (!isOpen) {
              setOpen(null)
            }
          }}
          handleConfirm={handleDelete}
          isLoading={deleteGuideMutation.isPending}
          className='max-w-md'
          title={`Delete ${currentRow.title} ?`}
          desc='Are you sure you want to delete this guide? This action cannot be undone.'
          confirmText='Delete'
        />
      )}
    </>
  )
}
