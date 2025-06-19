import EditObserver from '@/features/observers/components/EditObserver/EditObserver';
import { createFileRoute } from '@tanstack/react-router';
import { redirectIfNotAuth } from '@/lib/utils';
import { observerDetailsQueryOptions } from './$observerId';

export const Route = createFileRoute('/(app)/observers/$observerId/edit')({
  component: Edit,
  loader: ({ context: { queryClient }, params: { observerId } }) =>
    queryClient.ensureQueryData(observerDetailsQueryOptions(observerId)),
  beforeLoad: () => {
    redirectIfNotAuth();
  },
});

function Edit() {
  return (
    <div className='p-2'>
      <EditObserver />
    </div>
  );
}
