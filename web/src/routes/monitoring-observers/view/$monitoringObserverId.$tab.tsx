import type { FunctionComponent } from '@/common/types';
import MonitoringObserverDetails from '@/features/monitoring-observers/components/MonitoringObserverDetails/MonitoringObserverDetails';
import { monitoringObserverDetailsRouteSearchSchema } from '@/features/monitoring-observers/models/monitoring-observer';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute, redirect } from '@tanstack/react-router';
import { monitoringObserverDetailsQueryOptions } from '../edit.$monitoringObserverId';

export const Route = createFileRoute('/monitoring-observers/view/$monitoringObserverId/$tab')({
  beforeLoad: ({ params }) => {
    redirectIfNotAuth();

    const coercedTab = coerceTabSlug(params.tab);
    if (params.tab !== coercedTab) {
      throw redirect({
        to: `/monitoring-observers/view/$monitoringObserverId/$tab`,
        params: { tab: coercedTab, monitoringObserverId: params.monitoringObserverId },
      });
    }
  },
  component: Details,
  loader: ({ context: { queryClient, currentElectionRoundContext }, params: { monitoringObserverId } }) => {
    const electionRoundId = currentElectionRoundContext.getState().currentElectionRoundId;

    return queryClient.ensureQueryData(monitoringObserverDetailsQueryOptions(electionRoundId, monitoringObserverId));
  },
  validateSearch: monitoringObserverDetailsRouteSearchSchema,
});
const coerceTabSlug = (slug: string) => {
  if (slug?.toLowerCase()?.trim() === 'details') return 'details';
  if (slug?.toLowerCase()?.trim() === 'responses') return 'responses';
  if (slug?.toLowerCase()?.trim() === 'quick-reports') return 'quick-reports';
  // if (slug?.toLowerCase()?.trim() === 'incident-reports') return 'incident-reports';

  return 'details';
};
function Details(): FunctionComponent {
  return <MonitoringObserverDetails />;
}
