import { authApi } from '@/common/auth-api';
import ObserverDetails from '@/features/observers/components/ObserverDetails/ObserverDetails';
import { observersKeys } from '@/features/observers/hooks/observers-queries';
import { Observer } from '@/features/observers/models/observer';
import { redirectIfNotAuth, redirectIfNotPlatformAdmin } from '@/lib/utils';
import { queryOptions } from '@tanstack/react-query';
import { createFileRoute } from '@tanstack/react-router';

export const observerDetailsQueryOptions = (observerId: string) =>
  queryOptions({
    queryKey: observersKeys.detail(observerId),
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
    redirectIfNotPlatformAdmin();
  },
  component: Details,
  loader: ({ context: { queryClient }, params: { observerId } }) =>
    queryClient.ensureQueryData(observerDetailsQueryOptions(observerId)),
});

function Details() {
  return <ObserverDetails />;
}
