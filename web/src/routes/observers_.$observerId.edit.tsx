import EditObserver from '@/features/observers/components/EditObserver/EditObserver'
import { createFileRoute } from '@tanstack/react-router'
import { observerDetailsQueryOptions } from './observers/$observerId'
import { redirectIfNotAuth } from '@/lib/utils'

export const Route = createFileRoute('/observers_/$observerId/edit')({
  component: Edit,
  loader: ({ context: { queryClient }, params: { observerId } }) =>
    queryClient.ensureQueryData(observerDetailsQueryOptions(observerId)),
  beforeLoad: () => {
    redirectIfNotAuth()
  },
})

function Edit() {
  return (
    <div className="p-2">
      <EditObserver />
    </div>
  )
}
