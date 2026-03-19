import { getObserverDetails } from '@/api/observers/get-observer-details';
import ObserverProfile from '@/features/observers/components/ObserverProfile/ObserverProfile';
import { observersKeys } from '@/features/observers/hooks/observers-queries';
import { Observer } from '@/features/observers/models/observer';
import { redirectIfNotAuth, redirectIfNotPlatformAdmin } from '@/lib/utils';
import { queryOptions } from '@tanstack/react-query';
import { createFileRoute } from '@tanstack/react-router';

export const observerDetailsQueryOptions = (observerId: string) =>
  queryOptions({
    queryKey: observersKeys.detail(observerId),
    queryFn: async () => {
      return getObserverDetails(observerId);
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
  return <ObserverProfile />;
}
