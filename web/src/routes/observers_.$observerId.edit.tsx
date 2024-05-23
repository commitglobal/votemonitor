import EditObserver from '@/features/observers/components/EditObserver/EditObserver';
import { createFileRoute } from '@tanstack/react-router';
import { observerDetailsQueryOptions } from './observers/$observerId';

export const Route = createFileRoute('/observers/$observerId/edit')({
  component: Edit,
  loader: ({ context: { queryClient }, params: { observerId } }) =>
    queryClient.ensureQueryData(observerDetailsQueryOptions(observerId)),
});

function Edit() {
  return (
    <div className='p-2'>
      <EditObserver />
    </div>
  );
}
