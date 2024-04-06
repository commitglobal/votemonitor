import EditObserver from '@/features/observers/components/EditObserver/EditObserver';
import { Outlet, createFileRoute } from '@tanstack/react-router';
import { observerQueryOptions } from './observers/$observerId';

export const Route = createFileRoute('/observers/$observerId/edit')({
  component: Edit,
  loader: ({ context: { queryClient }, params: { observerId } }) =>
    queryClient.ensureQueryData(observerQueryOptions(observerId)),
});

function Edit() {
  return (
    <div className='p-2'>
      <EditObserver />
    </div>
  );
}
