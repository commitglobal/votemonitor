import { useResendMonitoringObserverInvitesMutation } from '@/mutations/monitoring-observers-mutations'
import { Route } from '@/routes/(app)/elections/$electionRoundId/observers'
import { toast } from 'sonner'
import { ConfirmDialog } from '@/components/ConfirmDialog'
import { useObservers } from './ObserversProvider'

/** The confirmation for the row actions, mounted once beside the table. */
export function ObserversDialogs() {
  const { open, setOpen, currentRow } = useObservers()
  const { electionRoundId } = Route.useParams()

  const resendMutation =
    useResendMonitoringObserverInvitesMutation(electionRoundId)

  if (!currentRow) {
    return null
  }

  const handleResendInvite = () => {
    // The endpoint takes a list because it also serves bulk resending; a single
    // row passes an array of one.
    resendMutation.mutate([currentRow.id], {
      onSuccess: () => {
        setOpen(null)
        toast.success('Invitation sent')
      },
      onError: () => {
        toast.error('Failed to send the invitation', {
          description:
            'Please try again or contact support if the problem persists.',
        })
      },
    })
  }

  return (
    <ConfirmDialog
      open={open === 'resendInvite'}
      onOpenChange={(isOpen) => {
        if (!isOpen) {
          setOpen(null)
        }
      }}
      handleConfirm={handleResendInvite}
      isLoading={resendMutation.isPending}
      className='max-w-md'
      title='Resend invitation email'
      desc={`A new invitation email will be sent to ${currentRow.email}.`}
      confirmText='Send'
    />
  )
}
