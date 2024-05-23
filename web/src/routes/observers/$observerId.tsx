import { authApi } from '@/common/auth-api';
import ObserverDetails from '@/features/observers/components/ObserverDetails/ObserverDetails';
import { Observer } from '@/features/observers/models/Observer';
import { redirectIfNotAuth } from '@/lib/utils';
import { queryOptions } from '@tanstack/react-query';
import { createFileRoute } from '@tanstack/react-router';

export const observerDetailsQueryOptions = (observerId: string) =>
  queryOptions({
    queryKey: ['observer', { observerId }],
    queryFn: async () => {
      const response = await authApi.get<Observer>(`/observers/${observerId}`);

      if (response.status !== 200) {
        throw new Error('Failed to fetch observer details');
      }

      return response.data;
    },
  });

export const Route = createFileRoute('/observers/$observerId')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: Details,
  loader: ({ context: { queryClient }, params: { observerId } }) =>
    queryClient.ensureQueryData(observerDetailsQueryOptions(observerId)),
});

function Details() {
  return (
    <div className='p-2'>
      <ObserverDetails />
    </div>
  );
}
